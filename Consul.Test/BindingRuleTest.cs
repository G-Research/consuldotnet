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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Consul.Filtering;
using Xunit;

namespace Consul.Test
{
    public class BindingRuleTest : BaseFixture
    {
        [SkippableTheory]
        [InlineData("Rule-Of-Thumb")]
        [InlineData("Machiavelli's-Rule")]
        public async Task BindingRule_CreateBindingRule(string description)
        {
            var authMethodsResponse = await _client.AuthMethod.List();
            var existingAuthMethod = authMethodsResponse.Response?.FirstOrDefault();
            Skip.If(existingAuthMethod == null, "No auth methods available in Consul for testing");

            var bindingRuleEntry = new BindingRuleEntry
            {
                Description = description,
                AuthMethod = existingAuthMethod.Name,
                Selector = "serviceaccount.namespace==default",
                BindType = "service",
                BindName = "{{ serviceaccount.name }}"
            };

            var createBindingRuleRequest = await _client.BindingRule.CreateBindingRule(bindingRuleEntry);
            var response = createBindingRuleRequest.Response;

            Assert.Equal(HttpStatusCode.OK, createBindingRuleRequest.StatusCode);
            Assert.NotNull(response);
            Assert.Equal(bindingRuleEntry.BindName, response.BindName);
            Assert.Equal(bindingRuleEntry.BindType, response.BindType);
            Assert.Equal(bindingRuleEntry.AuthMethod, response.AuthMethod);
            Assert.Equal(bindingRuleEntry.Description, response.Description);
            Assert.Equal(bindingRuleEntry.Selector, response.Selector);
            Assert.True(!string.IsNullOrEmpty(response.ID));
            Assert.True(response.ModifyIndex > 0);
            Assert.True(response.CreateIndex > 0);
        }
    }
}
