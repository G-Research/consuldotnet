// -----------------------------------------------------------------------
//  <copyright file="ITokenEndpoint.cs" company="G-Research Limited">
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
    /// The interface for the ACL Token API Endpoints
    /// </summary>
    public interface ITokenEndpoint
    {
        Task<WriteResult<TokenEntry>> Bootstrap(CancellationToken ct = default);
        Task<WriteResult<TokenEntry>> Bootstrap(WriteOptions q, CancellationToken ct = default);
        Task<WriteResult<TokenEntry>> Clone(string id, CancellationToken ct = default);
        Task<WriteResult<TokenEntry>> Clone(string id, WriteOptions q, CancellationToken ct = default);
        Task<WriteResult<TokenEntry>> Create(TokenEntry token, CancellationToken ct = default);
        Task<WriteResult<TokenEntry>> Create(TokenEntry token, WriteOptions q, CancellationToken ct = default);
        Task<WriteResult<bool>> Delete(string id, CancellationToken ct = default);
        Task<WriteResult<bool>> Delete(string id, WriteOptions q, CancellationToken ct = default);
        Task<QueryResult<TokenEntry>> Read(string id, CancellationToken ct = default);
        Task<QueryResult<TokenEntry>> Read(string id, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<TokenEntry[]>> List(CancellationToken ct = default);
        Task<QueryResult<TokenEntry[]>> List(QueryOptions q, CancellationToken ct = default);
        Task<WriteResult<TokenEntry>> Update(TokenEntry token, CancellationToken ct = default);
        Task<WriteResult<TokenEntry>> Update(TokenEntry token, WriteOptions q, CancellationToken ct = default);
    }
}
