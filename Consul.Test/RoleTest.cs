// -----------------------------------------------------------------------
//  <copyright file="RoleTest.cs" company="G-Research Limited">
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
    public class RoleTest : BaseFixture
    {
        [SkippableFact]
        public async Task Role_CreateDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var roleEntry = new RoleEntry()
            {
                Name = "APITestingRole",
                Description = "Role for API Testing (Role_CreateDelete)"
            };

            var newRoleResult = await _client.Role.Create(roleEntry);
            Assert.NotNull(newRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newRoleResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newRoleResult.Response.ID));
            Assert.Equal(roleEntry.Description, newRoleResult.Response.Description);
            Assert.Equal(roleEntry.Name, newRoleResult.Response.Name);

            var deleteResponse = await _client.Role.Delete(newRoleResult.Response.ID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Role_CreateUpdateDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var roleEntry = new RoleEntry
            {
                Name = "APITestingRole",
                Description = "Role for API Testing (Role_CreateUpdateDelete)"
            };

            var newRoleResult = await _client.Role.Create(roleEntry);
            Assert.NotNull(newRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newRoleResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newRoleResult.Response.ID));
            Assert.Equal(roleEntry.Description, newRoleResult.Response.Description);
            Assert.Equal(roleEntry.Name, newRoleResult.Response.Name);

            newRoleResult.Response.Description = "This is an updated role for API testing (Role_CreateUpdateDelete)";
            var updatedRoleResult = await _client.Role.Update(newRoleResult.Response);

            Assert.NotNull(updatedRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, updatedRoleResult.RequestTime);
            Assert.Equal(newRoleResult.Response.ID, updatedRoleResult.Response.ID);
            Assert.Equal(newRoleResult.Response.Name, updatedRoleResult.Response.Name);
            Assert.Equal(newRoleResult.Response.Description, updatedRoleResult.Response.Description);
            Assert.NotEqual(roleEntry.Description, updatedRoleResult.Response.Description);

            var deleteResponse = await _client.Role.Delete(updatedRoleResult.Response.ID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Role_Create_WithPolicyUpdateWithPoliciesDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var policyOneEntry = new PolicyEntry
            {
                Name = "RoleTestPolicyOne",
                Description = "ACL API Test Policy One",
                Rules = "key \"\" { policy = \"deny\" }",
                Datacenters = new string[] { }
            };

            var policyOne = await _client.Policy.Create(policyOneEntry);
            Assert.NotNull(policyOne.Response);

            var policyTwoEntry = new PolicyEntry
            {
                Name = "RoleTestPolicyTwo",
                Description = "ACL API Test Policy Two",
                Rules = "key_prefix \"test\" { policy = \"deny\" }",
                Datacenters = new[] { "DC1", "DC2" }
            };

            var policyTwo = await _client.Policy.Create(policyTwoEntry);
            Assert.NotNull(policyTwo.Response);

            var roleEntry = new RoleEntry
            {
                Name = "APITestingRole",
                Description = "Role for API Testing (Role_CreateWithPolicyUpdatePolicyDelete)",
                Policies = new PolicyLink[] { policyOne.Response }
            };

            var newRoleResult = await _client.Role.Create(roleEntry);
            Assert.NotNull(newRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newRoleResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newRoleResult.Response.ID));
            Assert.Equal(roleEntry.Description, newRoleResult.Response.Description);
            Assert.Equal(roleEntry.Name, newRoleResult.Response.Name);

            newRoleResult.Response.Description = "This is an updated role for API testing (Role_CreateWithPolicyUpdatePolicyDelete)";
            newRoleResult.Response.Policies = new PolicyLink[] { policyTwo.Response, policyOne.Response };
            var updatedRoleResult = await _client.Role.Update(newRoleResult.Response);

            Assert.NotNull(updatedRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, updatedRoleResult.RequestTime);
            Assert.Equal(newRoleResult.Response.ID, updatedRoleResult.Response.ID);
            Assert.Equal(newRoleResult.Response.Name, updatedRoleResult.Response.Name);
            Assert.Equal(newRoleResult.Response.Description, updatedRoleResult.Response.Description);
            Assert.NotEqual(roleEntry.Description, updatedRoleResult.Response.Description);

            var deleteResponse = await _client.Role.Delete(updatedRoleResult.Response.ID);
            Assert.True(deleteResponse.Response);
            deleteResponse = await _client.Policy.Delete(policyOne.Response.ID);
            Assert.True(deleteResponse.Response);
            deleteResponse = await _client.Policy.Delete(policyTwo.Response.ID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Role_Create_WithServiceIdentitiesUpdateWithServiceIdentityDelete()
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

            var roleEntry = new RoleEntry
            {
                Name = "APITestingRole",
                Description = "Role for API Testing (Role_CreateWithServiceIdentityUpdateServiceIdentityDelete)",
                ServiceIdentities = new ServiceIdentity[] { serviceIdentityOne, serviceIdentityTwo }
            };

            var newRoleResult = await _client.Role.Create(roleEntry);
            Assert.NotNull(newRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newRoleResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newRoleResult.Response.ID));
            Assert.Equal(roleEntry.Description, newRoleResult.Response.Description);
            Assert.Equal(roleEntry.Name, newRoleResult.Response.Name);

            newRoleResult.Response.Description = "This is an updated role for API testing (Role_CreateWithServiceIdentityUpdateServiceIdentityDelete)";
            newRoleResult.Response.ServiceIdentities = new ServiceIdentity[] { serviceIdentityTwo };
            var updatedRoleResult = await _client.Role.Update(newRoleResult.Response);

            Assert.NotNull(updatedRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, updatedRoleResult.RequestTime);
            Assert.Equal(newRoleResult.Response.ID, updatedRoleResult.Response.ID);
            Assert.Equal(newRoleResult.Response.Name, updatedRoleResult.Response.Name);
            Assert.Equal(newRoleResult.Response.Description, updatedRoleResult.Response.Description);
            Assert.NotEqual(roleEntry.Description, updatedRoleResult.Response.Description);

            var deleteResponse = await _client.Role.Delete(updatedRoleResult.Response.ID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Role_Create_WithPoliciesReadDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var policyOneEntry = new PolicyEntry
            {
                Name = "RoleTestPolicyOne",
                Description = "ACL API Test Policy One",
                Rules = "key \"\" { policy = \"deny\" }",
                Datacenters = new string[] { }
            };

            var policyOne = await _client.Policy.Create(policyOneEntry);
            Assert.NotNull(policyOne.Response);

            var policyTwoEntry = new PolicyEntry
            {
                Name = "RoleTestPolicyTwo",
                Description = "ACL API Test Policy Two",
                Rules = "key_prefix \"test\" { policy = \"deny\" }",
                Datacenters = new[] { "DC1", "DC2" }
            };

            var policyTwo = await _client.Policy.Create(policyTwoEntry);
            Assert.NotNull(policyTwo.Response);

            var roleEntry = new RoleEntry
            {
                Name = "APITestingRole",
                Description = "Role for API Testing (Role_CreateWithPoliciesReadDelete)",
                Policies = new PolicyLink[] { policyOne.Response, policyTwo.Response }
            };

            var newRoleResult = await _client.Role.Create(roleEntry);
            Assert.NotNull(newRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newRoleResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newRoleResult.Response.ID));
            Assert.Equal(roleEntry.Description, newRoleResult.Response.Description);
            Assert.Equal(roleEntry.Name, newRoleResult.Response.Name);

            var readRole = await _client.Role.Read(newRoleResult.Response.ID);
            Assert.NotNull(readRole.Response);
            Assert.NotEqual(TimeSpan.Zero, readRole.RequestTime);
            Assert.Equal(newRoleResult.Response.ID, readRole.Response.ID);
            Assert.Equal(newRoleResult.Response.Name, readRole.Response.Name);
            Assert.Equal(newRoleResult.Response.Description, readRole.Response.Description);

            var deleteResponse = await _client.Role.Delete(newRoleResult.Response.ID);
            Assert.True(deleteResponse.Response);
            deleteResponse = await _client.Policy.Delete(policyOne.Response.ID);
            Assert.True(deleteResponse.Response);
            deleteResponse = await _client.Policy.Delete(policyTwo.Response.ID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Role_Create_WithServiceIdentitiesReadByNameDelete()
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

            var roleEntry = new RoleEntry
            {
                Name = "APITestingRole",
                Description = "Role for API Testing (Role_CreateWithServicesReadByNameDelete)",
                ServiceIdentities = new ServiceIdentity[] { serviceIdentityOne, serviceIdentityTwo }

            };

            var newRoleResult = await _client.Role.Create(roleEntry);
            Assert.NotNull(newRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newRoleResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newRoleResult.Response.ID));
            Assert.Equal(roleEntry.Description, newRoleResult.Response.Description);
            Assert.Equal(roleEntry.Name, newRoleResult.Response.Name);

            var readRoleByName = await _client.Role.ReadByName("APITestingRole");
            Assert.NotNull(readRoleByName.Response);
            Assert.NotEqual(TimeSpan.Zero, readRoleByName.RequestTime);
            Assert.Equal(newRoleResult.Response.ID, readRoleByName.Response.ID);
            Assert.Equal(newRoleResult.Response.Name, readRoleByName.Response.Name);
            Assert.Equal(newRoleResult.Response.Description, readRoleByName.Response.Description);

            var deleteResponse = await _client.Role.Delete(newRoleResult.Response.ID);
            Assert.True(deleteResponse.Response);
        }

        [SkippableFact]
        public async Task Role_CreateListDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var roleEntry = new RoleEntry
            {
                Name = "APITestingRole",
                Description = "Role for API Testing (Role_CreateListDelete)"
            };

            var newRoleResult = await _client.Role.Create(roleEntry);
            Assert.NotNull(newRoleResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newRoleResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newRoleResult.Response.ID));
            Assert.Equal(roleEntry.Description, newRoleResult.Response.Description);
            Assert.Equal(roleEntry.Name, newRoleResult.Response.Name);

            var aclRoleList = await _client.Role.List();
            Assert.NotNull(aclRoleList.Response);
            Assert.NotEqual(TimeSpan.Zero, aclRoleList.RequestTime);
            Assert.True(aclRoleList.Response.Length >= 1);

            var deleteResponse = await _client.Role.Delete(newRoleResult.Response.ID);
            Assert.True(deleteResponse.Response);
        }
    }
}
