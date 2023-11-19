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

    public class ServiceDefaultsEntry : IConfigurationPayload
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

    public class TransparentProxyConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int OutboundListenerPort { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DialedDirectly { get; set; }
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

    public class MeshGatewayConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }
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
        public Task<WriteResult> ApplyConfig<TConfig>(TConfig cp, CancellationToken ct = default) where TConfig: IConfigurationPayload
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
