// -----------------------------------------------------------------------
//  <copyright file="IPreparedQueryEndpoint.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
//    Copyright 2020 G-Research Limited
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
using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    /// <summary>
    /// The interface for the Prepared Query API Endpoints
    /// </summary>
    public interface IPreparedQueryEndpoint
    {
        Task<WriteResult<string>> Create(PreparedQueryDefinition query, CancellationToken ct = default);
        Task<WriteResult<string>> Create(PreparedQueryDefinition query, WriteOptions q, CancellationToken ct = default);
        Task<WriteResult> Update(PreparedQueryDefinition query, CancellationToken ct = default);
        Task<WriteResult> Update(PreparedQueryDefinition query, WriteOptions q, CancellationToken ct = default);
        Task<QueryResult<PreparedQueryDefinition[]>> List(CancellationToken ct = default);
        Task<QueryResult<PreparedQueryDefinition[]>> List(QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<PreparedQueryDefinition[]>> Get(string queryID, CancellationToken ct = default);
        Task<QueryResult<PreparedQueryDefinition[]>> Get(string queryID, QueryOptions q, CancellationToken ct = default);
        Task<WriteResult> Delete(string queryID, CancellationToken ct = default);
        Task<WriteResult> Delete(string queryID, WriteOptions q, CancellationToken ct = default);
        Task<QueryResult<PreparedQueryExecuteResponse>> Execute(string queryIDOrName, CancellationToken ct = default);
        Task<QueryResult<PreparedQueryExecuteResponse>> Execute(string queryIDOrName, QueryOptions q, CancellationToken ct = default);
    }
}
