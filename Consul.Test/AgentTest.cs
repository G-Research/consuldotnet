// -----------------------------------------------------------------------
//  <copyright file="AgentTest.cs" company="PlayFab Inc">
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
using Consul.Filtering;
using Xunit;
using Xunit.Abstractions;

namespace Consul.Test
{
    public class AgentTest : BaseFixture
    {
        [Fact]
        public async Task Agent_GetSelf()
        {
            var info = await _client.Agent.Self();

            Assert.NotNull(info);
            Assert.False(string.IsNullOrEmpty(info.Response["Config"]["NodeName"]));
            Assert.False(string.IsNullOrEmpty(info.Response["Member"]["Tags"]["bootstrap"].ToString()));
        }

        [Fact]
        public async Task Agent_GetMembers()
        {
            var members = await _client.Agent.Members(false);

            Assert.NotNull(members);
            Assert.Single(members.Response);
        }

        [Fact]
        public async Task Agent_GetServices()
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

            await _client.Agent.ServiceRegister(registration);

            var services = await _client.Agent.Services();
            Assert.True(services.Response.ContainsKey(svcID));

            var checks = await _client.Agent.Checks();
            Assert.True(checks.Response.ContainsKey("service:" + svcID));

            Assert.Equal(HealthStatus.Critical, checks.Response["service:" + svcID].Status);

            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Agent_Services_CheckPassing()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentServiceRegistration
            {
                Name = svcID,
                Tags = new[] { "bar", "baz" },
                Port = 8000,
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(15),
                    Status = HealthStatus.Passing
                }
            };

            await _client.Agent.ServiceRegister(registration);

            var services = await _client.Agent.Services();
            Assert.True(services.Response.ContainsKey(svcID));

            var checks = await _client.Agent.Checks();
            Assert.True(checks.Response.ContainsKey("service:" + svcID));

