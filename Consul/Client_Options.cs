// -----------------------------------------------------------------------
//  <copyright file="Client_Options.cs" company="G-Research Limited">
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
#if !(NETSTANDARD || NETCOREAPP)
    using System.Security.Permissions;
    using System.Runtime.Serialization;
#endif

namespace Consul
{
    /// <summary>
    /// QueryOptions are used to parameterize a query
    /// </summary>
    public class QueryOptions
    {
        public static readonly QueryOptions Default = new QueryOptions()
        {
            Consistency = ConsistencyMode.Default,
            Datacenter = string.Empty,
            Token = string.Empty,
            WaitIndex = 0
        };

        /// <summary>
        /// Namespace overrides the `default` namespace.
        /// Note: Namespaces are available only in Consul Enterprise
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Partition overrides the `default` partition.
        /// Note: Partitions are available only in Consul Enterprise
        /// </summary>
        public string Partition { get; set; }

        /// <summary>
        /// Providing a datacenter overwrites the DC provided by the Config.
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        /// Providing a peer name in the query option.
        /// </summary>
        public string Peer { get; set; }

        /// <summary>
        /// AllowStale allows any Consul server (non-leader) to service a read.
        /// This allows for lower latency and higher throughput.
        /// </summary>
        public bool AllowStale { get; set; }

        /// <summary>
        /// The consistency level required for the operation.
        /// </summary>
        public ConsistencyMode Consistency { get; set; }

        /// <summary>
        /// UseCache requests that the agent cache results locally.
        /// See https://www.consul.io/api/features/caching.html for more details on the semantics.
        /// </summary>
        public bool UseCache { get; set; }

        /// <summary>
        /// MaxAge limits how old a cached value will be returned if UseCache is true.
        /// If there is a cached response that is older than the MaxAge, it is treated as a cache miss and a new fetch invoked.
        /// If the fetch fails, the error is returned.
        /// Clients that wish to allow for stale results on error can set StaleIfError to a longer duration to change this behavior.
        /// It is ignored if the endpoint supports background refresh caching.
        /// See https://www.consul.io/api/features/caching.html for more details.
        /// </summary>
        public TimeSpan? MaxAge { get; set; }

        /// <summary>
        /// StaleIfError specifies how stale the client will accept a cached response if the servers are unavailable to fetch a fresh one.
        /// Only makes sense when UseCache is true and MaxAge is set to a lower, non-zero value.
        /// It is ignored if the endpoint supports background refresh caching.
        /// See https://www.consul.io/api/features/caching.html for more details.
        /// </summary>
        public TimeSpan StaleIfError { get; set; }

        /// <summary>
        /// WaitIndex is used to enable a blocking query. Waits until the timeout or the next index is reached
        /// </summary>
        public ulong WaitIndex { get; set; }

        /// <summary>
        /// WaitHash is used by some endpoints instead of WaitIndex to perform blocking on state based on a hash of the response rather than a monotonic index.
        /// This is required when the state being blocked on is not stored in Raft, for example agent-local proxy configuration.
        /// </summary>
        public string WaitHash { get; set; }

        /// <summary>
        /// WaitTime is used to bound the duration of a wait. Defaults to that of the Config, but can be overridden.
        /// </summary>
        public TimeSpan? WaitTime { get; set; }

        /// <summary>
        /// Token is used to provide a per-request ACL token which overrides the agent's default token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Near is used to provide a node name that will sort the results
        /// in ascending order based on the estimated round trip time from
        /// that node. Setting this to "_agent" will use the agent's node
        /// for the sort.
        /// </summary>
        public string Near { get; set; }


        /// <summary>
        /// NodeMeta is used to filter results by nodes with the given metadata key/value pairs.
        /// Currently, only one key/value pair can be used for filtering.
        /// </summary>
        public Dictionary<string, string> NodeMeta { get; set; }

        /// <summary>
        /// RelayFactor is used in keyring operations to cause responses to be relayed back to the sender through N other random nodes.
        /// Must be a value from 0 to 5 (inclusive)
        /// </summary>
        public uint RelayFactor { get; set; }

        /// <summary>
        /// LocalOnly is used in keyring list operation to force the keyring query to only hit local servers (no WAN traffic).
        /// </summary>
        public bool LocalOnly { get; set; }

        /// <summary>
        /// Connect filters prepared query execution to only include Connect-capable services.
        /// This currently affects prepared query execution.
        /// </summary>
        public bool Connect { get; set; }

        /// <summary>
        /// Filter requests filtering data prior to it being returned.
        /// The string is a C# compatible boolean expression.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        // MergeCentralConfig returns a service definition merged with the proxy-defaults/global and service-defaults/:service config entries.
        // This can be used to ensure a full service definition is returned in the response especially when the service might not be written into the catalog that way.
        /// </summary>
        public bool MergeCentralConfig { get; set; }

        /// <summary>
	    /// Global is used to request information from all datacenters.
	    /// Currently only used for operator usage requests.
        /// </summary>
        public bool Global { get; set; }
    }

    /// <summary>
    /// WriteOptions are used to parameterize a write
    /// </summary>
    public class WriteOptions
    {
        public static readonly WriteOptions Default = new WriteOptions()
        {
            Datacenter = string.Empty,
            Token = string.Empty
        };

        /// <summary>
        /// Namespace overrides the `default` namespace
        /// Note: Namespaces are available only in Consul Enterprise
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Partition overrides the `default` partition
        /// Note: Partitions are available only in Consul Enterprise
        /// </summary>
        public string Partition { get; set; }

        /// <summary>
        /// Providing a datacenter overwrites the DC provided by the Config
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        /// Token is used to provide a per-request ACL token which overrides the agent's default token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// RelayFactor is used in keyring operations to cause responses to be relayed back to the sender through N other random nodes.
        /// Must be a value from 0 to 5 (inclusive)
        /// </summary>
        public uint RelayFactor { get; set; }

    }
}
