// -----------------------------------------------------------------------
//  <copyright file="Token.cs" company="G-Research Limited">
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
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consul
{
    /// <summary>
    /// TokenEntry is used to represent an ACL Token in Consul
    /// </summary>
    public class TokenEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AccessorID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SecretID { get; set; }
        public string Description { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PolicyLink[] Policies { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RoleLink[] Roles { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ServiceIdentity[] ServiceIdentities { get; set; }
        public bool Local { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AuthMethod { get; set; }


        public static bool ShouldSerializeCreateIndex()
        {
            return false;
        }

        public static bool ShouldSerializeModifyIndex()
        {
            return false;
        }

        public TokenEntry()
            : this(string.Empty, string.Empty, Array.Empty<PolicyLink>(), Array.Empty<RoleLink>(), Array.Empty<ServiceIdentity>())
        {
        }

        public TokenEntry(string description, PolicyLink[] policies)
            : this(string.Empty, description, policies, Array.Empty<RoleLink>(), Array.Empty<ServiceIdentity>())
        {
        }

        public TokenEntry(string description, RoleLink[] roles)
            : this(string.Empty, description, Array.Empty<PolicyLink>(), roles, Array.Empty<ServiceIdentity>())
        {
        }

        public TokenEntry(string description, ServiceIdentity[] serviceIdentities)
            : this(string.Empty, description, Array.Empty<PolicyLink>(), Array.Empty<RoleLink>(), serviceIdentities)
        {
        }

        public TokenEntry(string accessorId, string description)
            : this(accessorId, description, Array.Empty<PolicyLink>(), Array.Empty<RoleLink>(), Array.Empty<ServiceIdentity>())
        {
        }

        public TokenEntry(string accessorId, string description, PolicyLink[] policies, RoleLink[] roles, ServiceIdentity[] serviceIdentities)
        {
            AccessorID = accessorId;
            Description = description;
            Policies = policies;
            Roles = roles;
            ServiceIdentities = serviceIdentities;
        }
    }

    /// <summary>
    /// Token is used to interact with ACL Tokens in Consul through the API
    /// </summary>
    public class Token : ITokenEndpoint
    {
        private readonly ConsulClient _client;

        internal Token(ConsulClient c)
        {
            _client = c;
        }

        private class TokenActionResult : TokenEntry
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public DateTime? CreateTime { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Hash { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public ulong CreateIndex { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public ulong ModifyIndex { get; set; }
        }

        /// <summary>
        /// Creates the initial Management ACL Token in Consul
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL Token</returns>
        public Task<WriteResult<TokenEntry>> Bootstrap(CancellationToken ct = default)
        {
            return Bootstrap(WriteOptions.Default, ct);
        }

        /// <summary>
        /// Creates the initial Management ACL Token in Consul
        /// </summary>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL Token</returns>
        public async Task<WriteResult<TokenEntry>> Bootstrap(WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.PutReturning<TokenActionResult>("/v1/acl/bootstrap", writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<TokenEntry>(res, res.Response);
        }

        /// <summary>
        /// Creates a new ACL Token in Consul
        /// </summary>
        /// <param name="token">The new ACL Token</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL Token</returns>
        public Task<WriteResult<TokenEntry>> Create(TokenEntry token, CancellationToken ct = default)
        {
            return Create(token, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Creates a new ACL Token in Consul
        /// </summary>
        /// <param name="token">The new ACL Token</param>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL Token</returns>
        public async Task<WriteResult<TokenEntry>> Create(TokenEntry token, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.Put<TokenEntry, TokenActionResult>("/v1/acl/token", token, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<TokenEntry>(res, res.Response);
        }

        /// <summary>
        /// Updates an existing ACL Token in Consul
        /// </summary>
        /// <param name="token">The modified ACL Token</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the updated ACL Token</returns>
        public Task<WriteResult<TokenEntry>> Update(TokenEntry token, CancellationToken ct = default)
        {
            return Update(token, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Updates an existing ACL Token in Consul
        /// </summary>
        /// <param name="token">The modified ACL Token</param>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the updated ACL Token</returns>
        public async Task<WriteResult<TokenEntry>> Update(TokenEntry token, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.Put<TokenEntry, TokenActionResult>($"/v1/acl/token/{token.AccessorID}", token, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<TokenEntry>(res, res.Response);
        }

        /// <summary>
        /// Deletes an existing ACL Token from Consul
        /// </summary>
        /// <param name="id">The Accessor ID of the ACL Token to delete</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Success (true) or failure (false)</returns>
        public Task<WriteResult<bool>> Delete(string id, CancellationToken ct = default)
        {
            return Delete(id, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Deletes an existing ACL Token from Consul
        /// </summary>
        /// <param name="id">The Accessor ID of the ACL Token to delete</param>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Success (true) or failure (false)</returns>
        public Task<WriteResult<bool>> Delete(string id, WriteOptions writeOptions, CancellationToken ct = default)
        {
            return _client.DeleteReturning<bool>($"/v1/acl/token/{id}", writeOptions).Execute(ct);
        }

        /// <summary>
        /// Clones an existing ACL Token in Consul
        /// </summary>
        /// <param name="id">The Accessor ID of the ACL Token to clone</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL token</returns>
        public Task<WriteResult<TokenEntry>> Clone(string id, CancellationToken ct = default)
        {
            return Clone(id, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Clones an existing ACL Token in Consul
        /// </summary>
        /// <param name="id">The Accessor ID of the ACL Token to clone</param>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL token</returns>
        public Task<WriteResult<TokenEntry>> Clone(string id, WriteOptions writeOptions, CancellationToken ct = default)
        {
            return Clone(id, string.Empty, writeOptions, ct);
        }

        /// <summary>
        /// Clones an existing ACL Token in Consul
        /// </summary>
        /// <param name="id">The Accessor ID of the ACL Token to clone</param>
        /// <param name="description">The description for the cloned ACL Token</param>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL token</returns>
        public async Task<WriteResult<TokenEntry>> Clone(string id, string description, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var body = new Dictionary<string, string>
            {
                {"Description", description}
            };

            var res = await _client.Put<object, TokenActionResult>($"/v1/acl/token/{id}/clone", body, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<TokenEntry>(res, res.Response);
        }

        /// <summary>
        /// Gets an existing ACL Token from Consul
        /// </summary>
        /// <param name="id">The Accessor ID of the ACL Token to get</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Token</returns>
        public Task<QueryResult<TokenEntry>> Read(string id, CancellationToken ct = default)
        {
            return Read(id, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Gets an existing ACL Token from Consul
        /// </summary>
        /// <param name="id">The Accessor ID of the ACL Token to get</param>
        /// <param name="queryOptions">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Token</returns>
        public async Task<QueryResult<TokenEntry>> Read(string id, QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<TokenActionResult>($"/v1/acl/token/{id}", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<TokenEntry>(res, res.Response);
        }

        /// <summary>
        /// Lists the existing ACL Tokens in Consul
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing an array of ACL Tokens</returns>
        public Task<QueryResult<TokenEntry[]>> List(CancellationToken ct = default)
        {
            return List(QueryOptions.Default, ct);
        }

        /// <summary>
        /// Lists the existing ACL Tokens in Consul
        /// </summary>
        /// <param name="queryOptions">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing an array of ACL Tokens</returns>
        public Task<QueryResult<TokenEntry[]>> List(QueryOptions queryOptions, CancellationToken ct = default)
        {
            return _client.Get<TokenEntry[]>("/v1/acl/tokens", queryOptions).Execute(ct);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Token> _token;

        /// <summary>
        /// Token returns a handle to the ACL Token endpoints
        /// </summary>
        public ITokenEndpoint Token => _token.Value;
    }
}
