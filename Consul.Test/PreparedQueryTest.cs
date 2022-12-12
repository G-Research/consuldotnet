// -----------------------------------------------------------------------
//  <copyright file="PreparedQueryTest.cs" company="PlayFab Inc">
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
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class PreparedQueryTest : BaseFixture
    {
        [Fact]
        public async Task PreparedQuery_Test()
        {
            var registration = new CatalogRegistration
            {
                Datacenter = "dc1",
                Node = "foobaz",
                Address = "192.168.10.10",
                Service = new AgentService
                {
                    ID = "sql1",
                    Service = "sql",
                    Tags = new[] { "master", "v1" },
                    Port = 8000
                }
            };

            await _client.Catalog.Register(registration);

            Assert.NotNull((await _client.Catalog.Node("foobaz")).Response);

            var mgmtquerytoken = new QueryOptions
            {
                Token = TestHelper.MasterToken
            };

            var def = new PreparedQueryDefinition
            {
                Name = "Test-Query",
                Service = new ServiceQuery
                {
                    Service = "sql",
                    Near = "_agent",
                    OnlyPassing = true
                },
                DNS = new QueryDNSOptions
                {
                    TTL = TimeSpan.FromSeconds(5)
                }
            };

            var id = (await _client.PreparedQuery.Create(def)).Response;
            def.ID = id;

            var defs = (await _client.PreparedQuery.Get(id)).Response;

            Assert.NotNull(defs);
            Assert.True(defs.Length == 1);
            Assert.Equal(def.Service.Service, defs[0].Service.Service);

            defs = null;
            defs = (await _client.PreparedQuery.List(mgmtquerytoken)).Response;

            Assert.NotNull(defs);
            Assert.True(defs.Length == 1);
            Assert.Equal(def.Service.Service, defs[0].Service.Service);

            def.Name = "my-query";

            await _client.PreparedQuery.Update(def);

            defs = null;
            defs = (await _client.PreparedQuery.Get(id)).Response;

            Assert.NotNull(defs);
            Assert.True(defs.Length == 1);
            Assert.Equal(def.Name, defs[0].Name);

            var results = (await _client.PreparedQuery.Execute(id)).Response;

            Assert.NotNull(results);
            var nodes = results.Nodes.Where(n => n.Node.Name == "foobaz").ToArray();
            Assert.True(nodes.Length == 1);
            Assert.Equal("foobaz", nodes[0].Node.Name);

            results = null;
            results = (await _client.PreparedQuery.Execute("my-query")).Response;

            Assert.NotNull(results);
            nodes = results.Nodes.Where(n => n.Node.Name == "foobaz").ToArray();
            Assert.True(nodes.Length == 1);
            Assert.Equal("foobaz", results.Nodes[0].Node.Name);

            await _client.PreparedQuery.Delete(id);

            defs = null;
            defs = (await _client.PreparedQuery.List(mgmtquerytoken)).Response;

            Assert.True(defs.Length == 0);
        }
    }
}
