// -----------------------------------------------------------------------
//  <copyright file="IRoleEndpoint.cs" company="G-Research Limited">
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
    /// The interface for the ACL Role API Endpoints
    /// </summary>
    public interface IRoleEndpoint
    {
        Task<WriteResult<RoleEntry>> Create(RoleEntry role, CancellationToken ct = default(CancellationToken));
        Task<WriteResult<RoleEntry>> Create(RoleEntry role, WriteOptions q, CancellationToken ct = default(CancellationToken));
        Task<WriteResult<bool>> Delete(string id, CancellationToken ct = default(CancellationToken));
        Task<WriteResult<bool>> Delete(string id, WriteOptions q, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<RoleEntry[]>> List(CancellationToken ct = default(CancellationToken));
        Task<QueryResult<RoleEntry[]>> List(QueryOptions q, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<RoleEntry>> Read(string id, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<RoleEntry>> Read(string id, QueryOptions q, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<RoleEntry>> ReadByName(string name, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<RoleEntry>> ReadByName(string name, QueryOptions q, CancellationToken ct = default(CancellationToken));
        Task<WriteResult<RoleEntry>> Update(RoleEntry role, CancellationToken ct = default(CancellationToken));
        Task<WriteResult<RoleEntry>> Update(RoleEntry role, WriteOptions q, CancellationToken ct = default(CancellationToken));
    }
}
