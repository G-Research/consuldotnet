// -----------------------------------------------------------------------
//  <copyright file="CatalogTest.cs" company="PlayFab Inc">
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
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class CatalogTest : BaseFixture
    {
        [Fact]
        public async Task Catalog_GetDatacenters()
        {
            var datacenterList = await _client.Catalog.Datacenters();

            Assert.NotEmpty(datacenterList.Response);
        }

        [Fact]
        public async Task Catalog_GetNodes()
        {
            var nodeList = await _client.Catalog.Nodes();

            Assert.NotEqual((ulong)0, nodeList.LastIndex);
            Assert.NotEmpty(nodeList.Response);
            // make sure deserialization is working right
            Assert.NotNull(nodeList.Response[0].Address);
            Assert.NotNull(nodeList.Response[0].Name);
        }

        [Fact]
        public async Task Catalog_GetServices()
        {
            var servicesList = await _client.Catalog.Services();

            Assert.NotEqual((ulong)0, servicesList.LastIndex);
            Assert.NotEmpty(servicesList.Response);
        }

        [Fact]
        public async Task Catalog_GetConsulService()
        {
            var serviceList = await _client.Catalog.Service("consul");

            Assert.NotEqual((ulong)0, serviceList.LastIndex);
            Assert.NotEmpty(serviceList.Response);
        }

        [Fact]
        public async Task Catalog_GetLocalhostNode()
        {
            var node = await _client.Catalog.Node(await _client.Agent.GetNodeName());

            Assert.NotEqual((ulong)0, node.LastIndex);
            Assert.NotNull(node.Response.Services);
            Assert.Equal("127.0.0.1", node.Response.Node.Address);
            Assert.True(node.Response.Node.TaggedAddresses.Count > 0);
            Assert.True(node.Response.Node.TaggedAddresses.ContainsKey("wan"));
        }

        [Fact]
        public async Task Catalog_RegistrationDeregistration()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var service = new AgentService
            {
                ID = svcID,
                Service = "redis",
                Tags = new[] { "master", "v1" },
                Port = 8000
            };

            var check = new AgentCheck
            {
                Node = "foobar",
                CheckID = "service:" + svcID,
                Name = "Redis health check",
                Notes = "Script based health check",
                Status = HealthStatus.Passing,
                ServiceID = svcID
            };

            var registration = new CatalogRegistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10",
                Service = service,
                Check = check
            };

            await _client.Catalog.Register(registration);

            var node = await _client.Catalog.Node("foobar");
            Assert.True(node.Response.Services.ContainsKey(svcID));

            var health = await _client.Health.Node("foobar");
            Assert.Equal("service:" + svcID, health.Response[0].CheckID);

            var dereg = new CatalogDeregistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10",
                CheckID = "service:" + svcID
            };

            await _client.Catalog.Deregister(dereg);

            health = await _client.Health.Node("foobar");
            Assert.Empty(health.Response);

            dereg = new CatalogDeregistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10"
            };

            await _client.Catalog.Deregister(dereg);

            node = await _client.Catalog.Node("foobar");
            Assert.Null(node.Response);
        }

        [Fact]
        public async Task Catalog_GetTaggedAddressesService()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new CatalogRegistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10",
                Service = new AgentService
                {
                    ID = svcID,
                    Service = "redis",
                    Tags = new[] { "master", "v1" },
                    Port = 8000,
                    TaggedAddresses = new Dictionary<string, ServiceTaggedAddress>
                    {
                        {"lan", new ServiceTaggedAddress {Address = "127.0.0.1", Port = 80}},
                        {"wan", new ServiceTaggedAddress {Address = "192.168.10.10", Port = 8000}}
                    }
                }
            };

            await _client.Catalog.Register(registration);

            var services = await _client.Catalog.Service("redis");

            Assert.True(services.Response.Length > 0);
            Assert.True(services.Response[0].ServiceTaggedAddresses.Count > 0);
            Assert.True(services.Response[0].ServiceTaggedAddresses.ContainsKey("wan"));
            Assert.True(services.Response[0].ServiceTaggedAddresses.ContainsKey("lan"));
        }

        [Fact]
        public async Task Catalog_GetNodesForMeshCapableService()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentServiceRegistration
            {
                Name = svcID,
                Port = 8000,
                Connect = new AgentServiceConnect
                {

                    SidecarService = new AgentServiceRegistration
                    {
                        Name = "sidecar",
                        Port = 9000,
                    },
                },
            };
            await _client.Agent.ServiceRegister(registration);

            var services = await _client.Catalog.NodesForMeshCapableService(registration.Name);
            Assert.NotEmpty(services.Response);
            Assert.Equal(services.Response[0].ServiceID, registration.Name + "-sidecar-proxy");
        }

        [Fact]
        public async Task Catalog_EnableTagOverride()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var service = new AgentService
            {
                ID = svcID,
                Service = svcID,
                Tags = new[] { "master", "v1" },
                Port = 8000
            };

            var registration = new CatalogRegistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10",
                Service = service
            };

            using (IConsulClient client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            }))
            {
                await client.Catalog.Register(registration);

                var node = await client.Catalog.Node("foobar");

                Assert.Contains(svcID, node.Response.Services.Keys);
                Assert.False(node.Response.Services[svcID].EnableTagOverride);

                var services = await client.Catalog.Service(svcID);

                Assert.NotEmpty(services.Response);
                Assert.Equal(svcID, services.Response[0].ServiceName);

                Assert.False(services.Response[0].ServiceEnableTagOverride);
            }

            // Use a new scope
            using (IConsulClient client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            }))
            {
                service.EnableTagOverride = true;

                await client.Catalog.Register(registration);
                var node = await client.Catalog.Node("foobar");

                Assert.Contains(svcID, node.Response.Services.Keys);
                Assert.True(node.Response.Services[svcID].EnableTagOverride);

                var services = await client.Catalog.Service(svcID);

                Assert.NotEmpty(services.Response);
                Assert.Equal(svcID, services.Response[0].ServiceName);

                Assert.True(services.Response[0].ServiceEnableTagOverride);
            }
        }

        [SkippableFact]
        public async Task Catalog_ServicesForNodes()
        {
            var cutOffVersion = SemanticVersion.Parse("1.7.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `ServicesForNodes` is only supported from Consul {cutOffVersion}");

            var svcID = KVTest.GenerateTestKeyName();
            var svcID2 = KVTest.GenerateTestKeyName();
            var registration1 = new CatalogRegistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10",
                Service = new AgentService
                {
                    ID = svcID,
                    Service = "redis",
                    Tags = new[] { "master", "v1" },
                    Port = 8000,
                    TaggedAddresses = new Dictionary<string, ServiceTaggedAddress>
                    {
                        {"lan", new ServiceTaggedAddress {Address = "127.0.0.1", Port = 80}},
                        {"wan", new ServiceTaggedAddress {Address = "192.168.10.10", Port = 8000}}
                    }
                }
            };
            var registration2 = new CatalogRegistration
            {
                Datacenter = "dc1",
                Node = "foobar2",
                Address = "192.168.10.11",
                Service = new AgentService
                {
                    ID = svcID2,
                    Service = "redis",
                    Tags = new[] { "master", "v2" },
                    Port = 8000,
                    TaggedAddresses = new Dictionary<string, ServiceTaggedAddress>
                    {
                        { "lan", new ServiceTaggedAddress { Address = "127.0.0.1", Port = 81 } },
                        { "wan", new ServiceTaggedAddress { Address = "192.168.10.10", Port = 8001 } }
                    }
                }
            };

            await _client.Catalog.Register(registration1);
            await _client.Catalog.Register(registration2);
            var services = await _client.Catalog.ServicesForNode(registration1.Node, new QueryOptions { Datacenter = registration1.Datacenter });
            Assert.Contains(services.Response.Services, n => n.ID == svcID);
            Assert.DoesNotContain(services.Response.Services, n => n.ID == svcID2);
        }

        [SkippableFact]
        public async Task Catalog_GatewayServices()
        {
            var cutOffVersion = SemanticVersion.Parse("1.8.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but Terminating and Ingress GatewayEntrys are different since {cutOffVersion}");
            using (IConsulClient client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            }))
            {
                var terminatingGatewayName = "my-terminating-gateway";
                var ingressGatewayName = "my-ingress-gateway";

                var terminatingGatewayEntry = new CatalogRegistration
                {
                    Datacenter = "dc1",
                    Node = "bar",
                    Address = "192.168.10.11",
                    Service = new AgentService
                    {
                        ID = "redis",
                        Service = "redis",
                        Port = 6379
                    }
                };
                await client.Catalog.Register(terminatingGatewayEntry);

                var terminatingGatewayConfigEntry = new TerminatingGatewayEntry
                {
                    Name = terminatingGatewayName,
                    Services = new List<LinkedService>
                    {
                        new LinkedService
                        {
                            Name = "api",
                            CAFile = "api/ca.crt",
                            CertFile = "api/client.crt",
                            KeyFile = "api/client.key",
                            SNI = "my-domain"
                        },
                        new LinkedService
                        {
                            Name = "*",
                            CAFile = "ca.crt",
                            CertFile = "client.crt",
                            KeyFile = "client.key",
                            SNI = "my-alt-domain"
                        }
                    }
                };
                await client.Configuration.ApplyConfig(terminatingGatewayConfigEntry);

                var ingressGatewayConfigEntry = new IngressGatewayEntry
                {
                    Name = ingressGatewayName,
                    Listeners = new List<GatewayListener>
                    {
                        new GatewayListener
                        {
                            Port = 8888,
                            Services = new List<ExternalService>
                            {
                                new ExternalService
                                {
                                    Name = "api"
                                }
                            }
                        },
                        new GatewayListener
                        {
                            Port = 9999,
                            Services = new List<ExternalService>
                            {
                                new ExternalService
                                {
                                    Name = "redis"
                                }
                            }
                        }
                    }
                };

                await client.Configuration.ApplyConfig(ingressGatewayConfigEntry);

                var gatewayServices = await client.Catalog.GatewayService(terminatingGatewayName);
                Assert.NotEmpty(gatewayServices.Response);

                var terminatingService = gatewayServices.Response[0];
                Assert.NotNull(terminatingService.Gateway);
                Assert.Equal(terminatingGatewayName, terminatingService.Gateway.Name);
                Assert.NotNull(terminatingService.Service);
                Assert.Equal(ServiceKind.TerminatingGateway, terminatingService.GatewayKind);
                Assert.NotNull(terminatingService.CAFile);
                Assert.NotNull(terminatingService.CertFile);
                Assert.NotNull(terminatingService.KeyFile);
                Assert.NotNull(terminatingService.SNI);

                gatewayServices = await client.Catalog.GatewayService(ingressGatewayName);
                Assert.NotEmpty(gatewayServices.Response);

                var ingressService = gatewayServices.Response[0];
                Assert.NotNull(ingressService.Gateway);
                Assert.Equal(ingressGatewayName, ingressService.Gateway.Name);
                Assert.NotNull(ingressService.Service);
                Assert.Equal(ServiceKind.IngressGateway, ingressService.GatewayKind);
                Assert.Equal(8888, ingressService.Port);
                Assert.NotNull(ingressService.Protocol);
            }
        }
    }
}
