// -----------------------------------------------------------------------
//  <copyright file="Client_PostRequests.cs" company="G-Research Limited">
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    /// <summary>
    /// Performs a POST to API endpoints in Consul, returning a generic type
    /// </summary>
    /// <typeparam name="TOut">A generic type to be deserialised and returned</typeparam>
    public class PostReturningRequest<TOut> : ConsulRequest
    {
        public WriteOptions Options { get; set; }

        public PostReturningRequest(ConsulClient client, string url, WriteOptions options = null) : base(client, url, HttpMethod.Post)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(null, nameof(url));
            }

            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the POST request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the POST, including a deserialised generic type object</returns>
        public async Task<WriteResult<TOut>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            var timer = Stopwatch.StartNew();
            var result = new WriteResult<TOut>();

            HttpContent content = null;

            var message = new HttpRequestMessage(HttpMethod.Post, BuildConsulUri(Endpoint, Params));
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
    /// Performs a POST to API endpoints in Consul, sending a generic type
    /// </summary>
    /// <typeparam name="TIn">A generic type to be serialised and sent with the delete</typeparam>
    public class PostRequest<TIn> : ConsulRequest
    {
        public WriteOptions Options { get; set; }
        private readonly TIn _body;

        public PostRequest(ConsulClient client, string url, TIn body, WriteOptions options = null) : base(client, url, HttpMethod.Post)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(null, nameof(url));
            }
            _body = body;
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the POST request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the POST</returns>
        public async Task<WriteResult> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            var timer = Stopwatch.StartNew();
            var result = new WriteResult();

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

            var message = new HttpRequestMessage(HttpMethod.Post, BuildConsulUri(Endpoint, Params));
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

    /// <summary>
    /// Performs a POST to API endpoints in Consul, sending and returning generic types
    /// </summary>
    /// <typeparam name="TIn">A generic type to be serialised and sent with the POST</typeparam>
    /// <typeparam name="TOut">A generic type to be deserialised and returned</typeparam>
    public class PostRequest<TIn, TOut> : ConsulRequest
    {
        public WriteOptions Options { get; set; }
        private readonly TIn _body;

        public PostRequest(ConsulClient client, string url, TIn body, WriteOptions options = null) : base(client, url, HttpMethod.Post)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(null, nameof(url));
            }
            _body = body;
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the POST request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the POST, including a deserialised generic type object</returns>
        public async Task<WriteResult<TOut>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            var timer = Stopwatch.StartNew();
            var result = new WriteResult<TOut>();

            HttpContent content = null;

            if (typeof(TIn) == typeof(byte[]))
            {
                var bodyBytes = _body as byte[];
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

            var message = new HttpRequestMessage(HttpMethod.Post, BuildConsulUri(Endpoint, Params));
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
    /// Performs a POST to API endpoints in Consul
    /// </summary>
    public class PostRequest : ConsulRequest
    {
        public WriteOptions Options { get; set; }
        private readonly string _body;

        public PostRequest(ConsulClient client, string url, string body, WriteOptions options = null) : base(client, url, HttpMethod.Post)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(null, nameof(url));
            }
            _body = body;
            Options = options ?? WriteOptions.Default;
        }

        /// <summary>
        /// Execute the POST request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the POST, including a string of data</returns>
        public async Task<WriteResult<string>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            var timer = Stopwatch.StartNew();
            var result = new WriteResult<string>();
            var bodyBytes = Encoding.UTF8.GetBytes(_body);
            HttpContent content = new ByteArrayContent(bodyBytes);

            var message = new HttpRequestMessage(HttpMethod.Post, BuildConsulUri(Endpoint, Params));
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

            if (response.IsSuccessStatusCode)
            {
                using (var reader = new StreamReader(ResponseStream))
                {
                    result.Response = reader.ReadToEnd();
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
