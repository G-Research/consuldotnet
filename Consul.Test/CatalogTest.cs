// -----------------------------------------------------------------------
//  <copyright file="CatalogTest.cs" company="PlayFab Inc">
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
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class CatalogTest : IDisposable
    {
        private ConsulClient _client;

        public CatalogTest()
        {
            _client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            });
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        [Fact]
        public async Task Catalog_GetDatacenters()
        {
            var datacenterList = await _client.Catalog.Datacenters();

            Assert.NotEmpty(datacenterList.Response);
        }

        [Fact]
        public async Task Catalog_GetNodes()
        {
            var nodeList = await _client.Catalog.Nodes();

            Assert.NotEqual((ulong)0, nodeList.LastIndex);
            Assert.NotEmpty(nodeList.Response);
            // make sure deserialization is working right
            Assert.NotNull(nodeList.Response[0].Address);
            Assert.NotNull(nodeList.Response[0].Name);
        }

        [Fact]
        public async Task Catalog_GetServices()
        {
            var servicesList = await _client.Catalog.Services();

            Assert.NotEqual((ulong)0, servicesList.LastIndex);
            Assert.NotEmpty(servicesList.Response);
        }

        [Fact]
        public async Task Catalog_GetConsulService()
        {
            var serviceList = await _client.Catalog.Service("consul");

            Assert.NotEqual((ulong)0, serviceList.LastIndex);
            Assert.NotEmpty(serviceList.Response);
        }

        [Fact]
        public async Task Catalog_GetLocalhostNode()
        {
            var node = await _client.Catalog.Node(await _client.Agent.GetNodeName());

            Assert.NotEqual((ulong)0, node.LastIndex);
            Assert.NotNull(node.Response.Services);
            Assert.Equal("127.0.0.1", node.Response.Node.Address);
            Assert.True(node.Response.Node.TaggedAddresses.Count > 0);
            Assert.True(node.Response.Node.TaggedAddresses.ContainsKey("wan"));
        }

        [Fact]
        public async Task Catalog_RegistrationDeregistration()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var service = new AgentService
            {
                ID = svcID,
                Service = "redis",
                Tags = new[] { "master", "v1" },
                Port = 8000
            };

            var check = new AgentCheck
            {
                Node = "foobar",
                CheckID = "service:" + svcID,
                Name = "Redis health check",
                Notes = "Script based health check",
                Status = HealthStatus.Passing,
                ServiceID = svcID
            };

            var registration = new CatalogRegistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10",
                Service = service,
                Check = check
            };

            await _client.Catalog.Register(registration);

            var node = await _client.Catalog.Node("foobar");
            Assert.True(node.Response.Services.ContainsKey(svcID));

            var health = await _client.Health.Node("foobar");
            Assert.Equal("service:" + svcID, health.Response[0].CheckID);

            var dereg = new CatalogDeregistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10",
                CheckID = "service:" + svcID
            };

            await _client.Catalog.Deregister(dereg);

            health = await _client.Health.Node("foobar");
            Assert.Empty(health.Response);

            dereg = new CatalogDeregistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10"
            };

            await _client.Catalog.Deregister(dereg);

            node = await _client.Catalog.Node("foobar");
            Assert.Null(node.Response);
        }

        [Fact]
        public async Task Catalog_EnableTagOverride()
        {
            var svcID = KVTest.GenerateTestKeyName();
            var service = new AgentService
            {
                ID = svcID,
                Service = svcID,
                Tags = new[] { "master", "v1" },
                Port = 8000
            };

            var registration = new CatalogRegistration
            {
                Datacenter = "dc1",
                Node = "foobar",
                Address = "192.168.10.10",
                Service = service
            };

            using (IConsulClient client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            }))
            {
                await client.Catalog.Register(registration);

                var node = await client.Catalog.Node("foobar");

                Assert.Contains(svcID, node.Response.Services.Keys);
                Assert.False(node.Response.Services[svcID].EnableTagOverride);

                var services = await client.Catalog.Service(svcID);

                Assert.NotEmpty(services.Response);
                Assert.Equal(svcID, services.Response[0].ServiceName);

                Assert.False(services.Response[0].ServiceEnableTagOverride);
            }

            // Use a new scope
            using (IConsulClient client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            }))
            {
                service.EnableTagOverride = true;

                await client.Catalog.Register(registration);
                var node = await client.Catalog.Node("foobar");

                Assert.Contains(svcID, node.Response.Services.Keys);
                Assert.True(node.Response.Services[svcID].EnableTagOverride);

                var services = await client.Catalog.Service(svcID);

                Assert.NotEmpty(services.Response);
                Assert.Equal(svcID, services.Response[0].ServiceName);

                Assert.True(services.Response[0].ServiceEnableTagOverride);
            }
        }
    }
}
