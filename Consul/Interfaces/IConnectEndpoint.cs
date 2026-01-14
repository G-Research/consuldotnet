// -----------------------------------------------------------------------
//  <copyright file="IConnectEndpoint.cs" company="G-Research Limited">
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
    public interface IConnectEndpoint
    {
        Task<QueryResult<CARoots>> CARoots(QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<CARoots>> CARoots(CancellationToken ct = default);
        Task<QueryResult<CAConfig>> CAGetConfig(QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<CAConfig>> CAGetConfig(CancellationToken ct = default);
        Task<WriteResult> CASetConfig(CAConfig config, WriteOptions q, CancellationToken ct = default);
        Task<WriteResult> CASetConfig(CAConfig config, CancellationToken ct = default);
        Task<QueryResult<List<ServiceIntention>>> ListIntentions<ServiceIntention>(CancellationToken ct = default);
        Task<QueryResult<List<ServiceIntention>>> ListIntentions<ServiceIntention>(QueryOptions q, CancellationToken ct = default);
        Task<WriteResult<bool>> UpsertIntentionsByName(ServiceIntention intention, WriteOptions q, CancellationToken ct = default);
        Task<WriteResult<bool>> UpsertIntentionsByName(ServiceIntention intention, CancellationToken ct = default);
        Task<WriteResult> DeleteIntentionByName(string source, string destination, WriteOptions q, CancellationToken ct = default);
        Task<WriteResult> DeleteIntentionByName(string source, string destination, CancellationToken ct = default);
        Task<QueryResult<ServiceIntention>> ReadSpecificIntentionByName<ServiceIntention>(string source, string destination, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<ServiceIntention>> ReadSpecificIntentionByName<ServiceIntention>(string source, string destination, CancellationToken ct = default);
        Task<QueryResult<Dictionary<string, List<ServiceIntention>>>> ListMatchingIntentions(string by, string name, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<Dictionary<string, List<ServiceIntention>>>> ListMatchingIntentions(string by, string name, CancellationToken ct = default);
        Task<QueryResult<ServiceIntentionResultResponse>> CheckIntentionResult(string source, string destination, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<ServiceIntentionResultResponse>> CheckIntentionResult(string source, string destination, CancellationToken ct = default);
    }
}
