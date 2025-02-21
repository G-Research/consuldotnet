// -----------------------------------------------------------------------
//  <copyright file="IConfigEndpoint.cs" company="G-Research Limited">
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
    public interface IConfigurationEndpoint
    {
        Task<WriteResult> ApplyConfig<TConfig>(WriteOptions q, TConfig configurationEntry, CancellationToken ct = default) where TConfig : IConfigurationEntry;
        Task<WriteResult> ApplyConfig<TConfig>(TConfig configurationEntry, CancellationToken ct = default) where TConfig : IConfigurationEntry;
        Task<QueryResult<TConfig>> GetConfig<TConfig>(string kind, string name, QueryOptions q, CancellationToken ct = default) where TConfig : IConfigurationEntry;
        Task<QueryResult<TConfig>> GetConfig<TConfig>(string kind, string name, CancellationToken ct = default) where TConfig : IConfigurationEntry;
        Task<QueryResult<List<TConfig>>> ListConfig<TConfig>(string kind, QueryOptions q, CancellationToken ct = default) where TConfig : IConfigurationEntry;
        Task<QueryResult<List<TConfig>>> ListConfig<TConfig>(string kind, CancellationToken ct = default) where TConfig : IConfigurationEntry;
        Task<WriteResult> DeleteConfig(string kind, string name, WriteOptions q, CancellationToken ct = default);
        Task<WriteResult> DeleteConfig(string kind, string name, CancellationToken ct = default);
    }
}
