// -----------------------------------------------------------------------
//  <copyright file="Snapshot.cs" company="PlayFab Inc">
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

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    public class Snapshot : ISnapshotEndpoint
    {
        private readonly ConsulClient _client;

        /// <summary>
        /// Snapshot can be used to query the /v1/snapshot endpoint to take snapshots of
        /// Consul's internal state and restore snapshots for disaster recovery.
        /// </summary>
        /// <param name="c"></param>
        internal Snapshot(ConsulClient c)
        {
            _client = c;
        }

        public Task<WriteResult> Restore(Stream s, CancellationToken ct = default)
        {
            return Restore(s, WriteOptions.Default, ct);
        }

        public Task<WriteResult> Restore(Stream s, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Put("/v1/snapshot", s, q).Execute(ct);
        }

        public Task<QueryResult<Stream>> Save(CancellationToken ct = default)
        {
            return Save(QueryOptions.Default, ct);
        }

        public Task<QueryResult<Stream>> Save(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<Stream>("/v1/snapshot", q).ExecuteStreaming(ct);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Snapshot> _snapshot;

        /// <summary>
        /// Catalog returns a handle to the snapshot endpoints
        /// </summary>
        public ISnapshotEndpoint Snapshot => _snapshot.Value;
    }
}
