// -----------------------------------------------------------------------
//  <copyright file="PolicyTest.cs" company="G-Research Limited">
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
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class PolicyTest : BaseFixture
    {
        [SkippableFact]
        public async Task Policy_CreateDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var policyEntry = new PolicyEntry()
            {
                Name = "UnitTestPolicy",
                Description = "Policy for API Unit Testing",
                Rules = "key \"\" { policy = \"deny\" }"
            };

            var newPolicyResult = await _client.Policy.Create(policyEntry);
            Assert.NotNull(newPolicyResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newPolicyResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newPolicyResult.Response.ID));
            Assert.Equal(policyEntry.Description, newPolicyResult.Response.Description);
            Assert.Equal(policyEntry.Name, newPolicyResult.Response.Name);
            Assert.Equal(policyEntry.Rules, newPolicyResult.Response.Rules);

            var deleteResponse = await _client.Policy.Delete(newPolicyResult.Response.ID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Policy_Read()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var tokenEntry = await _client.Token.Read("self");
            var policyEntry = await _client.Policy.Read(tokenEntry.Response.Policies[0].ID);

            Assert.NotNull(policyEntry.Response);
            Assert.NotEqual(TimeSpan.Zero, policyEntry.RequestTime);
            Assert.Equal("00000000-0000-0000-0000-000000000001", policyEntry.Response.ID);
            Assert.Equal("global-management", policyEntry.Response.Name);
        }

        [SkippableFact]
        public async Task Policy_List()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var aclPolicyList = await _client.Policy.List();

            Assert.NotNull(aclPolicyList.Response);
            Assert.NotEqual(TimeSpan.Zero, aclPolicyList.RequestTime);
            Assert.True(aclPolicyList.Response.Length >= 1);
        }
    }
}
