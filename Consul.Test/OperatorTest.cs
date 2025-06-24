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
            Assert.True(config.CreateIndex >= 0);
            Assert.True(config.ModifyIndex >= 0);

            Assert.NotNull(config.RedundancyZoneTag);
            Assert.NotNull(config.UpgradeVersionTag);

            Assert.True(result.RequestTime > TimeSpan.Zero);
            Assert.True(result.LastIndex >= 0);
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
