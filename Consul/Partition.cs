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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Consul
{
    public class PartitionEntry
    {
        /// <summary>
        /// The partition name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Partition description
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// DisableGossip will not enable a gossip pool for the partition
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableGossip { get; set; }
    }

    public class PartitionActionResult : PartitionEntry
    {
        /// <summary>
        /// The time when the Partition was marked for deletion
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// The Raft index at which the Partition was created
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ulong? CreateIndex { get; set; }

        /// <summary>
        /// The latest Raft index at which the Partition was modified
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ModifyIndex { get; set; }
    }

    public class Partition : IPartitionEndpoint
    {
        private readonly ConsulClient _client;

        internal Partition(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// Creates a new partition 
        /// </summary>
        /// <param name="partition">The new partition</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created partition</returns>
        public Task<WriteResult<PartitionActionResult>> Create(PartitionEntry partition, CancellationToken ct)
        {
            return Create(partition, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Creates a new partition 
        /// </summary>
        /// <param name="partition">The new partition</param>
        /// <returns>A write result containing the created partition</returns>
        public Task<WriteResult<PartitionActionResult>> Create(PartitionEntry partition)
        {
            return Create(partition, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new partition 
        /// </summary>
        /// <param name="partition">The new partition</param>
        /// <param name="writeOptions">Customized write options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created partition</returns>
        public async Task<WriteResult<PartitionActionResult>> Create(PartitionEntry partition, WriteOptions writeOptions, CancellationToken ct = default)
        {
            var res = await _client.Put<PartitionEntry, PartitionActionResult>("/v1/partition", partition, writeOptions).Execute(ct).ConfigureAwait(false);
            return new WriteResult<PartitionActionResult>(res, res.Response);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        /// <summary>
        /// Partion returns a handle the Partion API endpoints
        /// </summary>
        public IPartitionEndpoint Partition { get; private set; }
    }
}
