// -----------------------------------------------------------------------
//  <copyright file="ConnectTest.cs" company="G-Research Limited">
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
using Microsoft.VisualStudio.TestPlatform.Utilities;
using NuGet.Versioning;
using Xunit;
using Xunit.Abstractions;

namespace Consul.Test
{
    public class ConnectTest : BaseFixture
    {
        [Fact]
        public async Task Connect_CARoots()
        {
            var req = await _client.Connect.CARoots();
            var result = req.Response;
            var root = result.Roots.First();
            Assert.Equal("11111111-2222-3333-4444-555555555555.consul", result.TrustDomain);
            Assert.NotEmpty(result.Roots);
            Assert.NotEmpty(result.ActiveRootID);
            Assert.NotEmpty(root.RootCert);
            Assert.NotEmpty(root.Name);
            Assert.NotNull(root.RootCert);
            Assert.NotNull(root.SigningKeyID);
        }

        [Fact]
        public async Task Connect_GetCAConfigurationTest()
        {
            var req = await _client.Connect.CAGetConfig();
            var result = req.Response;

            Assert.Equal("consul", result.Provider);
            Assert.NotEmpty(result.Config);
            Assert.False(result.ForceWithoutCrossSigning);
            Assert.NotEqual((ulong)0, result.CreateIndex);
            Assert.NotEqual((ulong)0, result.ModifyIndex);
        }

        [SkippableFact]
        public async Task Connect_CASetConfig()
        {
            var cutOffVersion = SemanticVersion.Parse("1.7.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but setting CA config is only supported from Consul {cutOffVersion}");

            var req = await _client.Connect.CAGetConfig();
            var config = req.Response;

            config.Config["test_state"] = new Dictionary<string, string> { { "foo", "bar" } };
            config.Config["PrivateKey"] = "";

            await _client.Connect.CASetConfig(config);
            req = await _client.Connect.CAGetConfig();
            var updatedConfig = req.Response;

            Assert.Equal("consul", updatedConfig.Provider);
            Assert.Equal("bar", updatedConfig.State["foo"]);
            Assert.Equal("", updatedConfig.Config["PrivateKey"]);
        }

        [SkippableFact]
        public async Task Connect_ListIntentions()
        {
            var cutOffVersion = SemanticVersion.Parse("1.9.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `service intentions` are only supported from Consul {cutOffVersion}");

            var firstEntry = new ServiceIntentionsEntry
            {
                Kind = "service-intentions",
                Name = "Autobots-Assembler",
                Sources = new List<SourceIntention>
                {
                    new SourceIntention
                    {
                        Name = "fortunate",
                        Action = "allow",
                        LegacyCreateTime = DateTime.UtcNow,
                        LegacyUpdateTime = DateTime.UtcNow,
                    },
                    new SourceIntention
                    {
                        Name = "Prad",
                        Action = "allow",
                        LegacyCreateTime = DateTime.UtcNow,
                        LegacyUpdateTime = DateTime.UtcNow,
                    },
                    new SourceIntention
                    {
                        Name = "Mehdi",
                        Action = "allow",
                        LegacyCreateTime = DateTime.UtcNow,
                        LegacyUpdateTime = DateTime.UtcNow,
                    }
                }
            };
            var secondEntry = new ServiceIntentionsEntry
            {
                Kind = "service-intentions",
                Name = "Second",
                Sources = new List<SourceIntention>
                {
                    new SourceIntention
                    {
                        Name = "Optimus-Prime",
                        Action = "allow",
                        LegacyCreateTime = DateTime.UtcNow,
                        LegacyUpdateTime = DateTime.UtcNow,
                    },
                    new SourceIntention
                    {
                        Name = "Megatron",
                        Action = "deny",
                        LegacyCreateTime = DateTime.UtcNow,
                        LegacyUpdateTime = DateTime.UtcNow,
                    },
                    new SourceIntention
                    {
                        Name = "Sentinel-Prime",
                        Action = "deny",
                        LegacyCreateTime = DateTime.UtcNow,
                        LegacyUpdateTime = DateTime.UtcNow,
                    }
                }
            };
            var resultOne = await _client.Configuration.ApplyConfig(firstEntry);
            Assert.Equal(HttpStatusCode.OK, resultOne.StatusCode);

            var resultTwo = await _client.Configuration.ApplyConfig(secondEntry);
            Assert.Equal(HttpStatusCode.OK, resultTwo.StatusCode);

            var intentionsQuery = await _client.Connect.ListIntentions<ServiceIntention>();
            Assert.Equal(HttpStatusCode.OK, intentionsQuery.StatusCode);

            var intentions = intentionsQuery.Response;
            Assert.NotNull(intentions);

            var testIntention = intentions.First(i => i.SourceName == "Optimus-Prime");
            Assert.NotEmpty(testIntention.DestinationName);
            Assert.NotEmpty(testIntention.SourceName);
            Assert.NotEmpty(testIntention.DestinationNS);
            Assert.NotEmpty(testIntention.SourceType);
            Assert.NotEmpty(testIntention.SourceNS);
            Assert.Contains(testIntention.Action, new[] { "allow", "deny" });
            Assert.True(testIntention.CreateIndex > 0);
            Assert.True(testIntention.ModifyIndex > 0);
            Assert.True(testIntention.Precedence > 0);
            Assert.Equal(secondEntry.Name, testIntention.DestinationName);

            await _client.Configuration.DeleteConfig(firstEntry.Kind, firstEntry.Name);
            await _client.Configuration.DeleteConfig(secondEntry.Kind, secondEntry.Name);
        }

