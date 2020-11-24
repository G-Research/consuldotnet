// -----------------------------------------------------------------------
//  <copyright file="ICatalogEndpoint.cs" company="PlayFab Inc">
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    /// <summary>
    /// The interface for the Catalog API Endpoints
    /// </summary>
    public interface ICatalogEndpoint
    {
        Task<QueryResult<string[]>> Datacenters(CancellationToken ct = default(CancellationToken));
        Task<WriteResult> Deregister(CatalogDeregistration reg, CancellationToken ct = default(CancellationToken));
        Task<WriteResult> Deregister(CatalogDeregistration reg, WriteOptions q, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<CatalogNode>> Node(string node, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<CatalogNode>> Node(string node, QueryOptions q, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<Node[]>> Nodes(CancellationToken ct = default(CancellationToken));
        Task<QueryResult<Node[]>> Nodes(QueryOptions q, CancellationToken ct = default(CancellationToken));
        Task<WriteResult> Register(CatalogRegistration reg, CancellationToken ct = default(CancellationToken));
        Task<WriteResult> Register(CatalogRegistration reg, WriteOptions q, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<CatalogService[]>> Service(string service, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<CatalogService[]>> Service(string service, string tag, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<CatalogService[]>> Service(string service, string tag, QueryOptions q, CancellationToken ct = default(CancellationToken));
        Task<QueryResult<Dictionary<string, string[]>>> Services(CancellationToken ct = default(CancellationToken));
        Task<QueryResult<Dictionary<string, string[]>>> Services(QueryOptions q, CancellationToken ct = default(CancellationToken));
    }
}
