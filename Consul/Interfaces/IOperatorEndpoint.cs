// -----------------------------------------------------------------------
//  <copyright file="IOperatorEndpoint.cs" company="PlayFab Inc">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    /// <summary>
    /// The interface for the Operator API Endpoints
    /// </summary>
    public interface IOperatorEndpoint
    {
        Task<QueryResult<RaftConfiguration>> RaftGetConfiguration(CancellationToken ct = default);
        Task<QueryResult<RaftConfiguration>> RaftGetConfiguration(QueryOptions q, CancellationToken ct = default);
        Task<WriteResult> RaftRemovePeerByAddress(string address, CancellationToken ct = default);
        Task<WriteResult> RaftRemovePeerByAddress(string address, WriteOptions q, CancellationToken ct = default);
        Task<WriteResult> KeyringInstall(string key, CancellationToken ct = default);
        Task<WriteResult> KeyringInstall(string key, WriteOptions q, CancellationToken ct = default);
        Task<QueryResult<KeyringResponse[]>> KeyringList(CancellationToken ct = default);
        Task<QueryResult<KeyringResponse[]>> KeyringList(QueryOptions q, CancellationToken ct = default);
        Task<WriteResult> KeyringRemove(string key, CancellationToken ct = default);
        Task<WriteResult> KeyringRemove(string key, WriteOptions q, CancellationToken ct = default);
        Task<WriteResult> KeyringUse(string key, CancellationToken ct = default);
        Task<WriteResult> KeyringUse(string key, WriteOptions q, CancellationToken ct = default);
        Task<QueryResult<ConsulLicense>> GetConsulLicense(string datacenter = "", CancellationToken ct = default);
    }
}
