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
using System.Security.Cryptography;
using System.Text;
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
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var rsaParams = new RSAParameters();
            string pubKeyPem;
            string jwt;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsaParams = rsa.ExportParameters(false);
                pubKeyPem = ExportRSAPublicKeyPem(rsa);
                jwt = CreateSignedJwt(rsa, "consul-login-test-issuer", "consul-login-test", "test-user");
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
            await _client.AuthMethod.Create(authMethodEntry);
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
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static string ExportRSAPublicKeyPem(RSACryptoServiceProvider rsa)
        {
            var parameters = rsa.ExportParameters(false);

            // Build DER-encoded SubjectPublicKeyInfo manually for net461 compatibility
            var modulus = parameters.Modulus;
            var exponent = parameters.Exponent;

            // ASN.1 INTEGER: pad with leading zero if high bit set
            byte[] EncodeInteger(byte[] value)
            {
                bool needsPadding = value[0] >= 0x80;
                int length = value.Length + (needsPadding ? 1 : 0);
                var result = new List<byte> { 0x02 };
                result.AddRange(EncodeLength(length));
                if (needsPadding) result.Add(0x00);
                result.AddRange(value);
                return result.ToArray();
            }

            byte[] EncodeLength(int length)
            {
                if (length < 0x80) return new[] { (byte)length };
                if (length <= 0xFF) return new byte[] { 0x81, (byte)length };
                return new byte[] { 0x82, (byte)(length >> 8), (byte)length };
            }

            var modulusEncoded = EncodeInteger(modulus);
            var exponentEncoded = EncodeInteger(exponent);

            // SEQUENCE { modulus, exponent }
            var rsaKeySequence = new List<byte> { 0x30 };
            var rsaKeyBody = new List<byte>();
            rsaKeyBody.AddRange(modulusEncoded);
            rsaKeyBody.AddRange(exponentEncoded);
            rsaKeySequence.AddRange(EncodeLength(rsaKeyBody.Count));
            rsaKeySequence.AddRange(rsaKeyBody);

            // BIT STRING wrapping the RSA key sequence
            var bitString = new List<byte> { 0x03 };
            bitString.AddRange(EncodeLength(rsaKeySequence.Count + 1));
            bitString.Add(0x00); // unused bits
            bitString.AddRange(rsaKeySequence);

            // AlgorithmIdentifier for RSA: SEQUENCE { OID 1.2.840.113549.1.1.1, NULL }
            var algorithmIdentifier = new byte[]
            {
                0x30, 0x0D,
                0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01,
                0x05, 0x00
            };

            // Outer SEQUENCE { algorithmIdentifier, bitString }
            var spkiBody = new List<byte>();
            spkiBody.AddRange(algorithmIdentifier);
            spkiBody.AddRange(bitString);

            var spki = new List<byte> { 0x30 };
            spki.AddRange(EncodeLength(spkiBody.Count));
            spki.AddRange(spkiBody);

            var base64 = Convert.ToBase64String(spki.ToArray(), Base64FormattingOptions.InsertLineBreaks);
            return $"-----BEGIN PUBLIC KEY-----\n{base64}\n-----END PUBLIC KEY-----";
        }
        private static string CreateSignedJwt(RSACryptoServiceProvider rsa, string issuer, string audience, string subject)
        {
            var now = DateTimeOffset.UtcNow;
            var header = $"{{\"alg\":\"RS256\",\"typ\":\"JWT\"}}";
            var payload = $"{{\"sub\":\"{subject}\",\"iss\":\"{issuer}\",\"aud\":\"{audience}\",\"iat\":{now.ToUnixTimeSeconds()},\"exp\":{now.AddHours(1).ToUnixTimeSeconds()}}}";
            var headerB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(header));
            var payloadB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payload));
            var signingInput = $"{headerB64}.{payloadB64}";
            var signature = rsa.SignData(Encoding.UTF8.GetBytes(signingInput), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return $"{signingInput}.{Base64UrlEncode(signature)}";
        }
    }
}
