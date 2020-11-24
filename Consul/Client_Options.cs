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
#if !(CORECLR || PORTABLE || PORTABLE40)
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
        /// Providing a datacenter overwrites the DC provided by the Config
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        /// The consistency level required for the operation
        /// </summary>
        public ConsistencyMode Consistency { get; set; }

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
        /// Providing a datacenter overwrites the DC provided by the Config
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        /// Token is used to provide a per-request ACL token which overrides the agent's default token.
        /// </summary>
        public string Token { get; set; }
    }
}
