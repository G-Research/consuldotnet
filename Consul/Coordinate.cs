// -----------------------------------------------------------------------
//  <copyright file="Coordinate.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
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
using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    public class CoordinateEntry
    {
        public string Node { get; set; }
        public string Segment { get; set; }
        public SerfCoordinate Coord { get; set; }
    }

    public class SerfCoordinate
    {
        public List<double> Vec { get; set; }
        public double Error { get; set; }
        public double Adjustment { get; set; }
        public double Height { get; set; }
        public SerfCoordinate()
        {
            Vec = new List<double>();
        }
    }

    // May want to rework this as Dictionary<string,List<CoordinateEntry>>
    public class CoordinateDatacenterMap
    {
        public string Datacenter { get; set; }
        public List<CoordinateEntry> Coordinates { get; set; }
        public CoordinateDatacenterMap()
        {
            Coordinates = new List<CoordinateEntry>();
        }
    }

    public class Coordinate : ICoordinateEndpoint
    {
        private readonly ConsulClient _client;

        internal Coordinate(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// Datacenters is used to return the coordinates of all the servers in the WAN pool.
        /// </summary>
        /// <returns>A query result containing a map of datacenters, each with a list of coordinates of all the servers in the WAN pool</returns>
        public Task<QueryResult<CoordinateDatacenterMap[]>> Datacenters(CancellationToken ct = default)
        {
            return _client.Get<CoordinateDatacenterMap[]>(string.Format("/v1/coordinate/datacenters")).Execute(ct);
        }

        /// <summary>
        /// Nodes is used to return the coordinates of all the nodes in the LAN pool.
        /// </summary>
        /// <returns>A query result containing coordinates of all the nodes in the LAN pool</returns>
        public Task<QueryResult<CoordinateEntry[]>> Nodes(CancellationToken ct = default)
        {
            return Nodes(QueryOptions.Default, ct);
        }

        /// <summary>
        /// Nodes is used to return the coordinates of all the nodes in the LAN pool.
        /// </summary>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A query result containing coordinates of all the nodes in the LAN pool</returns>
        public Task<QueryResult<CoordinateEntry[]>> Nodes(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<CoordinateEntry[]>(string.Format("/v1/coordinate/nodes"), q).Execute(ct);
        }

        /// <summary>
        /// Node returns the coordinates of a given node in the LAN pool.
        /// </summary>
        /// <param name="node">The node to query</param>"
        /// <param name="ct">The cancellation token</param>"
        /// <param name="q">Customized query options</param>"
        /// <returns>Node is used to return the coordinates of a given node in the LAN pool.</returns>
        public Task<QueryResult<CoordinateEntry[]>> Node(string node, QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<CoordinateEntry[]>(string.Format("/v1/coordinate/node/{0}", node), q).Execute(ct);
        }

        public Task<QueryResult<CoordinateEntry[]>> Node(string node, CancellationToken ct = default)
        {
            return Node(node, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Updates the LAN network coordinates for a node in a given datacenter.
        /// </summary>
        /// <param name="entry">The coordinate entry to update</param>
        /// <param name="q">Customized write options</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Update(CoordinateEntry entry, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Put("/v1/coordinate/update", entry, q).Execute(ct);
        }

        /// <summary>
        /// Updates the LAN network coordinates for a node in a given datacenter.
        /// </summary>
        /// <param name="entry">The coordinate entry to update</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Update(CoordinateEntry entry, CancellationToken ct = default)
        {
            return Update(entry, WriteOptions.Default, ct);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Coordinate> _coordinate;

        /// <summary>
        /// Session returns a handle to the session endpoints
        /// </summary>
        public ICoordinateEndpoint Coordinate
        {
            get { return _coordinate.Value; }
        }
    }
}
