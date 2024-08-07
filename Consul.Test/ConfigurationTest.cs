// -----------------------------------------------------------------------
//  <copyright file="ConfigurationTest.cs" company="G-Research Limited">
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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    [Collection(nameof(ExclusiveCollection))]
    public class ConfigurationTest : BaseFixture
    {
        [Theory]
        [InlineData("http")]
        [InlineData("https")]
        public async Task Configuration_ApplyConfig(string protocol)
        {
            var payload = new ServiceDefaultsEntry
            {
                Kind = "service-defaults",
                Name = "web",
                Protocol = protocol
            };

            var writeResult = await _client.Configuration.ApplyConfig(payload);
            Assert.Equal(HttpStatusCode.OK, writeResult.StatusCode);
            var queryResult = await _client.Configuration.GetConfig<ServiceDefaultsEntry>(payload.Kind, payload.Name);
            Assert.Equal(HttpStatusCode.OK, queryResult.StatusCode);
            Assert.Equal(payload.Name, queryResult.Response.Name);
            Assert.Equal(payload.Kind, queryResult.Response.Kind);
            Assert.Equal(payload.Protocol, queryResult.Response.Protocol);
        }

        [Fact]
        public async Task Configuration_ListConfig()
        {
            var firstPayload = new ServiceDefaultsEntry
            {
                Kind = "service-defaults",
                Name = "web",
                Protocol = "https"
            };

            var secondPayload = new ServiceDefaultsEntry
            {
                Kind = "service-defaults",
                Name = "db",
                Protocol = "https"
            };
            var writeResult = await _client.Configuration.ApplyConfig(firstPayload);
            Assert.Equal(HttpStatusCode.OK, writeResult.StatusCode);
            writeResult = await _client.Configuration.ApplyConfig(secondPayload);
            Assert.Equal(HttpStatusCode.OK, writeResult.StatusCode);
            var queryResult = await _client.Configuration.ListConfig<ServiceDefaultsEntry>(firstPayload.Kind);
            var configurations = queryResult.Response;
            var webConfig = configurations.SingleOrDefault(c => c.Name == firstPayload.Name);
            var dbConfig = configurations.SingleOrDefault(c => c.Name == secondPayload.Name);
            Assert.NotNull(dbConfig);
            Assert.NotNull(webConfig);

            Assert.Equal(firstPayload.Name, webConfig.Name);
            Assert.Equal(firstPayload.Kind, webConfig.Kind);
            Assert.Equal(firstPayload.Protocol, webConfig.Protocol);

            Assert.Equal(secondPayload.Name, dbConfig.Name);
            Assert.Equal(secondPayload.Kind, dbConfig.Kind);
            Assert.Equal(secondPayload.Protocol, dbConfig.Protocol);
        }

        [Fact]
        public async Task Configuration_DeleteConfig()
        {
            var payload = new ServiceDefaultsEntry
            {
                Kind = "service-defaults",
                Name = "test-service",
                Protocol = "http"
            };

            var writeResult = await _client.Configuration.ApplyConfig(payload);
            Assert.Equal(HttpStatusCode.OK, writeResult.StatusCode);

            var getConfigResult = await _client.Configuration.GetConfig<ServiceDefaultsEntry>(payload.Kind, payload.Name);
            Assert.Equal(HttpStatusCode.OK, getConfigResult.StatusCode);
            Assert.Equal(payload.Name, getConfigResult.Response.Name);
            Assert.Equal(payload.Kind, getConfigResult.Response.Kind);
            Assert.Equal(payload.Protocol, getConfigResult.Response.Protocol);

            var deleteResult = await _client.Configuration.DeleteConfig(payload.Kind, payload.Name);
            Assert.Equal(HttpStatusCode.OK, deleteResult.StatusCode);

            var getDeletedConfigResult = await _client.Configuration.GetConfig<ServiceDefaultsEntry>(payload.Kind, payload.Name);
            Assert.Equal(HttpStatusCode.NotFound, getDeletedConfigResult.StatusCode);
            Assert.Null(getDeletedConfigResult.Response);
        }

        [Fact]
        public async Task Configuration_ServiceResolverEntry()
        {
            var entry = new ServiceResolverEntry {
                Name = "test-failover",
                Namespace = "default",
                DefaultSubset = "v1",
                Subsets = new Dictionary<string, ServiceResolverSubset>()
                {
                    { "v1", new ServiceResolverSubset { Filter = "Service.Meta.version == v1"} },
                    { "v2", new ServiceResolverSubset { Filter = "Service.Meta.version == v2"} }
                },
                Failover = new Dictionary<string, ServiceResolverFailover>()
                {
                    { "*", new ServiceResolverFailover { Datacenters = new List<string> { "dc2" } } },
                    { "v1", new ServiceResolverFailover { Service = "alternate", Namespace = "default" } }
                },
                ConnectTimeout = new TimeSpan(0, 0, 5),
                Meta = new Dictionary<string, string>()
                {
                    { "foo", "bar" },
                    { "gir", "zim" }
                }
            };
            await _client.Configuration.ApplyConfig(entry);

            var result = await _client.Configuration.GetConfig<ServiceResolverEntry>("service-resolver", entry.Name);
            var returned = result.Response;

            Assert.Equal(entry.Name, returned.Name);
            Assert.Equal(entry.Namespace, returned.Namespace);
            Assert.Equal(entry.DefaultSubset, returned.DefaultSubset);
            Assert.Equal(entry.Subsets.Count, returned.Subsets.Count);
            Assert.Equal(entry.Subsets["v1"].Filter, returned.Subsets["v1"].Filter);
            Assert.Equal(entry.Subsets["v2"].Filter, returned.Subsets["v2"].Filter);
            Assert.Equal(entry.Failover.Count, returned.Failover.Count);
            Assert.Equal(entry.Failover["*"].Datacenters[0], returned.Failover["*"].Datacenters[0]);
            Assert.Equal(entry.Failover["v1"].Service, returned.Failover["v1"].Service);
            Assert.Equal(entry.Failover["v1"].Namespace, returned.Failover["v1"].Namespace);
            Assert.Equal(entry.ConnectTimeout.ToString(), returned.ConnectTimeout.ToString());
            Assert.Equal(entry.Meta.Count, returned.Meta.Count);
            Assert.Equal(entry.Meta["foo"], returned.Meta["foo"]);
            Assert.Equal(entry.Meta["gir"], returned.Meta["gir"]);

            entry = new ServiceResolverEntry
            {
                Name = "test-redirect",
                Namespace = "default",
                Redirect = new ServiceResolverRedirect
                {
                    Service = "test-failover",
                    ServiceSubset = "v2",
                    Namespace = "default",
                    Datacenter = "d"
                }
            };
            await _client.Configuration.ApplyConfig(entry);

            result = await _client.Configuration.GetConfig<ServiceResolverEntry>("service-resolver", entry.Name);
            returned = result.Response;

            Assert.Equal(entry.Name, returned.Name);
            Assert.Equal(entry.Namespace, returned.Namespace);
            Assert.Equal(entry.Redirect.Service, returned.Redirect.Service);
            Assert.Equal(entry.Redirect.ServiceSubset, returned.Redirect.ServiceSubset);
            Assert.Equal(entry.Redirect.Namespace, returned.Redirect.Namespace);
            Assert.Equal(entry.Redirect.Datacenter, returned.Redirect.Datacenter);
        }
    }
}
