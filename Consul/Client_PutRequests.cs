// -----------------------------------------------------------------------
//  <copyright file="Client_PutRequests.cs" company="G-Research Limited">
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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#if !(CORECLR || PORTABLE || PORTABLE40)
    using System.Security.Permissions;
    using System.Runtime.Serialization;
#endif

namespace Consul
{
    /// <summary>
    /// Performs a PUT to API endpoints in Consul, returning a generic type
    /// </summary>
    /// <typeparam name="TOut">A generic type to be deserialised and returned</typeparam>
    public class PutReturningRequest<TOut> : ConsulRequest
    {
        public WriteOptions Options { get; set; }

        public PutReturningRequest(ConsulClient client, string url, WriteOptions options = null) : base(client, url, HttpMethod.Put)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(nameof(url));
            }
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the PUT request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the PUT, including a deserialised generic type object</returns>
        public async Task<WriteResult<TOut>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            timer.Start();
            var result = new WriteResult<TOut>();

            var message = new HttpRequestMessage(HttpMethod.Put, BuildConsulUri(Endpoint, Params));
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
            timer.Stop();

            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == WriteOptions.Default)
            {
                return;
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
    /// Performs a PUT to API endpoints in Consul
    /// </summary>
    public class PutNothingRequest : ConsulRequest
    {
        public WriteOptions Options { get; set; }

        public PutNothingRequest(ConsulClient client, string url, WriteOptions options = null) : base(client, url, HttpMethod.Put)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(nameof(url));
            }
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the PUT request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the PUT</returns>
        public async Task<WriteResult> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            timer.Start();
            var result = new WriteResult();

            var message = new HttpRequestMessage(HttpMethod.Put, BuildConsulUri(Endpoint, Params));
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
            timer.Stop();

            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == WriteOptions.Default)
            {
                return;
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
    /// Performs a PUT to API endpoints in Consul, sending a generic type
    /// </summary>
    /// <typeparam name="TIn">A generic type to be serialised and sent</typeparam>
    public class PutRequest<TIn> : ConsulRequest
    {
        public WriteOptions Options { get; set; }
        private TIn _body;

        public PutRequest(ConsulClient client, string url, TIn body, WriteOptions options = null) : base(client, url, HttpMethod.Put)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(nameof(url));
            }
            _body = body;
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the PUT request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the PUT</returns>
        public async Task<WriteResult> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            timer.Start();
            var result = new WriteResult();

            HttpContent content;

            if (typeof(TIn) == typeof(byte[]))
            {
                content = new ByteArrayContent((_body as byte[]) ?? new byte[0]);
            }
            else if (typeof(TIn) == typeof(Stream))
            {
                content = new StreamContent((_body as Stream) ?? new MemoryStream());
            }
            else
            {
                content = new ByteArrayContent(Serialize(_body));
            }

            var message = new HttpRequestMessage(HttpMethod.Put, BuildConsulUri(Endpoint, Params));
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
            timer.Stop();

            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == WriteOptions.Default)
            {
                return;
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
    /// Performs a PUT to API endpoints in Consul, sending and returning generic types
    /// </summary>
    /// <typeparam name="TIn">A generic type to be serialised and sent</typeparam>
    /// <typeparam name="TOut">A generic type to be deserialised and returned</typeparam>
    public class PutRequest<TIn, TOut> : ConsulRequest
    {
        public WriteOptions Options { get; set; }
        private TIn _body;

        public PutRequest(ConsulClient client, string url, TIn body, WriteOptions options = null) : base(client, url, HttpMethod.Put)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(nameof(url));
            }
            _body = body;
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the PUT request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the PUT, including a deserialised generic type object</returns>
        public async Task<WriteResult<TOut>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            timer.Start();
            var result = new WriteResult<TOut>();

            HttpContent content = null;

            if (typeof(TIn) == typeof(byte[]))
            {
                var bodyBytes = (_body as byte[]);
                if (bodyBytes != null)
                {
                    content = new ByteArrayContent(bodyBytes);
                }
                // If body is null and should be a byte array, then just don't send anything.
            }
            else
            {
                content = new ByteArrayContent(Serialize(_body));
            }

            var message = new HttpRequestMessage(HttpMethod.Put, BuildConsulUri(Endpoint, Params));
            ApplyHeaders(message, Client.Config);
            message.Content = content;
            var response = await Client.HttpClient.SendAsync(message, ct).ConfigureAwait(false);

            result.StatusCode = response.StatusCode;

            ResponseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode && (
                (response.StatusCode != HttpStatusCode.NotFound && typeof(TOut) != typeof(TxnResponse)) ||
                (response.StatusCode != HttpStatusCode.Conflict && typeof(TOut) == typeof(TxnResponse))))
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

            if (response.IsSuccessStatusCode ||
                // Special case for KV txn operations
                (response.StatusCode == HttpStatusCode.Conflict && typeof(TOut) == typeof(TxnResponse)))
            {
                result.Response = Deserialize<TOut>(ResponseStream);
            }

            result.RequestTime = timer.Elapsed;
            timer.Stop();

            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == WriteOptions.Default)
            {
                return;
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
