// -----------------------------------------------------------------------
//  <copyright file="IRawEndpoint.cs" company="PlayFab Inc">
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

using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    /// <summary>
    /// The interface for the Raw API Endpoints
    /// </summary>
    public interface IRawEndpoint
    {
        Task<QueryResult<dynamic>> Query(string endpoint, QueryOptions q, CancellationToken ct = default(CancellationToken));
        Task<WriteResult<dynamic>> Write(string endpoint, object obj, WriteOptions q, CancellationToken ct = default(CancellationToken));
    }
}
