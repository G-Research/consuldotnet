// -----------------------------------------------------------------------
//  <copyright file="Raw.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
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
using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    /// <summary>
    /// Raw can be used to do raw queries against custom endpoints
    /// </summary>
    public class Raw : IRawEndpoint
    {
        private readonly ConsulClient _client;

        internal Raw(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// Query is used to do a GET request against an endpoint and deserialize the response into an interface using standard Consul conventions.
        /// </summary>
        /// <param name="endpoint">The URL endpoint to access</param>
        /// <param name="q">Custom query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The data returned by the custom endpoint</returns>
        public Task<QueryResult<dynamic>> Query(string endpoint, QueryOptions q, CancellationToken ct = default(CancellationToken))
        {
            return _client.Get<dynamic>(endpoint, q).Execute(ct);
        }

        /// <summary>
        /// Write is used to do a PUT request against an endpoint and serialize/deserialized using the standard Consul conventions.
        /// </summary>
        /// <param name="endpoint">The URL endpoint to access</param>
        /// <param name="obj">The object to serialize and send to the endpoint. Must be able to be JSON serialized, or be an object of type byte[], which is sent without serialzation.</param>
        /// <param name="q">Custom write options</param>
        /// <returns>The data returned by the custom endpoint in response to the write request</returns>
        public Task<WriteResult<dynamic>> Write(string endpoint, object obj, WriteOptions q, CancellationToken ct = default(CancellationToken))
        {
            return _client.Put<object, dynamic>(endpoint, obj, q).Execute(ct);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Raw> _raw;

        /// <summary>
        /// Raw returns a handle to query endpoints
        /// </summary>
        public IRawEndpoint Raw
        {
            get { return _raw.Value; }
        }
    }
}
