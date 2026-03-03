// -----------------------------------------------------------------------
//  <copyright file="PartitionTest.cs" company="G-Research Limited">
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class PartitionTest : BaseFixture
    {
        void SkipIfNotSupported()
        {
            var cutOffVersion = SemanticVersion.Parse("1.18.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but Partitions are only supported from Consul {cutOffVersion}");
        }

        [EnterpriseOnlyFact]
        public async Task Partition_Create()
        {
            SkipIfNotSupported();
            var partition = new PartitionEntry
            {
                Name = "na-west",
                Description = "Partition for North America West"
            };

            var writeResult = await _client.Partition.Create(partition);
            Assert.NotNull(writeResult.Response);
            Assert.Equal(partition.Name, writeResult.Response.Name);
            Assert.Equal(partition.Description, writeResult.Response.Description);
        }
    }
}
