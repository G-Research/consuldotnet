// -----------------------------------------------------------------------
//  <copyright file="IDiscoveryChainEndpoint.cs" company="G-Research Limited">
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

namespace Consul.Interfaces
{
    /// <summary>
    /// The interface for the Discovery Chain API Endpoints
    /// </summary>
    public interface IDiscoveryChainEndpoint
    {
        Task<QueryResult<DiscoveryChainResponse>> Get(string name, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<DiscoveryChainResponse>> Get(string name, CancellationToken ct = default);
        Task<WriteResult<DiscoveryChainResponse>> Get(string name, DiscoveryChainOptions options, WriteOptions q, string compileDataCenter = null, CancellationToken ct = default);
        Task<WriteResult<DiscoveryChainResponse>> Get(string name, DiscoveryChainOptions options, string compileDataCenter = null, CancellationToken ct = default);
    }
}
