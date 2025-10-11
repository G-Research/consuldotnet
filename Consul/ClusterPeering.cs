// -----------------------------------------------------------------------
//  <copyright file="ClusterPeering.cs" company="G-Research Limited">
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
using System.Threading;
using System.Threading.Tasks;
using Consul.Interfaces;

namespace Consul
{
    public class ClusterPeeringEntry
    {
        public string PeerName { get; set; }
        public Dictionary<string, string> Meta { get; set; }

        public ClusterPeeringEntry()
            : this(string.Empty, null)
        {
        }

        public ClusterPeeringEntry(string peerName, Dictionary<string, string> meta)
        {
            PeerName = peerName;
            Meta = meta;
        }
    }

    public class ClustingPeeringResponse
    {
        public string PeeringToken { get; set; }
    }

    /// <summary>
    /// ClusterPeering is used to interact with Cluster Peering in Consul through the API
    /// </summary>
    public class ClusterPeering : IClusterPeeringEndpoint
    {
        private readonly ConsulClient _client;

        internal ClusterPeering(ConsulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Generates a Peering Token in Consul
        /// </summary>
        /// <param name="entry">The new Cluster Peering Entry</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL AuthMethod</returns>
        public Task<WriteResult<ClustingPeeringResponse>> Create(ClusterPeeringEntry entry,
            CancellationToken ct = default)
        {
            return Create(entry, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Generates a Peering Token in Consul
        /// </summary>
        /// <param name="entry">A new Cluster Peering Entry</param>
        /// <param name="options"></param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A new Binding Rule</returns>
        public async Task<WriteResult<ClustingPeeringResponse>> Create(ClusterPeeringEntry entry, WriteOptions options,
            CancellationToken ct = default)
        {
            var res = await _client
                .Post<ClusterPeeringEntry, ClustingPeeringResponse>("/v1/peering/token", entry, options).Execute(ct)
                .ConfigureAwait(false);
            return new WriteResult<ClustingPeeringResponse>(res, res.Response);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<ClusterPeering> _clusterPeering;

        /// <summary>
        /// Cluster Peering returns a handle to the Cluster Peering endpoints
        /// </summary>
        public IClusterPeeringEndpoint ClusterPeering => _clusterPeering.Value;
    }
}
