// -----------------------------------------------------------------------
//  <copyright file="Client_GetRequests.cs" company="G-Research Limited">
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Consul.Filtering;

namespace Consul
{
    /// <summary>
    /// Performs a GET to API endpoints in Consul, returning a generic type
    /// </summary>
    /// <typeparam name="TOut">A generic type to be deserialised and returned</typeparam>
    public class GetRequest<TOut> : ConsulRequest
    {
        // TODO(marcink): Remove the setter in the next major release to make the class immutable
        public QueryOptions Options { get; set; }

        public IEncodable Filter { get; }

        public GetRequest(ConsulClient client, string url) :
            this(client, url, null, null)
        {
        }

        public GetRequest(ConsulClient client, string url, QueryOptions options) :
            this(client, url, options, null)
        {
        }

        public GetRequest(ConsulClient client, string url, QueryOptions options, IEncodable filter) : base(client, url, HttpMethod.Get)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(nameof(url));
            }
            Options = options ?? QueryOptions.Default;
            Filter = filter;
        }

        /// <summary>
        /// Execute the GET request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the GET, including a deserialised generic type object</returns>
        public async Task<QueryResult<TOut>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            timer.Start();
            var result = new QueryResult<TOut>();

            var message = new HttpRequestMessage(HttpMethod.Get, BuildConsulUri(Endpoint, Params));
            ApplyHeaders(message, Client.Config);
            var response = await Client.HttpClient.SendAsync(message, ct).ConfigureAwait(false);

            ParseQueryHeaders(response, result);
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

        /// <summary>
        /// Execute the GET request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The result of the GET, including a stream of data</returns>
        public async Task<QueryResult<Stream>> ExecuteStreaming(CancellationToken ct)
        {
            Client.CheckDisposed();
            timer.Start();
            var result = new QueryResult<Stream>();

            var message = new HttpRequestMessage(HttpMethod.Get, BuildConsulUri(Endpoint, Params));
            ApplyHeaders(message, Client.Config);
            var response = await Client.HttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

            ParseQueryHeaders(response, (result as QueryResult<TOut>));
            result.StatusCode = response.StatusCode;
            ResponseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            result.Response = ResponseStream;

            if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
            {
                throw new ConsulRequestException(string.Format("Unexpected response, status code {0}",
                    response.StatusCode), response.StatusCode);
            }

            result.RequestTime = timer.Elapsed;
            timer.Stop();

            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Filter != null)
            {
                Params["filter"] = Filter.Encode();
            }

            if (Options == QueryOptions.Default)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Options.Datacenter))
            {
                Params["dc"] = Options.Datacenter;
            }
            switch (Options.Consistency)
            {
                case ConsistencyMode.Consistent:
                    Params["consistent"] = string.Empty;
                    break;
                case ConsistencyMode.Stale:
                    Params["stale"] = string.Empty;
                    break;
                case ConsistencyMode.Default:
                    break;
            }
            if (Options.WaitIndex != 0)
            {
                Params["index"] = Options.WaitIndex.ToString();
            }
            if (Options.WaitTime.HasValue)
            {
                Params["wait"] = Options.WaitTime.Value.ToGoDuration();
            }
            if (!string.IsNullOrEmpty(Options.Near))
            {
                Params["near"] = Options.Near;
            }
        }

        protected void ParseQueryHeaders(HttpResponseMessage res, QueryResult<TOut> meta)
        {
            var headers = res.Headers;

            if (headers.Contains("X-Consul-Index"))
            {
                try
                {
                    meta.LastIndex = ulong.Parse(headers.GetValues("X-Consul-Index").First());
                }
                catch (Exception ex)
                {
                    throw new ConsulRequestException("Failed to parse X-Consul-Index", res.StatusCode, ex);
                }
            }

            if (headers.Contains("X-Consul-LastContact"))
            {
                try
                {
                    meta.LastContact = TimeSpan.FromMilliseconds(ulong.Parse(headers.GetValues("X-Consul-LastContact").First()));
                }
                catch (Exception ex)
                {
                    throw new ConsulRequestException("Failed to parse X-Consul-LastContact", res.StatusCode, ex);
                }
            }

            if (headers.Contains("X-Consul-KnownLeader"))
            {
                try
                {
                    meta.KnownLeader = bool.Parse(headers.GetValues("X-Consul-KnownLeader").First());
                }
                catch (Exception ex)
                {
                    throw new ConsulRequestException("Failed to parse X-Consul-KnownLeader", res.StatusCode, ex);
                }
            }

            if (headers.Contains("X-Consul-Translate-Addresses"))
            {
                try
                {
                    meta.AddressTranslationEnabled = bool.Parse(headers.GetValues("X-Consul-Translate-Addresses").First());
                }
                catch (Exception ex)
                {
                    throw new ConsulRequestException("Failed to parse X-Consul-Translate-Addresses", res.StatusCode, ex);
                }
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
    /// Performs a GET to API endpoints in Consul
    /// </summary>
    public class GetRequest : ConsulRequest
    {
        public QueryOptions Options { get; set; }

        public GetRequest(ConsulClient client, string url, QueryOptions options = null) : base(client, url, HttpMethod.Get)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(nameof(url));
            }
            Options = options ?? QueryOptions.Default;
        }

        /// <summary>
        /// Execute the GET request to the API
        /// </summary>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The results of the GET, including a string of returned data</returns>
        public async Task<QueryResult<string>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            timer.Start();
            var result = new QueryResult<string>();

            var message = new HttpRequestMessage(HttpMethod.Get, BuildConsulUri(Endpoint, Params));
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
                using (var reader = new StreamReader(ResponseStream))
                {
                    result.Response = reader.ReadToEnd();
                }
            }

            timer.Stop();
            result.RequestTime = timer.Elapsed;

            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == QueryOptions.Default)
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
