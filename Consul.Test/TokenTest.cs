// -----------------------------------------------------------------------
//  <copyright file="TokenTest.cs" company="G-Research Limited">
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
using Xunit;

namespace Consul.Test
{
    public class TokenTest : BaseFixture
    {
        [SkippableFact]
        public async Task Token_CreateDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var tokenEntry = new TokenEntry
            {
                Description = "API Testing Token",
                SecretID = "1ED8D9E5-7868-4A0A-AC2F-6F75BEC71830",
                Local = true
            };

            var newToken = await _client.Token.Create(tokenEntry);

            Assert.NotEqual(TimeSpan.Zero, newToken.RequestTime);
            Assert.NotNull(newToken.Response);
            Assert.False(string.IsNullOrEmpty(newToken.Response.AccessorID));
            Assert.Equal(tokenEntry.Description, newToken.Response.Description);
            Assert.Equal(tokenEntry.SecretID, newToken.Response.SecretID);
            Assert.Equal(tokenEntry.Local, newToken.Response.Local);

            var deleteResponse = await _client.Token.Delete(newToken.Response.AccessorID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Token_CreateWithPoliciesDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var policyOneEntry = new PolicyEntry
            {
                Name = "TokenTestPolicyOne",
                Description = "ACL API Test Policy One",
                Rules = "key \"\" { policy = \"deny\" }",
                Datacenters = new string[] { }
            };

            var policyTwoEntry = new PolicyEntry
            {
                Name = "TokenTestPolicyTwo",
                Description = "ACL API Test Policy Two",
                Rules = "key_prefix \"test\" { policy = \"deny\" }",
                Datacenters = new[] { "DC1", "DC2" }
            };

            var policyOne = await _client.Policy.Create(policyOneEntry);
            var policyTwo = await _client.Policy.Create(policyTwoEntry);
            Assert.NotNull(policyOne.Response);
            Assert.NotNull(policyTwo.Response);

            var tokenEntry = new TokenEntry
            {
                Description = "API Testing Token",
                SecretID = "2BF4CA45-5C67-471C-8391-E5D54C76A08B",
                Policies = new PolicyLink[] { policyOne.Response, policyTwo.Response },
                Local = true
            };

            var newToken = await _client.Token.Create(tokenEntry);
            Assert.NotEqual(TimeSpan.Zero, newToken.RequestTime);
            Assert.NotNull(newToken.Response);
            Assert.False(string.IsNullOrEmpty(newToken.Response.AccessorID));
            Assert.Equal(tokenEntry.Description, newToken.Response.Description);
            Assert.Equal(tokenEntry.SecretID, newToken.Response.SecretID);
            Assert.Equal(tokenEntry.Local, newToken.Response.Local);

            var deleteResponse = await _client.Token.Delete(newToken.Response.AccessorID);
            Assert.True(deleteResponse.Response);
            deleteResponse = await _client.Policy.Delete(policyOne.Response.ID);
            Assert.True(deleteResponse.Response);
            deleteResponse = await _client.Policy.Delete(policyTwo.Response.ID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Token_CreateWithRolesDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var roleEntryOne = new RoleEntry
            {
                Name = "TokenTestRoleOne",
                Description = "ACL API Test Role One"
            };

            var roleEntryTwo = new RoleEntry
            {
                Name = "TokenTestRoleTwo",
                Description = "ACL API Test Role Two"
            };

            var roleOne = await _client.Role.Create(roleEntryOne);
            var roleTwo = await _client.Role.Create(roleEntryTwo);
            Assert.NotNull(roleOne.Response);
            Assert.NotNull(roleTwo.Response);

            var tokenEntry = new TokenEntry
            {
                Description = "API Testing Token",
                SecretID = "2F3BF7FF-1297-42FD-9372-9418904ACAE1",
                Roles = new RoleLink[] { roleOne.Response, roleTwo.Response },
                Local = true
            };

            var newToken = await _client.Token.Create(tokenEntry);

            Assert.NotEqual(TimeSpan.Zero, newToken.RequestTime);
            Assert.NotNull(newToken.Response);
            Assert.False(string.IsNullOrEmpty(newToken.Response.AccessorID));
            Assert.Equal(tokenEntry.Description, newToken.Response.Description);
            Assert.Equal(tokenEntry.SecretID, newToken.Response.SecretID);
            Assert.Equal(tokenEntry.Local, newToken.Response.Local);

            var deleteResponse = await _client.Token.Delete(newToken.Response.AccessorID);
            Assert.True(deleteResponse.Response);
            deleteResponse = await _client.Role.Delete(roleOne.Response.ID);
            Assert.True(deleteResponse.Response);
            deleteResponse = await _client.Role.Delete(roleTwo.Response.ID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Token_CreateWithServiceIdentitiesDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var serviceIdentityOne = new ServiceIdentity
            {
                ServiceName = "apitestingdummyserviceidentityone",
                Datacenters = new[] { "dc1", "dc2" }
            };

            var serviceIdentityTwo = new ServiceIdentity
            {
                ServiceName = "apitestingdummyserviceidentitytwo",
                Datacenters = new string[] { }
            };

            var tokenEntry = new TokenEntry
            {
                Description = "API Testing Token",
                SecretID = "4A29E7DD-14B4-48FB-96B5-E084C5E05B60",
                ServiceIdentities = new ServiceIdentity[] { serviceIdentityOne, serviceIdentityTwo },
                Local = false
            };

            var newToken = await _client.Token.Create(tokenEntry);

            Assert.NotEqual(TimeSpan.Zero, newToken.RequestTime);
            Assert.NotNull(newToken.Response);
            Assert.False(string.IsNullOrEmpty(newToken.Response.AccessorID));
            Assert.Equal(tokenEntry.Description, newToken.Response.Description);
            Assert.Equal(tokenEntry.SecretID, newToken.Response.SecretID);
            Assert.Equal(tokenEntry.ServiceIdentities[0].ServiceName, newToken.Response.ServiceIdentities.OrderBy(x => x.ServiceName).ToArray()[0].ServiceName);
            Assert.Equal(tokenEntry.ServiceIdentities[1].ServiceName, newToken.Response.ServiceIdentities.OrderBy(x => x.ServiceName).ToArray()[1].ServiceName);
            Assert.Equal(tokenEntry.Local, newToken.Response.Local);

            var deleteResponse = await _client.Token.Delete(newToken.Response.AccessorID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Token_ReadCloneUpdateDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var selfTokenEntry = await _client.Token.Read("self");
            var clonedTokenEntry = await _client.Token.Clone(selfTokenEntry.Response.AccessorID);

            Assert.NotEqual(TimeSpan.Zero, clonedTokenEntry.RequestTime);
            Assert.Equal(selfTokenEntry.Response.Description, clonedTokenEntry.Response.Description);
            Assert.Equal(selfTokenEntry.Response.Local, clonedTokenEntry.Response.Local);

            clonedTokenEntry.Response.Description = "This is an updated clone of the self token";
            var updatedTokenEntry = await _client.Token.Update(clonedTokenEntry.Response);

            Assert.NotEqual(TimeSpan.Zero, updatedTokenEntry.RequestTime);
            Assert.Equal(clonedTokenEntry.Response.AccessorID, updatedTokenEntry.Response.AccessorID);
            Assert.Equal(clonedTokenEntry.Response.SecretID, updatedTokenEntry.Response.SecretID);
            Assert.Equal(clonedTokenEntry.Response.Local, updatedTokenEntry.Response.Local);
            Assert.Equal(clonedTokenEntry.Response.Description, updatedTokenEntry.Response.Description);

            var deleteResponse = await _client.Token.Delete(updatedTokenEntry.Response.AccessorID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Token_Read()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var tokenEntry = await _client.Token.Read("self");

            Assert.NotNull(tokenEntry.Response);
            Assert.NotEqual(TimeSpan.Zero, tokenEntry.RequestTime);
            Assert.Equal(tokenEntry.Response.SecretID, TestHelper.MasterToken);
        }

        [SkippableFact]
        public async Task Token_List()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var aclList = await _client.Token.List();

            Assert.NotNull(aclList.Response);
            Assert.NotEqual(TimeSpan.Zero, aclList.RequestTime);
            Assert.True(aclList.Response.Length >= 1);
        }
    }
}
