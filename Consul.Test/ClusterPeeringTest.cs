// -----------------------------------------------------------------------
//  <copyright file="ClusterPeeringTest.cs" company="G-Research Limited">
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
using Xunit.Abstractions;

namespace Consul.Test
{
    public class ClusterPeeringTest : BaseFixture
    {
        private readonly ITestOutputHelper _output;
        public ClusterPeeringTest(ITestOutputHelper output)
        {
            _output = output;

        }

        [SkippableFact]
        public async Task ClusterPeeringTest_Create()
        {
            var cutOffVersion = SemanticVersion.Parse("1.14.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but this test is only supported from Consul {cutOffVersion}");

            var clusterPeeringEntry = new ClusterPeeringTokenEntry
            {
                PeerName = "cluster-02",
                Meta = new Dictionary<string, string> { ["env"] = "production" }
            };
            var clusterPeeringCreateResponse = await _client.ClusterPeering.GenerateToken(clusterPeeringEntry);
            Assert.NotNull(clusterPeeringCreateResponse);
            Assert.NotNull(clusterPeeringCreateResponse.Response.PeeringToken);
        }

        [SkippableFact]
        public async Task ClusterPeeringTest_ListPeerings()
        {
            var cutOffVersion = SemanticVersion.Parse("1.14.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but this test is only supported from Consul {cutOffVersion}");
            var result = await _client.ClusterPeering.ListPeerings(QueryOptions.Default);
            Assert.NotNull(result.Response);
            var firstObject = result.Response.First();
            Assert.NotNull(firstObject);
            Assert.NotNull(firstObject.ID);
            Assert.NotNull(firstObject.Name);
        }

        [SkippableFact]
        public async Task ClusterPeeringTest_GetPeering()
        {
            var cutOffVersion = SemanticVersion.Parse("1.14.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but this test is only supported from Consul {cutOffVersion}");
            var clusterPeeringEntry = new ClusterPeeringTokenEntry
            {
                PeerName = "cluster-03",
                Meta = new Dictionary<string, string> { ["env"] = "production" }
            };
            var clusterPeeringCreateResponse = await _client.ClusterPeering.GenerateToken(clusterPeeringEntry);
            var result = await _client.ClusterPeering.GetPeering("cluster-03", QueryOptions.Default);
            Assert.NotNull(result.Response);
            Assert.NotNull(result.Response.ID);
            Assert.NotNull(result.Response.Remote);
            Assert.NotNull(result.Response.StreamStatus);

            // Reading a connection that has not been generated
            var newResult = await _client.ClusterPeering.GetPeering("cluster-05", QueryOptions.Default);
            Assert.Null(newResult.Response);
        }

    }
}
