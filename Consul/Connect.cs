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
    public class CARootList
    {
        public string ActiveRootID { get; set; }
        public string TrustDomain { get; set; }
        public List<CARoot> Roots { get; set; }
    }
    public class CARoot
    {
        /// <summary>
        /// ID is a globally unique ID (UUID) representing this CA root.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Name is a human-friendly name for this CA root.
        /// This value is opaque to Consul and is not used for anything internally.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// RootCertPEM is the PEM-encoded public certificate.
        /// </summary>
        public string RootCertPEM { get; set; }
        /// <summary>
        /// Active is true if this is the current active CA. This must only
        /// be true for exactly one CA. For any method that modifies roots in the
        /// state store, tests should be written to verify that multiple roots
        /// cannot be active.
        /// </summary>
        public bool Active { get; set; }
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
        public Task<QueryResult<CARootList>> CARoots(CancellationToken ct = default)
        {
            return CARoots(QueryOptions.Default, ct);
        }
        /// <summary>
        /// CARoots queries the list of available roots.
        /// </summary>
        public Task<QueryResult<CARootList>> CARoots(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<CARootList>("/v1/connect/ca/roots", q).Execute(ct);
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
