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
        public  Task<WriteResult> CASetConfig(CAConfig config, WriteOptions q, CancellationToken ct = default)
        {
            return  _client.Put("/v1/connect/ca/configuration", config, q).Execute(ct);
            
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
