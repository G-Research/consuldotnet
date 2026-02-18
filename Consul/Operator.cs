// -----------------------------------------------------------------------
//  <copyright file="Operator.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
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
#pragma warning disable RS0026
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consul
{
    /// <summary>
    /// RaftServer has information about a server in the Raft configuration.
    /// </summary>
    public class RaftServer
    {
        /// <summary>
        /// ID is the unique ID for the server. These are currently the same
        /// as the address, but they will be changed to a real GUID in a future
        /// release of Consul.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Node is the node name of the server, as known by Consul, or this
        /// will be set to "(unknown)" otherwise.
        /// </summary>
        public string Node { get; set; }

        /// <summary>
        /// Address is the IP:port of the server, used for Raft communications.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Leader is true if this server is the current cluster leader.
        /// </summary>
        public bool Leader { get; set; }

        /// <summary>
        /// Voter is true if this server has a vote in the cluster. This might
        /// be false if the server is staging and still coming online, or if
        /// it's a non-voting server, which will be added in a future release of
        /// Consul
        /// </summary>
        public bool Voter { get; set; }
    }

    /// <summary>
    /// RaftConfigration is returned when querying for the current Raft configuration.
    /// </summary>
    public class RaftConfiguration
    {
        /// <summary>
        /// Servers has the list of servers in the Raft configuration.
        /// </summary>
        public List<RaftServer> Servers { get; set; }

        /// <summary>
        /// Index has the Raft index of this configuration.
        /// </summary>
        public ulong Index { get; set; }
    }

    /// <summary>
    /// KeyringResponse is returned when listing the gossip encryption keys
    /// </summary>
    public class KeyringResponse
    {
        /// <summary>
        /// Whether this response is for a WAN ring
        /// </summary>
        public bool WAN { get; set; }
        /// <summary>
        /// The datacenter name this request corresponds to
        /// </summary>
        public string Datacenter { get; set; }
        /// <summary>
        /// A map of the encryption keys to the number of nodes they're installed on
        /// </summary>
        public IDictionary<string, int> Keys { get; set; }
        /// <summary>
        /// The total number of nodes in this ring
        /// </summary>
        public int NumNodes { get; set; }
    }

    public class AreaRequest
    {
        /// <summary>
        /// PeerDatacenter is the peer Consul datacenter that will make up the
        /// other side of this network area. Network areas always involve a pair
        /// of datacenters: the datacenter where the area was created, and the
        /// peer datacenter. This is required.
        /// </summary>
        public string PeerDatacenter { get; set; }

        /// <summary>
        /// RetryJoin specifies the address of Consul servers to join to, such as
	    /// an IPs or hostnames with an optional port number. This is optional.
        /// </summary>
        public string[] RetryJoin { get; set; }

        /// <summary>
        /// UseTLS specifies whether gossip over this area should be encrypted with TLS
        /// if possible.
        /// </summary>
        public bool UseTLS { get; set; }
    }

    public class Area : AreaRequest
    {
        /// <summary>
        /// ID is this identifier for an area (a UUID).
        /// </summary>
        public string ID { get; set; }
    }
    public class Operator : IOperatorEndpoint
    {
        private readonly ConsulClient _client;

        /// <summary>
        /// Operator can be used to perform low-level operator tasks for Consul.
        /// </summary>
        /// <param name="c"></param>
        internal Operator(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// KeyringRequest is used for performing Keyring operations
        /// </summary>
        private class KeyringRequest
        {
            [JsonProperty]
            internal string Key { get; set; }
        }

        /// <summary>
        /// RaftGetConfiguration is used to query the current Raft peer set.
        /// </summary>
        public Task<QueryResult<RaftConfiguration>> RaftGetConfiguration(CancellationToken ct)
        {
            return RaftGetConfiguration(QueryOptions.Default, ct);
        }

        // Add a parameterless overload for convenience
        public Task<QueryResult<RaftConfiguration>> RaftGetConfiguration()
        {
            return RaftGetConfiguration(QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// RaftGetConfiguration is used to query the current Raft peer set.
        /// </summary>
        public Task<QueryResult<RaftConfiguration>> RaftGetConfiguration(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<RaftConfiguration>("/v1/operator/raft/configuration", q).Execute(ct);
        }

        /// <summary>
        /// RaftRemovePeerByAddress is used to kick a stale peer (one that it in the Raft
        /// quorum but no longer known to Serf or the catalog) by address in the form of
        /// "IP:port".
        /// </summary>
        public Task<WriteResult> RaftRemovePeerByAddress(string address, CancellationToken ct)
        {
            return RaftRemovePeerByAddress(address, WriteOptions.Default, ct);
        }

        public Task<WriteResult> RaftRemovePeerByAddress(string address)
        {
            return RaftRemovePeerByAddress(address, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// RaftRemovePeerByAddress is used to kick a stale peer (one that it in the Raft
        /// quorum but no longer known to Serf or the catalog) by address in the form of
        /// "IP:port".
        /// </summary>
        public Task<WriteResult> RaftRemovePeerByAddress(string address, WriteOptions q, CancellationToken ct = default)
        {
            var req = _client.Delete("/v1/operator/raft/peer", q);

            // From Consul repo:
            // TODO (slackpad) Currently we made address a query parameter. Once
            // IDs are in place this will be DELETE /v1/operator/raft/peer/<id>.
            req.Params["address"] = address;

            return req.Execute(ct);
        }
        /// <summary>
        /// Transfers Raft leadership to another server
        /// </summary>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>A write result indicating the success of the operation</returns>
        public Task<WriteResult> RaftTransferLeader(CancellationToken ct)
        {
            return RaftTransferLeader(WriteOptions.Default, ct);
        }

        public Task<WriteResult> RaftTransferLeader()
        {
            return RaftTransferLeader(WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Transfers Raft leadership to another server with write options
        /// </summary>
        /// <param name="q">Write options including datacenter and token</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>A write result indicating the success of the operation</returns>
        public Task<WriteResult> RaftTransferLeader(WriteOptions q, CancellationToken ct)
        {
            return _client.Post<object>("/v1/operator/raft/transfer-leader", null, q).Execute(ct);
        }

        public Task<WriteResult> RaftTransferLeader(WriteOptions q)
        {
            return RaftTransferLeader(q, CancellationToken.None);
        }

        /// <summary>
        /// Transfers Raft leadership to another server by ID
        /// </summary>
        /// <param name="id">The node ID of the Raft peer to transfer leadership to</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>A write result indicating the success of the operation</returns>
        public Task<WriteResult> RaftTransferLeader(string id, CancellationToken ct)
        {
            return RaftTransferLeader(id, WriteOptions.Default, ct);
        }

        public Task<WriteResult> RaftTransferLeader(string id)
        {
            return RaftTransferLeader(id, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Transfers Raft leadership to another server by ID with write options
        /// </summary>
        /// <param name="id">The node ID of the Raft peer to transfer leadership to</param>
        /// <param name="q">Write options including datacenter and token</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>A write result indicating the success of the operation</returns>
        public Task<WriteResult> RaftTransferLeader(string id, WriteOptions q, CancellationToken ct = default)
        {
            var req = _client.Post<object>("/v1/operator/raft/transfer-leader", null, q);
            req.Params["id"] = id;
            return req.Execute(ct);
        }

        /// <summary>
        /// KeyringInstall is used to install a new gossip encryption key into the cluster
        /// </summary>
        public Task<WriteResult> KeyringInstall(string key, CancellationToken ct)
        {
            return KeyringInstall(key, WriteOptions.Default, ct);
        }

        public Task<WriteResult> KeyringInstall(string key)
        {
            return KeyringInstall(key, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// KeyringInstall is used to install a new gossip encryption key into the cluster
        /// </summary>
        public Task<WriteResult> KeyringInstall(string key, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Post("/v1/operator/keyring", new KeyringRequest() { Key = key }, q).Execute(ct);
        }

        /// <summary>
        /// KeyringList is used to list the gossip keys installed in the cluster
        /// </summary>
        public Task<QueryResult<KeyringResponse[]>> KeyringList(CancellationToken ct)
        {
            return KeyringList(QueryOptions.Default, ct);
        }

        public Task<QueryResult<KeyringResponse[]>> KeyringList()
        {
            return KeyringList(QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// KeyringList is used to list the gossip keys installed in the cluster
        /// </summary>
        public Task<QueryResult<KeyringResponse[]>> KeyringList(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<KeyringResponse[]>("/v1/operator/keyring", q).Execute(ct);
        }

        /// <summary>
        /// KeyringRemove is used to remove a gossip encryption key from the cluster
        /// </summary>
        public Task<WriteResult> KeyringRemove(string key, CancellationToken ct)
        {
            return KeyringRemove(key, WriteOptions.Default, ct);
        }

        public Task<WriteResult> KeyringRemove(string key)
        {
            return KeyringRemove(key, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// KeyringRemove is used to remove a gossip encryption key from the cluster
        /// </summary>
        public Task<WriteResult> KeyringRemove(string key, WriteOptions q, CancellationToken ct = default)
        {
            return _client.DeleteAccepting("/v1/operator/keyring", new KeyringRequest() { Key = key }, q).Execute(ct);
        }

        /// <summary>
        /// KeyringUse is used to change the active gossip encryption key
        /// </summary>
        public Task<WriteResult> KeyringUse(string key, CancellationToken ct)
        {
            return KeyringUse(key, WriteOptions.Default, ct);
        }

        public Task<WriteResult> KeyringUse(string key)
        {
            return KeyringUse(key, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// KeyringUse is used to change the active gossip encryption key
        /// </summary>
        public Task<WriteResult> KeyringUse(string key, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Put("/v1/operator/keyring", new KeyringRequest() { Key = key }, q).Execute(ct);
        }

        public Task<QueryResult<ConsulLicense>> GetConsulLicense(string datacenter = "", CancellationToken ct = default)
        {
            return _client.Get<ConsulLicense>("/v1/operator/license", new QueryOptions { Datacenter = datacenter }).Execute(ct);
        }

        /// <summary>
        /// // SegmentList returns all the available LAN segments.
        /// </summary>
        public Task<QueryResult<string[]>> SegmentList(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<string[]>("/v1/operator/segment", q).Execute(ct);
        }

        /// <summary>
        /// // SegmentList returns all the available LAN segments.
        /// </summary>
        public Task<QueryResult<string[]>> SegmentList(CancellationToken ct)
        {
            return SegmentList(QueryOptions.Default, ct);
        }

        public Task<QueryResult<string[]>> SegmentList()
        {
            return SegmentList(QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// CreateArea will create a new network area, a generated ID will be returned on success.
        /// </summary>
        public Task<WriteResult<string>> AreaCreate(AreaRequest area, CancellationToken ct)
        {
            return AreaCreate(area, WriteOptions.Default, ct);
        }

        public Task<WriteResult<string>> AreaCreate(AreaRequest area)
        {
            return AreaCreate(area, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// CreateArea will create a new network area, a generated ID will be returned on success.
        /// </summary>
        public async Task<WriteResult<string>> AreaCreate(AreaRequest area, WriteOptions q, CancellationToken ct = default)
        {
            var req = await _client.Post<AreaRequest, Area>("/v1/operator/area", area, q).Execute(ct).ConfigureAwait(false);
            return new WriteResult<string>(req, req.Response.ID);
        }

        /// <summary>
        /// AreaList returns all the available network areas
        /// </summary>
        public Task<QueryResult<List<Area>>> AreaList(CancellationToken ct)
        {
            return AreaList(QueryOptions.Default, ct);
        }

        public Task<QueryResult<List<Area>>> AreaList()
        {
            return AreaList(QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// AreaList returns all the available network areas
        /// </summary>
        public Task<QueryResult<List<Area>>> AreaList(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<List<Area>>("/v1/operator/area", q).Execute(ct);
        }
        /// <summary>
        /// AreaUpdate will update the configuration of the network area with the given area Id.
        /// </summary>
        public Task<WriteResult<string>> AreaUpdate(AreaRequest area, string areaId, CancellationToken ct)
        {
            return AreaUpdate(area, areaId, WriteOptions.Default, ct);
        }

        public Task<WriteResult<string>> AreaUpdate(AreaRequest area, string areaId)
        {
            return AreaUpdate(area, areaId, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// AreaUpdate will update the configuration of the network area with the given area Id.
        /// </summary>
        public async Task<WriteResult<string>> AreaUpdate(AreaRequest area, string areaId, WriteOptions q, CancellationToken ct = default)
        {
            var req = await _client.Put<AreaRequest, Area>($"/v1/operator/area/{areaId}", area, q).Execute(ct).ConfigureAwait(false);
            return new WriteResult<string>(req, req.Response.ID);
        }
        /// <summary>
        /// AreaGet returns a single network area
        /// </summary>
        public Task<QueryResult<Area[]>> AreaGet(string areaId, CancellationToken ct)
        {
            return AreaGet(areaId, QueryOptions.Default, ct);
        }

        public Task<QueryResult<Area[]>> AreaGet(string areaId)
        {
            return AreaGet(areaId, QueryOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// AreaGet returns a single network area
        /// </summary>
        public Task<QueryResult<Area[]>> AreaGet(string areaId, QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<Area[]>($"/v1/operator/area/{areaId}", q).Execute(ct);
        }
        /// <summary>
        /// AreaDelete deletes the given network area.
        /// </summary>
        public Task<WriteResult> AreaDelete(string areaId, CancellationToken ct)
        {
            return AreaDelete(areaId, WriteOptions.Default, ct);
        }

        public Task<WriteResult> AreaDelete(string areaId)
        {
            return AreaDelete(areaId, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// AreaDelete deletes the given network area.
        /// </summary>
        public Task<WriteResult> AreaDelete(string areaId, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Delete($"/v1/operator/area/{areaId}", q).Execute(ct);
        }

        /// <summary>
        /// Retrieves the current Autopilot configuration
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>A query result containing the Autopilot configuration</returns>
        public Task<QueryResult<AutopilotConfiguration>> AutopilotGetConfiguration(CancellationToken cancellationToken)
        {
            return AutopilotGetConfiguration(null, cancellationToken);
        }

        public Task<QueryResult<AutopilotConfiguration>> AutopilotGetConfiguration()
        {
            return AutopilotGetConfiguration(null, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves the current Autopilot configuration with query options
        /// </summary>
        /// <param name="q">Query options including datacenter and consistency mode</param>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>A query result containing the Autopilot configuration</returns>
        public Task<QueryResult<AutopilotConfiguration>> AutopilotGetConfiguration(QueryOptions q, CancellationToken cancellationToken = default)
        {
            return _client.Get<AutopilotConfiguration>("/v1/operator/autopilot/configuration", q).Execute(cancellationToken);
        }

        /// <summary>
        /// Updates the autopilot configuration of the cluster (synchronous version)
        /// </summary>
        /// <param name="configuration">The autopilot configuration to set</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The write result</returns>
        public Task<WriteResult> AutopilotSetConfiguration(AutopilotConfiguration configuration, CancellationToken cancellationToken)
        {
            return AutopilotSetConfiguration(configuration, WriteOptions.Default, cancellationToken);
        }

        public Task<WriteResult> AutopilotSetConfiguration(AutopilotConfiguration configuration)
        {
            return AutopilotSetConfiguration(configuration, WriteOptions.Default, CancellationToken.None);
        }

        /// <summary>
        /// Updates the autopilot configuration of the cluster
        /// </summary>
        /// <param name="configuration">The autopilot configuration to set</param>
        /// <param name="q">Write options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The write result</returns>
        public async Task<WriteResult> AutopilotSetConfiguration(AutopilotConfiguration configuration, WriteOptions q, CancellationToken cancellationToken = default)
        {
            var req = await _client.Put<AutopilotConfiguration>("/v1/operator/autopilot/configuration", configuration, q).Execute(cancellationToken).ConfigureAwait(false);
            return new WriteResult<AutopilotConfiguration>(req);
        }

        /// <summary>
        /// Retrieves the autopilot health status of the cluster (synchronous version)
        /// </summary>
        /// <param name="cancellationToken">Query parameters</param>
        /// <returns>The autopilot health information</returns>
        public Task<QueryResult<AutopilotHealth>> AutopilotGetHealth(CancellationToken cancellationToken)
        {
            return AutopilotGetHealth(null, cancellationToken);
        }

        public Task<QueryResult<AutopilotHealth>> AutopilotGetHealth()
        {
            return AutopilotGetHealth(null, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves the autopilot health status of the cluster
        /// </summary>
        /// <param name="q">Query parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The autopilot health information</returns>
        public Task<QueryResult<AutopilotHealth>> AutopilotGetHealth(QueryOptions q, CancellationToken cancellationToken = default)
        {
            return _client.Get<AutopilotHealth>("/v1/operator/autopilot/health", q).Execute(cancellationToken);
        }

        /// <summary>
        /// Retrieves the autopilot state of the cluster
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The autopilot state information</returns>
        public Task<QueryResult<AutopilotState>> AutopilotGetState(CancellationToken cancellationToken)
        {
            return AutopilotGetState(null, cancellationToken);
        }

        public Task<QueryResult<AutopilotState>> AutopilotGetState()
        {
            return AutopilotGetState(null, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves the autopilot state of the cluster
        /// </summary>
        /// <param name="q">Query parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The autopilot state information</returns>
        public Task<QueryResult<AutopilotState>> AutopilotGetState(QueryOptions q, CancellationToken cancellationToken = default)
        {
            return _client.Get<AutopilotState>("/v1/operator/autopilot/state", q).Execute(cancellationToken);
        }

        public Task<QueryResult<OperatorUsageInformation>> OperatorGetUsage(CancellationToken cancellationToken)
        {
            return OperatorGetUsage(null, cancellationToken);
        }

        public Task<QueryResult<OperatorUsageInformation>> OperatorGetUsage()
        {
            return OperatorGetUsage(null, CancellationToken.None);
        }

        public Task<QueryResult<OperatorUsageInformation>> OperatorGetUsage(QueryOptions q,
            CancellationToken cancellationToken = default)
        {
            return _client.Get<OperatorUsageInformation>("/v1/operator/usage", q).Execute(cancellationToken);
        }
    }

    public class ConsulLicense
    {
        public bool Valid { get; set; }
        public License License { get; set; }
        public string[] Warnings { get; set; }
    }

    public class License
    {
        [JsonProperty("license_id")]
        public string LicenseId { get; set; }
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }
        [JsonProperty("installation_id")]
        public string InstallationId { get; set; }

        [JsonProperty("issue_time")]
        public string IssueTime { get; set; }
        [JsonProperty("start_time")]
        public string StartTime { get; set; }
        [JsonProperty("expiration_time")]
        public string ExpirationTime { get; set; }
        public string Product { get; set; }
        public Flags Flags { get; set; }
        public string[] Features { get; set; }
        public bool Temporary { get; set; }
    }

    public class AutopilotConfiguration
    {
        [JsonProperty("CleanupDeadServers")]
        public bool CleanupDeadServers { get; set; }

        [JsonProperty("LastContactThreshold")]
        public string LastContactThreshold { get; set; }

        [JsonProperty("MaxTrailingLogs")]
        public int MaxTrailingLogs { get; set; }

        [JsonProperty("MinQuorum")]
        public int MinQuorum { get; set; }

        [JsonProperty("ServerStabilizationTime")]
        public string ServerStabilizationTime { get; set; }

        [JsonProperty("RedundancyZoneTag")]
        public string RedundancyZoneTag { get; set; }

        [JsonProperty("DisableUpgradeMigration")]
        public bool DisableUpgradeMigration { get; set; }

        [JsonProperty("UpgradeVersionTag")]
        public string UpgradeVersionTag { get; set; }

        [JsonProperty("CreateIndex")]
        public ulong CreateIndex { get; set; }

        [JsonProperty("ModifyIndex")]
        public ulong ModifyIndex { get; set; }
    }

    public class AutopilotHealth
    {
        [JsonProperty("Healthy")]
        public bool Healthy { get; set; }

        [JsonProperty("FailureTolerance")]
        public int FailureTolerance { get; set; }

        [JsonProperty("Servers")]
        public List<AutopilotServerHealth> Servers { get; set; }
    }

    public class AutopilotServerHealth
    {
        [JsonProperty("ID")]
        public string ID { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        [JsonProperty("SerfStatus")]
        public string SerfStatus { get; set; }

        [JsonProperty("Version")]
        public string Version { get; set; }

        [JsonProperty("Leader")]
        public bool Leader { get; set; }

        [JsonProperty("LastContact")]
        public string LastContact { get; set; }

        [JsonProperty("LastTerm")]
        public long LastTerm { get; set; }

        [JsonProperty("LastIndex")]
        public long LastIndex { get; set; }

        [JsonProperty("Healthy")]
        public bool Healthy { get; set; }

        [JsonProperty("Voter")]
        public bool Voter { get; set; }

        [JsonProperty("StableSince")]
        public DateTime StableSince { get; set; }
    }

    public class AutopilotState
    {
        public bool Healthy { get; set; }
        public int FailureTolerance { get; set; }
        public int OptimisticFailureTolerance { get; set; }
        public Dictionary<string, AutopilotServerState> Servers { get; set; }
        public string Leader { get; set; }
        public List<string> Voters { get; set; }
        public Dictionary<string, object> RedundancyZones { get; set; }
        public List<string> ReadReplicas { get; set; }
        public object Upgrade { get; set; }
    }

    public class AutopilotServerState
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string NodeStatus { get; set; }
        public string Version { get; set; }
        public string LastContact { get; set; }
        public long LastTerm { get; set; }
        public long LastIndex { get; set; }
        public bool Healthy { get; set; }
        public DateTime StableSince { get; set; }
        public bool ReadReplica { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> Meta { get; set; }
        public string NodeType { get; set; }
        public string RedundancyZone { get; set; }
        public string UpgradeVersion { get; set; }
    }

    public class OperatorUsageInformation
    {
        [JsonProperty("Usage")]
        public Dictionary<string, DatacenterUsage> Usage { get; set; }
    }

    public class DatacenterUsage
    {
        [JsonProperty("Services")]
        public int Services { get; set; }

        [JsonProperty("ServiceInstances")]
        public int ServiceInstances { get; set; }

        [JsonProperty("ConnectServiceInstances")]
        public ConnectServiceInstances ConnectServiceInstances { get; set; }

        [JsonProperty("BillableServiceInstances")]
        public int BillableServiceInstances { get; set; }

        [JsonProperty("Nodes")]
        public int Nodes { get; set; }
    }

    public class ConnectServiceInstances
    {
        [JsonProperty("connect-native")]
        public int ConnectNative { get; set; }

        [JsonProperty("connect-proxy")]
        public int ConnectProxy { get; set; }

        [JsonProperty("ingress-gateway")]
        public int IngressGateway { get; set; }

        [JsonProperty("mesh-gateway")]
        public int MeshGateway { get; set; }

        [JsonProperty("terminating-gateway")]
        public int TerminatingGateway { get; set; }
    }

    public class Flags
    {
        public string Package { get; set; }
    }

    public partial class ConsulClient : IConsulClient
    {
        /// <summary>
        /// Operator returns a handle to the operator endpoints.
        /// </summary>
        public IOperatorEndpoint Operator { get; private set; }
    }
}