            Assert.Equal(HealthStatus.Passing, checks.Response["service:" + svcID].Status);

            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Agent_Services_CheckTTLNote()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentServiceRegistration
            {
                Name = svcID,
                Tags = new[] { "bar", "baz" },
                Port = 8000,
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(15),
                    Status = HealthStatus.Critical
                }
            };

            await _client.Agent.ServiceRegister(registration);

            var services = await _client.Agent.Services();
            Assert.True(services.Response.ContainsKey(svcID));

            var checks = await _client.Agent.Checks();
            Assert.True(checks.Response.ContainsKey("service:" + svcID));

            Assert.Equal(HealthStatus.Critical, checks.Response["service:" + svcID].Status);

            await _client.Agent.PassTTL("service:" + svcID, "test is ok");
            checks = await _client.Agent.Checks();

            Assert.True(checks.Response.ContainsKey("service:" + svcID));
            Assert.Equal(HealthStatus.Passing, checks.Response["service:" + svcID].Status);
            Assert.Equal("test is ok", checks.Response["service:" + svcID].Output);

            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Agent_ServiceAddress()
        {
            var svcID1 = KVTest.GenerateTestKeyName();
            var svcID2 = KVTest.GenerateTestKeyName();
            var registration1 = new AgentServiceRegistration
            {
                Name = svcID1,
                Port = 8000,
                Address = "192.168.0.42"
            };
            var registration2 = new AgentServiceRegistration
            {
                Name = svcID2,
                Port = 8000
            };

            await _client.Agent.ServiceRegister(registration1);
            await _client.Agent.ServiceRegister(registration2);

            var services = await _client.Agent.Services();
            Assert.True(services.Response.ContainsKey(svcID1));
            Assert.True(services.Response.ContainsKey(svcID2));
            Assert.Equal("192.168.0.42", services.Response[svcID1].Address);
            Assert.True(string.IsNullOrEmpty(services.Response[svcID2].Address));

            await _client.Agent.ServiceDeregister(svcID1);
            await _client.Agent.ServiceDeregister(svcID2);
        }

        [Fact]
        public async Task Agent_ServiceTaggedAddresses()
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

            var services = await _client.Agent.Services();
            Assert.True(services.Response.ContainsKey(svcID));
            Assert.True(services.Response[svcID].TaggedAddresses.Count > 0);
            Assert.True(services.Response[svcID].TaggedAddresses.ContainsKey("wan"));
            Assert.True(services.Response[svcID].TaggedAddresses.ContainsKey("lan"));


            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Agent_EnableTagOverride()
        {
            var svcID1 = KVTest.GenerateTestKeyName();
            var svcID2 = KVTest.GenerateTestKeyName();
            var reg1 = new AgentServiceRegistration
            {
                Name = svcID1,
                Port = 8000,
                Address = "192.168.0.42",
                EnableTagOverride = true
            };

            var reg2 = new AgentServiceRegistration
            {
                Name = svcID2,
                Port = 8000
            };

            await _client.Agent.ServiceRegister(reg1);
            await _client.Agent.ServiceRegister(reg2);

            var services = await _client.Agent.Services();

            Assert.Contains(svcID1, services.Response.Keys);
            Assert.True(services.Response[svcID1].EnableTagOverride);

            Assert.Contains(svcID2, services.Response.Keys);
            Assert.False(services.Response[svcID2].EnableTagOverride);
        }

        [Fact]
        public async Task Agent_Services_MultipleChecks()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentServiceRegistration
            {
                Name = svcID,
                Tags = new[] { "bar", "baz" },
                Port = 8000,
                Checks = new[]
                {
                    new AgentServiceCheck
                    {
                        TTL = TimeSpan.FromSeconds(15)
                    },
                    new AgentServiceCheck
                    {
                        TTL = TimeSpan.FromSeconds(15)
                    }
                }
            };
            await _client.Agent.ServiceRegister(registration);

            var services = await _client.Agent.Services();
            Assert.True(services.Response.ContainsKey(svcID));

            var checks = await _client.Agent.Checks();
            Assert.True(checks.Response.ContainsKey("service:" + svcID + ":1"));
            Assert.True(checks.Response.ContainsKey("service:" + svcID + ":2"));

            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Agent_SetTTLStatus()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentServiceRegistration
            {
                Name = svcID,
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(15)
                }
            };
            await _client.Agent.ServiceRegister(registration);

            await _client.Agent.WarnTTL("service:" + svcID, "warning");
            var checks = await _client.Agent.Checks();
            Assert.Contains("service:" + svcID, checks.Response.Keys);
            Assert.Equal(HealthStatus.Warning, checks.Response["service:" + svcID].Status);
            Assert.Equal("warning", checks.Response["service:" + svcID].Output);

            await _client.Agent.PassTTL("service:" + svcID, "passing");
            checks = await _client.Agent.Checks();
            Assert.Contains("service:" + svcID, checks.Response.Keys);
            Assert.Equal(HealthStatus.Passing, checks.Response["service:" + svcID].Status);
            Assert.Equal("passing", checks.Response["service:" + svcID].Output);

            await _client.Agent.FailTTL("service:" + svcID, "failing");
            checks = await _client.Agent.Checks();
            Assert.Contains("service:" + svcID, checks.Response.Keys);
            Assert.Equal(HealthStatus.Critical, checks.Response["service:" + svcID].Status);
            Assert.Equal("failing", checks.Response["service:" + svcID].Output);

            await _client.Agent.UpdateTTL("service:" + svcID, svcID, TTLStatus.Pass);
            checks = await _client.Agent.Checks();
            Assert.Contains("service:" + svcID, checks.Response.Keys);
            Assert.Equal(HealthStatus.Passing, checks.Response["service:" + svcID].Status);
            Assert.Equal(svcID, checks.Response["service:" + svcID].Output);

            await _client.Agent.UpdateTTL("service:" + svcID, "foo warning", TTLStatus.Warn);
            checks = await _client.Agent.Checks();
            Assert.Contains("service:" + svcID, checks.Response.Keys);
            Assert.Equal(HealthStatus.Warning, checks.Response["service:" + svcID].Status);
            Assert.Equal("foo warning", checks.Response["service:" + svcID].Output);

            await _client.Agent.UpdateTTL("service:" + svcID, "foo failing", TTLStatus.Critical);
            checks = await _client.Agent.Checks();
            Assert.Contains("service:" + svcID, checks.Response.Keys);
            Assert.Equal(HealthStatus.Critical, checks.Response["service:" + svcID].Status);
            Assert.Equal("foo failing", checks.Response["service:" + svcID].Output);

            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Agent_Checks()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentCheckRegistration
            {
                Name = svcID,
                TTL = TimeSpan.FromSeconds(15)
            };
            await _client.Agent.CheckRegister(registration);

            var checks = await _client.Agent.Checks();
            Assert.True(checks.Response.ContainsKey(svcID));
            Assert.Equal(HealthStatus.Critical, checks.Response[svcID].Status);

            await _client.Agent.CheckDeregister(svcID);
        }

        [Fact]
        public async Task Agent_Checks_Docker()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var serviceReg = new AgentServiceRegistration
            {
                Name = svcID
            };
            await _client.Agent.ServiceRegister(serviceReg);

            var reg = new AgentCheckRegistration
            {
                Name = "redischeck",
                ServiceID = svcID,
                DockerContainerID = "f972c95ebf0e",
                Args = new string[] { "/bin/true" },
                Shell = "/bin/bash",
                Interval = TimeSpan.FromSeconds(10)
            };
            await _client.Agent.CheckRegister(reg);

            var checks = await _client.Agent.Checks();
            Assert.True(checks.Response.ContainsKey("redischeck"));
            Assert.Equal(svcID, checks.Response["redischeck"].ServiceID);

            await _client.Agent.CheckDeregister("redischeck");
            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Agent_CheckStartPassing()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentCheckRegistration
            {
                Name = svcID,
                Status = HealthStatus.Passing,
                TTL = TimeSpan.FromSeconds(15)
            };
            await _client.Agent.CheckRegister(registration);

            var checks = await _client.Agent.Checks();
            Assert.True(checks.Response.ContainsKey(svcID));
            Assert.Equal(HealthStatus.Passing, checks.Response[svcID].Status);

            await _client.Agent.CheckDeregister(svcID);
        }

        [Fact]
        public async Task Agent_Checks_ServiceBound()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var serviceReg = new AgentServiceRegistration
            {
                Name = svcID
            };
            await _client.Agent.ServiceRegister(serviceReg);

            var reg = new AgentCheckRegistration
            {
                Name = "redischeck",
                ServiceID = svcID,
                TTL = TimeSpan.FromSeconds(15),
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(90)
            };
            await _client.Agent.CheckRegister(reg);

            var checks = await _client.Agent.Checks();
            Assert.True(checks.Response.ContainsKey("redischeck"));
            Assert.Equal(svcID, checks.Response["redischeck"].ServiceID);

            await _client.Agent.CheckDeregister("redischeck");
            await _client.Agent.ServiceDeregister(svcID);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Agent_UseCache(bool useCache)
        {

            var opts = new QueryOptions
            {
                UseCache = useCache,
                MaxAge = TimeSpan.FromSeconds(10),
                StaleIfError = TimeSpan.FromSeconds(10),
            };

            var response = await _client.Catalog.Datacenters(opts);

            if (useCache)
            {
                Assert.True(response.Headers.TryGetValues("X-Cache", out var cacheResult));
                Assert.Contains(cacheResult.Single(), new[] { "MISS", "HIT" });
            }
            else
            {
                Assert.False(response.Headers.TryGetValues("X-Cache", out _));
            }
        }

        [Fact]
        public async Task Agent_Join()
        {
            var info = await _client.Agent.Self();
            await _client.Agent.Join(info.Response["DebugConfig"]["AdvertiseAddrLAN"], false);
            // Success is not throwing an exception
        }

        [Fact]
        public async Task Agent_ForceLeave()
        {
            var info = await _client.Agent.Self();
            await _client.Agent.ForceLeave(info.Response["Config"]["NodeName"]);
            // Success is not throwing an exception
        }

        [Fact]
        public async Task Agent_ServiceMaintenance()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var serviceReg = new AgentServiceRegistration
            {
                Name = svcID
            };
            await _client.Agent.ServiceRegister(serviceReg);

            await _client.Agent.EnableServiceMaintenance(svcID, "broken");

            var checks = await _client.Agent.Checks();
            var found = false;
            foreach (var check in checks.Response)
            {
                if (check.Value.CheckID.Contains("maintenance"))
                {
                    found = true;
                    Assert.Equal(HealthStatus.Critical, check.Value.Status);
                    Assert.Equal("broken", check.Value.Notes);
                }
            }
            Assert.True(found);

            await _client.Agent.DisableServiceMaintenance(svcID);

            checks = await _client.Agent.Checks();
            foreach (var check in checks.Response)
            {
                Assert.DoesNotContain("maintenance", check.Value.CheckID);
            }

            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Agent_NodeMaintenance()
        {
            await _client.Agent.EnableNodeMaintenance("broken");
            var checks = await _client.Agent.Checks();

            var found = false;
            foreach (var check in checks.Response)
            {
                if (check.Value.CheckID.Contains("maintenance"))
                {
                    found = true;
                    Assert.Equal(HealthStatus.Critical, check.Value.Status);
                    Assert.Equal("broken", check.Value.Notes);
                }
            }
            Assert.True(found);

            await _client.Agent.DisableNodeMaintenance();

            checks = await _client.Agent.Checks();
            foreach (var check in checks.Response)
            {
                Assert.DoesNotContain("maintenance", check.Value.CheckID);
            }
        }

        [Fact]
        public async Task Agent_Monitor()
        {
            using (var logs = await _client.Agent.Monitor(LogLevel.Trace))
            {
                var counter = 0;
                var logsTask = Task.Run(async () =>
                {
                    // to get some logs
                    await _client.Agent.Self();

                    foreach (var line in logs)
                    {
                        // Make a request each time so we get more logs
                        await _client.Agent.Self();
                        Assert.False(string.IsNullOrEmpty(await line));
                        counter++;
                        if (counter > 5)
                        {
                            break;
                        }
                    }
                });

                await TimeoutUtils.WithTimeout(logsTask);
            }
        }

        [Fact]
        public async Task Agent_FilterServices()
        {
            var svcID1 = KVTest.GenerateTestKeyName();
            var svcID2 = KVTest.GenerateTestKeyName();
            var uniqueMeta = KVTest.GenerateTestKeyName();

            var reg1 = new AgentServiceRegistration
            {
                Name = svcID1,
                Meta = new Dictionary<string, string> { { uniqueMeta, "bar1" } },
            };

            var reg2 = new AgentServiceRegistration
            {
                Name = svcID2,
                Meta = new Dictionary<string, string> { { uniqueMeta, "bar2" } },
            };

            await _client.Agent.ServiceRegister(reg1);
            await _client.Agent.ServiceRegister(reg2);

            var idSelector = new StringFieldSelector("ID");
            var metaSelector = new MetaSelector();

            Assert.Equal(svcID1, (await _client.Agent.Services(idSelector == svcID1)).Response.Keys.Single());
            Assert.Equal(svcID2, (await _client.Agent.Services(idSelector == svcID2)).Response.Keys.Single());
            Assert.Equal(svcID1, (await _client.Agent.Services(metaSelector[uniqueMeta] == "bar1")).Response.Keys.Single());
            Assert.Equal(svcID2, (await _client.Agent.Services(metaSelector[uniqueMeta] == "bar2")).Response.Keys.Single());
            Assert.Equal(svcID1, (await _client.Agent.Services(Selectors.Service == svcID1)).Response.Keys.Single());
            Assert.Equal(svcID2, (await _client.Agent.Services(Selectors.Service == svcID2)).Response.Keys.Single());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Agent_ReRegister_ReplaceExistingChecks(bool replaceExistingChecks)
        {
            var svcID = KVTest.GenerateTestKeyName();
            var check1Name = svcID + "1";
            var check2Name = svcID + "2";
            var check3Name = svcID + "3";
            var registration1 = new AgentServiceRegistration
            {
                Name = svcID,
                Port = 8000,
                Checks = new[]
                {
                    new AgentServiceCheck
                    {
                        Name = check1Name,
                        TTL = TimeSpan.FromSeconds(15)
                    },
                    new AgentServiceCheck
                    {
                        Name = check2Name,
                        TTL = TimeSpan.FromSeconds(15)
                    }
                }
            };
            var registration2 = new AgentServiceRegistration
            {
                Name = svcID,
                Port = 8000,
                Check = new AgentServiceCheck
                {
                    Name = check3Name,
                    TTL = TimeSpan.FromSeconds(15)
                }
            };

            await _client.Agent.ServiceRegister(registration1);
            await _client.Agent.ServiceRegister(registration2, replaceExistingChecks: replaceExistingChecks);

            var checks = await _client.Agent.Checks();

            if (replaceExistingChecks)
            {
                Assert.DoesNotContain(check1Name, checks.Response.Values.Select(c => c.Name));
                Assert.DoesNotContain(check2Name, checks.Response.Values.Select(c => c.Name));
            }
            else
            {
                Assert.Contains(check1Name, checks.Response.Values.Select(c => c.Name));
                Assert.Contains(check2Name, checks.Response.Values.Select(c => c.Name));
            }
            Assert.Contains(check3Name, checks.Response.Values.Select(c => c.Name));

            await _client.Agent.ServiceDeregister(svcID);
        }

        [Fact]
        public async Task Agent_Register_UseCustomCheckId()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var check1Id = svcID + "_checkId";
            var check1Name = svcID + "_checkName";
            var registration1 = new AgentServiceRegistration
            {
                Name = svcID,
                Port = 8000,
                Checks = new[]
                {
                    new AgentServiceCheck
                    {
                        Name = check1Name,
                        CheckID = check1Id,
                        TTL = TimeSpan.FromSeconds(15),
                    },
                }
            };

            await _client.Agent.ServiceRegister(registration1);

            var checks = await _client.Agent.Checks();
            Assert.Contains(check1Id, checks.Response.Keys);

            var check = checks.Response[check1Id];
            Assert.Equal(check.Name, check1Name);
            Assert.Equal(check.CheckID, check1Id);
        }

        [Fact]
        public async Task Agent_Register_UseAliasCheck()
        {
            // 120 is a lot, but otherwise the test is flaky
            var ttl = TimeSpan.FromSeconds(120);
            var svcID = KVTest.GenerateTestKeyName();
            var svcID1 = svcID + "1";
            var svcID2 = svcID + "2";
            var check1Id = svcID1 + "_checkId";
            var check1Name = svcID1 + "_checkName";
            var registration1 = new AgentServiceRegistration
            {
                ID = svcID1,
                Name = svcID1,
                Port = 8000,
                Checks = new[]
                {
                    new AgentServiceCheck
                    {
                        Name = check1Name,
                        CheckID = check1Id,
                        TTL = ttl,
                        Status = HealthStatus.Critical,
                    },
                },
            };

            var check2Id = svcID2 + "_checkId";
            var check2Name = svcID2 + "_checkName";
            var registration2 = new AgentServiceRegistration
            {
                ID = svcID2,
                Name = svcID2,
                Port = 8000,
                Checks = new[]
                {
                    new AgentServiceCheck
                    {
                        Name = check2Name,
                        CheckID = check2Id,
                        AliasService = svcID1,
                        Status = HealthStatus.Critical,
                    },
                },
            };

            await _client.Agent.ServiceRegister(registration1);
            await _client.Agent.ServiceRegister(registration2);

            var checks = await _client.Agent.Checks();
            Assert.Equal(HealthStatus.Critical, checks.Response[check1Id].Status);
            Assert.NotEqual("test is ok", checks.Response[check1Id].Output);
            Assert.Equal(HealthStatus.Critical, checks.Response[check2Id].Status);
            Assert.NotEqual("test is ok", checks.Response[check2Id].Output);

            await _client.Agent.PassTTL(check1Id, "test is ok");

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                checks = await _client.Agent.Checks();

                if (checks.Response[check1Id].Status != HealthStatus.Passing ||
                    checks.Response[check2Id].Status == HealthStatus.Passing)
                {
                    break;
                }
            }

            Assert.Equal(HealthStatus.Passing, checks.Response[check1Id].Status);
            Assert.Equal("test is ok", checks.Response[check1Id].Output);
            Assert.Equal(HealthStatus.Passing, checks.Response[check2Id].Status);
            Assert.Equal("All checks passing.", checks.Response[check2Id].Output);
        }

        [Fact]
        public async Task Agent_Service_Register_With_Connect()
        {
            // Arrange
            var destinationServiceID = KVTest.GenerateTestKeyName();
            var destinationServiceRegistrationParameters = new AgentServiceRegistration
            {
                ID = destinationServiceID,
                Name = destinationServiceID,
                Port = 8000,
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(15)
                },
                Connect = new AgentServiceConnect
                {
                    SidecarService = new AgentServiceRegistration
                    {
                        Port = 8001
                    }
                }
            };

            var sourceServiceID = KVTest.GenerateTestKeyName();
            var sourceServiceRegistrationParameters = new AgentServiceRegistration
            {
                ID = sourceServiceID,
                Name = sourceServiceID,
                Port = 9000,
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(15)
                },
                Tags = new string[] { "tag1", "tag2" },
                Connect = new AgentServiceConnect
                {
                    SidecarService = new AgentServiceRegistration
                    {
                        Port = 9001,
                        Proxy = new AgentServiceProxy
                        {
                            Upstreams = new AgentServiceProxyUpstream[] { new AgentServiceProxyUpstream { DestinationName = destinationServiceID, LocalBindPort = 9002 } }
                        }
                    }
                }
            };

            // Act
            await _client.Agent.ServiceRegister(destinationServiceRegistrationParameters);
            await _client.Agent.ServiceRegister(sourceServiceRegistrationParameters);

            // Assert
            var services = await _client.Agent.Services();

            // Assert SourceService
            var sourceProxyServiceID = $"{sourceServiceID}-sidecar-proxy";
            Assert.Contains(sourceServiceID, services.Response.Keys);
            Assert.Contains(sourceProxyServiceID, services.Response.Keys);
            AgentService sourceProxyService = services.Response[sourceProxyServiceID];
            Assert.Equal(sourceServiceRegistrationParameters.Tags, sourceProxyService.Tags);
            Assert.Equal(sourceServiceRegistrationParameters.Connect.SidecarService.Port, sourceProxyService.Port);
            Assert.Equal(sourceServiceID, sourceProxyService.Proxy.DestinationServiceName);
            Assert.Equal(sourceServiceID, sourceProxyService.Proxy.DestinationServiceID);
            Assert.Equal("127.0.0.1", sourceProxyService.Proxy.LocalServiceAddress);
            Assert.Equal(sourceServiceRegistrationParameters.Port, sourceProxyService.Proxy.LocalServicePort);
            Assert.Equal(ServiceKind.ConnectProxy, sourceProxyService.Kind);
            Assert.Single(sourceProxyService.Proxy.Upstreams);
            Assert.Equal(sourceServiceRegistrationParameters.Connect.SidecarService.Proxy.Upstreams[0].DestinationName, sourceProxyService.Proxy.Upstreams[0].DestinationName);
            Assert.Equal(sourceServiceRegistrationParameters.Connect.SidecarService.Proxy.Upstreams[0].LocalBindPort, sourceProxyService.Proxy.Upstreams[0].LocalBindPort);

            // Assert DestinationService
            var destinationProxyServiceID = $"{destinationServiceID}-sidecar-proxy";
            Assert.Contains(destinationServiceID, services.Response.Keys);
            Assert.Contains(destinationProxyServiceID, services.Response.Keys);
            AgentService destinationProxyService = services.Response[destinationProxyServiceID];
            Assert.Equal(destinationServiceRegistrationParameters.Connect.SidecarService.Port, destinationProxyService.Port);
            Assert.Equal(destinationServiceID, destinationProxyService.Proxy.DestinationServiceName);
            Assert.Equal(destinationServiceID, destinationProxyService.Proxy.DestinationServiceID);
            Assert.Equal("127.0.0.1", destinationProxyService.Proxy.LocalServiceAddress);
            Assert.Equal(destinationServiceRegistrationParameters.Port, destinationProxyService.Proxy.LocalServicePort);
            Assert.Null(destinationProxyService.Proxy.Upstreams);
            Assert.Equal(ServiceKind.ConnectProxy, destinationProxyService.Kind);
        }

        [Fact]
        public async Task Agent_FilterChecks()
        {
            // Arrange
            string svcName = KVTest.GenerateTestKeyName();
            string svcID = $"{svcName}-1";
            string checkName = $"Check {svcID}";

            await _client.Agent.ServiceRegister(new AgentServiceRegistration
            {
                Name = svcName,
                ID = svcID,
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(15),
                    Name = checkName
                }
            });

            // mass service
            await _client.Agent.ServiceRegister(new AgentServiceRegistration
            {
                Name = KVTest.GenerateTestKeyName(),
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(15),
                }
            });

            // Act
            Dictionary<string, AgentCheck> actual = (await _client.Agent.Checks(Selectors.ServiceName == svcName)).Response;

            // Assert
            Assert.Single(actual);
            Assert.Equal(checkName, actual.Values.First().Name);
        }
    }
}
