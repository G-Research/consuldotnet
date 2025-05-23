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
using System.Net;
using System.Threading.Tasks;
using NuGet.Versioning;
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

        [SkippableFact]
        public async Task Policy_PreviewATemplatedPolicyByName()
        {
            var cutOffVersion = SemanticVersion.Parse("1.17.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Templated Policies` are only supported from Consul {cutOffVersion}");

            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var templatedPolicyName = "builtin/dns";
            var templatedPolicy = await _client.Policy.PreviewTemplatedPolicy(templatedPolicyName);

            Assert.Equal(HttpStatusCode.OK, templatedPolicy.StatusCode);
            Assert.NotNull(templatedPolicy.Response);
            Assert.NotNull(templatedPolicy.Response.ID);
            Assert.True(!string.IsNullOrEmpty(templatedPolicy.Response.Name));
            Assert.True(!string.IsNullOrEmpty(templatedPolicy.Response.Rules));
            Assert.True(!string.IsNullOrEmpty(templatedPolicy.Response.Description));
        }

        [SkippableFact]
        public async Task Policy_ListTemplatedPolicies()
        {
            var cutOffVersion = SemanticVersion.Parse("1.17.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Templated Policies` are only supported from Consul {cutOffVersion}");

            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var templatedPolicyList = await _client.Policy.ListTemplatedPolicies();

            Assert.Equal(HttpStatusCode.OK, templatedPolicyList.StatusCode);
            Assert.NotNull(templatedPolicyList.Response);
            Assert.True(templatedPolicyList.Response.Count > 0);
        }

        [SkippableFact]
        public async Task Policy_ReadTemplatedPolicyByName()
        {
            var cutOffVersion = SemanticVersion.Parse("1.17.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Templated Policies` are only supported from Consul {cutOffVersion}");

            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var templatedPolicyName = "builtin/api-gateway";
            var templatedPolicy = await _client.Policy.ReadTemplatedPolicyByName(templatedPolicyName);

            Assert.Equal(HttpStatusCode.OK, templatedPolicy.StatusCode);
            Assert.NotNull(templatedPolicy.Response);
            Assert.Equal(templatedPolicyName, templatedPolicy.Response.TemplateName);
            Assert.True(!string.IsNullOrEmpty(templatedPolicy.Response.Template));
            Assert.True(!string.IsNullOrEmpty(templatedPolicy.Response.Schema));
        }

        [SkippableFact]
        public async Task Policy_ReadPolicyByName()
        {
            var cutOffVersion = SemanticVersion.Parse("1.7.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `ACL Policies` are only supported from Consul {cutOffVersion}");

            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var policyEntry = await _client.Policy.ReadPolicyByName("global-management");

            Assert.NotNull(policyEntry.Response);
            Assert.NotEqual(TimeSpan.Zero, policyEntry.RequestTime);
            Assert.Equal("00000000-0000-0000-0000-000000000001", policyEntry.Response.ID);
        }
    }
}
