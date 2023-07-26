// -----------------------------------------------------------------------
//  <copyright file="ACLTest.cs" company="PlayFab Inc">
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
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class ACLTest : BaseFixture
    {
        void SkipIfAclNotSupported()
        {
            var cutOffVersion = SemanticVersion.Parse("1.11.0");
            Skip.If(AgentVersion >= cutOffVersion, $"Current version is {AgentVersion}, but the legacy ACL system was removed in Consul {cutOffVersion}");
        }

        [SkippableFact]
        public async Task ACL_CreateDestroyLegacyToken()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            SkipIfAclNotSupported();

#pragma warning disable CS0618 // Type or member is obsolete
            var aclEntry = new ACLEntry
            {
                Name = "API Test",
                Type = ACLType.Client,
                Rules = "key \"\" { policy = \"deny\" }"
            };
            var res = await _client.ACL.Create(aclEntry);
            var id = res.Response;

            Assert.NotEqual(TimeSpan.Zero, res.RequestTime);
            Assert.False(string.IsNullOrEmpty(res.Response));

            var aclEntry2 = await _client.ACL.Info(id);

            Assert.NotNull(aclEntry2.Response);
            Assert.Equal(aclEntry2.Response.Name, aclEntry.Name);
            Assert.Equal(aclEntry2.Response.Type, aclEntry.Type);
            Assert.Equal(aclEntry2.Response.Rules, aclEntry.Rules);

            var destroyResponse = await _client.ACL.Destroy(id);
            Assert.True(destroyResponse.Response);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [SkippableFact]
        public async Task ACL_CreateCloneUpdateDestroyLegacyToken()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            SkipIfAclNotSupported();

#pragma warning disable CS0618 // Type or member is obsolete
            var newAclEntry = new ACLEntry
            {
                Name = "API Test",
                Type = ACLType.Client,
                Rules = "key \"\" { policy = \"deny\" }"
            };

            var res = await _client.ACL.Create(newAclEntry);
            var newAclId = res.Response;

            Assert.NotEqual(TimeSpan.Zero, res.RequestTime);
            Assert.False(string.IsNullOrEmpty(res.Response));

            var cloneRequest = await _client.ACL.Clone(newAclId);
            var aclID = cloneRequest.Response;

            var aclEntry = await _client.ACL.Info(aclID);
            aclEntry.Response.Rules = "key \"\" { policy = \"deny\" }";
            await _client.ACL.Update(aclEntry.Response);

            var aclEntry2 = await _client.ACL.Info(aclID);
            Assert.Equal("key \"\" { policy = \"deny\" }", aclEntry2.Response.Rules);

            var id = cloneRequest.Response;

            Assert.NotEqual(TimeSpan.Zero, cloneRequest.RequestTime);
            Assert.False(string.IsNullOrEmpty(aclID));

            var destroyNewAclEntry = await _client.ACL.Destroy(newAclId);
            Assert.True(destroyNewAclEntry.Response);

            var destroyClonedAclEntry = await _client.ACL.Destroy(id);
            Assert.True(destroyClonedAclEntry.Response);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [SkippableFact]
        public async Task ACL_GetLegacyTokenInfo()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            SkipIfAclNotSupported();

#pragma warning disable CS0618 // Type or member is obsolete
            var aclEntry = await _client.ACL.Info(TestHelper.MasterToken);

            Assert.NotNull(aclEntry.Response);
            Assert.NotEqual(TimeSpan.Zero, aclEntry.RequestTime);
            Assert.Equal(aclEntry.Response.ID, TestHelper.MasterToken);
            Assert.Equal(aclEntry.Response.Type, ACLType.Management);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [SkippableFact]
        public async Task ACL_ListLegacyTokens()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            SkipIfAclNotSupported();

#pragma warning disable CS0618 // Type or member is obsolete
            var aclList = await _client.ACL.List();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.NotNull(aclList.Response);
            Assert.NotEqual(TimeSpan.Zero, aclList.RequestTime);
            Assert.True(aclList.Response.Length >= 1);
        }

        [SkippableFact]
        public async Task ACL_TranslateRule()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            SkipIfAclNotSupported();

            var legacyRule = "agent \"\" {\n policy = \"read\"\n}";
            var newRule = "agent_prefix \"\" {\n  policy = \"read\"\n}";

#pragma warning disable CS0618 // Type or member is obsolete
            var translatedRule = await _client.ACL.TranslateRules(legacyRule);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.NotNull(translatedRule.Response);
            Assert.Equal(newRule, translatedRule.Response);
        }

        [SkippableFact]
        public async Task ACL_CreateTranslateLegacyTokenRuleDestroy()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            SkipIfAclNotSupported();

            var uniqueTokenName = "API Test " + DateTime.Now.ToLongTimeString();
#pragma warning disable CS0618 // Type or member is obsolete
            var aclEntry = new ACLEntry
            {
                Name = uniqueTokenName,
                Type = ACLType.Client,
                Rules = "agent \"\" { policy = \"read\" }"
            };

            var newRule = "agent_prefix \"\" {\n  policy = \"read\"\n}";
            var newLegacyToken = await _client.ACL.Create(aclEntry);
            Assert.NotNull(newLegacyToken.Response);

            var tokens = await _client.Token.List();
            var theLegacyToken = tokens.Response.SingleOrDefault(token => token.Description == aclEntry.Name);

            Assert.NotNull(theLegacyToken);
            if (string.IsNullOrEmpty(theLegacyToken.AccessorID))
            {
                await Task.Delay(5000);
                tokens = await _client.Token.List();
                theLegacyToken = tokens.Response.SingleOrDefault(token => token.Description == aclEntry.Name);
            }
            var translatedRule = await _client.ACL.TranslateLegacyTokenRules(theLegacyToken.AccessorID);

            Assert.NotNull(translatedRule.Response);
            Assert.Equal(newRule, translatedRule.Response);

            var destroyResponse = await _client.ACL.Destroy(newLegacyToken.Response);
            Assert.True(destroyResponse.Response);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
