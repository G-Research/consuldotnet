// -----------------------------------------------------------------------
//  <copyright file="IEventEndpoint.cs" company="PlayFab Inc">>
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
    /// The interface for the Event API Endpoints
    /// </summary>
    public interface IEventEndpoint
    {
        Task<WriteResult<string>> Fire(UserEvent ue, CancellationToken ct = default);
        Task<WriteResult<string>> Fire(UserEvent ue, WriteOptions q, CancellationToken ct = default);
        ulong IDToIndex(string uuid);
        Task<QueryResult<UserEvent[]>> List(CancellationToken ct = default);
        Task<QueryResult<UserEvent[]>> List(string name, CancellationToken ct = default);
        Task<QueryResult<UserEvent[]>> List(string name, QueryOptions q, CancellationToken ct = default);
    }
}
