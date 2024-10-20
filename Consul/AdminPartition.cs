// -----------------------------------------------------------------------
//  <copyright file="Partition.cs" company="G-Research Limited">
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

namespace Consul
{
    /// <summary>
    /// Partition is the configuration of a single admin partition. Admin Partitions are a Consul Enterprise feature.
    /// </summary>
    public class Partition
    {
        /// <summary>
        /// // Name is the name of the Partition.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description is where the user puts any information they want
        /// about the admin partition. It is not used internally.
        /// </summary>
        public string Description { get; set; }
    }

    public class PartitionResponse : Partition
    {
        /// <summary>
        /// DeletedAt is the time when the Partition was marked for deletion
        /// This is nullable so that we can omit if empty when encoding in JSON
        /// </summary>
        public DateTime DeletedAt { get; set; }
        /// <summary>
        /// CreateIndex is the Raft index at which the Partition was created
        /// </summary>
        public ulong CreateIndex { get; set; }
        /// <summary>
        /// ModifyIndex is the latest Raft index at which the Partition was modified.
        /// </summary>
        public ulong ModifyIndex { get; set; }
    }
    public class AdminPartition : IAdminPartitionEndpoint
    {
        private readonly ConsulClient _client;
        internal AdminPartition(ConsulClient c)
        {
            _client = c;
        }
        public async Task<WriteResult<PartitionResponse>> Create(Partition p, CancellationToken ct = default)
        {
            return await Create(p, WriteOptions.Default, ct).ConfigureAwait(false);
        }
        public async Task<WriteResult<PartitionResponse>> Create(Partition p, WriteOptions q, CancellationToken ct = default)
        {
            var res = await _client.Put<Partition, PartitionResponse>("/v1/partition", p, q).Execute(ct).ConfigureAwait(false);

            return new WriteResult<PartitionResponse>(res, res.Response);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<AdminPartition> _adminPartition;

        /// <summary>
        /// Partition returns a handle to the Partition endpoints
        /// </summary>
        public IAdminPartitionEndpoint AdminPartition => _adminPartition.Value;
    }
}
