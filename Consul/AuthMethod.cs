// -----------------------------------------------------------------------
//  <copyright file="AuthMethod.cs" company="G-Research Limited">
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
    /// AuthMethodEntry is used to represent an ACL Auth Method entry
    /// </summary>
    public class AuthMethodEntry
    {
        public string Name { get; set; }
        public string Type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Config { get; set; }

        public bool ShouldSerializeCreateIndex()
        {
            return false;
        }

        public bool ShouldSerializeModifyIndex()
        {
            return false;
        }

        public AuthMethodEntry()
            : this(string.Empty, string.Empty, string.Empty, new Dictionary<string, string>())
        {
        }

        public AuthMethodEntry(string name, string type, Dictionary<string, string> config)
            : this(name, type, string.Empty, config)
        {
        }

        public AuthMethodEntry(string name, string type, string description, Dictionary<string, string> config)
        {
            Name = name;
            Type = type;
            Description = description;
            Config = config;
        }
    }

    /// <summary>
    /// AuthMethod can be used to query the ACL Auth Method endpoints
    /// </summary>
    public class AuthMethod : IAuthMethodEndpoint
    {
        private readonly ConsulClient _client;

        internal AuthMethod(ConsulClient c)
        {
            _client = c;
        }

        private class AuthMethodActionResult : AuthMethodEntry
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public ulong CreateIndex { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public ulong ModifyIndex { get; set; }
        }

        /// <summary>
        /// Creates a new ACL AuthMethod in Consul
        /// </summary>
        /// <param name="authMethod">The new ACL AuthMethod</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL AuthMethod</returns>
        public Task<WriteResult<AuthMethodEntry>> Create(AuthMethodEntry authMethod, CancellationToken ct = default)
        {
            return Create(authMethod, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Creates a new ACL AuthMethod in Consul
        /// </summary>
        /// <param name="authMethod">The new ACL AuthMethod</param>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL AuthMethod</returns>
        public async Task<WriteResult<AuthMethodEntry>> Create(AuthMethodEntry authMethod, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.Put<AuthMethodEntry, AuthMethodActionResult>("/v1/acl/auth-method", authMethod, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<AuthMethodEntry>(res, res.Response);
        }

        /// <summary>
        /// Updates and existing ACL AuthMethod in Consul
        /// </summary>
        /// <param name="authMethod">The modified ACL AuthMethod</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the updated ACL AuthMethod</returns>
        public Task<WriteResult<AuthMethodEntry>> Update(AuthMethodEntry authMethod, CancellationToken ct = default)
        {
            return Update(authMethod, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Updates an existing ACL AuthMethod in Consul
        /// </summary>
        /// <param name="authMethod">The modified ACL AuthMethod</param>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the updated ACL AuthMethod</returns>
        public async Task<WriteResult<AuthMethodEntry>> Update(AuthMethodEntry authMethod, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.Put<AuthMethodEntry, AuthMethodActionResult>($"/v1/acl/auth-method/{authMethod.Name}", authMethod, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<AuthMethodEntry>(res, res.Response);
        }

        /// <summary>
        /// Deletes an existing ACL AuthMethod from Consul
        /// </summary>
        /// <param name="name">The name of the ACL AuthMethod to delete</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Success (true) or failure (false)</returns>
        public Task<WriteResult<bool>> Delete(string name, CancellationToken ct = default)
        {
            return Delete(name, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Deletes an existing ACL AuthMethod from Consul
        /// </summary>
        /// <param name="name">The name of the ACL AuthMethod to delete</param>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Success (true) or failure (false)</returns>
        public Task<WriteResult<bool>> Delete(string name, WriteOptions writeOptions, CancellationToken ct = default)
        {
            return _client.DeleteReturning<bool>($"/v1/acl/auth-method/{name}", writeOptions).Execute(ct);
        }

        /// <summary>
        /// Gets an existing ACL AuthMethod from Consul
        /// </summary>
        /// <param name="name">The Name of the ACL AuthMethod to get</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL AuthMethod</returns>
        public Task<QueryResult<AuthMethodEntry>> Read(string name, CancellationToken ct = default)
        {
            return Read(name, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Gets an existing ACL AuthMethod from Consul
        /// </summary>
        /// <param name="name">The Name of the ACL AuthMethod to get</param>
        /// <param name="queryOptions">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL AuthMethod</returns>
        public async Task<QueryResult<AuthMethodEntry>> Read(string name, QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<AuthMethodActionResult>($"/v1/acl/auth-method/{name}", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<AuthMethodEntry>(res, res.Response);
        }

        /// <summary>
        /// Lists the existing ACL AuthMethods in Consul
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing an array of ACL AuthMethods</returns>
        public Task<QueryResult<AuthMethodEntry[]>> List(CancellationToken ct = default)
        {
            return List(QueryOptions.Default, ct);
        }

        /// <summary>
        /// Lists the existing ACL AuthMethods in Consul
        /// </summary>
        /// <param name="queryOptions">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing an array of ACL AuthMethods</returns>
        public Task<QueryResult<AuthMethodEntry[]>> List(QueryOptions queryOptions, CancellationToken ct = default)
        {
            return _client.Get<AuthMethodEntry[]>("/v1/acl/auth-methods", queryOptions).Execute(ct);
        }

        /// <summary>
        /// Login to ACL Auth Method
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing an ACL Token for the login</returns>
        public Task<WriteResult<TokenEntry>> Login(CancellationToken ct = default)
        {
            return Login(WriteOptions.Default, ct);
        }

        /// <summary>
        /// Login to ACL Auth Method
        /// </summary>
        /// <param name="writeOptions"></param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>>A write result containing an ACL Token for the login</returns>
        public async Task<WriteResult<TokenEntry>> Login(WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.PutReturning<TokenEntry>("/v1/acl/login", writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<TokenEntry>(res, res.Response);
        }

        /// <summary>
        /// Logout from ACL Auth Method
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result</returns>
        public Task<WriteResult> Logout(CancellationToken ct = default)
        {
            return Logout(WriteOptions.Default, ct);
        }

        /// <summary>
        /// Logout from ACL Auth Method
        /// </summary>
        /// <param name="writeOptions"></param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result</returns>
        public async Task<WriteResult> Logout(WriteOptions writeOptions, CancellationToken ct = default)
        {
            return await _client.Post($"/v1/acl/logout", null, writeOptions).Execute(ct).ConfigureAwait(false);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<AuthMethod> _authMethod;

        /// <summary>
        /// AuthMethod returns a handle to the ACL AuthMethod endpoints
        /// </summary>
        public IAuthMethodEndpoint AuthMethod
        {
            get { return _authMethod.Value; }
        }
    }
}
