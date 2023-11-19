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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul.Interfaces;
using Newtonsoft.Json;

namespace Consul
{
    public class ControlPlaneRequestLimit
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; } = "control-plane-request-limit";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int ReadRate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int WriteRate { get; set; }

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
        public int ReadRate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int WriteRate { get; set; }
    }

    public class ACLRateLimit
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int ReadRate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int WriteRate { get; set; }
    }

    public class CatalogRateLimit
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int ReadRate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int WriteRate { get; set; }
    }
    public class TerminalGatewayEntry
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

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CAFile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertFile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string KeyFile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SNI { get; set; }
    }

    public class LinkedService
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }
    }
    public class ServiceSplitterEntry
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
        public int Weight { get; set; }

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
    
    public class ServiceRouterEntry
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
        public int RequestTimeout { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int IdleTimeout { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int NumRetries { get; set; } = 1;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool RetryOnConnectFailure { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RetryOn { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> RetryOnStatusCodes { get; set; }

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
    public class ServiceResolverEntry
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
        public int ChoiceCount { get; set; } = 2;
    }

    public class RingHashConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MinimumRingSize { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MaximumRingSize { get; set; }
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
    public class ServiceIntentionsEntry
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
        public int Precedence { get; set; }

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
    public class SamenessGroupEntry
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
    public class ProxyDefaultEntry
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
        public int OutboundListenerPort { get; set; }

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
        public int LocalPathPort { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int ListenerPort { get; set; }

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

    public class ExportedServiceEntry
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

    public class MeshEntry
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

    public class TLSConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TLSMinVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TLSMaxVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> CipherSuites { get; set; }
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

    public class ServiceDefaultsEntry : IConfigurationPayload
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
        public int MaxInboundConnections { get; set; }

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
        public int RequestsPerSecond { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int RequestsMaxBurst { get; set; }

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
        public int RequestsPerSecond { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int RequestMaxBurst { get; set; }
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
        public int ConnectTimeoutMs { get; set; }

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
        public int ConnectTimeoutMs { get; set; }

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
        public int MaxConnections { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MaxPendingRequests { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MaxConcurrentRequests { get; set; }
    }

    public class PassiveHealthCheckConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Interval { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MaxFailures { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int EnforcingConsecutive5xx { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MaxEjectionPercent { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BaseEjectionTime { get; set; }
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
        public int Port { get; set; }
    }

    public interface IConfigurationPayload
    {
        string Name { get; set; }
        string Kind { get; set; }
    }

    public class Configuration : IConfigurationEndpoint
    {
        private readonly ConsulClient _client;

        internal Configuration(ConsulClient c)
        {
            _client = c;
        }
        public Task<WriteResult> ApplyConfig<TConfig>(TConfig cp, CancellationToken ct = default) where TConfig : IConfigurationPayload
        {
            return ApplyConfig(string.Empty, 0, cp, WriteOptions.Default, ct);
        }
        public Task<WriteResult> ApplyConfig<TConfig>(string dc, TConfig cp, CancellationToken ct = default) where TConfig : IConfigurationPayload
        {
            return ApplyConfig(dc, 0, cp, WriteOptions.Default, ct);
        }

        public Task<WriteResult> ApplyConfig<TConfig>(int cas, TConfig cp, CancellationToken ct = default) where TConfig : IConfigurationPayload
        {
            return ApplyConfig(string.Empty, cas, cp, WriteOptions.Default, ct);
        }
        public Task<WriteResult> ApplyConfig<TConfig>(string dc, int cas, TConfig cp, WriteOptions q, CancellationToken ct = default) where TConfig : IConfigurationPayload
        {
            var req = _client.Put("/v1/config", cp, q);
            if (!string.IsNullOrEmpty(dc))
            {
                req.Params["dc"] = dc;
            }
            if (cas > 0)
            {
                req.Params["cas"] = cas.ToString();
            }
            return req.Execute(ct);
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
