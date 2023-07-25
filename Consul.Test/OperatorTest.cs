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

        [EnterpriseOnlyFact]
        public async Task Operator_GetLicense()
        {
            var queryResult = await _client.Operator.GetConsulLicense();
            Assert.NotNull(queryResult.Response);
        }
    }
}
