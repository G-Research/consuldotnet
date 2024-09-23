// -----------------------------------------------------------------------
//  <copyright file="Catalog.cs" company="PlayFab Inc">
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
using Consul.Filtering;
using Newtonsoft.Json;

namespace Consul
{
    public class Node
    {
        // Cannot be "Node" as in the Go API because in C#, properties cannot
        // have the same name as their enclosing class.
        [JsonProperty(PropertyName = "Node")]
        public string Name { get; set; }
        public string Address { get; set; }
        public string Datacenter { get; set; }
        public Dictionary<string, string> TaggedAddresses { get; set; }
    }

    public class ServiceTaggedAddress
    {
        public string Address { get; set; }

        public int Port { get; set; }
    }

    public class CatalogService
    {
        public string Node { get; set; }
        public string Address { get; set; }
        public string ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAddress { get; set; }
        public string[] ServiceTags { get; set; }
        public int ServicePort { get; set; }
        public Dictionary<string, ServiceTaggedAddress> ServiceTaggedAddresses { get; set; }
        public bool ServiceEnableTagOverride { get; set; }
        public IDictionary<string, string> ServiceMeta { get; set; }
    }

    public class CatalogNode
    {
        public Node Node { get; set; }
        public Dictionary<string, AgentService> Services { get; set; }

        public CatalogNode()
        {
            Services = new Dictionary<string, AgentService>();
        }
    }

    public class CatalogRegistration
    {
        public string Node { get; set; }
        public string Address { get; set; }
        public string Datacenter { get; set; }
        public AgentService Service { get; set; }
        public AgentCheck Check { get; set; }
    }

    public class CatalogDeregistration
    {
        public string Node { get; set; }
        public string Address { get; set; }
        public string Datacenter { get; set; }
        public string ServiceID { get; set; }
        public string CheckID { get; set; }
    }

    public class NodeService
    {
        public NodeInfo Node { get; set; }
        public List<ServiceInfo> Services { get; set; }
    }

    public class NodeInfo
    {
        public string ID { get; set; }
        public string Node { get; set; }
        public string Address { get; set; }
        public string Datacenter { get; set; }
        public Dictionary<string, string> TaggedAddresses { get; set; }
        public Dictionary<string, string> Meta { get; set; }
    }

    public class ServiceInfo
    {
        public string ID { get; set; }
        public string Service { get; set; }
        public string[] Tags { get; set; }
        public Dictionary<string, string> Meta { get; set; }
        public int Port { get; set; }
        public string Namespace { get; set; }
        public Dictionary<string, ServiceAddress> TaggedAddresses { get; set; }
    }

    public class ServiceAddress
    {
        public string Address { get; set; }
        public int Port { get; set; }
    }

    public class CompoundServiceName
    {
        public string Namespace { get; set; }
        public string Partition { get; set; }
        public string Name { get; set; }
    }

    public class GatewayService
    {
        public CompoundServiceName Gateway { get; set; }
        public CompoundServiceName Service { get; set; }
        public ServiceKind GatewayKind { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
        public List<string> Hosts { get; set; }
        public string CAFile { get; set; }
        public string CertFile { get; set; }
        public string KeyFile { get; set; }
        public string SNI { get; set; }
        public bool FromWildcard { get; set; }
    }

    /// <summary>
    /// Catalog can be used to query the Catalog endpoints
    /// </summary>
    public class Catalog : ICatalogEndpoint
    {
        private readonly ConsulClient _client;

