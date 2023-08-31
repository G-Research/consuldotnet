// -----------------------------------------------------------------------
//  <copyright file="ACLReplication.cs" company="G-Research Limited">
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
    /// Represents an Entry in Consul for ACL Replication
    /// </summary>
    public class ACLReplicationEntry
    {
        public bool Enabled { get; set; }
        public bool Running { get; set; }
        public string SourceDatacenter { get; set; }
        public string ReplicationType { get; set; }
        public ulong ReplicatedIndex { get; set; }
        public ulong ReplicatedRoleIndex { get; set; }
        public ulong ReplicatedTokenIndex { get; set; }
        public DateTime LastSuccess { get; set; }
        public DateTime LastError { get; set; }

        public ACLReplicationEntry(bool enabled, bool running) :
            this(enabled,
                running,
                string.Empty,
                string.Empty,
                0,
                0,
                0,
                DateTime.MinValue,
                DateTime.MinValue)
        {
        }

        [JsonConstructor]
        public ACLReplicationEntry(bool enabled,
            bool running,
            string sourceDatacenter,
            string replicationType,
            ulong replicatedIndex,
            ulong replicatedRoleIndex,
            ulong replicatedTokenIndex,
            DateTime lastSuccess,
            DateTime lastError)
        {
            Enabled = enabled;
            Running = running;
            SourceDatacenter = sourceDatacenter;
            ReplicationType = replicationType;
            ReplicatedIndex = replicatedIndex;
            ReplicatedRoleIndex = replicatedRoleIndex;
            ReplicatedTokenIndex = replicatedTokenIndex;
            LastSuccess = lastSuccess;
            LastError = lastError;
        }
    }

    /// <summary>
    /// Allows querying of ACL Replication in Consul
    /// </summary>
    public class ACLReplication : IACLReplicationEndpoint
    {
        private readonly ConsulClient _client;

        internal ACLReplication(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// Get the current status of ACL Replication in Consul
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result with details of the ACL Replication system in Consul</returns>
        public Task<QueryResult<ACLReplicationEntry>> Status(CancellationToken ct = default)
        {
            return Status(QueryOptions.Default, ct);
        }

        /// <summary>
        /// Get the current status of ACL Replication in Consul
        /// </summary>
        /// <param name="queryOptions">Any query options for the request</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result with details of the ACL Replication system in Consul</returns>
        public async Task<QueryResult<ACLReplicationEntry>> Status(QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<ACLReplicationEntry>("/v1/acl/replication", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<ACLReplicationEntry>(res, res.Response);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<ACLReplication> _aclReplication;

        /// <summary>
        /// ACLReplication returns a handle to the ACLReplication endpoints
        /// </summary>
        public IACLReplicationEndpoint ACLReplication
        {
            get { return _aclReplication.Value; }
        }
    }
}
