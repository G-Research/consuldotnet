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

        [Fact]
        public async Task Connect_ListIntentions()
        {
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
                        Name = "Medhi",
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

            Assert.Contains(intentions, i => !string.IsNullOrEmpty(i.DestinationName));
            Assert.Contains(intentions, i => !string.IsNullOrEmpty(i.SourceName));
            Assert.Contains(intentions, i => !string.IsNullOrEmpty(i.DestinationNS));
            Assert.Contains(intentions, i => !string.IsNullOrEmpty(i.SourceType));
            Assert.Contains(intentions, i => !string.IsNullOrEmpty(i.SourceNS));
            Assert.Contains(intentions, i => !string.IsNullOrEmpty(i.Action) && i.Action == "allow" || i.Action == "deny");
            Assert.Contains(intentions, i => i.CreateIndex > 0);
            Assert.Contains(intentions, i => i.ModifyIndex > 0);
            Assert.Contains(intentions, i => i.Precedence > 0);
            Assert.Contains(intentions, i => i.SourceName == "fortunate" && i.DestinationName == firstEntry.Name);
            Assert.Contains(intentions, i => i.SourceName == "Optimus-Prime" && i.DestinationName == secondEntry.Name);

            await _client.Configuration.DeleteConfig(firstEntry.Kind, firstEntry.Name);
            await _client.Configuration.DeleteConfig(secondEntry.Kind, secondEntry.Name);
        }
    }
}
