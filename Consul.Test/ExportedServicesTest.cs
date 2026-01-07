// -----------------------------------------------------------------------
//  <copyright file="ExportedServicesTest.cs" company="G-Research Limited">
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class ExportedServicesTest : BaseFixture
    {
        [Fact]
        public async Task ExportedServices_List()
        {

            var svcID = KVTest.GenerateTestKeyName();
            var tagsInit = new[] { "web", "monitoring", "platform" };
            var registration = new AgentServiceRegistration
            {
                Name = svcID,
                Tags = tagsInit,
                Port = 8000,
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(15)
                }
            };

            await _client.Agent.ServiceRegister(registration);
            var services = await _client.Agent.Services();
            var tags = services.Response.FirstOrDefault(d => d.Key == svcID).Value.Tags;

            Assert.NotNull(services.Response);
            Assert.Equal(tagsInit.Length, tags.Length);

            var config = new ExportedServiceEntry
            {
                Kind = "exported-services",
                Name = "default",
                Services = new List<ServiceDefinition>
                {
                    new ServiceDefinition
                    {
                        Name = "*",
                        Namespace = "*",
                        Consumers = tagsInit.Select(t => new ConsumerDefinition { Peer = t }).ToList()
                    }
                }
            };

            var writeResult = await _client.Configuration.ApplyConfig<ExportedServiceEntry>(config);

            Assert.NotNull(writeResult);
            Assert.Equal(HttpStatusCode.OK, writeResult.StatusCode);

            var querytoken = new QueryOptions
            {
                Token = TestHelper.MasterToken
            };

            var res = await _client.ExportedServices.List(querytoken);
            var peers = res.Response.FirstOrDefault(b => b.Service == svcID).Consumers.Peers;

            Assert.NotNull(res);
            Assert.Equal(tags.Length, peers.Count);
        }
    }
}
