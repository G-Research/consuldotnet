// -----------------------------------------------------------------------
//  <copyright file="Policy.cs" company="G-Research Limited">
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consul
{
    /// <summary>
    /// PolicyLink is the base for representing an ACL Policy in Consul
    /// </summary>
    public class PolicyLink
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public PolicyLink()
        : this(string.Empty, string.Empty)
        {
        }
        public PolicyLink(string id)
        : this(id, string.Empty)
        {
        }
        public PolicyLink(string id, string name)
        {
            ID = id;
            Name = name;
        }
    }

    /// <summary>
    /// PolicyEntry represents an ACL Policy in Consul
    /// </summary>
    public class PolicyEntry
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Rules { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Datacenters { get; set; }

        public static bool ShouldSerializeCreateIndex()
        {
            return false;
        }

        public static bool ShouldSerializeModifyIndex()
        {
            return false;
        }

        public PolicyEntry()
            : this(string.Empty, string.Empty, string.Empty, string.Empty, new string[] { })
        {
        }

        public PolicyEntry(string id)
            : this(id, string.Empty, string.Empty, string.Empty, new string[] { })
        {
        }

        public PolicyEntry(string id, string name)
            : this(id, name, string.Empty, string.Empty, new string[] { })
        {
        }

        public PolicyEntry(string id, string name, string description, string rules, string[] datacenters)
        {
            ID = id;
            Name = name;
            Description = description;
            Rules = rules;
            Datacenters = datacenters;
        }
        public static implicit operator PolicyLink(PolicyEntry p) => new PolicyLink(p.ID);
    }
    /// <summary>
    /// Handles responses for templated policies
    /// </summary>
    public class TemplatedPolicyResponse
    {
        public string TemplateName { get; set; }
        public string Schema { get; set; }
        public string Template { get; set; }
    }

    /// <summary>
    /// Policy is used to interact with ACL Policies in Consul through the API
    /// </summary>
    public class Policy : IPolicyEndpoint
    {
        private readonly ConsulClient _client;

        internal Policy(ConsulClient c)
        {
            _client = c;
        }

        private class PolicyActionResult : PolicyEntry
        {
            public string Hash { get; set; }
            public ulong CreateIndex { get; set; }
            public ulong ModifyIndex { get; set; }
        }

        /// <summary>
        /// Creates a new ACL Policy in Consul
        /// </summary>
        /// <param name="policy">The new ACL PolicyEntry</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL Policy</returns>
        public Task<WriteResult<PolicyEntry>> Create(PolicyEntry policy, CancellationToken ct)
        {
            return Create(policy, WriteOptions.Default, ct);
        }

        public Task<WriteResult<PolicyEntry>> Create(PolicyEntry policy)
        {
            return Create(policy, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new ACL Policy in Consul
        /// </summary>
        /// <param name="policy">The new ACL PolicyEntry</param>
        /// <param name="writeOptions">Customised write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL Policy</returns>
        public async Task<WriteResult<PolicyEntry>> Create(PolicyEntry policy, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.Put<PolicyEntry, PolicyActionResult>("/v1/acl/policy", policy, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<PolicyEntry>(res, res.Response);
        }

        /// <summary>
        /// Deletes an existing ACL Policy in Consul
        /// </summary>
        /// <param name="id">The ID of the ACL Policy to delete</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Success (true) or failure (false)</returns>
        public Task<WriteResult<bool>> Delete(string id, CancellationToken ct)
        {
            return Delete(id, WriteOptions.Default, ct);
        }

        public Task<WriteResult<bool>> Delete(string id)
        {
            return Delete(id, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Deletes an existing ACL Policy in Consul
        /// </summary>
        /// <param name="id">The ID of the ACL Policy to delete</param>
        /// <param name="writeOptions">Customised write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Success (true) or failure (false)</returns>
        public async Task<WriteResult<bool>> Delete(string id, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.DeleteReturning<bool>($"/v1/acl/policy/{id}", writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<bool>(res, res.Response);
        }

        /// <summary>
        /// Lists the existing ACL Policies in Consul
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing an array of ACL Policies</returns>
        public Task<QueryResult<PolicyEntry[]>> List(CancellationToken ct)
        {
            return List(QueryOptions.Default, ct);
        }

        public Task<QueryResult<PolicyEntry[]>> List()
        {
            return List(QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Lists the existing ACL Policies in Consul
        /// </summary>
        /// <param name="queryOptions">Customised query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing an array of ACL Policies</returns>
        public async Task<QueryResult<PolicyEntry[]>> List(QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<PolicyEntry[]>("/v1/acl/policies", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<PolicyEntry[]>(res, res.Response);
        }

        /// <summary>
        /// Gets the requested ACL Policy from Consul
        /// </summary>
        /// <param name="id">The ID of the ACL Policy to get</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Policy</returns>
        public Task<QueryResult<PolicyEntry>> Read(string id, CancellationToken ct)
        {
            return Read(id, QueryOptions.Default, ct);
        }

        public Task<QueryResult<PolicyEntry>> Read(string id)
        {
            return Read(id, QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Gets the requested ACL Policy from Consul
        /// </summary>
        /// <param name="id">The ID of the ACL Policy to get</param>
        /// <param name="queryOptions">Customised query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Policy</returns>
        public async Task<QueryResult<PolicyEntry>> Read(string id, QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<PolicyEntry>($"/v1/acl/policy/{id}", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<PolicyEntry>(res, res.Response);
        }

        /// <summary>
        /// Updates an existing ACL Policy in Consul
        /// </summary>
        /// <param name="policy">The modified ACL Policy</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the updated ACL Policy</returns>
        public Task<WriteResult<PolicyEntry>> Update(PolicyEntry policy, CancellationToken ct)
        {
            return Update(policy, WriteOptions.Default, ct);
        }

        public Task<WriteResult<PolicyEntry>> Update(PolicyEntry policy)
        {
            return Update(policy, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Updates an existing ACL Policy in Consul
        /// </summary>
        /// <param name="policy">The modified ACL Policy</param>
        /// <param name="writeOptions">Customised write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the updated ACL policy</returns>
        public async Task<WriteResult<PolicyEntry>> Update(PolicyEntry policy, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.Put<PolicyEntry, PolicyActionResult>($"/v1/acl/policy/{policy.ID}", policy, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<PolicyEntry>(res, res.Response);
        }

        /// <summary>
        /// Shows the policy created from a templated policy.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <param name="ct"></param>
        /// <returns>An ACL Policy</returns>
        public async Task<WriteResult<PolicyEntry>> PreviewTemplatedPolicy(string name, WriteOptions options, CancellationToken ct = default)
        {
            var res = await _client.Post<string, PolicyEntry>($"/v1/acl/templated-policy/preview/{name}", null, options).Execute(ct).ConfigureAwait(false);
            return new WriteResult<PolicyEntry>(res, res.Response);
        }

        /// <summary>
        /// Shows the policy created from a templated policy.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ct"></param>
        /// <returns>An ACL Policy</returns>
        public Task<WriteResult<PolicyEntry>> PreviewTemplatedPolicy(string name, CancellationToken ct)
        {
            return PreviewTemplatedPolicy(name, WriteOptions.Default, ct);
        }

        public Task<WriteResult<PolicyEntry>> PreviewTemplatedPolicy(string name)
        {
            return PreviewTemplatedPolicy(name, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Returns a list of all templated policies
        /// </summary>
        /// <param name="q"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<QueryResult<Dictionary<string, TemplatedPolicyResponse>>> ListTemplatedPolicies(QueryOptions q, CancellationToken ct = default)
        {
            var res = _client.Get<Dictionary<string, TemplatedPolicyResponse>>("v1/acl/templated-policies", q);
            return res.Execute(ct);
        }

        /// <summary>
        /// Returns a list of all templated policies
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<QueryResult<Dictionary<string, TemplatedPolicyResponse>>> ListTemplatedPolicies(CancellationToken ct)
        {
            return ListTemplatedPolicies(QueryOptions.Default, ct);
        }

        public Task<QueryResult<Dictionary<string, TemplatedPolicyResponse>>> ListTemplatedPolicies()
        {
            return ListTemplatedPolicies(QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Reads an ACL policy with the given ID or name parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="q"></param>
        /// <param name="ct"></param>
        /// <returns>Returns an ACL Policy</returns>
        public async Task<QueryResult<TemplatedPolicyResponse>> ReadTemplatedPolicyByName(string name, QueryOptions q, CancellationToken ct = default)
        {
            var res = await _client.Get<TemplatedPolicyResponse>($"v1/acl/templated-policy/name/{name}", q).Execute(ct).ConfigureAwait(false);
            return new QueryResult<TemplatedPolicyResponse>(res, res.Response);
        }

        /// <summary>
        /// Reads an ACL policy with the given ID or name parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ct"></param>
        /// <returns>Returns an ACL Policy</returns>
        public Task<QueryResult<TemplatedPolicyResponse>> ReadTemplatedPolicyByName(string name, CancellationToken ct)
        {
            return ReadTemplatedPolicyByName(name, QueryOptions.Default, ct);
        }

        public Task<QueryResult<TemplatedPolicyResponse>> ReadTemplatedPolicyByName(string name)
        {
            return ReadTemplatedPolicyByName(name, QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Gets the requested ACL Policy from Consul
        /// </summary>
        /// <param name="name">The name of the ACL Policy to get</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Policy</returns>
        public Task<QueryResult<PolicyEntry>> ReadPolicyByName(string name, CancellationToken ct)
        {
            return ReadPolicyByName(name, QueryOptions.Default, ct);
        }

        public Task<QueryResult<PolicyEntry>> ReadPolicyByName(string name)
        {
            return ReadPolicyByName(name, QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Gets the requested ACL Policy from Consul
        /// </summary>
        /// <param name="name">The name of the ACL Policy to get</param>
        /// <param name="queryOptions">Customised query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Policy</returns>
        public async Task<QueryResult<PolicyEntry>> ReadPolicyByName(string name, QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<PolicyEntry>($"/v1/acl/policy/name/{name}", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<PolicyEntry>(res, res.Response);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        /// <summary>
        /// Policy returns a handle to the ACL Policy endpoints
        /// </summary>
        public IPolicyEndpoint Policy { get; private set; }
    }
}
