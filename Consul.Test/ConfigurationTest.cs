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
    }
}
