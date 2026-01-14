// -----------------------------------------------------------------------
//  <copyright file="DiscoveryChain.cs" company="G-Research Limited">
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
    public class DiscoveryChainResponse
    {
        public CompiledDiscoveryChain Chain { get; set; }
    }

    public class CompiledDiscoveryChain
    {
        public string ServiceName { get; set; }
        public string Namespace { get; set; }
        public string Datacenter { get; set; }

        // CustomizationHash is a unique hash of any data that affects the
        // compilation of the discovery chain other than config entries or the
        // name/namespace/datacenter evaluation criteria.
        //
        // If set, this value should be used to prefix/suffix any generated load
        // balancer data plane objects to avoid sharing customized and
        // non-customized versions.
        public string CustomizationHash { get; set; }

        // Protocol is the overall protocol shared by everything in the chain.
        public string Protocol { get; set; }
        // StartNode is the first key into the Nodes map that should be followed
        // when walking the discovery chain.
        public string StartNode { get; set; }

        // Nodes contains all nodes available for traversal in the chain keyed by a
        // unique name.  You can walk this by starting with StartNode.
        //
        // NOTE: The names should be treated as opaque values and are only
        // guaranteed to be consistent within a single compilation.
        public Dictionary<string, DiscoveryGraphNode> Nodes { get; set; }

        // Targets is a list of all targets used in this chain.
        // NOTE: The names should be treated as opaque values and are only
        // guaranteed to be consistent within a single compilation.
        public Dictionary<string, DiscoveryTarget> Targets { get; set; }
    }

    public class DiscoveryGraphNode
    {
        public string Type { get; set; }
        public string Name { get; set; } // This is NOT necessarily a service

        // fields for Type==router
        public List<DiscoveryRoute> Routes { get; set; }

        // fields for Type==splitter
        public List<DiscoverySplit> Splits { get; set; }

        // fields for Type==resolver
        public DiscoveryResolver Resolver { get; set; }

        // shared by Type==resolver || Type==splitter
        public LoadBalancerConfig LoadBalancer { get; set; }
    }

    public class DiscoverySplit
    {
        public float Weight { get; set; }
        public string NextNode { get; set; }
    }

    public class DiscoveryRoute
    {
        public Routes Definition { get; set; }
        public string NextNode { get; set; }
    }

    public class DiscoveryResolver
    {
        public bool Default { get; set; }

        [JsonConverter(typeof(DurationTimespanConverter))]
        public TimeSpan? ConnectTimeout { get; set; }

        public string Target { get; set; }
        public DiscoveryFailover Failover { get; set; }
    }
    public class DiscoveryFailover
    {
        public List<string> Targets { get; set; }
    }

    public class DiscoveryTarget
    {
        public string ID { get; set; }
        public string Service { get; set; }
        public string ServiceSubset { get; set; }
        public string Namespace { get; set; }
        public string Datacenter { get; set; }
        public MeshGatewayConfig MeshGateway { get; set; }
        public ServiceResolverEntry Subset { get; set; }
        public bool External { get; set; }
        public string SNI { get; set; }
        public string Name { get; set; }
    }

    public class DiscoveryChainOptions
    {
        /// <summary>
        /// OverrideMeshGateway allows for the mesh gateway setting to be overridden
        /// For any resolver in the compiled chain.
        /// </summary>
        public MeshGatewayConfig OverrideMeshGateway { get; set; }

        /// <summary>
        /// OverrideProtocol allows for the final protocol for the chain to be
        /// altered.
        ///
        /// - If the chain ordinarily would be TCP and an L7 protocol is passed here
        /// the chain will not include Routers or Splitters.
        ///
        /// - If the chain ordinarily would be L7 and TCP is passed here the chain
        /// will not include Routers or Splitters.
        /// </summary>
        public string OverrideProtocol { get; set; }

        /// <summary>
        /// OverrideConnectTimeout allows for the ConnectTimeout setting to be
        /// Overridden for any resolver in the compiled chain.
        /// </summary>
        [JsonConverter(typeof(DurationTimespanConverter))]
        public TimeSpan? OverrideConnectTimeout { get; set; }
    }

    public class DiscoveryChain : IDiscoveryChainEndpoint
    {
        public const string DiscoveryGraphNodeTypeRouter = "router";
        public const string DiscoveryGraphNodeTypeSplitter = "splitter";
        public const string DiscoveryGraphNodeTypeResolver = "resolver";

        private readonly ConsulClient _client;

        internal DiscoveryChain(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// Get is used to return the compiled discovery chain for a service.
        /// </summary>
        /// <param name="name">Specifies the service to query when compiling the discovery chain</param>
        /// <param name="q">Query Options</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>An empty write result</returns>
        public Task<QueryResult<DiscoveryChainResponse>> Get(string name, QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<DiscoveryChainResponse>($"/v1/discovery-chain/{name}", q).Execute(ct);
        }

        /// <summary>
        /// Get is used to return the compiled discovery chain for a service.
        /// </summary>
        /// <param name="name">Specifies the service to query when compiling the discovery chain</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>An empty write result</returns>
        public Task<QueryResult<DiscoveryChainResponse>> Get(string name, CancellationToken ct = default)
        {
            return Get(name, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Get is used to return the compiled discovery chain for a service.
        /// </summary>
        /// <param name="name">Name of the service</param>
        /// <param name="options">Discovery Chain Options</param>
        /// <param name="ct">Cancellation Token</param>
        /// <param name="compileDataCenter">Datacenter to evaluate the discovery chain in</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult<DiscoveryChainResponse>> Get(string name, DiscoveryChainOptions options, string compileDataCenter = null, CancellationToken ct = default)
        {
            return Get(name, options, WriteOptions.Default, compileDataCenter, ct);
        }

        /// <summary>
        /// Get is used to return the compiled discovery chain for a service.
        /// </summary>
        /// <param name="name">Name of the service</param>
        /// <param name="options">Discovery Chain Options</param>
        /// <param name="compileDataCenter">Datacenter to evaluate the discovery chain in</param>
        /// <param name="q">Write Options</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult<DiscoveryChainResponse>> Get(string name, DiscoveryChainOptions options, WriteOptions q, string compileDataCenter = null, CancellationToken ct = default)
        {
            var request = _client.Post<DiscoveryChainOptions, DiscoveryChainResponse>($"/v1/discovery-chain/{name}", options, q);
            if (!string.IsNullOrEmpty(compileDataCenter))
            {
                request.Params["compile-dc"] = compileDataCenter;
            }
            return request.Execute(ct);
        }

    }

    public partial class ConsulClient : IConsulClient
    {
        /// <summary>
        /// DiscoveryChain returns a handle to the discovery chain endpoints
        /// </summary>
        public IDiscoveryChainEndpoint DiscoveryChain { get; private set; }
    }
}
