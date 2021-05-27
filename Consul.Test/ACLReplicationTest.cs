// -----------------------------------------------------------------------
//  <copyright file="ACLReplicationTest.cs" company="G-Research Limited">
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
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Consul.Test
{
    public class ACLReplicationTest : BaseFixture
    {
        [SkippableFact]
        public async Task ACLReplication_GetStatus()
        {
            Skip.If(string.IsNullOrEmpty(TestHelper.MasterToken));

            var aclReplicationEntry = new ACLReplicationEntry(false, false);
            var aclReplicationStatus = await _client.ACLReplication.Status();
            Assert.NotNull(aclReplicationStatus.Response);
            Assert.NotEqual(TimeSpan.Zero, aclReplicationStatus.RequestTime);
            aclReplicationStatus.Response.Should().BeEquivalentTo(aclReplicationEntry);
        }
    }
}
