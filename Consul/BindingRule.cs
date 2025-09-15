// -----------------------------------------------------------------------
//  <copyright file="BindingRule.cs" company="G-Research Limited">
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul.Interfaces;
using Newtonsoft.Json;

namespace Consul
{
    /// <summary>
    /// BindingRuleEntry represents an ACL Binding-Rule in Consul
    /// </summary>
    public class BindingRuleEntry
    {
        public string Description { get; set; }
        public string AuthMethod { get; set; }
        public string Selector { get; set; }
        public string BindType { get; set; }
        public string BindName { get; set; }
        public ACLTemplatedPolicyVariable BindVars { get; set; }
    }

    /// <summary>
    /// BindingRuleResponse represents an ACL Binding-Rule response
    /// </summary>
    public class BindingRuleResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AuthMethod { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Selector { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BindType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BindName { get; set; }
        public ulong CreateIndex { get; set; }
        public ulong ModifyIndex { get; set; }
    }

    /// <summary>
    /// BindingRule is used to interact with ACL Binding-Rules on Consul through the API
    /// </summary>
    public class BindingRule : IBindingRuleEndpoint
    {
        private readonly ConsulClient _client;

        public BindingRule(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// Creates a new binding rule
        /// </summary>
        /// <param name="entry">A new Binding Rule Entry</param>
        /// <param name="options"></param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A new Binding Rule</returns>
        public Task<WriteResult<BindingRuleResponse>> CreateBindingRule(BindingRuleEntry entry, WriteOptions options, CancellationToken ct = default)
        {
            var res = _client.Put<BindingRuleEntry, BindingRuleResponse>("v1/acl/binding-rule", entry, options);
            return res.Execute(ct);
        }

        /// <summary>
        /// Creates a new binding rule
        /// </summary>
        /// <param name="entry">A new Binding Rule Entry</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A new Binding Rule</returns>
        public Task<WriteResult<BindingRuleResponse>> CreateBindingRule(BindingRuleEntry entry, CancellationToken ct = default)
        {
            return CreateBindingRule(entry, WriteOptions.Default, ct);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<BindingRule> _bindingRule;

        /// <summary>
        /// BindingRule returns a handle to the ACL Binding-Rules endpoints
        /// </summary>
        public IBindingRuleEndpoint BindingRule => _bindingRule.Value;
    }
}
