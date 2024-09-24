// -----------------------------------------------------------------------
//  <copyright file="Config.cs" company="G-Research Limited">
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
    /// <summary>
    /// The JWTProviderEntry configures Consul to use a JSON Web Token (JWT) and JSON Web Key Set (JWKS) in order to add JWT validation to proxies in the service mesh.
    /// </summary>
    public class JWTProviderEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "jwt-provider";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Issuer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JSONWebKeySetConfig> JSONWebKeySet { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Audiences { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ProviderLocation> Locations { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ForwardingConfig Forwarding { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ClockSkewSeconds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CacheConfig CacheConfig { get; set; }
    }

    public class JSONWebKeySetConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LocalJSONWebKeySet Local { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RemoteJSONWebKeySet Remote { get; set; }
    }

    public class LocalJSONWebKeySet
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JWKS { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Filename { get; set; }
    }

    public class RemoteJSONWebKeySet
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string URI { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RequestTimeoutMs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CacheDuration { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool FetchAsynchronously { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RetryPolicyConfig RetryPolicy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JWKSClusterConfig JWKSCluster { get; set; }
    }

    public class RetryPolicyConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string NumRetries { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RetryPolicyBackOffConfig RetryPolicyBackOff { get; set; }
    }

    public class RetryPolicyBackOffConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BaseInterval { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MaxInterval { get; set; }
    }

    public class JWKSClusterConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DiscoveryType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConnectTimeout { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TLSCertificatesConfig TLSCertificates { get; set; }
    }

    public class TLSCertificatesConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TrustedCAConfig TrustedCA { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CaCertificateProviderInstanceConfig CaCertificateProviderInstance { get; set; }
    }

    public class TrustedCAConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Filename { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EnvironmentVariable { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InlineString { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InlineBytes { get; set; }
    }

    public class CaCertificateProviderInstanceConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertificateName { get; set; }
    }

    public class ProviderLocation
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HeaderLocation Header { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public QueryParamLocation QueryParam { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CookieLocation Cookie { get; set; }
    }

    public class HeaderLocation
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ValuePrefix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Forward { get; set; }
    }

    public class QueryParamLocation
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public class CookieLocation
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public class ForwardingConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool PadForwardPayloadHeader { get; set; }
    }

    public class CacheConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Size { get; set; }
    }

    /// <summary>
    /// IngressGatewayEntry provides configuration for the Ingress Gateway Proxy
    /// </summary>
    public class IngressGatewayEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "ingress-gateway";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TLSConfig TLS { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GatewayDefaults Defaults { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<GatewayListener> Listeners { get; set; }
    }

    public class TLSConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Enabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TLSMinVersion { get; set; } = "TLSv1_2";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TLSMaxVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> CipherSuites { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SDSConfig SDS { get; set; }
    }

    public class SDSConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ClusterName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertResource { get; set; }
    }

    public class GatewayDefaults
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxConnections { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxPendingRequests { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxConcurrentRequests { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PassiveHealthCheckConfig PassiveHealthCheck { get; set; }
    }

    public class PassiveHealthCheckConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Interval { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxFailures { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? EnforcingConsecutive5xx { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxEjectionPercent { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BaseEjectionTime { get; set; }
    }

    public class GatewayListener
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Port { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Protocol { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ExternalService> Services { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TLSConfig TLS { get; set; }
    }

    public class ExternalService
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Hosts { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HeaderModification RequestHeaders { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HeaderModification ResponseHeaders { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TLSConfig TLS { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GatewayDefaults Defaults { get; set; }
    }

    public class HeaderModification
    {
        [JsonProperty("Add", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Add { get; set; }

        [JsonProperty("Set", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Set { get; set; }

        [JsonProperty("Remove", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Remove { get; set; }
    }

    /// <summary>
    /// The InlineCertificateEntry configures the gateway inline certificate configuration entry
    /// </summary>
    public class InlineCertificateEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "inline-certificate";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Certificate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PrivateKey { get; set; }
    }
    /// <summary>
    /// The TcpRouteEntry configures TCP route resources.
    /// </summary>
    public class TcpRouteEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "tcp-route";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TcpRouteService> Services { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiGatewayReference> Parents { get; set; }
    }

    public class TcpRouteService
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }
    }



    /// <summary>
    /// The HttpRouteEntry configures HTTP route resources.
    /// </summary>
    public class HttpRouteEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "http-route";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Hostnames { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiGatewayReference> Parents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpRouteRule> Rules { get; set; }
    }

    public class ApiGatewayReference
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SectionName { get; set; }
    }

    public class HttpRouteRule
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpRouteFilter> Filters { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpRouteMatch> Matches { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpRouteService> Services { get; set; }
    }

    public class HttpRouteFilter
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpHeaderOperation> Headers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpURLRewriteOperation> URLRewrite { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JWTSettings JWT { get; set; }
    }

    public class HttpRouteMatch
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpHeaderMatch> Headers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpPathMatch> Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpQueryMatch> Query { get; set; }
    }

    public class JWTSettings
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JWTProvider> Providers { get; set; }
    }

    public class JWTProvider
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public VerifyClaims VerifyClaims { get; set; }
    }

    public class VerifyClaims
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }

    public class HttpRouteService
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Weight { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpRouteFilter> Filters { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HttpRouteFilter> ResponseFilters { get; set; }
    }

    public class HttpHeaderOperation
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HeaderKeyValuePair> Add { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Remove { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HeaderKeyValuePair> Set { get; set; }
    }

    public class HttpURLRewriteOperation
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }
    }

    public class HeaderKeyValuePair
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }

    public class HttpHeaderMatch
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Match { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }

    public class HttpPathMatch
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Match { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }

    public class HttpQueryMatch
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Match { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }

    /// <summary>
    ///  Configures the API gateway configuration entry that you can deploy to networks in virtual machine (VM) environments.
    /// </summary>
    public class ApiGatewayEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "api-gateway";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiGatewayListener> Listeners { get; set; }
    }

    public class ApiGatewayListener
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Port { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Protocol { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ApiGatewayTLS TLS { get; set; }

        [JsonProperty("default", NullValueHandling = NullValueHandling.Ignore)]
        public ApiGatewayOverrideSettings Default { get; set; }

        [JsonProperty("override", NullValueHandling = NullValueHandling.Ignore)]
        public ApiGatewayOverrideSettings Override { get; set; }
    }

    public class ApiGatewayTLS
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MaxVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MinVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> CipherSuites { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiGatewayCertificate> Certificates { get; set; }
    }

    public class ApiGatewayCertificate
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "inline-certificate";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }
    }

    public class ApiGatewayOverrideSettings
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ApiGatewayJWTSettings JWT { get; set; }
    }

    public class ApiGatewayJWTSettings
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiGatewayJWTProvider> Providers { get; set; }
    }

    public class ApiGatewayJWTProvider
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ApiGatewayVerifyClaims VerifyClaims { get; set; }
    }

    public class ApiGatewayVerifyClaims
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }

    /// <summary>
    /// Configuration options for the control-plane-request-limit configuration entry
    /// </summary>
    public class ControlPlaneRequestLimitEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "control-plane-request-limit";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ReadRate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? WriteRate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public KVRateLimit KV { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ACLRateLimit ACL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CatalogRateLimit Catalog { get; set; }
    }

    public class KVRateLimit
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ReadRate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? WriteRate { get; set; }
    }

    public class ACLRateLimit
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ReadRate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? WriteRate { get; set; }
    }

    public class CatalogRateLimit
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ReadRate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? WriteRate { get; set; }
    }

    /// <summary>
    /// Configures terminating gateways to proxy traffic from services in the Consul service mesh to services registered with Consul that do not have a service mesh sidecar proxy
    /// </summary>
    public class TerminatingGatewayEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "terminating-gateway";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<LinkedService> Services { get; set; }
    }

    public class LinkedService
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CAFile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertFile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string KeyFile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SNI { get; set; }
    }

    /// <summary>
    /// Configures and apply service splitters to redirect a percentage of incoming traffic requests for a service to one or more specific service instances.
    /// </summary>
    public class ServiceSplitterEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "service-splitter";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SplitConfig> Splits { get; set; }
    }

    public class SplitConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Weight { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Service { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceSubset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Dictionary<string, string>> RequestHeaders { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Dictionary<string, string>> ResponseHeaders { get; set; }
    }


    /// <summary>
    /// Service routers use L7 network information to redirect a traffic request for a service to one or more specific service instances.
    /// </summary>
    public class ServiceRouterEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "service-router";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; } = "Enterprise";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; } = "Enterprise";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Routes> Routes { get; set; }
    }

    public class Routes
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public MapConfig Match { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HTTPConfig HTTP { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Destination { get; set; }
    }

    public class MapConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PathExact { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PathPrefix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PathRegex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Methods { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<HeaderConfig> Header { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<QueryParamConfig> QueryParam { get; set; }
    }

    public class HTTPConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PrefixRewrite { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? RequestTimeout { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? IdleTimeout { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? NumRetries { get; set; } = 1;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool RetryOnConnectFailure { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RetryOn { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int?> RetryOnStatusCodes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> RequestHeaders { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> ResponseHeaders { get; set; }
    }

    public class HeaderConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Present { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Exact { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Prefix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Suffix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Regex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Invert { get; set; }
    }

    public class QueryParamConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Present { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Exact { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Regex { get; set; }
    }

    /// <summary>
    /// Configures and apply service resolvers to create named subsets of service instances and define their behavior when satisfying upstream requests.
    /// </summary>
    public class ServiceResolverEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "service-resolver";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConnectTimeout { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RequestTimeout { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Subsets { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Filter { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool OnlyPassing { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultSubset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Redirect { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LoadBalancerConfig LoadBalancer { get; set; }
    }

    public class LoadBalancerConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Policy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LeastRequestConfig LeastRequestConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RingHashConfig RingHashConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> HashPolicies { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CookieConfig CookieConfig { get; set; }
    }

    public class LeastRequestConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ChoiceCount { get; set; } = 2;
    }

    public class RingHashConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MinimumRingSize { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaximumRingSize { get; set; }
    }

    public class CookieConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Session { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TTL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool SourceIP { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Terminal { get; set; }
    }

    /// <summary>
    /// Configures control access between services in the service mesh.
    /// </summary>
    public class ServiceIntentionsEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "service-intentions";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> JWT { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Provider> Providers { get; set; }
    }

    public class Provider
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<VerifyClaim> VerifyClaims { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Sources { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Peer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SamenessGroup { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Permissions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> HTTP { get; set; }
    }

    public class VerifyClaim
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }

    public class Header
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Present { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Exact { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Prefix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Suffix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Regex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Invert { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Precedence { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LegacyID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> LegacyMeta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LegacyCreateTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LegacyUpdateTime { get; set; }
    }

    /// <summary>
    /// Sameness groups associate identical admin partitions to facilitate traffic between identical services.
    /// </summary>
    public class SamenessGroupEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "sameness-group";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DefaultForFailover { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool IncludeLocal { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SamenessGroupMember> Members { get; set; }
    }

    public class SamenessGroupMember
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Peer { get; set; }
    }

    /// <summary>
    /// Proxy defaults configuration entries set global passthrough Envoy settings for proxies in the service mesh, including sidecars and gateways.
    /// </summary>
    public class ProxyDefaultEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "proxy-defaults";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; } = "default";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Config { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<EnvoyExtension> EnvoyExtensions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TransparentProxyConfig TransparentProxy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MutualTLSMode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public MeshGatewayConfig MeshGateway { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExposeConfig Expose { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AccessLogsConfig AccessLogs { get; set; }
    }

    public class EnvoyExtension
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Required { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Arguments { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConsulVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EnvoyVersion { get; set; }
    }

    public class TransparentProxyConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? OutboundListenerPort { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DialedDirectly { get; set; }
    }

    public class MeshGatewayConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }
    }

    public class ExposeConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Checks { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<PathConfig> Paths { get; set; }
    }

    public class PathConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? LocalPathPort { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ListenerPort { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Protocol { get; set; }
    }

    public class AccessLogsConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Enabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DisableListenerLogs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JSONFormat { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TextFormat { get; set; }
    }

    /// <summary>
    /// The Exported Service configuration entry enables Consul to export service instances to other clusters from a single file and connect services across clusters.
    /// </summary>
    public class ExportedServiceEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "exported-services";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ServiceDefinition> Services { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }
    }

    public class ServiceDefinition
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ConsumerDefinition> Consumers { get; set; }
    }

    public class ConsumerDefinition
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Peer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SamenessGroup { get; set; }
    }

    /// <summary>
    /// The mesh configuration entry allows you to define a global default configuration that applies to all service mesh proxies.
    /// </summary>
    public class MeshEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "mesh";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TransparentProxyConfig TransparentProxy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool MeshDestinationsOnly { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool AllowEnablingPermissiveMutualTLS { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TLSConfig TLS { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TLSDirectionConfig Incoming { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TLSDirectionConfig Outgoing { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HTTPConfig HTTP { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool SanitizeXForwardedClientCert { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PeeringMeshConfig Peering { get; set; }
    }

    public class TLSDirectionConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TLSMinVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TLSMaxVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> CipherSuites { get; set; }
    }


    public class PeeringMeshConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool PeerThroughMeshGateways { get; set; }
    }

    /// <summary>
    /// The service defaults configuration entry contains common configuration settings for service mesh services, such as upstreams and gateways.
    /// </summary>
    public class ServiceDefaultsEntry : IConfigurationEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "service-defaults";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partition { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Protocol { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BalanceInboundConnections { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RateLimitsConfig RateLimits { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public UpstreamConfig UpstreamConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DefaultsConfig Defaults { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TransparentProxyConfig TransparentProxy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MutualTLSMode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<EnvoyExtensionConfig> EnvoyExtensions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DestinationConfig Destination { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxInboundConnections { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public MeshGatewayConfig MeshGateway { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ExternalSNI { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExposeConfig Expose { get; set; }
    }

    public class RateLimitsConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public InstanceLevelConfig InstanceLevel { get; set; }
    }

    public class InstanceLevelConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? RequestsPerSecond { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? RequestsMaxBurst { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<RouteConfig> Routes { get; set; }
    }

    public class RouteConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PathExact { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PathPrefix { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PathRegex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? RequestsPerSecond { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? RequestMaxBurst { get; set; }
    }

    public class UpstreamConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<OverrideConfig> Overrides { get; set; }
    }

    public class DefaultsConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Protocol { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ConnectTimeoutMs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public MeshGatewayConfig MeshGateway { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BalanceOutboundConnections { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LimitsConfig Limits { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PassiveHealthCheckConfig PassiveHealthCheck { get; set; }
    }

    public class OverrideConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Peer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Protocol { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ConnectTimeoutMs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public MeshGatewayConfig MeshGateway { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BalanceOutboundConnections { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LimitsConfig Limits { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PassiveHealthCheckConfig PassiveHealthCheck { get; set; }
    }

    public class LimitsConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxConnections { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxPendingRequests { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxConcurrentRequests { get; set; }
    }

    public class EnvoyExtensionConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Required { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Arguments { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConsulVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EnvoyVersion { get; set; }
    }

    public class DestinationConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Addresses { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Port { get; set; }
    }

    public interface IConfigurationEntry
    {
        string Kind { get; set; }
    }

    public class Configuration : IConfigurationEndpoint
    {
        private readonly ConsulClient _client;

        internal Configuration(ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        ///  This creates or updates the given config entry.
        /// </summary>
        /// <param name="q">Write Options</param>
        /// <param name="configurationEntry">The configuration entry</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> ApplyConfig<TConfig>(WriteOptions q, TConfig configurationEntry, CancellationToken ct = default) where TConfig : IConfigurationEntry
        {
            var req = _client.Put("/v1/config", configurationEntry, q);
            return req.Execute(ct);
        }

        /// <summary>
        ///  This creates or updates the given config entry.
        /// </summary>
        /// <param name="configurationEntry">The configuration entry</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> ApplyConfig<TConfig>(TConfig configurationEntry, CancellationToken ct = default) where TConfig : IConfigurationEntry
        {
            return ApplyConfig<TConfig>(WriteOptions.Default, configurationEntry, ct);
        }

        /// <summary>
        /// This Retrieves the given config entry.
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="kind">The kind of config entry</param>
        /// <param name="name">The name of config entry</param>
        /// <param name="q">Query Options</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A config entry</returns>
        public Task<QueryResult<TConfig>> GetConfig<TConfig>(string kind, string name, QueryOptions q, CancellationToken ct = default) where TConfig : IConfigurationEntry
        {
            var req = _client.Get<TConfig>($"/v1/config/{kind}/{name}", q);
            return req.Execute(ct);
        }

        /// <summary>
        /// This Retrieves the given config entry.
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="kind">The kind of config entry</param>
        /// <param name="name">The name of config entry</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A config entry</returns>
        public Task<QueryResult<TConfig>> GetConfig<TConfig>(string kind, string name, CancellationToken ct = default) where TConfig : IConfigurationEntry
        {
            return GetConfig<TConfig>(kind, name, QueryOptions.Default, ct);
        }

        /// <summary>
        /// This Retrieves the list of config for an entry kind.
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="kind">The kind of config entry</param>
        /// <param name="q">Query Options</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A config entry</returns>
        public Task<QueryResult<List<TConfig>>> ListConfig<TConfig>(string kind, QueryOptions q, CancellationToken ct = default) where TConfig : IConfigurationEntry
        {
            var req = _client.Get<List<TConfig>>($"/v1/config/{kind}", q);
            return req.Execute(ct);
        }
        /// <summary>
        /// This Retrieves the list of config for an entry kind.
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="kind">The kind of config entry</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A list of config entries</returns>
        public Task<QueryResult<List<TConfig>>> ListConfig<TConfig>(string kind, CancellationToken ct = default) where TConfig : IConfigurationEntry
        {
            return ListConfig<TConfig>(kind, QueryOptions.Default, ct);
        }

        /// <summary>
        /// This Deletes the given config entry.
        /// </summary>
        /// <param name="kind">The kind of config entry</param>
        /// <param name="name">The name of config entry</param>
        /// <param name="q">Write Options</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A Write Result</returns>
        public Task<WriteResult> DeleteConfig(string kind, string name, WriteOptions q, CancellationToken ct = default)
        {
            var req = _client.Delete($"/v1/config/{kind}/{name}", q);
            return req.Execute(ct);
        }

        /// <summary>
        /// This Deletes the given config entry.
        /// </summary>
        /// <param name="kind">The kind of config entry</param>
        /// <param name="name">The name of config entry</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A config entry</returns>
        public Task<WriteResult> DeleteConfig(string kind, string name, CancellationToken ct = default)
        {
            return DeleteConfig(kind, name, WriteOptions.Default, ct);
        }
    }
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Configuration> _configuration;

        /// <summary>
        /// ConsulClient returns a handle to the catalog endpoints
        /// </summary>
        public IConfigurationEndpoint Configuration => _configuration.Value;
    }
}
