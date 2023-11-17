// -----------------------------------------------------------------------
//  <copyright file="Config.cs" company="G-Research Limited">
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

namespace Consul
{
    public class ConfigPayload
    {
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Protocol { get; set; }
    }
    public class Config
    {
        public Task<WriteResult> ApplyConfig(ConfigPayload cp, CancellationToken ct = default)
        {
            return ApplyConfig(string.Empty, 0, cp, WriteOptions.Default, ct);
        }
        public Task<WriteResult> ApplyConfig(string dc, ConfigPayload cp, CancellationToken ct = default)
        {
            return ApplyConfig(dc, 0, cp, WriteOptions.Default, ct);
        }

        public Task<WriteResult> ApplyConfig(int cas, ConfigPayload cp, CancellationToken ct = default)
        {
            return ApplyConfig(string.Empty, cas, cp, WriteOptions.Default, ct);
        }
        public Task<WriteResult> ApplyConfig(string dc = "", int cas = 0, ConfigPayload cp, WriteOptions q, CancellationToken ct = default)
        {
            var req = _client.Get<CatalogService[]>(string.Format("/v1/catalog/service/{0}", service), q);
            return _client.Put("/v1/catalog/deregister", reg, q).Execute(ct);
            if (string.IsNullOrEmpty(dc))
            {

            }
        }
    }

}
