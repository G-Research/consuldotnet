// -----------------------------------------------------------------------
//  <copyright file="Connect.cs" company="G-Research Limited">
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
using System.Xml.Linq;
using Consul.Interfaces;
using Newtonsoft.Json;

namespace Consul
{
    public class CAConfig
    {    /// <summary>
         /// Provider is the CA provider implementation to use.
         /// </summary>
        public string Provider { get; set; }
        /// <summary>
        /// Configuration is arbitrary configuration for the provider. This
        /// should only contain primitive values and containers (such as lists and maps).
        /// </summary>
        public Dictionary<string, object> Config { get; set; }
        /// <summary>
        ///  State is read-only data that the provider might have persisted for use
        ///  after restart or leadership transition. For example this might include
        ///  UUIDs of resources it has created. Setting this when writing a configuration is an error.
        /// </summary>
        public Dictionary<string, string> State { get; set; }
        /// <summary>
        /// ForceWithoutCrossSigning indicates that the CA reconfiguration should go
        /// ahead even if the current CA is unable to cross sign certificates. This
        /// risks temporary connection failures during the rollout as new leafs will be
        /// rejected by proxies that have not yet observed the new root cert but is the
        /// only option if a CA that doesn't support cross signing needs to be reconfigured or mirated away from.
        /// </summary>
        public bool ForceWithoutCrossSigning { get; set; }
        public ulong CreateIndex { get; set; }
        public ulong ModifyIndex { get; set; }
    }

