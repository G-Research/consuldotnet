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
            var result = await _client.ExportedServices.ListExportedService();
            Assert.NotNull(result.Response);
        }
    }
}
