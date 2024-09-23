// -----------------------------------------------------------------------
//  <copyright file="Client_Request.cs" company="G-Research Limited">
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
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Consul
{
    /// <summary>
    /// The consistency mode of a request.
    /// </summary>
    /// <remarks>
    /// <seealso href="http://www.consul.io/docs/agent/http.html"/>
    /// </remarks>
    public enum ConsistencyMode
    {
        /// <summary>
        /// Default is strongly consistent in almost all cases. However, there is a small window in which a new leader may be elected during which the old leader may service stale values. The trade-off is fast reads but potentially stale values. The condition resulting in stale reads is hard to trigger, and most clients should not need to worry about this case. Also, note that this race condition only applies to reads, not writes.
        /// </summary>
        Default,

        /// <summary>
        /// Consistent forces the read to be fully consistent. This mode is strongly consistent without caveats. It requires that a leader verify with a quorum of peers that it is still leader. This introduces an additional round-trip to all server nodes. The trade-off is increased latency due to an extra round trip. Most clients should not use this unless they cannot tolerate a stale read.
        /// </summary>
        Consistent,

        /// <summary>
        /// Stale allows any Consul server (non-leader) to service a read. This mode allows any server to service the read regardless of whether it is the leader. This means reads can be arbitrarily stale; however, results are generally consistent to within 50 milliseconds of the leader. The trade-off is very fast and scalable reads with a higher likelihood of stale values. Since this mode allows reads without a leader, a cluster that is unavailable will still be able to respond to queries.
        /// </summary>
        Stale
    }

    public abstract class ConsulRequest
    {
        internal ConsulClient Client { get; set; }
        internal HttpMethod Method { get; set; }
        internal Dictionary<string, string> Params { get; set; }
        internal Stream ResponseStream { get; set; }
        internal string Endpoint { get; set; }

        internal readonly JsonSerializer _serializer = new JsonSerializer();

        internal ConsulRequest(ConsulClient client, string url, HttpMethod method)
        {
            Client = client;
            Method = method;
            Endpoint = url;

            Params = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(client.Config.Datacenter))
            {
                Params["dc"] = client.Config.Datacenter;
            }
            if (client.Config.WaitTime.HasValue)
            {
                Params["wait"] = client.Config.WaitTime.Value.ToGoDuration();
            }
            if (!string.IsNullOrEmpty(client.Config.Namespace))
            {
                Params["ns"] = client.Config.Namespace;
            }
        }

        protected abstract void ApplyOptions(ConsulClientConfiguration clientConfig);
        protected abstract void ApplyHeaders(HttpRequestMessage message, ConsulClientConfiguration clientConfig);

        protected internal Uri BuildConsulUri(string url, Dictionary<string, string> p)
        {
            var builder = new UriBuilder(Client.Config.Address);
            builder.Path += url;
            builder.Path = builder.Path.Replace("//", "/");

            ApplyOptions(Client.Config);

            var queryParams = new List<string>(Params.Count / 2);
            foreach (var queryParam in Params)
            {
                if (!string.IsNullOrEmpty(queryParam.Value))
                {
                    queryParams.Add(string.Format("{0}={1}", Uri.EscapeDataString(queryParam.Key),
                        Uri.EscapeDataString(queryParam.Value)));
                }
                else
                {
                    queryParams.Add(string.Format("{0}", Uri.EscapeDataString(queryParam.Key)));
                }
            }

            builder.Query = string.Join("&", queryParams);
            return builder.Uri;
        }

        protected TOut Deserialize<TOut>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return _serializer.Deserialize<TOut>(jsonReader);
                }
            }
        }

        protected static byte[] Serialize(object value) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
    }
}
