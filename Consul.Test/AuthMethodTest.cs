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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class AuthMethodTest : BaseFixture
    {
        private readonly string _host = "https://192.0.2.42:8443";
        private readonly string _caCert = "-----BEGIN CERTIFICATE-----\nMIIHQDCCBiigAwIBAgIQD9B43Ujxor1NDyupa2A4/jANBgkqhkiG9w0BAQsFADBN\nMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMScwJQYDVQQDEx5E\naWdpQ2VydCBTSEEyIFNlY3VyZSBTZXJ2ZXIgQ0EwHhcNMTgxMTI4MDAwMDAwWhcN\nMjAxMjAyMTIwMDAwWjCBpTELMAkGA1UEBhMCVVMxEzARBgNVBAgTCkNhbGlmb3Ju\naWExFDASBgNVBAcTC0xvcyBBbmdlbGVzMTwwOgYDVQQKEzNJbnRlcm5ldCBDb3Jw\nb3JhdGlvbiBmb3IgQXNzaWduZWQgTmFtZXMgYW5kIE51bWJlcnMxEzARBgNVBAsT\nClRlY2hub2xvZ3kxGDAWBgNVBAMTD3d3dy5leGFtcGxlLm9yZzCCASIwDQYJKoZI\nhvcNAQEBBQADggEPADCCAQoCggEBANDwEnSgliByCGUZElpdStA6jGaPoCkrp9vV\nrAzPpXGSFUIVsAeSdjF11yeOTVBqddF7U14nqu3rpGA68o5FGGtFM1yFEaogEv5g\nrJ1MRY/d0w4+dw8JwoVlNMci+3QTuUKf9yH28JxEdG3J37Mfj2C3cREGkGNBnY80\neyRJRqzy8I0LSPTTkhr3okXuzOXXg38ugr1x3SgZWDNuEaE6oGpyYJIBWZ9jF3pJ\nQnucP9vTBejMh374qvyd0QVQq3WxHrogy4nUbWw3gihMxT98wRD1oKVma1NTydvt\nhcNtBfhkp8kO64/hxLHrLWgOFT/l4tz8IWQt7mkrBHjbd2XLVPkCAwEAAaOCA8Ew\nggO9MB8GA1UdIwQYMBaAFA+AYRyCMWHVLyjnjUY4tCzhxtniMB0GA1UdDgQWBBRm\nmGIC4AmRp9njNvt2xrC/oW2nvjCBgQYDVR0RBHoweIIPd3d3LmV4YW1wbGUub3Jn\nggtleGFtcGxlLmNvbYILZXhhbXBsZS5lZHWCC2V4YW1wbGUubmV0ggtleGFtcGxl\nLm9yZ4IPd3d3LmV4YW1wbGUuY29tgg93d3cuZXhhbXBsZS5lZHWCD3d3dy5leGFt\ncGxlLm5ldDAOBgNVHQ8BAf8EBAMCBaAwHQYDVR0lBBYwFAYIKwYBBQUHAwEGCCsG\nAQUFBwMCMGsGA1UdHwRkMGIwL6AtoCuGKWh0dHA6Ly9jcmwzLmRpZ2ljZXJ0LmNv\nbS9zc2NhLXNoYTItZzYuY3JsMC+gLaArhilodHRwOi8vY3JsNC5kaWdpY2VydC5j\nb20vc3NjYS1zaGEyLWc2LmNybDBMBgNVHSAERTBDMDcGCWCGSAGG/WwBATAqMCgG\nCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAgGBmeBDAEC\nAjB8BggrBgEFBQcBAQRwMG4wJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2lj\nZXJ0LmNvbTBGBggrBgEFBQcwAoY6aHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29t\nL0RpZ2lDZXJ0U0hBMlNlY3VyZVNlcnZlckNBLmNydDAMBgNVHRMBAf8EAjAAMIIB\nfwYKKwYBBAHWeQIEAgSCAW8EggFrAWkAdwCkuQmQtBhYFIe7E6LMZ3AKPDWYBPkb\n37jjd80OyA3cEAAAAWdcMZVGAAAEAwBIMEYCIQCEZIG3IR36Gkj1dq5L6EaGVycX\nsHvpO7dKV0JsooTEbAIhALuTtf4wxGTkFkx8blhTV+7sf6pFT78ORo7+cP39jkJC\nAHYAh3W/51l8+IxDmV+9827/Vo1HVjb/SrVgwbTq/16ggw8AAAFnXDGWFQAABAMA\nRzBFAiBvqnfSHKeUwGMtLrOG3UGLQIoaL3+uZsGTX3MfSJNQEQIhANL5nUiGBR6g\nl0QlCzzqzvorGXyB/yd7nttYttzo8EpOAHYAb1N2rDHwMRnYmQCkURX/dxUcEdkC\nwQApBo2yCJo32RMAAAFnXDGWnAAABAMARzBFAiEA5Hn7Q4SOyqHkT+kDsHq7ku7z\nRDuM7P4UDX2ft2Mpny0CIE13WtxJAUr0aASFYZ/XjSAMMfrB0/RxClvWVss9LHKM\nMA0GCSqGSIb3DQEBCwUAA4IBAQBzcIXvQEGnakPVeJx7VUjmvGuZhrr7DQOLeP4R\n8CmgDM1pFAvGBHiyzvCH1QGdxFl6cf7wbp7BoLCRLR/qPVXFMwUMzcE1GLBqaGZM\nv1Yh2lvZSLmMNSGRXdx113pGLCInpm/TOhfrvr0TxRImc8BdozWJavsn1N2qdHQu\nN+UBO6bQMLCD0KHEdSGFsuX6ZwAworxTg02/1qiDu7zW7RyzHvFYA4IAjpzvkPIa\nX6KjBtpdvp/aXabmL95YgBjT8WJ7pqOfrqhpcmOBZa6Cg6O1l4qbIFH/Gj9hQB5I\n0Gs4+eH6F9h3SojmPTYkT+8KuZ9w84Mn+M8qBXUQoYoKgIjN\n-----END CERTIFICATE-----\n";
        private readonly string _serviceAccountJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNzA4MzQ1MTIzLCJleHAiOjE3MDgzNTUxMjN9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        [Fact]
        public async Task AuthMethod_CreateDelete()
        {
            var authMethodEntry = new AuthMethodEntry
            {
                Name = "AuthMethodCreateDeleteTest",
                Type = "kubernetes",
                Description = "Auth Method for API Unit Testing",
                Config = new Dictionary<string, object>
                {
                    ["Host"] = _host,
                    ["CACert"] = _caCert,
                    ["ServiceAccountJWT"] = _serviceAccountJWT
                }
            };

            var newAuthMethodResult = await _client.AuthMethod.Create(authMethodEntry);
            Assert.NotNull(newAuthMethodResult.Response);
            Assert.NotEqual(TimeSpan.Zero, newAuthMethodResult.RequestTime);
            Assert.False(string.IsNullOrEmpty(newAuthMethodResult.Response.Name));
            Assert.Equal(authMethodEntry.Name, newAuthMethodResult.Response.Name);
            Assert.Equal(authMethodEntry.Description, newAuthMethodResult.Response.Description);

            var deleteResult = await _client.AuthMethod.Delete(newAuthMethodResult.Response.Name);
            Assert.True(deleteResult.Response);
        }

        [Fact]
        public async Task AuthMethod_CreateUpdateDelete()
        {
            var authMethodEntry = new AuthMethodEntry
            {
                Name = "AuthMethodUpdateTest",
                Type = "kubernetes",
                Description = "Auth Method for API Unit Testing",
                Config = new Dictionary<string, object>
                {
                    ["Host"] = _host,
                    ["CACert"] = _caCert,
                    ["ServiceAccountJWT"] = _serviceAccountJWT
                }
            };

            var newAuthMethodResult = await _client.AuthMethod.Create(authMethodEntry);
            Assert.NotNull(newAuthMethodResult.Response);

            authMethodEntry.Description = "Updated Auth Method Description";
            var updatedAuthMethodResult = await _client.AuthMethod.Update(authMethodEntry);
            Assert.NotNull(updatedAuthMethodResult.Response);
            Assert.Equal("Updated Auth Method Description", updatedAuthMethodResult.Response.Description);

            var deleteResult = await _client.AuthMethod.Delete(authMethodEntry.Name);
            Assert.True(deleteResult.Response);
        }

        [Fact]
        public async Task AuthMethod_CreateReadDelete()
        {
            var authMethodEntry = new AuthMethodEntry
            {
                Name = "AuthMethodReadTest",
                Type = "kubernetes",
                Description = "Auth Method for API Unit Testing",
                Config = new Dictionary<string, object>
                {
                    ["Host"] = _host,
                    ["CACert"] = _caCert,
                    ["ServiceAccountJWT"] = _serviceAccountJWT
                }
            };

            var newAuthMethodResult = await _client.AuthMethod.Create(authMethodEntry);
            Assert.NotNull(newAuthMethodResult.Response);

            var readResult = await _client.AuthMethod.Read(authMethodEntry.Name);
            Assert.NotNull(readResult.Response);
            Assert.Equal(authMethodEntry.Name, readResult.Response.Name);
            Assert.Equal(authMethodEntry.Type, readResult.Response.Type);
            Assert.Equal(authMethodEntry.Description, readResult.Response.Description);

            var deleteResult = await _client.AuthMethod.Delete(authMethodEntry.Name);
            Assert.True(deleteResult.Response);
        }

        [Fact]
        public async Task AuthMethod_List()
        {
            var authMethodEntry = new AuthMethodEntry
            {
                Name = "AuthMethodListTest",
                Type = "kubernetes",
                Description = "Auth Method for API Unit Testing",
                Config = new Dictionary<string, object>
                {
                    ["Host"] = _host,
                    ["CACert"] = _caCert,
                    ["ServiceAccountJWT"] = _serviceAccountJWT
                }
            };

            var newAuthMethodResult = await _client.AuthMethod.Create(authMethodEntry);
            Assert.NotNull(newAuthMethodResult.Response);

            var listResult = await _client.AuthMethod.List();
            Assert.NotNull(listResult.Response);
            Assert.NotEmpty(listResult.Response);
            var existing = listResult.Response.FirstOrDefault(a => a.Name == authMethodEntry.Name);
            Assert.NotNull(existing);

            var deleteResult = await _client.AuthMethod.Delete(authMethodEntry.Name);
            Assert.True(deleteResult.Response);

            var listResult2 = await _client.AuthMethod.List();
            var deleted = listResult2.Response?.FirstOrDefault(a => a.Name == authMethodEntry.Name);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task AuthMethod_Login()
        {
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

            var bindingRule = new ACLBindingRule
            {
                AuthMethod = authMethodEntry.Name,
                BindType = "service",
                BindName = "web",
                Selector = ""
            };
            await _client.BindingRule.Create(bindingRule);

            try
            {
                var loginResult = await _client.AuthMethod.Login(authMethodEntry.Name, jwt);
                Assert.NotNull(loginResult.Response);
                Assert.Equal(authMethodEntry.Name, loginResult.Response.AuthMethod);
                Assert.False(string.IsNullOrEmpty(loginResult.Response.AccessorID));
                Assert.False(string.IsNullOrEmpty(loginResult.Response.SecretID));
            }
            finally
            {
                await _client.AuthMethod.Delete(authMethodEntry.Name);
            }
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
