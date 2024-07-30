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
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;
using Consul;

namespace Consul.Test
{
    public class DiscoveryChainTest : BaseFixture
    {
        public const string TestClusterID = "11111111-2222-3333-4444-555555555555";

        [Fact]
        public async Task DiscoveryChain_Get()
        {
            var check1 = new CompiledDiscoveryChain
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
                                ConnectionTimeout = new TimeSpan(0, 0, 5),
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

            await Task.Delay(1000 * 20);
            var response = await _client.DiscoveryChain.Get("web");
            Assert.NotNull(response.Response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(check1, response.Response);
        }
    }
}
