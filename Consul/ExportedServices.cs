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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul.Interfaces;
using Newtonsoft.Json;

namespace Consul
{
    public class ResolvedExportedService
    {
        /// <summary>
        /// Service is the name of the service which is exported.
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Partition of the service
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        /// <summary>
        /// Namespace of the service
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        /// <summary>
        /// Consumers is a list of downstream consumers of the service
        /// </summary>
        public ResolvedConsumers Consumers { get; set; }
    }

    public class ResolvedConsumers
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Peers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Partitions { get; set; }
    }

    public class ExportedServices : IExportedServicesEnpoint
    {
        private readonly ConsulClient _client;
        internal ExportedServices(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// List of exported services, as well as the admin partitions and cluster peers that consume the services
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns> A query result containing an array of exported service</returns>
        public Task<QueryResult<ResolvedExportedService[]>> List(CancellationToken ct = default)
        {
            return List(QueryOptions.Default, ct);
        }

        /// <summary>
        /// List of exported services, as well as the admin partitions and cluster peers that consume the services
        /// </summary>
        /// <param name="queryOptions">Customised query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns> A query result containing an array of exported service</returns>
        public async Task<QueryResult<ResolvedExportedService[]>> List(QueryOptions queryOptions, CancellationToken ct = default)
        {
            var res = await _client.Get<ResolvedExportedService[]>("/v1/exported-services", queryOptions).Execute(ct).ConfigureAwait(false);
            return new QueryResult<ResolvedExportedService[]>(res, res.Response);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<ExportedServices> _exportedServices;

        /// <summary>
        /// Exported services returns a handle to the exported services endpoints
        /// </summary>
        public IExportedServicesEnpoint ExportedServices => _exportedServices.Value;
    }
}
