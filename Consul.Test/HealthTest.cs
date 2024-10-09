// -----------------------------------------------------------------------
//  <copyright file="HealthTest.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
//    Copyright 2020 G-Research Limited
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class HealthTest : BaseFixture
    {
        [Fact]
        public async Task Health_GetLocalNode()
        {
            var info = await _client.Agent.Self();
            var checks = await _client.Health.Node((string)info.Response["Config"]["NodeName"]);

            Assert.NotEqual((ulong)0, checks.LastIndex);
            Assert.NotEmpty(checks.Response);
        }

        [Fact]
        public async Task Health_Checks()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentServiceRegistration
            {
                Name = svcID,
                Tags = new[] { "bar", "baz" },
                Port = 8000,
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(15)
                }
            };
            try
            {
                await _client.Agent.ServiceRegister(registration);
                var checks = await _client.Health.Checks(svcID);
                Assert.NotEqual((ulong)0, checks.LastIndex);
                Assert.NotEmpty(checks.Response);
                Assert.Equal(checks.Response[0].ServiceTags, registration.Tags);
            }
            finally
            {
                await _client.Agent.ServiceDeregister(svcID);
            }
        }

        [Fact]
        public async Task Health_GetConsulService()
        {
            var checks = await _client.Health.Service("consul", "", false);
            Assert.NotEqual((ulong)0, checks.LastIndex);
            Assert.NotEmpty(checks.Response);
        }

        [Fact]
        public async Task Health_GetServiceWithTaggedAddresses()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentServiceRegistration
            {
                Name = svcID,
                Port = 8000,
                TaggedAddresses = new Dictionary<string, ServiceTaggedAddress>
                    {
                        {"lan", new ServiceTaggedAddress {Address = "127.0.0.1", Port = 80}},
                        {"wan", new ServiceTaggedAddress {Address = "192.168.10.10", Port = 8000}}
                    }
            };

            await _client.Agent.ServiceRegister(registration);

            var checks = await _client.Health.Service(svcID, "", false);
            Assert.NotEqual((ulong)0, checks.LastIndex);
            Assert.NotEmpty(checks.Response);

            Assert.True(checks.Response[0].Service.TaggedAddresses.Count > 0);
            Assert.True(checks.Response[0].Service.TaggedAddresses.ContainsKey("wan"));
            Assert.True(checks.Response[0].Service.TaggedAddresses.ContainsKey("lan"));

            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Health_GetState()
        {
            var checks = await _client.Health.State(HealthStatus.Any);
            Assert.NotEqual((ulong)0, checks.LastIndex);
            Assert.NotEmpty(checks.Response);
        }

        private struct AggregatedStatusResult
        {
            public string Name;
            public List<HealthCheck> Checks;
            public HealthStatus Expected;

        }

        [Fact]
        public async Task Health_Connect()
        {
            var destinationServiceID = KVTest.GenerateTestKeyName();

            var registration = new AgentServiceRegistration
            {
                ID = destinationServiceID,
                Name = destinationServiceID,
                Port = 8000,
                Check = new AgentServiceCheck { TTL = TimeSpan.FromSeconds(15) },
                Connect = new AgentServiceConnect { SidecarService = new AgentServiceRegistration { Port = 8001 } }
            };

            try
            {
                await _client.Agent.ServiceRegister(registration);
                QueryResult<ServiceEntry[]> checks;
                ulong lastIndex = 0;
                do
                {
                    var q = new QueryOptions { WaitIndex = lastIndex, };
                    // Use the Health.Connect method to query health information for Connect-enabled services
                    checks = await _client.Health.Connect(destinationServiceID, "", false, q, null,
                        new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token);
                    Assert.Equal(HttpStatusCode.OK, checks.StatusCode);
                    Assert.True(checks.LastIndex > q.WaitIndex);
                    lastIndex = checks.LastIndex;
                } while (!checks.Response.Any());

                Assert.Single(checks.Response);
                Assert.Equal(registration.Connect.SidecarService.Port, checks.Response[0].Service.Port);
            }
            finally
            {
                await _client.Agent.ServiceDeregister(destinationServiceID);
            }
        }

        [SkippableFact]
        public async void Health_Ingress()
        {
            var cutOffVersion = SemanticVersion.Parse("1.8.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but Terminating and Ingress GatewayEntrys are different since {cutOffVersion}");

            var registration = new AgentServiceRegistration
            {
                Name = "foo-ingress",
                Port = 8000
            };
            await _client.Agent.ServiceRegister(registration);

            var gatewayRegistration = new AgentServiceRegistration
            {
                Name = "foo-ingress-gateway",
                Port = 8001,
                Kind = ServiceKind.IngressGateway
            };
            await _client.Agent.ServiceRegister(gatewayRegistration);

            var ingressGatewayConfigEntry = new IngressGatewayEntry
            {
                Name = "foo-ingress-gateway",
                Listeners = new List<GatewayListener>
                {
                    new GatewayListener
                    {
                        Port = 2222,
                        Protocol = "tcp",
                        Services = new List<ExternalService>
                        {
                            new ExternalService
                            {
                                Name = "foo-ingress"
                            }
                        }
                    }
                }
            };
            await _client.Configuration.ApplyConfig(ingressGatewayConfigEntry);

            var services = await _client.Health.Ingress("foo-ingress", "", false);
            Assert.Single(services.Response);
            Assert.NotEmpty(services.Response[0].Node.Datacenter);
            Assert.Equal(gatewayRegistration.Name, services.Response[0].Service.Service);
        }

        [Fact]
        public void Health_GetAggregatedStatus()
        {
            var cases = new List<AggregatedStatusResult>()
            {
                new AggregatedStatusResult() {Name="empty", Expected=HealthStatus.Passing, Checks = null},
                new AggregatedStatusResult() {Name="passing", Expected=HealthStatus.Passing, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() {Status = HealthStatus.Passing }
                }},
                new AggregatedStatusResult() {Name="warning", Expected=HealthStatus.Warning, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() {Status = HealthStatus.Warning }
                }},
                new AggregatedStatusResult() {Name="critical", Expected=HealthStatus.Critical, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() {Status = HealthStatus.Critical }
                }},
                new AggregatedStatusResult() {Name="node_maintenance", Expected=HealthStatus.Maintenance, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() { CheckID=HealthStatus.NodeMaintenance }
                }},
                new AggregatedStatusResult() {Name="service_maintenance", Expected=HealthStatus.Maintenance, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() { CheckID=HealthStatus.ServiceMaintenancePrefix + "service"}
                }},
                new AggregatedStatusResult() {Name="unknown", Expected=HealthStatus.Passing, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() { Status = HealthStatus.Any}
                }},
                new AggregatedStatusResult() {Name="maintenance_over_critical", Expected=HealthStatus.Maintenance, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() { CheckID=HealthStatus.NodeMaintenance },
                    new HealthCheck() {Status = HealthStatus.Critical }
                }},
                new AggregatedStatusResult() {Name="critical_over_warning", Expected=HealthStatus.Critical, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() {Status = HealthStatus.Critical },
                    new HealthCheck() {Status = HealthStatus.Warning }
                }},
                new AggregatedStatusResult() {Name="warning_over_passing", Expected=HealthStatus.Warning, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() {Status = HealthStatus.Warning },
                    new HealthCheck() {Status = HealthStatus.Passing }
                }},
                new AggregatedStatusResult() {Name="lots", Expected=HealthStatus.Warning, Checks = new List<HealthCheck>()
                {
                    new HealthCheck() {Status = HealthStatus.Passing },
                    new HealthCheck() {Status = HealthStatus.Passing },
                    new HealthCheck() {Status = HealthStatus.Warning },
                    new HealthCheck() {Status = HealthStatus.Passing }
                }}
            };
            foreach (var test_case in cases)
            {
                Assert.Equal(test_case.Expected, test_case.Checks.AggregatedStatus());
            }
        }
    }
}
