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
#pragma warning disable RS0026
using System.Threading;
using System.Threading.Tasks;
using Consul.Interfaces;
using Newtonsoft.Json;

namespace Consul
{
    public class ACLTemplatedPolicyVariables
    {
        public string Name { get; set; }
    }

    public class ACLBindingRule
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public string AuthMethod { get; set; }
        public string Selector { get; set; }
        public string BindType { get; set; }
        public string BindName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ACLTemplatedPolicyVariables BindVars { get; set; }
    }

    public class ACLBindingRuleResponse : ACLBindingRule
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ulong CreateIndex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ulong ModifyIndex { get; set; }
    }

    /// <summary>
    /// BindingRule is used to interact with ACL Binding Rules in Consul through the API
    /// </summary>
    public class BindingRule : IBindingRuleEndpoint
    {
        private readonly ConsulClient _client;

        internal BindingRule(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// Creates a new ACL BindingRule in Consul
        /// </summary>
        /// <param name="entry">The new ACL AuthMethod</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL AuthMethod</returns>
        public Task<WriteResult<ACLBindingRuleResponse>> Create(ACLBindingRule entry, CancellationToken ct = default)
        {
            return Create(entry, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Creates a new ACL BindingRule in Consul
        /// </summary>
        /// <param name="entry">A new Binding Rule Entry</param>
        /// <param name="options"></param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A new Binding Rule</returns>
        public async Task<WriteResult<ACLBindingRuleResponse>> Create(ACLBindingRule entry, WriteOptions options,
            CancellationToken ct = default)
        {
            var res = await _client.Put<ACLBindingRule, ACLBindingRuleResponse>("/v1/acl/binding-rule", entry, options)
                .Execute(ct).ConfigureAwait(false);
            return new WriteResult<ACLBindingRuleResponse>(res, res.Response);
        }

        /// <summary>
        /// Reads a binding rule in Consul
        /// </summary>
        /// <param name="id">UUID of the binding rule to read</param>
        /// <param name="ct">>Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>An existing Binding rule with the specified id</returns>
        public Task<QueryResult<ACLBindingRuleResponse>> Read(string id, CancellationToken ct = default)
        {
            return Read(id, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Reads an ACL binding rule in Consul
        /// </summary>
        /// <param name="id">UUID of the ACL binding rule to read</param>
        /// <param name="options"></param>
        /// <param name="ct">>Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>An existing Binding rule with the specified id</returns>
        public async Task<QueryResult<ACLBindingRuleResponse>> Read(string id, QueryOptions options, CancellationToken ct = default)
        {
            var res = await _client.Get<ACLBindingRuleResponse>($"/v1/acl/binding-rule/{id}").Execute(ct).ConfigureAwait(false);
            return new QueryResult<ACLBindingRuleResponse>(res, res.Response);
        }

        /// <summary>
        /// Updates an existing ACL binding rule
        /// </summary>
        /// <param name="entry">Specifies the ACL binding rule you update</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Returns the updated Binding Rule</returns>
        public Task<WriteResult<ACLBindingRuleResponse>> Update(ACLBindingRule entry, CancellationToken ct = default)
        {
            return Update(entry, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Updates an existing ACL binding rule
        /// </summary>
        /// <param name="entry">Specifies the ACL binding rule you update</param>
        /// <param name="options"></param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Returns the updated Binding Rule</returns>
        public async Task<WriteResult<ACLBindingRuleResponse>> Update(ACLBindingRule entry, WriteOptions options, CancellationToken ct = default)
        {
            var res = await _client.Put<ACLBindingRule, ACLBindingRuleResponse>($"/v1/acl/binding-rule/{entry.ID}", entry, options).Execute(ct).ConfigureAwait(false);
            return new WriteResult<ACLBindingRuleResponse>(res, res.Response);
        }

        /// <summary>
        /// Deletes an ACL binding rule.
        /// </summary>
        /// <param name="id">Specifies the UUID of the binding rule you delete</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns></returns>
        public Task<WriteResult> Delete(string id, CancellationToken ct = default)
        {
            return Delete(id, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Deletes an ACL binding rule.
        /// </summary>
        /// <param name="id">Specifies the UUID of the binding rule you delete</param>
        /// <param name="options"></param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns></returns>
        public async Task<WriteResult> Delete(string id, WriteOptions options, CancellationToken ct = default)
        {
            var res = await _client.Delete($"/v1/acl/binding-rule/{id}", options).Execute(ct).ConfigureAwait(false);
            return res;
        }

        /// <summary>
        /// Retrieves a listing of all binding rules
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A lists of all the ACL binding rules</returns>
        public Task<QueryResult<ACLBindingRuleResponse[]>> List(CancellationToken ct = default)
        {
            return List(QueryOptions.Default, ct);
        }

        /// <summary>
        /// Retrieves a listing of all binding rules
        /// </summary>
        /// <param name="options"></param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A lists of all the ACL binding rules</returns>
        public async Task<QueryResult<ACLBindingRuleResponse[]>> List(QueryOptions options, CancellationToken ct = default)
        {
            var res = await _client.Get<ACLBindingRuleResponse[]>("/v1/acl/binding-rules").Execute(ct).ConfigureAwait(false);
            return new QueryResult<ACLBindingRuleResponse[]>(res, res.Response);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        /// <summary>
        /// ACL Binding Rule returns a handle to the ACL Binding Rules endpoints
        /// </summary>
        public IBindingRuleEndpoint BindingRule { get; private set; }
    }
}
