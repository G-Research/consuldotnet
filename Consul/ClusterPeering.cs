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
using Newtonsoft.Json;

namespace Consul
{
    public class ClusterPeeringTokenEntry
    {
        public string PeerName { get; set; }
        public Dictionary<string, string> Meta { get; set; }
    }

    public class ClusterPeeringTokenResponse
    {
        public string PeeringToken { get; set; }
    }

    /// <summary>
    /// Represents the status of a cluster peering connection in Consul.
    /// </summary>
    public class ClusterPeeringStatus
    {
        [JsonProperty("ID")]
        public string ID { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("PeerID")]
        public string PeerID { get; set; }

        [JsonProperty("PeerServerName")]
        public string PeerServerName { get; set; }

        [JsonProperty("PeerServerAddresses")]
        public string[] PeerServerAddresses { get; set; }

        [JsonProperty("StreamStatus")]
        public PeeringStreamStatus StreamStatus { get; set; }

        [JsonProperty("CreateIndex")]
        public long CreateIndex { get; set; }

        [JsonProperty("ModifyIndex")]
        public long ModifyIndex { get; set; }

        [JsonProperty("Remote")]
        public PeeringRemoteInfo Remote { get; set; }
    }

    /// <summary>
    /// Details the state of the data stream between peered clusters.
    /// </summary>
    public class PeeringStreamStatus
    {
        [JsonProperty("ImportedServices")]
        public List<string> ImportedServices { get; set; }

        [JsonProperty("ExportedServices")]
        public List<string> ExportedServices { get; set; }

        [JsonProperty("LastHeartbeat")]
        public DateTime? LastHeartbeat { get; set; }

        [JsonProperty("LastReceive")]
        public DateTime? LastReceive { get; set; }

        [JsonProperty("LastSend")]
        public DateTime? LastSend { get; set; }
    }

    /// <summary>
    /// Specifies the remote datacenter and partition for the peering connection.
    /// </summary>
    public class PeeringRemoteInfo
    {
        [JsonProperty("Partition")]
        public string Partition { get; set; }

        [JsonProperty("Datacenter")]
        public string Datacenter { get; set; }
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
        /// <param name="tokenEntry">The new Cluster Peering Entry</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the created ACL AuthMethod</returns>
        public Task<WriteResult<ClusterPeeringTokenResponse>> GenerateToken(ClusterPeeringTokenEntry tokenEntry,
            CancellationToken ct = default)
        {
            return GenerateToken(tokenEntry, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Generates a Peering Token in Consul
        /// </summary>
        /// <param name="tokenEntry">A new Cluster Peering Entry</param>
        /// <param name="options"></param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A new Binding Rule</returns>
        public async Task<WriteResult<ClusterPeeringTokenResponse>> GenerateToken(ClusterPeeringTokenEntry tokenEntry, WriteOptions options,
            CancellationToken ct = default)
        {
            var res = await _client
                .Post<ClusterPeeringTokenEntry, ClusterPeeringTokenResponse>("/v1/peering/token", tokenEntry, options).Execute(ct)
                .ConfigureAwait(false);
            return new WriteResult<ClusterPeeringTokenResponse>(res, res.Response);
        }

        /// <summary>
        /// ListPeerings is used to list peering connections
        /// </summary>
        public Task<QueryResult<ClusterPeeringStatus[]>> ListPeerings(CancellationToken cancellationToken = default)
        {
            return ListPeerings(null, cancellationToken);
        }
        /// <summary>
        /// ListPeerings is used to list peering connections
        /// </summary>
        public Task<QueryResult<ClusterPeeringStatus[]>> ListPeerings(QueryOptions q,
            CancellationToken cancellationToken = default)
        {
            return _client.Get<ClusterPeeringStatus[]>("/v1/peerings", q).Execute(cancellationToken);
        }

        /// <summary>
        /// GetPeering is used to query a specific connection by its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A specific connection instance.</returns>
        public Task<QueryResult<ClusterPeeringStatus>> GetPeering(string name, CancellationToken cancellationToken)
        {
            return GetPeering(name, null, cancellationToken);
        }

        /// <summary>
        /// GetPeering is used to query a specific connection by its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A specific connection instance.</returns>
        public Task<QueryResult<ClusterPeeringStatus>> GetPeering(string name, QueryOptions options, CancellationToken cancellationToken)
        {
            var res = _client.Get<ClusterPeeringStatus>(string.Format("/v1/peering/{0}", name), options);
            return res.Execute(cancellationToken);
        }

        /// <summary>
        /// DeletePeering is used to delete a specific connection by its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A specific connection instance.</returns>
        public Task<WriteResult> DeletePeering(string name, CancellationToken cancellationToken)
        {
            return DeletePeering(name, null, cancellationToken);
        }

        /// <summary>
        /// DeletePeering is used to delete a specific connection by its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A specific connection instance.</returns>
        public Task<WriteResult> DeletePeering(string name, WriteOptions options, CancellationToken cancellationToken)
        {
            var res = _client.Delete(string.Format("/v1/peering/{0}", name), options);
            return res.Execute(cancellationToken);
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
