// -----------------------------------------------------------------------
//  <copyright file="DiscoveryChainTest.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class DiscoveryChainTest : BaseFixture
    {
        public const string TestClusterID = "11111111-2222-3333-4444-555555555555";

        [Fact]
        public async Task DiscoveryChain_Get()
        {
            var check = new CompiledDiscoveryChain
            {
                ServiceName = "web",
                Namespace = "default",
                Datacenter = "dc1",
                Protocol = "tcp",
                StartNode = "resolver:web.default.dc1",
                Nodes = new Dictionary<string, DiscoveryGraphNode>()
                {
                    { "resolver:web.default.dc1",
                        new DiscoveryGraphNode {
                            Type = DiscoveryChain.DiscoveryGraphNodeTypeResolver,
                            Name = "web.default.dc1",
                            Resolver = new DiscoveryResolver
                            {
                                Default = true,
                                ConnectTimeout = new TimeSpan(0, 0, 5),
                                Target = "web.default.dc1"
                            }
                        }
                    }
                },
                Targets = new Dictionary<string, DiscoveryTarget>()
                {
                    { "web.default.dc1",
                        new DiscoveryTarget
                        {
                            ID = "web.default.dc1",
                            Service = "web",
                            Namespace = "default",
                            Datacenter = "dc1",
                            SNI = "web.default.dc1.internal." + TestClusterID + ".consul",
                            Name = "web.default.dc1.internal." + TestClusterID + ".consul"
                        }
                    }
                }
            };

            var response = await _client.DiscoveryChain.Get("web");
            Assert.NotNull(response.Response);
            var chain = response.Response.Chain;
            Assert.Equal(check.ServiceName, chain.ServiceName);
            Assert.Equal(check.Namespace, chain.Namespace);
            Assert.Equal(check.Datacenter, chain.Datacenter);
            Assert.Equal(check.Protocol, chain.Protocol);
            Assert.Equal(check.StartNode, chain.StartNode);
            var nodeCheck = check.Nodes["resolver:web.default.dc1"];
            var nodeChain = chain.Nodes["resolver:web.default.dc1"];
            Assert.Equal(nodeCheck.Type, nodeChain.Type);
            Assert.Equal(nodeCheck.Name, nodeChain.Name);
            Assert.Equal(nodeCheck.Resolver.Default, nodeChain.Resolver.Default);
            Assert.Equal(nodeCheck.Resolver.Target, nodeChain.Resolver.Target);
            Assert.Equal(nodeCheck.Resolver.ConnectTimeout.ToString(), nodeChain.Resolver.ConnectTimeout.ToString());
            var targetCheck = check.Targets["web.default.dc1"];
            var targetChain = chain.Targets["web.default.dc1"];
            Assert.Equal(targetCheck.ID, targetChain.ID);
            Assert.Equal(targetCheck.Service, targetChain.Service);
            Assert.Equal(targetCheck.Namespace, targetChain.Namespace);
            Assert.Equal(targetCheck.Datacenter, targetChain.Datacenter);
            Assert.Equal(targetCheck.SNI, targetChain.SNI);
            Assert.Equal(targetCheck.Name, targetChain.Name);

            check = new CompiledDiscoveryChain
            {
                ServiceName = "web",
                Namespace = "default",
                Datacenter = "dc2",
                Protocol = "tcp",
                StartNode = "resolver:web.default.dc2",
                Nodes = new Dictionary<string, DiscoveryGraphNode>()
                {
                    { "resolver:web.default.dc2",
                        new DiscoveryGraphNode {
                            Type = DiscoveryChain.DiscoveryGraphNodeTypeResolver,
                            Name = "web.default.dc2",
                            Resolver = new DiscoveryResolver
                            {
                                Default = true,
                                ConnectTimeout = new TimeSpan(0, 0, 5),
                                Target = "web.default.dc2"
                            }
                        }
                    }
                },
                Targets = new Dictionary<string, DiscoveryTarget>()
                {
                    { "web.default.dc2",
                        new DiscoveryTarget
                        {
                            ID = "web.default.dc2",
                            Service = "web",
                            Namespace = "default",
                            Datacenter = "dc2",
                            SNI = "web.default.dc2.internal." + TestClusterID + ".consul",
                            Name = "web.default.dc2.internal." + TestClusterID + ".consul"
                        }
                    }
                }
            };
            var options = new DiscoveryChainOptions { EvaluateInDatacenter = "dc2" };
            var responsePost = await _client.DiscoveryChain.Get("web", options);
            Assert.NotNull(response.Response);

            chain = responsePost.Response.Chain;
            Assert.Equal(check.ServiceName, chain.ServiceName);
            Assert.Equal(check.Namespace, chain.Namespace);
            Assert.Equal(check.Datacenter, chain.Datacenter);
            Assert.Equal(check.Protocol, chain.Protocol);
            Assert.Equal(check.StartNode, chain.StartNode);
            nodeCheck = check.Nodes["resolver:web.default.dc2"];
            nodeChain = chain.Nodes["resolver:web.default.dc2"];
            Assert.Equal(nodeCheck.Type, nodeChain.Type);
            Assert.Equal(nodeCheck.Name, nodeChain.Name);
            Assert.Equal(nodeCheck.Resolver.Default, nodeChain.Resolver.Default);
            Assert.Equal(nodeCheck.Resolver.Target, nodeChain.Resolver.Target);
            Assert.Equal(nodeCheck.Resolver.ConnectTimeout.ToString(), nodeChain.Resolver.ConnectTimeout.ToString());
            targetCheck = check.Targets["web.default.dc2"];
            targetChain = chain.Targets["web.default.dc2"];
            Assert.Equal(targetCheck.ID, targetChain.ID);
            Assert.Equal(targetCheck.Service, targetChain.Service);
            Assert.Equal(targetCheck.Namespace, targetChain.Namespace);
            Assert.Equal(targetCheck.Datacenter, targetChain.Datacenter);
            Assert.Equal(targetCheck.SNI, targetChain.SNI);
            Assert.Equal(targetCheck.Name, targetChain.Name);

            var config = new ServiceResolverEntry
            {
                Name = "web",
                ConnectTimeout = new TimeSpan(0, 0, 33)
            };
            await _client.Configuration.ApplyConfig(config);

            check = new CompiledDiscoveryChain
            {
                ServiceName = "web",
                Namespace = "default",
                Datacenter = "dc1",
                Protocol = "tcp",
                StartNode = "resolver:web.default.dc1",
                Nodes = new Dictionary<string, DiscoveryGraphNode>()
                {
                    { "resolver:web.default.dc1",
                        new DiscoveryGraphNode {
                            Type = DiscoveryChain.DiscoveryGraphNodeTypeResolver,
                            Name = "web.default.dc1",
                            Resolver = new DiscoveryResolver
                            {
                                Default = false,
                                ConnectTimeout = new TimeSpan(0, 0, 33),
                                Target = "web.default.dc1"
                            }
                        }
                    }
                },
                Targets = new Dictionary<string, DiscoveryTarget>()
                {
                    { "web.default.dc1",
                        new DiscoveryTarget
                        {
                            ID = "web.default.dc1",
                            Service = "web",
                            Namespace = "default",
                            Datacenter = "dc1",
                            SNI = "web.default.dc1.internal." + TestClusterID + ".consul",
                            Name = "web.default.dc1.internal." + TestClusterID + ".consul"
                        }
                    }
                }
            };
            response = await _client.DiscoveryChain.Get("web");
            Assert.NotNull(response.Response);

            chain = response.Response.Chain;
            Assert.Equal(check.ServiceName, chain.ServiceName);
            Assert.Equal(check.Namespace, chain.Namespace);
            Assert.Equal(check.Datacenter, chain.Datacenter);
            Assert.Equal(check.Protocol, chain.Protocol);
            Assert.Equal(check.StartNode, chain.StartNode);
            nodeCheck = check.Nodes["resolver:web.default.dc1"];
            nodeChain = chain.Nodes["resolver:web.default.dc1"];
            Assert.Equal(nodeCheck.Type, nodeChain.Type);
            Assert.Equal(nodeCheck.Name, nodeChain.Name);
            Assert.Equal(nodeCheck.Resolver.Default, nodeChain.Resolver.Default);
            Assert.Equal(nodeCheck.Resolver.Target, nodeChain.Resolver.Target);
            Assert.Equal(nodeCheck.Resolver.ConnectTimeout.ToString(), nodeChain.Resolver.ConnectTimeout.ToString());
            targetCheck = check.Targets["web.default.dc1"];
            targetChain = chain.Targets["web.default.dc1"];
            Assert.Equal(targetCheck.ID, targetChain.ID);
            Assert.Equal(targetCheck.Service, targetChain.Service);
            Assert.Equal(targetCheck.Namespace, targetChain.Namespace);
            Assert.Equal(targetCheck.Datacenter, targetChain.Datacenter);
            Assert.Equal(targetCheck.SNI, targetChain.SNI);
            Assert.Equal(targetCheck.Name, targetChain.Name);

            check = new CompiledDiscoveryChain
            {
                ServiceName = "web",
                Namespace = "default",
                Datacenter = "dc2",
                Protocol = "grpc",
                CustomizationHash = "98809527",
                StartNode = "resolver:web.default.dc2",
                Nodes = new Dictionary<string, DiscoveryGraphNode>()
                {
                    { "resolver:web.default.dc2",
                        new DiscoveryGraphNode {
                            Type = DiscoveryChain.DiscoveryGraphNodeTypeResolver,
                            Name = "web.default.dc2",
                            Resolver = new DiscoveryResolver
                            {
                                Default = false,
                                ConnectTimeout = new TimeSpan(0, 0, 22),
                                Target = "web.default.dc2"
                            }
                        }
                    }
                },
                Targets = new Dictionary<string, DiscoveryTarget>()
                {
                    { "web.default.dc2",
                        new DiscoveryTarget
                        {
                            ID = "web.default.dc2",
                            Service = "web",
                            Namespace = "default",
                            Datacenter = "dc2",
                            MeshGateway = new MeshGatewayConfig { Mode = "local" },
                            SNI = "web.default.dc2.internal." + TestClusterID + ".consul",
                            Name = "web.default.dc2.internal." + TestClusterID + ".consul"
                        }
                    }
                }
            };
            options = new DiscoveryChainOptions
            {
                EvaluateInDatacenter = "dc2",
                OverrideMeshGateway = new MeshGatewayConfig { Mode = "local" },
                OverrideProtocol = "grpc",
                OverrideConnectTimeout = new TimeSpan(0, 0, 22)
            };
            responsePost = await _client.DiscoveryChain.Get("web", options);
            Assert.NotNull(response.Response);

            chain = responsePost.Response.Chain;
            Assert.Equal(check.ServiceName, chain.ServiceName);
            Assert.Equal(check.Namespace, chain.Namespace);
            Assert.Equal(check.Datacenter, chain.Datacenter);
            Assert.Equal(check.Protocol, chain.Protocol);
            Assert.Equal(check.StartNode, chain.StartNode);
            nodeCheck = check.Nodes["resolver:web.default.dc2"];
            nodeChain = chain.Nodes["resolver:web.default.dc2"];
            Assert.Equal(nodeCheck.Type, nodeChain.Type);
            Assert.Equal(nodeCheck.Name, nodeChain.Name);
            Assert.Equal(nodeCheck.Resolver.Default, nodeChain.Resolver.Default);
            Assert.Equal(nodeCheck.Resolver.Target, nodeChain.Resolver.Target);
            Assert.Equal(nodeCheck.Resolver.ConnectTimeout.ToString(), nodeChain.Resolver.ConnectTimeout.ToString());
            targetCheck = check.Targets["web.default.dc2"];
            targetChain = chain.Targets["web.default.dc2"];
            Assert.Equal(targetCheck.ID, targetChain.ID);
            Assert.Equal(targetCheck.Service, targetChain.Service);
            Assert.Equal(targetCheck.Namespace, targetChain.Namespace);
            Assert.Equal(targetCheck.Datacenter, targetChain.Datacenter);
            Assert.Equal(targetCheck.SNI, targetChain.SNI);
            Assert.Equal(targetCheck.Name, targetChain.Name);
        }
    }
}
