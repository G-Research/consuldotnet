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

using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class BindingRuleTest : BaseFixture
    {
        [SkippableFact]
        public async Task BindingRule_Create()
        {
            var authMethodsResponse = await _client.AuthMethod.List();
            var existingAuthMethod = authMethodsResponse.Response?.FirstOrDefault();
            Skip.If(existingAuthMethod == null, "No auth methods available in Consul for testing");
            var BindingRuleEntry = new BindingRuleEntry
            {
                Description = "ACL Binding Rule for API Unit Testing",
                AuthMethod = existingAuthMethod.Name,
                Selector = "serviceaccount.namespace==default",
                BindType = "service",
                BindName = "{{ serviceaccount.name }}"
            };

            var newBindingRuleResult = await _client.BindingRule.Create(BindingRuleEntry);
            Assert.NotNull(newBindingRuleResult.Response);
            Assert.Equal(BindingRuleEntry.Description, newBindingRuleResult.Response.Description);
            Assert.Equal(BindingRuleEntry.AuthMethod, newBindingRuleResult.Response.AuthMethod);
            Assert.False(string.IsNullOrEmpty(newBindingRuleResult.Response.ID));
        }
    }
}