        [SkippableFact]
        public async Task Connect_UpsertIntentionByName()
        {
            var cutOffVersion = SemanticVersion.Parse("1.9.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `service intentions` is only supported from Consul {cutOffVersion}");

            var newEntry = new ServiceIntention
            {
                Action = "allow",
                SourceType = "consul",
                DestinationName = "Lakers",
                SourceName = "Luka"
            };

            var req = await _client.Connect.UpsertIntentionsByName(newEntry);
            Assert.Equal(HttpStatusCode.OK, req.StatusCode);
            Assert.True(req.Response);

            var intentionsQuery = await _client.Connect.ListIntentions<ServiceIntention>();
            Assert.Equal(HttpStatusCode.OK, intentionsQuery.StatusCode);

            var intentions = intentionsQuery.Response;
            Assert.NotNull(intentions);

            var testIntention = intentions.First(i => i.SourceName == newEntry.SourceName);
            Assert.NotEmpty(testIntention.DestinationName);
            Assert.NotEmpty(testIntention.SourceName);
            Assert.NotEmpty(testIntention.DestinationNS);
            Assert.NotEmpty(testIntention.SourceType);
            Assert.NotEmpty(testIntention.SourceNS);
            Assert.Equal("allow", testIntention.Action);
            Assert.True(testIntention.CreateIndex > 0);
            Assert.True(testIntention.ModifyIndex > 0);
            Assert.True(testIntention.Precedence > 0);
            await _client.Configuration.DeleteConfig("service-intentions", newEntry.DestinationName);
        }

        [SkippableFact]
        public async Task Connect_DeleteIntentionByName()
        {
            var cutOffVersion = SemanticVersion.Parse("1.9.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `service intentions` is only supported from Consul {cutOffVersion}");

            var newEntry = new ServiceIntention
            {
                Action = "allow",
                SourceType = "consul",
                DestinationName = "Haran",
                SourceName = "Kyle Crane"
            };

            var req = await _client.Connect.UpsertIntentionsByName(newEntry);
            Assert.Equal(HttpStatusCode.OK, req.StatusCode);
            Assert.True(req.Response);

            var intentionsQuery = await _client.Connect.ListIntentions<ServiceIntention>();
            Assert.Equal(HttpStatusCode.OK, intentionsQuery.StatusCode);

            var intentions = intentionsQuery.Response;
            Assert.NotNull(intentions);

            var intention = intentions.First(i => i.SourceName == newEntry.SourceName);

            var deleteReq = await _client.Connect.DeleteIntentionByName(intention.SourceName, intention.DestinationName);
            Assert.Equal(HttpStatusCode.OK, deleteReq.StatusCode);

            var allIntentions = await _client.Connect.ListIntentions<ServiceIntention>();
            Assert.Equal(HttpStatusCode.OK, allIntentions.StatusCode);

            var deletedIntentionExists = allIntentions.Response.Any(x => x.SourceName == newEntry.SourceName);
            Assert.False(deletedIntentionExists);
        }

