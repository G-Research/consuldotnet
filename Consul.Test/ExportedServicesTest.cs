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
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class ExportedServicesTest : BaseFixture
    {
        [SkippableFact]
        public async Task ExportedServicesTest_ListExportedServices()
        {
            var cutOffVersion = SemanticVersion.Parse("1.17.3");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but this test is only supported from Consul {cutOffVersion}");

            var serviceName = KVTest.GenerateTestKeyName();
            var peerName = "test-peer";
            var serviceRegistration = new AgentServiceRegistration
            {
                ID = serviceName + "-1",
                Name = serviceName,
                Port = 8080,
                Tags = new[] { "test" }
            };

            await _client.Agent.ServiceRegister(serviceRegistration);

            try
            {
                var exportConfig = new ExportedServicesConfigEntry
                {
                    Name = "default",
                    Services = new[]
                    {
                        new ExportedServiceConfig
                        {
                            Name = serviceName,
                            Consumers = new[]{ new ServiceConsumer
                            {
                                Peer = peerName
                            }
                            }
                        }
                    }
                };

                await _client.Configuration.ApplyConfig(exportConfig);

                var result = await _client.ExportedServices.ListExportedService();
                Assert.NotNull(result.Response);
                Assert.NotEmpty(result.Response);

                // Verify the service is in the list
                Assert.Contains(result.Response, svc => svc.Service == serviceName);
                // Verify peer is not empty in the response
                Assert.Contains(result.Response, con => con.Consumers.Peers.First() == peerName);
            }
            finally
            {
                // Deregister the service
                await _client.Agent.ServiceDeregister(serviceRegistration.ID);
            }
        }
    }
}
