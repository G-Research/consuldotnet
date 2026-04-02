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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using NuGet.Versioning;
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
                Config = new Dictionary<string, object>
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

        [SkippableFact]
        public async Task AuthMethod_Login()
        {
            var cutOffVersion = SemanticVersion.Parse("1.8.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current Consul version {AgentVersion} does not support AuthMethod.Type=\"jwt\". Requires >= {cutOffVersion}");
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

#if NET5_0_OR_GREATER
            string pubKeyPem;
            string jwt;
            using (var rsa = RSA.Create(2048))
            {
                pubKeyPem = rsa.ExportSubjectPublicKeyInfoPem();
                var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
                var token = new JwtSecurityToken(
                    issuer: "consul-login-test-issuer",
                    audience: "consul-login-test",
                    claims: new[] { new Claim("sub", "test-user") },
                    notBefore: DateTime.UtcNow.AddMinutes(-1),
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: signingCredentials
                );
                jwt = new JwtSecurityTokenHandler().WriteToken(token);
            }

            var authMethodEntry = new AuthMethodEntry
            {
                Name = "AuthMethodLoginTest",
                Type = "jwt",
                Description = "JWT Auth Method for Login Testing",
                Config = new Dictionary<string, object>
                {
                    ["BoundAudiences"] = new[] { "consul-login-test" },
                    ["BoundIssuer"] = "consul-login-test-issuer",
                    ["JWTValidationPubKeys"] = new[] { pubKeyPem },
                    ["ClaimMappings"] = new Dictionary<string, string> { ["sub"] = "user_name" }
                }
            };
            var authMethod = await _client.AuthMethod.Create(authMethodEntry);
            Assert.NotNull(authMethod.Response);

            var bindingRule = new ACLBindingRule
            {
                AuthMethod = authMethodEntry.Name,
                BindType = "service",
                BindName = "web",
                Selector = ""
            };
            var bindingRuleResponse = await _client.BindingRule.Create(bindingRule);
            Assert.NotNull(bindingRuleResponse.Response);

            var res = await _client.AuthMethod.Login(authMethod.Response.Name, jwt);
            Assert.NotEmpty(res.Response.AccessorID);
            Assert.NotEmpty(res.Response.SecretID);
            Assert.Equal(res.Response.AuthMethod, authMethodEntry.Name);

            // Cleanup
            await _client.AuthMethod.Delete(authMethod.Response.Name);
#else
            Skip.If(true, "RSA.ExportSubjectPublicKeyInfoPem() is not avaible befre NET5.0");
            await Task.CompletedTask;
#endif
        }
    }
}