        internal Catalog(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        /// Register a new catalog item
        /// </summary>
        /// <param name="reg">A catalog registration</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Register(CatalogRegistration reg, CancellationToken ct = default)
        {
            return Register(reg, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Register a new catalog item
        /// </summary>
        /// <param name="reg">A catalog registration</param>
        /// <param name="q">Customized write options</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Register(CatalogRegistration reg, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Put("/v1/catalog/register", reg, q).Execute(ct);
        }

        /// <summary>
        /// Deregister an existing catalog item
        /// </summary>
        /// <param name="reg">A catalog deregistration</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Deregister(CatalogDeregistration reg, CancellationToken ct = default)
        {
            return Deregister(reg, WriteOptions.Default, ct);
        }

        /// <summary>
        /// Deregister an existing catalog item
        /// </summary>
        /// <param name="reg">A catalog deregistration</param>
        /// <param name="q">Customized write options</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Deregister(CatalogDeregistration reg, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Put("/v1/catalog/deregister", reg, q).Execute(ct);
        }

        /// <summary>
        /// Datacenters is used to query for all the known datacenters
        /// </summary>
        /// <returns>A list of datacenter names</returns>
        public Task<QueryResult<string[]>> Datacenters(CancellationToken ct = default)
        {
            return _client.Get<string[]>("/v1/catalog/datacenters").Execute(ct);
        }

        /// <summary>
        /// Datacenters is used to query for all the known datacenters with custom query options
        /// </summary>
        /// <returns>A list of datacenter names</returns>
        public Task<QueryResult<string[]>> Datacenters(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<string[]>("/v1/catalog/datacenters", q).Execute(ct);
        }

        /// <summary>
        /// Nodes is used to query all the known nodes
        /// </summary>
        /// <returns>A list of all nodes</returns>
        public Task<QueryResult<Node[]>> Nodes(CancellationToken ct = default)
        {
            return Nodes(QueryOptions.Default, ct);
        }

        /// <summary>
        /// Nodes is used to query all the known nodes
        /// </summary>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A list of all nodes</returns>
        public Task<QueryResult<Node[]>> Nodes(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<Node[]>("/v1/catalog/nodes", q).Execute(ct);
        }

        /// <summary>
        /// Services is used to query for all known services
        /// </summary>
        /// <returns>A list of all services</returns>
        public Task<QueryResult<Dictionary<string, string[]>>> Services(CancellationToken ct = default)
        {
            return Services(QueryOptions.Default, ct);
        }

        /// <summary>
        /// Services is used to query for all known services
        /// </summary>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A list of all services</returns>
        public Task<QueryResult<Dictionary<string, string[]>>> Services(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<Dictionary<string, string[]>>("/v1/catalog/services", q).Execute(ct);
        }

        /// <summary>
        /// Service is used to query catalog entries for a given service
        /// </summary>
        /// <param name="service">The service ID</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A list of service instances</returns>
        public Task<QueryResult<CatalogService[]>> Service(string service, CancellationToken ct = default)
        {
            return Service(service, string.Empty, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Service is used to query catalog entries for a given service
        /// </summary>
        /// <param name="service">The service ID</param>
        /// <param name="tag">A tag to filter on</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A list of service instances</returns>
        public Task<QueryResult<CatalogService[]>> Service(string service, string tag, CancellationToken ct = default)
        {
            return Service(service, tag, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Service is used to query catalog entries for a given service
        /// </summary>
        /// <param name="service">The service ID</param>
        /// <param name="tag">A tag to filter on</param>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A list of service instances</returns>
        public Task<QueryResult<CatalogService[]>> Service(string service, string tag, QueryOptions q, CancellationToken ct)
        {
            var req = _client.Get<CatalogService[]>(string.Format("/v1/catalog/service/{0}", service), q);
            if (!string.IsNullOrEmpty(tag))
            {
                req.Params["tag"] = tag;
            }
            return req.Execute(ct);
        }

        /// <summary>
        /// Returns the nodes providing a mesh-capable service in a given datacenter. 
        /// </summary>
        /// <param name="service">The service ID</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A list of service instances</returns>
        public Task<QueryResult<CatalogService[]>> NodesForMeshCapableService(string service, CancellationToken ct = default)
        {
            return NodesForMeshCapableService(service, QueryOptions.Default, null, ct);
        }

        /// <summary>
        /// Returns the nodes providing a mesh-capable service in a given datacenter.
        /// </summary>
        /// <param name="service">The service ID</param>
        /// <param name="filter">Specifies the expression used to filter the queries results prior to returning the data</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A list of service instances</returns>
        public Task<QueryResult<CatalogService[]>> NodesForMeshCapableService(string service, Filter filter, CancellationToken ct = default)
        {
            return NodesForMeshCapableService(service, QueryOptions.Default, filter, ct);
        }

        /// <summary>
        /// Returns the nodes providing a mesh-capable service in a given datacenter.
        /// </summary>
        /// <param name="service">The service ID</param>
        /// <param name="q">Customized Query options</param>
        /// <param name="filter">Specifies the expression used to filter the queries results prior to returning the data</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A list of service instances</returns>
        public Task<QueryResult<CatalogService[]>> NodesForMeshCapableService(string service, QueryOptions q, Filter filter, CancellationToken ct = default)
        {
            return _client.Get<CatalogService[]>(string.Format("/v1/catalog/connect/{0}", service), q, filter).Execute(ct);
        }

        /// <summary>
        /// Node is used to query for service information about a single node
        /// </summary>
        /// <param name="node">The node name</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The node information including a list of services</returns>
        public Task<QueryResult<CatalogNode>> Node(string node, CancellationToken ct = default)
        {
            return Node(node, QueryOptions.Default, ct);
        }

        /// <summary>
        /// Node is used to query for service information about a single node
        /// </summary>
        /// <param name="node">The node name</param>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>The node information including a list of services</returns>
        public Task<QueryResult<CatalogNode>> Node(string node, QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<CatalogNode>(string.Format("/v1/catalog/node/{0}", node), q).Execute(ct);
        }

        /// <summary>
        /// ServicesForNode is used to query for the services provided by a node
        /// </summary>
        /// <param name="node">Node Name</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Node Services</returns>
        public Task<QueryResult<NodeService>> ServicesForNode(string node, CancellationToken ct = default)
        {
            return ServicesForNode(node, QueryOptions.Default, ct);
        }

        /// <summary>
        /// ServicesForNode is used to query for the services provided by a node
        /// </summary>
        /// <param name="node">Node Name</param>
        /// <param name="q">Query Parameters</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Node Services</returns>
        public Task<QueryResult<NodeService>> ServicesForNode(string node, QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<NodeService>(string.Format("/v1/catalog/node-services/{0}", node), q).Execute(ct);
        }


        /// <summary>
        /// GatewayServices is used to query for the services associated with an ingress gateway or terminating gateway
        /// </summary>
        /// <param name="gateway">Gateway name</param>
        /// <param name="q">Query Parameters</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Gateway services</returns>
        public Task<QueryResult<GatewayService[]>> GatewayService(string gateway, QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<GatewayService[]>($"/v1/catalog/gateway-services/{gateway}", q).Execute(ct);
        }

        /// <summary>
        /// GatewayServices is used to query for the services associated with an ingress gateway or terminating gateway
        /// </summary>
        /// <param name="gateway">Gateway name</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Gateway services</returns>
        public Task<QueryResult<GatewayService[]>> GatewayService(string gateway, CancellationToken ct = default)
        {
            return GatewayService(gateway, QueryOptions.Default, ct);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Catalog> _catalog;

        /// <summary>
        /// Catalog returns a handle to the catalog endpoints
        /// </summary>
        public ICatalogEndpoint Catalog => _catalog.Value;
    }
}
