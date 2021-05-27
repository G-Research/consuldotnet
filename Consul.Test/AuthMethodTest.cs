// -----------------------------------------------------------------------
//  <copyright file="AuthMethodTest.cs" company="G-Research Limited">
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
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class AuthMethodTest : BaseFixture
    {
        [SkippableFact]
        public async Task AuthMethod_CreateDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            var aclReplicationStatus = await _client.ACLReplication.Status();
            Skip.If(!aclReplicationStatus.Response.Enabled, "ACL Replication must be running to use AuthMethods.");

            var authMethodEntry = new AuthMethodEntry
            {
                Name = "AuthMethodApiTest",
                Type = "kubernetes",
                Description = "Auth Method for API Unit Testing",
                Config = new Dictionary<string, string>
                {
                    ["Host"] = "https://192.0.2.42:8443",
                    ["CACert"] = "-----BEGIN CERTIFICATE-----\n...\n-----END CERTIFICATE-----\n",
                    ["ServiceAccountJWT"] = "eyJhbGciOiJSUzI1NiIsImtpZCI6IiJ9..."
                }
            };

            var newAuthMethodResult = await _client.AuthMethod.Create(authMethodEntry);
            Assert.NotNull(newAuthMethodResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newAuthMethodResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newAuthMethodResult.Response.Name));
            Assert.Equal(authMethodEntry.Description, newAuthMethodResult.Response.Description);

            var deleteResult = await _client.Policy.Delete(newAuthMethodResult.Response.Name);
            Assert.True(deleteResult.Response);
        }

        [SkippableFact]
        public async Task AuthMethod_CreateUpdateDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            var aclReplicationStatus = await _client.ACLReplication.Status();
            Skip.If(!aclReplicationStatus.Response.Enabled, "ACL Replication must be running to use AuthMethods.");

            throw new NotImplementedException();
        }

        [SkippableFact]
        public async Task AuthMethod_CreateReadDelete()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            var aclReplicationStatus = await _client.ACLReplication.Status();
            Skip.If(!aclReplicationStatus.Response.Enabled, "ACL Replication must be running to use AuthMethods.");

            throw new NotImplementedException();
        }

        [SkippableFact]
        public async Task AuthMethod_List()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));
            var aclReplicationStatus = await _client.ACLReplication.Status();
            Skip.If(!aclReplicationStatus.Response.Enabled, "ACL Replication must be running to use AuthMethods.");

            throw new NotImplementedException();
        }
    }
}
