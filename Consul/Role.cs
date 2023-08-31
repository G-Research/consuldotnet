// -----------------------------------------------------------------------
//  <copyright file="Role.cs" company="G-Research Limited">
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
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consul
{
    /// <summary>
    /// RoleLink is the base for representing an ACL Role in Consul
    /// </summary>
    public class RoleLink
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public RoleLink()
        : this(string.Empty, string.Empty)
        {
        }
        public RoleLink(string id)
        : this(id, string.Empty)
        {
        }
        public RoleLink(string id, string name)
        {
            ID = id;
            Name = name;
        }
    }

    /// <summary>
    /// RoleEntry represents an ACL Role in Consul
    /// </summary>
    public class RoleEntry
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PolicyLink[] Policies { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ServiceIdentity[] ServiceIdentities { get; set; }

        public static bool ShouldSerializeCreateIndex()
        {
            return false;
        }

        public static bool ShouldSerializeModifyIndex()
        {
            return false;
        }

        public RoleEntry()
            : this(string.Empty, string.Empty, string.Empty, Array.Empty<PolicyLink>(), Array.Empty<ServiceIdentity>())
        {
        }

        public RoleEntry(string name)
            : this(string.Empty, name, string.Empty, Array.Empty<PolicyLink>(), Array.Empty<ServiceIdentity>())
        {
        }

        public RoleEntry(string name, string description)
            : this(string.Empty, name, description, Array.Empty<PolicyLink>(), Array.Empty<ServiceIdentity>())
        {
        }

        public RoleEntry(string id, string name, string description)
            : this(id, name, description, Array.Empty<PolicyLink>(), Array.Empty<ServiceIdentity>())
        {
        }

        public RoleEntry(string id, string name, string description, PolicyLink[] policies)
            : this(id, name, description, policies, Array.Empty<ServiceIdentity>())
        {
        }

        public RoleEntry(string id, string name, string description, ServiceIdentity[] serviceIdentities)
            : this(id, name, description, Array.Empty<PolicyLink>(), serviceIdentities)
        {
        }

        public RoleEntry(string id, string name, string description, PolicyLink[] policies, ServiceIdentity[] serviceIdentities)
        {
            ID = id;
            Name = name;
            Description = description;
            Policies = policies;
            ServiceIdentities = serviceIdentities;
        }
        public static implicit operator RoleLink(RoleEntry r) => new RoleLink(r.ID);
    }

    /// <summary>
    /// Role is used to interact with ACL Roles in Consul through the API
    /// </summary>
    public class Role : IRoleEndpoint
    {
        private readonly ConsulClient _client;

        internal Role(ConsulClient c)
        {
            _client = c;
        }

        private class RoleActionResult : RoleEntry
        {
            public string Hash { get; set; }
            public ulong CreateIndex { get; set; }
            public ulong ModifyIndex { get; set; }
        }

        /// <summary>
        /// Creates a new ACL Role in Consul
        /// </summary>
        /// <param name="policy">The new ACL Role</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL Role</returns>
        public Task<WriteResult<RoleEntry>> Create(RoleEntry policy, CancellationToken ct = default)
        {
            return Create(policy, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Creates a new ACL Role in Consul
        /// </summary>
        /// <param name="policy">The new ACL Role</param>
        /// <param name="writeOptions">Customised write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL Role</returns>
        public async Task<WriteResult<RoleEntry>> Create(RoleEntry policy, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.Put<RoleEntry, RoleActionResult>("/v1/acl/role", policy, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<RoleEntry>(res, res.Response);
        }

        /// <summary>
        /// Deletes and existing ACL Role from Consul
        /// </summary>
        /// <param name="id">The ID of the ACL Role to delete</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Success (true) or failure (false)</returns>
        public Task<WriteResult<bool>> Delete(string id, CancellationToken ct = default)
        {
            return Delete(id, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Deletes an existing ACL Role from Consul
        /// </summary>
        /// <param name="id">The ID of the ACL Role to delete</param>
        /// <param name="writeOptions">Customised write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>Success (true) or failure (false)</returns>
        public async Task<WriteResult<bool>> Delete(string id, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.DeleteReturning<bool>($"/v1/acl/role/{id}", writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<bool>(res, res.Response);
        }

        /// <summary>
        /// Lists the existing ACL Roles in Consul
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing an array of ACL Roles</returns>
        public Task<QueryResult<RoleEntry[]>> List(CancellationToken ct = default)
        {
            return List(QueryOptions.Default, ct);
        }

        /// <summary>
        /// Lists the existing ACL Roles in Consul
        /// </summary>
        /// <param name="queryOptions">Customised query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing an array of ACL Roles</returns>
        public async Task<QueryResult<RoleEntry[]>> List(QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<RoleEntry[]>("/v1/acl/roles", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<RoleEntry[]>(res, res.Response);
        }

        /// <summary>
        /// Gets the requested ACL Role from Consul
        /// </summary>
        /// <param name="id">The ID of the ACL Role to get</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Role</returns>
        public Task<QueryResult<RoleEntry>> Read(string id, CancellationToken ct = default)
        {
            return Read(id, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Gets the requested ACL Role from Consul
        /// </summary>
        /// <param name="id">The ID of the ACL Role to get</param>
        /// <param name="queryOptions">Customised query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Role</returns>
        public async Task<QueryResult<RoleEntry>> Read(string id, QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<RoleEntry>($"/v1/acl/role/{id}", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<RoleEntry>(res, res.Response);
        }

        /// <summary>
        /// Gets the requested ACL Role from Consul
        /// </summary>
        /// <param name="name">The Name of the ACL Role to get</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Role</returns>
        public Task<QueryResult<RoleEntry>> ReadByName(string name, CancellationToken ct = default)
        {
            return ReadByName(name, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Gets the requested ACL Role from Consul
        /// </summary>
        /// <param name="name">The Name of the ACL Role to get</param>
        /// <param name="queryOptions">Customised query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the requested ACL Role</returns>
        public async Task<QueryResult<RoleEntry>> ReadByName(string name, QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<RoleEntry>($"/v1/acl/role/name/{name}", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<RoleEntry>(res, res.Response);
        }

        /// <summary>
        /// Updates an existing ACL Role in Consul
        /// </summary>
        /// <param name="role">The modified ACL Role</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the updated ACL Role</returns>
        public Task<WriteResult<RoleEntry>> Update(RoleEntry role, CancellationToken ct = default)
        {
            return Update(role, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Updates an existing ACL Role in Consul
        /// </summary>
        /// <param name="role">The modified ACL Role</param>
        /// <param name="writeOptions">Customised write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the updated ACL Role</returns>
        public async Task<WriteResult<RoleEntry>> Update(RoleEntry role, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.Put<RoleEntry, RoleActionResult>($"/v1/acl/role/{role.ID}", role, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<RoleEntry>(res, res.Response);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Role> _role;

        /// <summary>
        /// Role returns a handle to the ACL Role endpoints
        /// </summary>
        public IRoleEndpoint Role => _role.Value;
    }
}
