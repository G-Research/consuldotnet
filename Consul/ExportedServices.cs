// -----------------------------------------------------------------------
//  <copyright file="ExportedServices.cs" company="G-Research Limited">
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
using System.Threading;
using System.Threading.Tasks;
using Consul.Interfaces;
using Newtonsoft.Json;

namespace Consul
{
    public class ExportedServicesConfigEntry : IConfigurationEntry
    {
        [JsonProperty("Kind")]
        public string Kind { get; set; } = "exported-services";

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Services")]
        public ExportedServiceConfig[] Services { get; set; }
    }

    public class ExportedServiceConfig
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Consumers")]
        public ServiceConsumer[] Consumers { get; set; }
    }

    public class ServiceConsumer
    {
        [JsonProperty("Peer", NullValueHandling = NullValueHandling.Ignore)]
        public string Peer { get; set; }

        [JsonProperty("Partition", NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }
    }

    public class ResolvedExportedService
    {
        [JsonProperty("Service")]
        public string Service { get; set; }

        [JsonProperty("Consumers")]
        public ResolvedConsumer Consumers { get; set; }
    }

    public class ResolvedConsumer
    {
        [JsonProperty("Peers")]
        public List<string> Peers { get; set; }

        [JsonProperty("Partitions")]
        public List<string> Partitions { get; set; }
    }
    public class ExportedServices : IExportedServicesEndpoint
    {
        private readonly ConsulClient _client;
        internal ExportedServices(ConsulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Lists the exported services
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The exported services</returns>
        public Task<QueryResult<ResolvedExportedService[]>> ListExportedService(CancellationToken cancellationToken = default)
        {
            return ListExportedService(null, cancellationToken);
        }

        /// <summary>
        /// Lists the exported services
        /// </summary>
        /// <param name="q">Query parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The exported services</returns>
        public Task<QueryResult<ResolvedExportedService[]>> ListExportedService(QueryOptions q,
            CancellationToken cancellationToken = default)
        {
            return _client.Get<ResolvedExportedService[]>("/v1/exported-services", q).Execute(cancellationToken);
        }
    }
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<ExportedServices> _exportedServices;

        /// <summary>
        /// Exported Services returns a handle to the Exported Services endpoints
        /// </summary>
        public IExportedServicesEndpoint ExportedServices => _exportedServices.Value;
    }
}