    /// <summary>
    /// Configures control access between services in the service mesh.
    /// </summary>
    public class ServiceIntentionsEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "service-intentions";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SourceIntention> Sources { get; set; }
    }

    public class SourceIntention
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Peer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<IntentionPermission> Permissions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Precedence { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LegacyID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> LegacyMeta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime LegacyCreateTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime LegacyUpdateTime { get; set; }
    }

    public class IntentionPermission
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IntentionHTTPPermission HTTP { get; set; }
    }

    public class IntentionHTTPPermission
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PathExact { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PathPrefix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PathRegex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Methods { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<IntentionHTTPHeaderPermission> Header { get; set; }

    }

    public class IntentionHTTPHeaderPermission
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Present { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Exact { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Prefix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Suffix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Regex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Invert { get; set; }
    }

    /// <summary>
    /// This handles the response for any operation carried out on the ServiceIntentionsEntry Model
    /// </summary>
    public class ServiceIntention
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SourceNS { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SourceName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DestinationNS { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DestinationName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SourceType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Precedence { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ulong CreateIndex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ulong ModifyIndex { get; set; }
    }

    public class Connect : IConnectEndpoint
    {
        private readonly ConsulClient _client;

        internal Connect(ConsulClient c)
        {
            _client = c;
        }
        /// <summary>
        /// CARoots queries the list of available roots.
        /// </summary>
        public Task<QueryResult<CARoots>> CARoots(CancellationToken ct = default)
        {
            return CARoots(QueryOptions.Default, ct);
        }
        /// <summary>
        /// CARoots queries the list of available roots.
        /// </summary>
        public Task<QueryResult<CARoots>> CARoots(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<CARoots>("/v1/connect/ca/roots", q).Execute(ct);
        }
        /// <summary>
        /// CAGetConfig returns the current CA configuration.
        /// </summary>
        public Task<QueryResult<CAConfig>> CAGetConfig(CancellationToken ct = default)
        {
            return CAGetConfig(QueryOptions.Default, ct);
        }

        /// <summary>
        /// CAGetConfig returns the current CA configuration.
        /// </summary>
        public Task<QueryResult<CAConfig>> CAGetConfig(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<CAConfig>("/v1/connect/ca/configuration", q).Execute(ct);
        }

        /// <summary>
        /// CASetConfig sets the current CA configuration.
        /// </summary>
        public Task<WriteResult> CASetConfig(CAConfig config, CancellationToken ct = default)
        {
            return CASetConfig(config, WriteOptions.Default, ct);
        }

        /// <summary>
        /// CASetConfig sets the current CA configuration.
        /// </summary>
        public Task<WriteResult> CASetConfig(CAConfig config, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Put("/v1/connect/ca/configuration", config, q).Execute(ct);

        }

        /// <summary>
        /// Retrieves a list of all configured service intentions
        /// </summary>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A list of service intentions</returns>
        public Task<QueryResult<List<ServiceIntention>>> ListIntentions<ServiceIntention>(CancellationToken ct = default)
        {
            return ListIntentions<ServiceIntention>(QueryOptions.Default, ct);
        }

        /// <summary>
        /// Retrieves a list of all configured service intentions with query options
        /// </summary>
        /// <param name="q">Custom query options</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A list of service intentions</returns>
        ///
        public Task<QueryResult<List<ServiceIntention>>> ListIntentions<ServiceIntention>(QueryOptions q, CancellationToken ct = default)
        {
            var req = _client.Get<List<ServiceIntention>>("/v1/connect/intentions", q);
            return req.Execute(ct);
        }

        /// <summary>
        /// Creates a new intention.
        /// The intentions created by this endpoint will not be assigned the following fields: ID, CreatedAt, UpdatedAt.
        /// Additionally, the Meta field cannot be persisted using this endpoint and will require editing the enclosing service-intentions config entry for the destination.
        /// </summary>
        /// <param name="intention"></param>
        /// <param name="q"></param>
        /// <param name="ct"></param>
        /// <returns>True if the intention was created successfully or False if not</returns>
        public Task<WriteResult<bool>> UpsertIntentionsByName(ServiceIntention intention, WriteOptions q, CancellationToken ct = default)
        {
            var req = _client.Put<ServiceIntention, bool>($"v1/connect/intentions/exact", intention, q);
            req.Params["source"] = intention.SourceName;
            req.Params["destination"] = intention.DestinationName;
            var res = req.Execute(ct);
            return res;
        }

        /// <summary>
        /// Creates a new intention.
        /// The intentions created by this endpoint will not be assigned the following fields: ID, CreatedAt, UpdatedAt.
        /// Additionally, the Meta field cannot be persisted using this endpoint and will require editing the enclosing service-intentions config entry for the destination.
        /// </summary>
        /// <param name="intention"></param>
        /// <param name="ct"></param>
        /// <returns>True if the intention was created successfully or False if not</returns>
        public Task<WriteResult<bool>> UpsertIntentionsByName(ServiceIntention intention, CancellationToken ct = default)
        {
            return UpsertIntentionsByName(intention, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Deletes a specific intention by its unique source and destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="q"></param>
        /// <param name="ct"></param>
        /// <returns>A Write Option</returns>
        public Task<WriteResult> DeleteIntentionByName(string source, string destination, WriteOptions q, CancellationToken ct = default)
        {
            var req = _client.Delete("v1/connect/intentions/exact", q);
            req.Params["source"] = source;
            req.Params["destination"] = destination;
            return req.Execute(ct);
        }

        /// <summary>
        /// Deletes a specific intention by its unique source and destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="ct"></param>
        /// <returns>A Write Option</returns>
        public Task<WriteResult> DeleteIntentionByName(string source, string destination, CancellationToken ct = default)
        {
            return DeleteIntentionByName(source, destination, WriteOptions.Default, ct);
        }

        /// <summary>
        /// reads a specific intention by its unique source and destination.
        /// </summary>
        /// <typeparam name="ServiceIntention"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="q"></param>
        /// <param name="ct"></param>
        /// <returns>A service intention</returns>
        public Task<QueryResult<ServiceIntention>> ReadSpecificIntentionByName<ServiceIntention>(string source, string destination, QueryOptions q, CancellationToken ct = default)
        {
            var req = _client.Get<ServiceIntention>("v1/connect/intentions/exact", q);
            req.Params["source"] = source;
            req.Params["destination"] = destination;
            return req.Execute(ct);
        }

        /// <summary>
        /// reads a specific intention by its unique source and destination.
        /// </summary>
        /// <typeparam name="ServiceIntention"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="ct"></param>
        /// <returns>A service intention</returns>
        public Task<QueryResult<ServiceIntention>> ReadSpecificIntentionByName<ServiceIntention>(string source, string destination, CancellationToken ct = default)
        {
            return ReadSpecificIntentionByName<ServiceIntention>(source, destination, QueryOptions.Default, ct);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Connect> _connect;

        /// <summary>
        /// Connect returns a handle to the Connect endpoints
        /// </summary>
        public IConnectEndpoint Connect => _connect.Value;
    }
}
