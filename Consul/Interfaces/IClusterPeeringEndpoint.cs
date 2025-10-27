// -----------------------------------------------------------------------
//  <copyright file="IClusterPeering.cs" company="G-Research Limited">
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Consul.Interfaces
{
    /// <summary>
    /// The interface for Cluster Peering API Endpoints
    /// </summary>
    public interface IClusterPeeringEndpoint
    {
        Task<WriteResult<ClusterPeeringTokenResponse>> GenerateToken(ClusterPeeringTokenEntry tokenEntry, CancellationToken ct = default);

        Task<WriteResult<ClusterPeeringTokenResponse>> GenerateToken(ClusterPeeringTokenEntry tokenEntry, WriteOptions options,
            CancellationToken ct = default);
        Task<QueryResult<ClusterPeeringStatus[]>> ListPeerings(CancellationToken cancellationToken = default);
        Task<QueryResult<ClusterPeeringStatus[]>> ListPeerings(QueryOptions q, CancellationToken cancellationToken = default);

        Task<QueryResult<ClusterPeeringStatus>> GetPeering(string name, CancellationToken cancellationToken = default);
        Task<QueryResult<ClusterPeeringStatus>> GetPeering(string name, QueryOptions q, CancellationToken cancellationToken = default);
        Task<WriteResult> DeletePeering(string name, CancellationToken cancellationToken = default);
        Task<WriteResult> DeletePeering(string name, WriteOptions q, CancellationToken cancellationToken = default);
    }
}
