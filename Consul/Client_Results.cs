// -----------------------------------------------------------------------
//  <copyright file="Client_Results.cs" company="G-Research Limited">
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
using System.Net;

namespace Consul
{
    public abstract class ConsulResult
    {
        /// <summary>
        /// How long the request took
        /// </summary>
        public TimeSpan RequestTime { get; set; }

        /// <summary>
        /// Exposed so that the consumer can to check for a specific status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        public ConsulResult() { }
        public ConsulResult(ConsulResult other)
        {
            RequestTime = other.RequestTime;
            StatusCode = other.StatusCode;
        }
    }
    /// <summary>
    /// The result of a Consul API query
    /// </summary>
    public class QueryResult : ConsulResult
    {
        public enum CacheResult
        {
            Miss,
            Hit
        }

        /// <summary>
        /// In all cases the HTTP `X-Cache` header is always set in the response to either `HIT` or `MISS` indicating whether the response was served from cache or not.
        /// </summary>
        public CacheResult? XCache { get; set; }

        /// <summary>
        /// For cache hits, the HTTP `Age` header is always set in the response to indicate how many seconds since that response was fetched from the servers.
        /// As long as the local agent has an active connection to the servers, the age will always be 0 since the value is up-to-date.
        /// </summary>
        public TimeSpan Age { get; set; }

        /// <summary>
        /// The index number when the query was serviced. This can be used as a WaitIndex to perform a blocking query
        /// </summary>
        public ulong LastIndex { get; set; }

        /// <summary>
        /// Time of last contact from the leader for the server servicing the request
        /// </summary>
        public TimeSpan LastContact { get; set; }

        /// <summary>
        /// Is there a known leader
        /// </summary>
        public bool KnownLeader { get; set; }

        /// <summary>
        /// Is address translation enabled for HTTP responses on this agent
        /// </summary>
        public bool AddressTranslationEnabled { get; set; }

        public QueryResult() { }
        public QueryResult(QueryResult other) : base(other)
        {
            LastIndex = other.LastIndex;
            LastContact = other.LastContact;
            KnownLeader = other.KnownLeader;
        }
    }

    /// <summary>
    /// The result of a Consul API query
    /// </summary>
    /// <typeparam name="T">Must be able to be deserialized from JSON</typeparam>
    public class QueryResult<T> : QueryResult
    {
        /// <summary>
        /// The result of the query
        /// </summary>
        public T Response { get; set; }
        public QueryResult() { }
        public QueryResult(QueryResult other) : base(other) { }
        public QueryResult(QueryResult other, T value) : base(other)
        {
            Response = value;
        }
    }

    /// <summary>
    /// The result of a Consul API write
    /// </summary>
    public class WriteResult : ConsulResult
    {
        public WriteResult() { }
        public WriteResult(WriteResult other) : base(other) { }
    }
    /// <summary>
    /// The result of a Consul API write
    /// </summary>
    /// <typeparam name="T">Must be able to be deserialized from JSON. Some writes return nothing, in which case this should be an empty Object</typeparam>
    public class WriteResult<T> : WriteResult
    {
        /// <summary>
        /// The result of the write
        /// </summary>
        public T Response { get; set; }
        public WriteResult() { }
        public WriteResult(WriteResult other) : base(other) { }
        public WriteResult(WriteResult other, T value) : base(other)
        {
            Response = value;
        }
    }
}
