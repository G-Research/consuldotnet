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
        /// Namespace is the name of the namespace to send along for the request when no other Namespace is present in the QueryOptions.
        /// Namespace is an Enterprise-only feature.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Providing a datacenter overwrites the DC provided by the Config.
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        /// The consistency level required for the operation.
        /// </summary>
        public ConsistencyMode Consistency { get; set; }

        /// <summary>
        /// WaitIndex is used to enable a blocking query. Waits until the timeout or the next index is reached.
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
        public TimeSpan? StaleIfError { get; set; }

        /// <summary>
        /// WaitIndex is used to enable a blocking query. Waits until the timeout or the next index is reached
        /// </summary>
        public ulong WaitIndex { get; set; }

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
        /// Namespace is the name of the namespace to send along for the request when no other Namespace is present in the QueryOptions
        /// Namespace is an Enterprise-only feature.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Providing a datacenter overwrites the DC provided by the Config
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        /// Token is used to provide a per-request ACL token which overrides the agent's default token.
        /// </summary>
        public string Token { get; set; }
    }
}
