// -----------------------------------------------------------------------
//  <copyright file="OperatorTest.cs" company="PlayFab Inc">
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
    public class OperatorTest : BaseFixture
    {
        [Fact]
        public async Task Operator_GetRaftGetConfiguration()
        {
            var servers = await _client.Operator.RaftGetConfiguration();

            Assert.Single(servers.Response.Servers);
            Assert.True(servers.Response.Servers[0].Leader);
            Assert.True(servers.Response.Servers[0].Voter);
        }

        [Fact]
        public async Task Operator_GetRaftRemovePeerByAddress()
        {
            try
            {
                await _client.Operator.RaftRemovePeerByAddress("nope");
            }
            catch (ConsulRequestException e)
            {
                Assert.Contains("address \"nope\" was not found in the Raft configuration", e.Message);
            }
        }

        [Fact]
        public async Task Operator_KeyringInstallListPutRemove()
        {
            const string oldKey = "d8wu8CSUrqgtjVsvcBPmhQ==";
            const string newKey = "qxycTi/SsePj/TZzCBmNXw==";

            await _client.Operator.KeyringInstall(oldKey);
            await _client.Operator.KeyringUse(oldKey);
            await _client.Operator.KeyringInstall(newKey);

            var listResponses = await _client.Operator.KeyringList();

            Assert.Equal(2, listResponses.Response.Length);

            foreach (var response in listResponses.Response)
            {
                Assert.Equal(2, response.Keys.Count);
                Assert.True(response.Keys.ContainsKey(oldKey));
                Assert.True(response.Keys.ContainsKey(newKey));
            }

            await _client.Operator.KeyringUse(newKey);

            await _client.Operator.KeyringRemove(oldKey);

            listResponses = await _client.Operator.KeyringList();
            Assert.Equal(2, listResponses.Response.Length);

            foreach (var response in listResponses.Response)
            {
                Assert.Equal(1, response.Keys.Count);
                Assert.False(response.Keys.ContainsKey(oldKey));
                Assert.True(response.Keys.ContainsKey(newKey));
            }
        }

        [Fact]
        public async Task Operator_AutopilotGetConfiguration_ReturnsConfiguration()
        {
            var result = await _client.Operator.AutopilotGetConfiguration();

            Assert.NotNull(result);
            Assert.NotNull(result.Response);

            var config = result.Response;

            Assert.NotEmpty(config.LastContactThreshold);
            Assert.NotEmpty(config.ServerStabilizationTime);
            Assert.True(config.MaxTrailingLogs >= 0);
            Assert.True(config.MinQuorum >= 0);
            Assert.True(config.CreateIndex >= 0);
            Assert.True(config.ModifyIndex >= 0);

            Assert.NotNull(config.RedundancyZoneTag);
            Assert.NotNull(config.UpgradeVersionTag);

            Assert.True(result.RequestTime > TimeSpan.Zero);
            Assert.True(result.LastIndex >= 0);
        }

        [Fact]
        public async Task Operator_AutopilotGetHealth_ReturnsHealthStatus()
        {
            var result = await _client.Operator.AutopilotGetHealth();

            Assert.NotNull(result);
            Assert.NotNull(result.Response);
            Assert.NotNull(result.Response.Servers);

            var health = result.Response;

            Assert.IsType<List<AutopilotServerHealth>>(health.Servers);
            Assert.True(health.FailureTolerance >= 0);

            if (health.Servers.Count > 0)
            {
                foreach (var server in health.Servers)
                {
                    Assert.False(string.IsNullOrEmpty(server.ID));
                    Assert.False(string.IsNullOrEmpty(server.Name));
                    Assert.False(string.IsNullOrEmpty(server.Address));
                    Assert.False(string.IsNullOrEmpty(server.Version));
                    Assert.True(server.LastTerm > 0);
                    Assert.True(server.LastIndex > 0);
                    Assert.NotEqual(DateTime.MinValue, server.StableSince);

                    if (health.Servers.Count == 1)
                    {
                        Assert.True(server.Leader);
                        Assert.True(server.Voter);
                    }
                }
            }
        }

        [SkippableFact]
        public async Task Operator_AutopilotGetState_ReturnsStateInformation()
        {
            var cutOffVersion = SemanticVersion.Parse("1.9.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Autopilot state` is only supported from Consul {cutOffVersion}");

            var result = await _client.Operator.AutopilotGetState();
            Assert.NotNull(result.Response);
            Assert.NotNull(result.Response.Servers);

            var state = result.Response;

            Assert.IsType<Dictionary<string, AutopilotServerState>>(state.Servers);
            Assert.True(state.FailureTolerance >= 0);
            Assert.True(state.OptimisticFailureTolerance >= 0);
            Assert.NotNull(state.Voters);
            Assert.NotNull(state.Servers);

            foreach (var kvp in state.Servers)
            {
                var serverId = kvp.Key;
                var server = kvp.Value;

                Assert.False(string.IsNullOrEmpty(serverId));
                Assert.NotNull(server);
                Assert.False(string.IsNullOrEmpty(server.ID));
                Assert.Equal(serverId, server.ID);
                Assert.False(string.IsNullOrEmpty(server.Name));
                Assert.False(string.IsNullOrEmpty(server.Address));
                Assert.False(string.IsNullOrEmpty(server.Version));
                Assert.False(string.IsNullOrEmpty(server.NodeStatus));
                Assert.False(string.IsNullOrEmpty(server.Status));
                Assert.False(string.IsNullOrEmpty(server.LastContact));
                Assert.NotEqual(DateTime.MinValue, server.StableSince);
                Assert.True(server.LastTerm >= 0);
                Assert.True(server.LastIndex >= 0);
                Assert.NotNull(server.Meta);
            }

            Assert.False(string.IsNullOrEmpty(state.Leader));
            Assert.Contains(state.Leader, state.Servers.Keys);

            var leaderServer = state.Servers[state.Leader];
            Assert.Equal("leader", leaderServer.Status);

            Assert.NotNull(state.Voters);
            Assert.All(state.Voters, voter => Assert.Contains(voter, state.Servers.Keys));
        }

        [Theory]
        [InlineData(true, "500ms", 100, 3, "30s", "az", false, "")]
        [InlineData(false, "1s", 200, 5, "59s", "zone-b", true, "version")]
        public async Task Operator_AutopilotSetConfiguration_UpdatesConfiguration(
            bool cleanupDeadServers,
            string lastContactThreshold,
            int maxTrailingLogs,
            int minQuorum,
            string serverStabilizationTime,
            string redundancyZoneTag,
            bool disableUpgradeMigration,
            string upgradeVersionTag)
        {
            var configuration = new AutopilotConfiguration
            {
                CleanupDeadServers = cleanupDeadServers,
                LastContactThreshold = lastContactThreshold,
                MaxTrailingLogs = maxTrailingLogs,
                MinQuorum = minQuorum,
                ServerStabilizationTime = serverStabilizationTime,
                RedundancyZoneTag = redundancyZoneTag,
                DisableUpgradeMigration = disableUpgradeMigration,
                UpgradeVersionTag = upgradeVersionTag
            };

            var result = await _client.Operator.AutopilotSetConfiguration(configuration);

            Assert.NotNull(result);
            Assert.True(result.RequestTime > TimeSpan.Zero);

            var getResult = await _client.Operator.AutopilotGetConfiguration();
            Assert.NotNull(getResult);
            Assert.NotNull(getResult.Response);

            var retrievedConfig = getResult.Response;
            Assert.Equal(configuration.CleanupDeadServers, retrievedConfig.CleanupDeadServers);
            Assert.Equal(configuration.LastContactThreshold, retrievedConfig.LastContactThreshold);
            Assert.Equal(configuration.MaxTrailingLogs, retrievedConfig.MaxTrailingLogs);
            Assert.Equal(configuration.MinQuorum, retrievedConfig.MinQuorum);
            Assert.Equal(configuration.ServerStabilizationTime, retrievedConfig.ServerStabilizationTime);
            Assert.Equal(configuration.RedundancyZoneTag, retrievedConfig.RedundancyZoneTag);
            Assert.Equal(configuration.DisableUpgradeMigration, retrievedConfig.DisableUpgradeMigration);
            Assert.Equal(configuration.UpgradeVersionTag, retrievedConfig.UpgradeVersionTag);

            Assert.True(retrievedConfig.CreateIndex > 0);
            Assert.True(retrievedConfig.ModifyIndex > 0);
            Assert.True(retrievedConfig.ModifyIndex >= retrievedConfig.CreateIndex);
        }
        
        [EnterpriseOnlyFact]
        public async Task Operator_GetLicense()
        {
            var queryResult = await _client.Operator.GetConsulLicense();
            Assert.NotNull(queryResult.Response);
        }

        [EnterpriseOnlyFact]
        public async Task Segment_List()
        {
            var segments = await _client.Operator.SegmentList();
            Assert.NotEmpty(segments.Response);
        }

        [EnterpriseOnlyFact]
        public async Task Operator_AreaCreate()
        {
            var peerDataCenter = KVTest.GenerateTestKeyName();
            var check = new AreaRequest { PeerDatacenter = peerDataCenter, UseTLS = false, RetryJoin = null };

            var response = await _client.Operator.AreaCreate(check);
            Assert.NotNull(response.Response);
        }
        [EnterpriseOnlyFact]
        public async Task Operator_AreaList()
        {
            var peerDataCenter = KVTest.GenerateTestKeyName();
            var area = new AreaRequest { PeerDatacenter = peerDataCenter, UseTLS = false, RetryJoin = new string[] { "10.1.2.3", "10.1.2.4" } };

            await _client.Operator.AreaCreate(area);

            var req = await _client.Operator.AreaList();
            var result = req.Response.Single(x => x.PeerDatacenter == area.PeerDatacenter);

            Assert.Equal(area.UseTLS, result.UseTLS);
            Assert.Equal(area.RetryJoin, result.RetryJoin);
        }
        [EnterpriseOnlyFact]
        public async Task Operator_AreaUpdate()
        {
            var peerDataCenter = KVTest.GenerateTestKeyName();
            var area = new AreaRequest { PeerDatacenter = peerDataCenter, UseTLS = false, RetryJoin = null };
            var createResult = await _client.Operator.AreaCreate(area);
            var areaId = createResult.Response;

            area = new AreaRequest { PeerDatacenter = peerDataCenter, UseTLS = true, RetryJoin = new string[] { "10.1.2.9", "10.1.2.0" } };
            var updateResult = await _client.Operator.AreaUpdate(area, areaId);

            var listResult = await _client.Operator.AreaList();
            var updatedArea = listResult.Response.Single(x => x.ID == areaId);

            Assert.Equal(areaId, updateResult.Response);
            Assert.Equal(area.UseTLS, updatedArea.UseTLS);
            Assert.Equal(area.RetryJoin, updatedArea.RetryJoin);
            Assert.Equal(area.PeerDatacenter, updatedArea.PeerDatacenter);
        }
        [EnterpriseOnlyFact]
        public async Task Operator_AreaGet()
        {
            var peerDataCenter = KVTest.GenerateTestKeyName();
            var area = new AreaRequest { PeerDatacenter = peerDataCenter, UseTLS = true, RetryJoin = new string[] { "10.1.2.7", "10.1.2.0" } };
            var createResult = await _client.Operator.AreaCreate(area);
            var areaId = createResult.Response;

            var req = await _client.Operator.AreaGet(areaId);
            var result = req.Response.Single(x => x.ID == areaId);

            Assert.Equal(areaId, result.ID);
            Assert.Equal(area.UseTLS, result.UseTLS);
            Assert.Equal(area.RetryJoin, result.RetryJoin);
            Assert.Equal(area.PeerDatacenter, result.PeerDatacenter);
        }

        [EnterpriseOnlyFact]
        public async Task Operetor_AreaDelete()
        {
            var peerDataCenter = KVTest.GenerateTestKeyName();
            var area = new AreaRequest { PeerDatacenter = peerDataCenter, UseTLS = true, RetryJoin = new string[] { "10.1.2.7", "10.1.2.0" } };
            var createResult = await _client.Operator.AreaCreate(area);
            var areaId = createResult.Response;

            await _client.Operator.AreaDelete(areaId);

            var req = await _client.Operator.AreaGet(areaId);

            Assert.Null(req.Response);
        }
    }
}