        [SkippableFact]
        public async Task Connect_ReadSpecificIntentionByName()
        {
            var cutOffVersion = SemanticVersion.Parse("1.9.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `service intentions` are only supported from Consul {cutOffVersion}");

            string sourceName = "Kanye West";
            string destinationName = "Oscars";

            var newEntry = new ServiceIntentionsEntry
            {
                Kind = "service-intentions",
                Name = destinationName,
                Sources = new List<SourceIntention>
                {
                    new SourceIntention
                    {
                        Name = sourceName,
                        Action = "deny",
                        LegacyCreateTime = DateTime.UtcNow,
                        LegacyUpdateTime = DateTime.UtcNow,
                    }
                }
            };

            var req = await _client.Configuration.ApplyConfig(newEntry);
            Assert.Equal(HttpStatusCode.OK, req.StatusCode);

            var intentionQuery = await _client.Connect.ReadSpecificIntentionByName<ServiceIntention>(sourceName, destinationName);
            var intention = intentionQuery.Response;

            Assert.NotNull(intention);
            Assert.NotEmpty(intention.DestinationName);
            Assert.NotEmpty(intention.SourceName);
            Assert.NotEmpty(intention.DestinationNS);
            Assert.NotEmpty(intention.SourceType);
            Assert.NotEmpty(intention.SourceNS);
            Assert.Equal("deny", intention.Action);
            Assert.True(intention.CreateIndex > 0);
            Assert.True(intention.ModifyIndex > 0);
            Assert.True(intention.Precedence > 0);

            await _client.Configuration.DeleteConfig("service-intentions", destinationName);
        }

        [SkippableFact]
        public async Task Connect_ListMatchingIntentions()
        {
            var cutOffVersion = SemanticVersion.Parse("1.9.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `service intentions` are only supported from Consul {cutOffVersion}");

            var firstIntention = new ServiceIntention
            {
                Action = "allow",
                SourceType = "consul",
                DestinationName = "Krusty-Krab",
                SourceName = "Sponge-Bob"
            };
            var secondIntention = new ServiceIntention
            {
                Action = "deny",
                SourceType = "consul",
                DestinationName = "Krusty-Krab",
                SourceName = "Plankton"
            };

            var req1 = await _client.Connect.UpsertIntentionsByName(firstIntention);
            Assert.Equal(HttpStatusCode.OK, req1.StatusCode);

            var req2 = await _client.Connect.UpsertIntentionsByName(secondIntention);
            Assert.Equal(HttpStatusCode.OK, req2.StatusCode);

            var matchingIntentionQuery1 = await _client.Connect.ListMatchingIntentions("destination", firstIntention.DestinationName);
            Assert.Equal(HttpStatusCode.OK, matchingIntentionQuery1.StatusCode);
            Assert.NotNull(matchingIntentionQuery1.Response);

            var matchingIntentionsList1 = matchingIntentionQuery1.Response;

            var keysForMatchingIntentions1 = matchingIntentionsList1.Keys;

            Assert.Contains(keysForMatchingIntentions1, i => i.Contains("Krusty-Krab"));

            var krustyMatchingIntentions = matchingIntentionsList1["Krusty-Krab"];
            Assert.Contains(krustyMatchingIntentions, i => i.SourceName == "Sponge-Bob");
            Assert.Contains(krustyMatchingIntentions, i => i.SourceName == "Plankton");
            Assert.All(krustyMatchingIntentions, i => Assert.Equal("Krusty-Krab", i.DestinationName));
            Assert.All(krustyMatchingIntentions, i => Assert.Contains(i.Action, new[] { "allow", "deny" }));

            var matchingIntentionQuery2 = await _client.Connect.ListMatchingIntentions("source", secondIntention.SourceName);
            Assert.Equal(HttpStatusCode.OK, matchingIntentionQuery2.StatusCode);
            Assert.NotNull(matchingIntentionQuery2.Response);

            var matchingIntentionsList2 = matchingIntentionQuery2.Response;

            var keysForMatchingIntentions2 = matchingIntentionsList2.Keys;

            Assert.Contains(keysForMatchingIntentions2, i => i.Contains("Plankton"));

            var planktonMatchingIntentions = matchingIntentionsList2["Plankton"];
            Assert.All(planktonMatchingIntentions, i => Assert.Equal("Plankton", i.SourceName));
            Assert.All(planktonMatchingIntentions, i => Assert.Equal("Krusty-Krab", i.DestinationName));
            Assert.All(planktonMatchingIntentions, i => Assert.Equal("deny", i.Action));

            await _client.Connect.DeleteIntentionByName(firstIntention.SourceName, firstIntention.DestinationName);
            await _client.Connect.DeleteIntentionByName(secondIntention.SourceName, secondIntention.DestinationName);
        }
    }
}
