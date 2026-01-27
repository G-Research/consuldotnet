// -----------------------------------------------------------------------
//  <copyright file="BindingRuleTest.cs" company="G-Research Limited">
//    Copyright 2020 G-Research Limited
//
//    Licensed under the Apache License, Version 2.0 (the "License"),
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class BindingRuleTest : BaseFixture
    {
        private readonly string _host = "https://192.0.2.42:8443";
        private readonly string _caCert = "-----BEGIN CERTIFICATE-----\nMIIHQDCCBiigAwIBAgIQD9B43Ujxor1NDyupa2A4/jANBgkqhkiG9w0BAQsFADBN\nMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMScwJQYDVQQDEx5E\naWdpQ2VydCBTSEEyIFNlY3VyZSBTZXJ2ZXIgQ0EwHhcNMTgxMTI4MDAwMDAwWhcN\nMjAxMjAyMTIwMDAwWjCBpTELMAkGA1UEBhMCVVMxEzARBgNVBAgTCkNhbGlmb3Ju\naWExFDASBgNVBAcTC0xvcyBBbmdlbGVzMTwwOgYDVQQKEzNJbnRlcm5ldCBDb3Jw\nb3JhdGlvbiBmb3IgQXNzaWduZWQgTmFtZXMgYW5kIE51bWJlcnMxEzARBgNVBAsT\nClRlY2hub2xvZ3kxGDAWBgNVBAMTD3d3dy5leGFtcGxlLm9yZzCCASIwDQYJKoZI\nhvcNAQEBBQADggEPADCCAQoCggEBANDwEnSgliByCGUZElpdStA6jGaPoCkrp9vV\nrAzPpXGSFUIVsAeSdjF11yeOTVBqddF7U14nqu3rpGA68o5FGGtFM1yFEaogEv5g\nrJ1MRY/d0w4+dw8JwoVlNMci+3QTuUKf9yH28JxEdG3J37Mfj2C3cREGkGNBnY80\neyRJRqzy8I0LSPTTkhr3okXuzOXXg38ugr1x3SgZWDNuEaE6oGpyYJIBWZ9jF3pJ\nQnucP9vTBejMh374qvyd0QVQq3WxHrogy4nUbWw3gihMxT98wRD1oKVma1NTydvt\nhcNtBfhkp8kO64/hxLHrLWgOFT/l4tz8IWQt7mkrBHjbd2XLVPkCAwEAAaOCA8Ew\nggO9MB8GA1UdIwQYMBaAFA+AYRyCMWHVLyjnjUY4tCzhxtniMB0GA1UdDgQWBBRm\nmGIC4AmRp9njNvt2xrC/oW2nvjCBgQYDVR0RBHoweIIPd3d3LmV4YW1wbGUub3Jn\nggtleGFtcGxlLmNvbYILZXhhbXBsZS5lZHWCC2V4YW1wbGUubmV0ggtleGFtcGxl\nLm9yZ4IPd3d3LmV4YW1wbGUuY29tgg93d3cuZXhhbXBsZS5lZHWCD3d3dy5leGFt\ncGxlLm5ldDAOBgNVHQ8BAf8EBAMCBaAwHQYDVR0lBBYwFAYIKwYBBQUHAwEGCCsG\nAQUFBwMCMGsGA1UdHwRkMGIwL6AtoCuGKWh0dHA6Ly9jcmwzLmRpZ2ljZXJ0LmNv\nbS9zc2NhLXNoYTItZzYuY3JsMC+gLaArhilodHRwOi8vY3JsNC5kaWdpY2VydC5j\nb20vc3NjYS1zaGEyLWc2LmNybDBMBgNVHSAERTBDMDcGCWCGSAGG/WwBATAqMCgG\nCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAgGBmeBDAEC\nAjB8BggrBgEFBQcBAQRwMG4wJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2lj\nZXJ0LmNvbTBGBggrBgEFBQcwAoY6aHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29t\nL0RpZ2lDZXJ0U0hBMlNlY3VyZVNlcnZlckNBLmNydDAMBgNVHRMBAf8EAjAAMIIB\nfwYKKwYBBAHWeQIEAgSCAW8EggFrAWkAdwCkuQmQtBhYFIe7E6LMZ3AKPDWYBPkb\n37jjd80OyA3cEAAAAWdcMZVGAAAEAwBIMEYCIQCEZIG3IR36Gkj1dq5L6EaGVycX\nsHvpO7dKV0JsooTEbAIhALuTtf4wxGTkFkx8blhTV+7sf6pFT78ORo7+cP39jkJC\nAHYAh3W/51l8+IxDmV+9827/Vo1HVjb/SrVgwbTq/16ggw8AAAFnXDGWFQAABAMA\nRzBFAiBvqnfSHKeUwGMtLrOG3UGLQIoaL3+uZsGTX3MfSJNQEQIhANL5nUiGBR6g\nl0QlCzzqzvorGXyB/yd7nttYttzo8EpOAHYAb1N2rDHwMRnYmQCkURX/dxUcEdkC\nwQApBo2yCJo32RMAAAFnXDGWnAAABAMARzBFAiEA5Hn7Q4SOyqHkT+kDsHq7ku7z\nRDuM7P4UDX2ft2Mpny0CIE13WtxJAUr0aASFYZ/XjSAMMfrB0/RxClvWVss9LHKM\nMA0GCSqGSIb3DQEBCwUAA4IBAQBzcIXvQEGnakPVeJx7VUjmvGuZhrr7DQOLeP4R\n8CmgDM1pFAvGBHiyzvCH1QGdxFl6cf7wbp7BoLCRLR/qPVXFMwUMzcE1GLBqaGZM\nv1Yh2lvZSLmMNSGRXdx113pGLCInpm/TOhfrvr0TxRImc8BdozWJavsn1N2qdHQu\nN+UBO6bQMLCD0KHEdSGFsuX6ZwAworxTg02/1qiDu7zW7RyzHvFYA4IAjpzvkPIa\nX6KjBtpdvp/aXabmL95YgBjT8WJ7pqOfrqhpcmOBZa6Cg6O1l4qbIFH/Gj9hQB5I\n0Gs4+eH6F9h3SojmPTYkT+8KuZ9w84Mn+M8qBXUQoYoKgIjN\n-----END CERTIFICATE-----\n";
        private readonly string _serviceAccountJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNzA4MzQ1MTIzLCJleHAiOjE3MDgzNTUxMjN9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        [SkippableFact]
        public async Task BindingRule_Create()
        {
            var authMethodEntry = new AuthMethodEntry
            {
                Name = "AuthMethodApiTest",
                Type = "kubernetes",
                Description = "Auth Method for API Unit Testing",
                Config = new Dictionary<string, string>
                {
                    ["Host"] = _host,
                    ["CACert"] = _caCert,
                    ["ServiceAccountJWT"] = _serviceAccountJWT
                }
            };
            await _client.AuthMethod.Create(authMethodEntry);
            var authMethodsResponse = await _client.AuthMethod.List();
            var existingAuthMethod = authMethodsResponse.Response?.FirstOrDefault();
            Skip.If(existingAuthMethod == null, "No auth methods available in Consul for testing");
            var BindingRuleEntry = new ACLBindingRule
            {
                Description = "ACL Binding Rule for API Unit Testing",
                AuthMethod = existingAuthMethod.Name,
                Selector = "serviceaccount.namespace==default",
                BindType = "service",
                BindName = "${serviceaccount.name}"
            };
            var newBindingRuleResult = await _client.BindingRule.Create(BindingRuleEntry);
            Assert.NotNull(newBindingRuleResult.Response);
            Assert.Equal(BindingRuleEntry.Description, newBindingRuleResult.Response.Description);
            Assert.Equal(BindingRuleEntry.AuthMethod, newBindingRuleResult.Response.AuthMethod);
            Assert.False(string.IsNullOrEmpty(newBindingRuleResult.Response.ID));
        }

        [SkippableFact]
        public async Task BindingRule_Read()
        {
            var authMethodEntry = new AuthMethodEntry
            {
                Name = "AuthMethodApiTest",
                Type = "kubernetes",
                Description = "Auth Method for API Unit Testing",
                Config = new Dictionary<string, string>
                {
                    ["Host"] = _host,
                    ["CACert"] = _caCert,
                    ["ServiceAccountJWT"] = _serviceAccountJWT
                }
            };

            await _client.AuthMethod.Create(authMethodEntry);
            var authMethodsResponse = await _client.AuthMethod.List();
            var existingAuthMethod = authMethodsResponse.Response?.FirstOrDefault();

            Skip.If(existingAuthMethod == null, "No auth methods available in Consul for testing");

            var BindingRuleEntry = new ACLBindingRule
            {
                Description = "ACL Binding Rule for API Unit Testing",
                AuthMethod = existingAuthMethod.Name,
                Selector = "serviceaccount.namespace==default",
                BindType = "service",
                BindName = "${serviceaccount.name}"
            };
            var newBindingRuleResult = await _client.BindingRule.Create(BindingRuleEntry);

            Assert.NotNull(newBindingRuleResult.Response);
            Assert.Equal(BindingRuleEntry.Description, newBindingRuleResult.Response.Description);
            Assert.Equal(BindingRuleEntry.AuthMethod, newBindingRuleResult.Response.AuthMethod);
            Assert.False(string.IsNullOrEmpty(newBindingRuleResult.Response.ID));

            var bindingRuleById = await _client.BindingRule.Read(newBindingRuleResult.Response.ID);

            Assert.NotNull(bindingRuleById.Response);
            Assert.False(string.IsNullOrEmpty(bindingRuleById.Response.ID));
            Assert.Equal(newBindingRuleResult.Response.ID, bindingRuleById.Response.ID);
            Assert.Equal(newBindingRuleResult.Response.Description, bindingRuleById.Response.Description);

        }
    }
}
