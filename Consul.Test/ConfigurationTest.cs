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
    [CollectionDefinition("NonParallelCollection")]
    public class NonParallelCollection : ICollectionFixture<ConfigurationTest>
    {
    }

    [Collection("NonParallelCollection")]
    public class ConfigurationTest : BaseFixture
    {
        [Fact]
        public async Task Configuration_ApplyConfig()
        {
            var payload = new ServiceDefaultsEntry
            {
                Kind = "service-defaults",
                Name = "web",
                Protocol = "http"
            };
            var writeResult = await _client.Configuration.ApplyConfig(payload);
            Assert.Equal(HttpStatusCode.OK, writeResult.StatusCode);
        }
    }

}
