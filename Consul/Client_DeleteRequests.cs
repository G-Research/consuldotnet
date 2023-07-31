// -----------------------------------------------------------------------
//  <copyright file="Client_DeleteRequests.cs" company="G-Research Limited">
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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    /// <summary>
    /// Performs a DELETE to API endpoints in Consul, returning a generic type
    /// </summary>
    /// <typeparam name="TOut">A generic type to be deserialised and returned</typeparam>
    public class DeleteReturnRequest<TOut> : ConsulRequest
    {
        public WriteOptions Options { get; set; }

        public DeleteReturnRequest(ConsulClient client, string url, WriteOptions options = null) : base(client, url, HttpMethod.Delete)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(null, nameof(url));
            }
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the DELETE request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the DELETE, including a deserialised generic type object</returns>
        public async Task<WriteResult<TOut>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            var timer = Stopwatch.StartNew();
            var result = new WriteResult<TOut>();

            var message = new HttpRequestMessage(HttpMethod.Delete, BuildConsulUri(Endpoint, Params));
            ApplyHeaders(message, Client.Config);
            var response = await Client.HttpClient.SendAsync(message, ct).ConfigureAwait(false);

            result.StatusCode = response.StatusCode;

            ResponseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
            {
                if (ResponseStream == null)
                {
                    throw new ConsulRequestException(string.Format("Unexpected response, status code {0}",
                        response.StatusCode), response.StatusCode);
                }
                using (var sr = new StreamReader(ResponseStream))
                {
                    throw new ConsulRequestException(string.Format("Unexpected response, status code {0}: {1}",
                        response.StatusCode, sr.ReadToEnd()), response.StatusCode);
                }
            }

            if (response.IsSuccessStatusCode)
            {
                result.Response = Deserialize<TOut>(ResponseStream);
            }

            result.RequestTime = timer.Elapsed;

            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == WriteOptions.Default)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Options.Namespace))
            {
                Params["ns"] = Options.Namespace;
            }

            if (!string.IsNullOrEmpty(Options.Datacenter))
            {
                Params["dc"] = Options.Datacenter;
            }
        }

        protected override void ApplyHeaders(HttpRequestMessage message, ConsulClientConfiguration clientConfig)
        {
            if (!string.IsNullOrEmpty(Options.Token))
            {
                message.Headers.Add("X-Consul-Token", Options.Token);
            }
        }
    }

    /// <summary>
    /// Performs a DELETE to API endpoints in Consul
    /// </summary>
    public class DeleteRequest : ConsulRequest
    {
        public WriteOptions Options { get; set; }

        public DeleteRequest(ConsulClient client, string url, WriteOptions options = null) : base(client, url, HttpMethod.Delete)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(null, nameof(url));
            }
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the DELETE request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the DELETE</returns>
        public async Task<WriteResult> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            var timer = Stopwatch.StartNew();
            var result = new WriteResult();

            var message = new HttpRequestMessage(HttpMethod.Delete, BuildConsulUri(Endpoint, Params));
            ApplyHeaders(message, Client.Config);
            var response = await Client.HttpClient.SendAsync(message, ct).ConfigureAwait(false);

            result.StatusCode = response.StatusCode;

            ResponseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
            {
                if (ResponseStream == null)
                {
                    throw new ConsulRequestException(string.Format("Unexpected response, status code {0}",
                        response.StatusCode), response.StatusCode);
                }
                using (var sr = new StreamReader(ResponseStream))
                {
                    throw new ConsulRequestException(string.Format("Unexpected response, status code {0}: {1}",
                        response.StatusCode, sr.ReadToEnd()), response.StatusCode);
                }
            }

            result.RequestTime = timer.Elapsed;
            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == WriteOptions.Default)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Options.Namespace))
            {
                Params["ns"] = Options.Namespace;
            }

            if (!string.IsNullOrEmpty(Options.Datacenter))
            {
                Params["dc"] = Options.Datacenter;
            }
        }

        protected override void ApplyHeaders(HttpRequestMessage message, ConsulClientConfiguration clientConfig)
        {
            if (!string.IsNullOrEmpty(Options.Token))
            {
                message.Headers.Add("X-Consul-Token", Options.Token);
            }
        }
    }

    /// <summary>
    /// Performs a DELETE to API endpoints in Consul
    /// </summary>
    /// <typeparam name="TIn">A generic type to be serialised and sent with the DELETE</typeparam>
    public class DeleteAcceptingRequest<TIn> : ConsulRequest
    {
        public WriteOptions Options { get; set; }
        private readonly TIn _body;

        public DeleteAcceptingRequest(ConsulClient client, string url, TIn body, WriteOptions options = null) : base(client, url, HttpMethod.Delete)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(null, nameof(url));
            }
            _body = body;
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the DELETE request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the DELETE</returns>
        public async Task<WriteResult> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            var result = new WriteResult();
            var timer = Stopwatch.StartNew();

            HttpContent content;

            if (typeof(TIn) == typeof(byte[]))
            {
                content = new ByteArrayContent((_body as byte[]) ?? Array.Empty<byte>());
            }
            else if (typeof(TIn) == typeof(Stream))
            {
                content = new StreamContent((_body as Stream) ?? new MemoryStream());
            }
            else
            {
                content = new ByteArrayContent(Serialize(_body));
            }

            var message = new HttpRequestMessage(HttpMethod.Delete, BuildConsulUri(Endpoint, Params));
            ApplyHeaders(message, Client.Config);
            message.Content = content;

            var response = await Client.HttpClient.SendAsync(message, ct).ConfigureAwait(false);

            result.StatusCode = response.StatusCode;

            ResponseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
            {
                if (ResponseStream == null)
                {
                    throw new ConsulRequestException(string.Format("Unexpected response, status code {0}",
                        response.StatusCode), response.StatusCode);
                }
                using (var sr = new StreamReader(ResponseStream))
                {
                    throw new ConsulRequestException(string.Format("Unexpected response, status code {0}: {1}",
                        response.StatusCode, sr.ReadToEnd()), response.StatusCode);
                }
            }

            result.RequestTime = timer.Elapsed;
            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == WriteOptions.Default)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Options.Namespace))
            {
                Params["ns"] = Options.Namespace;
            }

            if (!string.IsNullOrEmpty(Options.Datacenter))
            {
                Params["dc"] = Options.Datacenter;
            }
        }

        protected override void ApplyHeaders(HttpRequestMessage message, ConsulClientConfiguration clientConfig)
        {
            if (!string.IsNullOrEmpty(Options.Token))
            {
                message.Headers.Add("X-Consul-Token", Options.Token);
            }
        }
    }

}
