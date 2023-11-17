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
using Consul.Interfaces;

namespace Consul
{
    public class ConfigurationPayload
    {
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Protocol { get; set; }
    }


    public class Configuration : IConfigurationEndpoint
    {
        private readonly ConsulClient _client;

        internal Configuration(ConsulClient c)
        {
            _client = c;
        }
        public Task<WriteResult> ApplyConfig(ConfigurationPayload cp, CancellationToken ct = default)
        {
            return ApplyConfig(string.Empty, 0, cp, WriteOptions.Default, ct);
        }
        public Task<WriteResult> ApplyConfig(string dc, ConfigurationPayload cp, CancellationToken ct = default)
        {
            return ApplyConfig(dc, 0, cp, WriteOptions.Default, ct);
        }

        public Task<WriteResult> ApplyConfig(int cas, ConfigurationPayload cp, CancellationToken ct = default)
        {
            return ApplyConfig(string.Empty, cas, cp, WriteOptions.Default, ct);
        }
        public Task<WriteResult> ApplyConfig(string dc, int cas, ConfigurationPayload cp, WriteOptions q, CancellationToken ct = default)
        {
            var req = _client.Put("/v1/config", cp, q);
            if (!string.IsNullOrEmpty(dc))
            {
                req.Params["dc"] = dc;
            }
            if (cas > 0)
            {
                req.Params["cas"] = cas.ToString();
            }
            return req.Execute(ct);
        }


    }
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Configuration> _configuration;

        /// <summary>
        /// ConsulClient returns a handle to the catalog endpoints
        /// </summary>
        public IConfigurationEndpoint Configuration => _configuration.Value;
    }
}
