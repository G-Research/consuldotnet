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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;
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
        public async Task Token_CreateWithNodeIdentitiesDelete()
        {
            var cutOffVersion = SemanticVersion.Parse("1.8.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but Service Tokens are only supported from Consul {cutOffVersion}");
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var nodeIdentityOne = new NodeIdentity
            {
                NodeName = "node-1",
                Datacenter = "dc1"
            };

            var nodeIdentityTwo = new NodeIdentity
            {
                NodeName = "node-2",
                Datacenter = "dc2"
            };

            var tokenEntry = new TokenEntry
            {
                Description = "API Testing Token for Node Identity",
                SecretID = Guid.NewGuid().ToString(),
                NodeIdentities = new NodeIdentity[] { nodeIdentityOne, nodeIdentityTwo },
                Local = false
            };

            var newToken = await _client.Token.Create(tokenEntry);

            Assert.NotEqual(TimeSpan.Zero, newToken.RequestTime);
            Assert.NotNull(newToken.Response);
            Assert.False(string.IsNullOrEmpty(newToken.Response.AccessorID));
            Assert.Equal(tokenEntry.Description, newToken.Response.Description);
            Assert.Equal(tokenEntry.SecretID, newToken.Response.SecretID);
            Assert.Equal(tokenEntry.NodeIdentities[0].NodeName, newToken.Response.NodeIdentities.OrderBy(x => x.NodeName).ToArray()[0].NodeName);
            Assert.Equal(tokenEntry.NodeIdentities[1].NodeName, newToken.Response.NodeIdentities.OrderBy(x => x.NodeName).ToArray()[1].NodeName);
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

        [SkippableFact]
        public async Task Token_List_FilterBy()
        {
            var cutOffVersion = SemanticVersion.Parse("1.14.0");
            Skip.If(AgentVersion < cutOffVersion, $"This Consul server version({AgentVersion}) doesn't support `ServiceIdentity`. Requires >= {cutOffVersion}.");
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            // Create a specific Policy to filter by
            var policyEntry = new PolicyEntry
            {
                Name = KVTest.GenerateTestKeyName(),
                Description = "Policy used to test token list filtering",
                Rules = "key \"\" { policy = \"read\" }",
            };
            var policy = await _client.Policy.Create(policyEntry);
            Assert.NotNull(policy.Response.Name);

            // create a test role to filter by
            var roleEntry = new RoleEntry
            {
                Name = KVTest.GenerateTestKeyName(),
                Description = "Test Expanded Role",
                Policies = new PolicyLink[] { policy.Response },
            };
            var role = await _client.Role.Create(roleEntry);
            Assert.NotNull(role.Response);

            var serviceIdentity = new ServiceIdentity
            {
                ServiceName = "web"
            };

            var host = "https://192.0.2.42:8443";
            var caCert = "-----BEGIN CERTIFICATE-----\nMIIHQDCCBiigAwIBAgIQD9B43Ujxor1NDyupa2A4/jANBgkqhkiG9w0BAQsFADBN\nMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMScwJQYDVQQDEx5E\naWdpQ2VydCBTSEEyIFNlY3VyZSBTZXJ2ZXIgQ0EwHhcNMTgxMTI4MDAwMDAwWhcN\nMjAxMjAyMTIwMDAwWjCBpTELMAkGA1UEBhMCVVMxEzARBgNVBAgTCkNhbGlmb3Ju\naWExFDASBgNVBAcTC0xvcyBBbmdlbGVzMTwwOgYDVQQKEzNJbnRlcm5ldCBDb3Jw\nb3JhdGlvbiBmb3IgQXNzaWduZWQgTmFtZXMgYW5kIE51bWJlcnMxEzARBgNVBAsT\nClRlY2hub2xvZ3kxGDAWBgNVBAMTD3d3dy5leGFtcGxlLm9yZzCCASIwDQYJKoZI\nhvcNAQEBBQADggEPADCCAQoCggEBANDwEnSgliByCGUZElpdStA6jGaPoCkrp9vV\nrAzPpXGSFUIVsAeSdjF11yeOTVBqddF7U14nqu3rpGA68o5FGGtFM1yFEaogEv5g\nrJ1MRY/d0w4+dw8JwoVlNMci+3QTuUKf9yH28JxEdG3J37Mfj2C3cREGkGNBnY80\neyRJRqzy8I0LSPTTkhr3okXuzOXXg38ugr1x3SgZWDNuEaE6oGpyYJIBWZ9jF3pJ\nQnucP9vTBejMh374qvyd0QVQq3WxHrogy4nUbWw3gihMxT98wRD1oKVma1NTydvt\nhcNtBfhkp8kO64/hxLHrLWgOFT/l4tz8IWQt7mkrBHjbd2XLVPkCAwEAAaOCA8Ew\nggO9MB8GA1UdIwQYMBaAFA+AYRyCMWHVLyjnjUY4tCzhxtniMB0GA1UdDgQWBBRm\nmGIC4AmRp9njNvt2xrC/oW2nvjCBgQYDVR0RBHoweIIPd3d3LmV4YW1wbGUub3Jn\nggtleGFtcGxlLmNvbYILZXhhbXBsZS5lZHWCC2V4YW1wbGUubmV0ggtleGFtcGxl\nLm9yZ4IPd3d3LmV4YW1wbGUuY29tgg93d3cuZXhhbXBsZS5lZHWCD3d3dy5leGFt\ncGxlLm5ldDAOBgNVHQ8BAf8EBAMCBaAwHQYDVR0lBBYwFAYIKwYBBQUHAwEGCCsG\nAQUFBwMCMGsGA1UdHwRkMGIwL6AtoCuGKWh0dHA6Ly9jcmwzLmRpZ2ljZXJ0LmNv\nbS9zc2NhLXNoYTItZzYuY3JsMC+gLaArhilodHRwOi8vY3JsNC5kaWdpY2VydC5j\nb20vc3NjYS1zaGEyLWc2LmNybDBMBgNVHSAERTBDMDcGCWCGSAGG/WwBATAqMCgG\nCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAgGBmeBDAEC\nAjB8BggrBgEFBQcBAQRwMG4wJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2lj\nZXJ0LmNvbTBGBggrBgEFBQcwAoY6aHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29t\nL0RpZ2lDZXJ0U0hBMlNlY3VyZVNlcnZlckNBLmNydDAMBgNVHRMBAf8EAjAAMIIB\nfwYKKwYBBAHWeQIEAgSCAW8EggFrAWkAdwCkuQmQtBhYFIe7E6LMZ3AKPDWYBPkb\n37jjd80OyA3cEAAAAWdcMZVGAAAEAwBIMEYCIQCEZIG3IR36Gkj1dq5L6EaGVycX\nsHvpO7dKV0JsooTEbAIhALuTtf4wxGTkFkx8blhTV+7sf6pFT78ORo7+cP39jkJC\nAHYAh3W/51l8+IxDmV+9827/Vo1HVjb/SrVgwbTq/16ggw8AAAFnXDGWFQAABAMA\nRzBFAiBvqnfSHKeUwGMtLrOG3UGLQIoaL3+uZsGTX3MfSJNQEQIhANL5nUiGBR6g\nl0QlCzzqzvorGXyB/yd7nttYttzo8EpOAHYAb1N2rDHwMRnYmQCkURX/dxUcEdkC\nwQApBo2yCJo32RMAAAFnXDGWnAAABAMARzBFAiEA5Hn7Q4SOyqHkT+kDsHq7ku7z\nRDuM7P4UDX2ft2Mpny0CIE13WtxJAUr0aASFYZ/XjSAMMfrB0/RxClvWVss9LHKM\nMA0GCSqGSIb3DQEBCwUAA4IBAQBzcIXvQEGnakPVeJx7VUjmvGuZhrr7DQOLeP4R\n8CmgDM1pFAvGBHiyzvCH1QGdxFl6cf7wbp7BoLCRLR/qPVXFMwUMzcE1GLBqaGZM\nv1Yh2lvZSLmMNSGRXdx113pGLCInpm/TOhfrvr0TxRImc8BdozWJavsn1N2qdHQu\nN+UBO6bQMLCD0KHEdSGFsuX6ZwAworxTg02/1qiDu7zW7RyzHvFYA4IAjpzvkPIa\nX6KjBtpdvp/aXabmL95YgBjT8WJ7pqOfrqhpcmOBZa6Cg6O1l4qbIFH/Gj9hQB5I\n0Gs4+eH6F9h3SojmPTYkT+8KuZ9w84Mn+M8qBXUQoYoKgIjN\n-----END CERTIFICATE-----\n";
            var serviceAccountJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNzA4MzQ1MTIzLCJleHAiOjE3MDgzNTUxMjN9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            var authMethodEntry = new AuthMethodEntry
            {
                Name = "AuthMethodApiTest",
                Type = "kubernetes",
                Description = "Auth Method for API Unit Testing",
                Config = new Dictionary<string, string>
                {
                    ["Host"] = host,
                    ["CACert"] = caCert,
                    ["ServiceAccountJWT"] = serviceAccountJWT
                }
            };
            var authMethod = await _client.AuthMethod.Create(authMethodEntry);
            Assert.Equal(authMethodEntry.Name, authMethod.Response.Name);

            var tokenEntry1 = new TokenEntry
            {
                Description = "Token Linked to Filter Policy",
                Policies = new PolicyLink[] { policy.Response },
                Roles = new RoleLink[] { role.Response },
                Local = true
            };
            var token1 = await _client.Token.Create(tokenEntry1);
            Assert.NotEmpty(token1.Response.Policies);
            Assert.NotEmpty(token1.Response.Roles);

            var tokenEntry2 = new TokenEntry
            {
                Description = "Token NOT Linked to Filter Policy",
                Local = true,
                ServiceIdentities = new ServiceIdentity[] { serviceIdentity },
                AuthMethod = authMethod.Response.Name
            };
            var token2 = await _client.Token.Create(tokenEntry2);
            Assert.NotEmpty(token2.Response.Description);
            Assert.Equal(serviceIdentity.ServiceName, token2.Response.ServiceIdentities.First().ServiceName);
            Assert.Equal(authMethodEntry.Name, token2.Response.AuthMethod);

            // List tokens filtering by the specific PolicyID
            var filteredList = await _client.Token.List(policy.Response.ID, null, null, null);
            Assert.NotEmpty(filteredList.Response);
            Assert.Contains(filteredList.Response, t => t.AccessorID == token1.Response.AccessorID);
            Assert.DoesNotContain(filteredList.Response, t => t.AccessorID == token2.Response.AccessorID);

            // List tokens filtering by the specific RoleID
            filteredList = await _client.Token.List(null, role.Response.ID, null, null);
            Assert.NotEmpty(filteredList.Response);
            Assert.Contains(filteredList.Response, t => t.AccessorID == token1.Response.AccessorID);
            Assert.DoesNotContain(filteredList.Response, t => t.AccessorID == token2.Response.AccessorID);

            // List tokens filtering by the specific ServiceName
            filteredList = await _client.Token.List(null, null, serviceIdentity.ServiceName, null);
            Assert.NotEmpty(filteredList.Response);
            Assert.Contains(filteredList.Response, t => t.AccessorID == token2.Response.AccessorID);
            Assert.DoesNotContain(filteredList.Response, t => t.AccessorID == token1.Response.AccessorID);

            // List tokens filtering by the specific AuthMethod
            filteredList = await _client.Token.List(null, null, null, authMethodEntry.Name);
            Assert.NotEmpty(filteredList.Response);
            Assert.Contains(filteredList.Response, t => t.AccessorID == token2.Response.AccessorID);
            Assert.DoesNotContain(filteredList.Response, t => t.AccessorID == token1.Response.AccessorID);

            // Cleanup
            await _client.Token.Delete(token1.Response.AccessorID);
            await _client.Token.Delete(token2.Response.AccessorID);
            await _client.Policy.Delete(policy.Response.ID);
            await _client.Role.Delete(role.Response.ID);
            await _client.AuthMethod.Delete(authMethod.Response.Name);
        }

        [SkippableFact]
        public async Task Token_ReadSelfToken()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var tokenEntry = await _client.Token.Read("self");
            Assert.NotNull(tokenEntry.Response);

            var token = tokenEntry.Response.SecretID;
            var selfTokenEntry = await _client.Token.ReadSelfToken(token);

            Assert.NotNull(selfTokenEntry.Response);
            Assert.NotEqual(TimeSpan.Zero, selfTokenEntry.RequestTime);
            Assert.Equal(selfTokenEntry.Response.SecretID, TestHelper.MasterToken);
            Assert.Equal(selfTokenEntry.Response.SecretID, tokenEntry.Response.SecretID);
            Assert.Equal(selfTokenEntry.Response.AccessorID, tokenEntry.Response.AccessorID);
        }

        [SkippableFact]
        public async Task Token_ReadExpanded()
        {
            var cutOffVersion = SemanticVersion.Parse("1.12.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current Consul version {AgentVersion} does not support the `-expanded` flag to display detailed role and policy information for the token. Requires >= {cutOffVersion}.");
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            // create test policy
            var policyEntry = new PolicyEntry
            {
                Name = KVTest.GenerateTestKeyName(),
                Description = "Test Expanded Policy",
                Rules = "key \"\" { policy = \"read\" }"
            };
            var policy = await _client.Policy.Create(policyEntry);
            Assert.NotEmpty(policy.Response.Name);
            Assert.NotEmpty(policy.Response.Description);
            Assert.NotEmpty(policy.Response.Rules);

            // create test role
            var roleEntry = new RoleEntry
            {
                Name = KVTest.GenerateTestKeyName(),
                Description = "Test Expanded Role",
                Policies = new PolicyLink[] { policy.Response },
            };
            var role = await _client.Role.Create(roleEntry);
            Assert.NotEmpty(role.Response.Name);
            Assert.NotEmpty(role.Response.Description);
            Assert.NotEmpty(role.Response.Policies);

            var tokenEntry = new TokenEntry
            {
                Description = "Expansion Test Token",
                Policies = new PolicyLink[] { policy.Response },
                Roles = new RoleLink[] { role.Response },
                Local = true
            };
            var newToken = await _client.Token.Create(tokenEntry);
            var accessorId = newToken.Response.AccessorID;

            // when expanded=false
            var unexpandedRead = await _client.Token.Read(accessorId, false, QueryOptions.Default);
            Assert.NotEmpty(unexpandedRead.Response.Policies);
            Assert.Equal(policyEntry.Name, unexpandedRead.Response.Policies.First().Name);
            Assert.NotEmpty(unexpandedRead.Response.Description);
            Assert.Null(unexpandedRead.Response.AgentACLDefaultPolicy);
            Assert.Null(unexpandedRead.Response.AgentACLDownPolicy);
            Assert.Null(unexpandedRead.Response.ResolvedByAgent);
            Assert.Null(unexpandedRead.Response.ExpandedPolicies);
            Assert.Null(unexpandedRead.Response.ExpandedRoles);

            // when expanded=true
            var expandedRead = await _client.Token.Read(accessorId, true, QueryOptions.Default);
            Assert.NotEmpty(expandedRead.Response.AgentACLDefaultPolicy);
            Assert.NotEmpty(expandedRead.Response.AgentACLDownPolicy);
            Assert.NotEmpty(expandedRead.Response.ResolvedByAgent);
            Assert.NotEmpty(expandedRead.Response.ExpandedPolicies);
            Assert.Equal(policyEntry.Rules, expandedRead.Response.ExpandedPolicies.First().Rules);
            Assert.NotEmpty(expandedRead.Response.ExpandedRoles);
            Assert.Equal(roleEntry.Name, expandedRead.Response.ExpandedRoles.First().Name);
            Assert.NotEmpty(expandedRead.Response.ExpandedRoles.First().Policies);

            // Cleanup
            await _client.Token.Delete(accessorId);
            await _client.Policy.Delete(policy.Response.ID);
            await _client.Role.Delete(role.Response.ID);
        }
    }
}
