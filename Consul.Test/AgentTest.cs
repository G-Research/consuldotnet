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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class AgentTest : IDisposable
    {
        private ConsulClient _client;

        public AgentTest()
        {
            _client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            });
        }

        public void Dispose()
        {
            _client.Dispose();
        }

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
    }
}
