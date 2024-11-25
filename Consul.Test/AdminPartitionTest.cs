// -----------------------------------------------------------------------
//  <copyright file="AdminPartitionTest.cs" company="G-Research Limited">
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
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class AdminPartitionTest : BaseFixture
    {
        [EnterpriseOnlyFact]
        public async Task AdminPartition_CreateAdminPartition()
        {
            var cutOffVersion = SemanticVersion.Parse("1.11.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Admin Partition` is only supported from Consul {cutOffVersion}");

            var check = new Partition { Name = "na-west", Description = "Partition for North America West" };

            var request = await _client.AdminPartition.Create(check);

            Assert.Equal(check.Name, request.Response.Name);
        }
    }
}
