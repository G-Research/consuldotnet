// -----------------------------------------------------------------------
//  <copyright file="StatusTest.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
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
using Xunit;

namespace Consul.Test
{
    public class StatusTest : IDisposable
    {
        private AsyncReaderWriterLock.Releaser _lock;
        private ConsulClient _client;

        public StatusTest()
        {
            _lock = AsyncHelpers.RunSync(() => SelectiveParallel.NoParallel());
            _client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            });
        }

        public void Dispose()
        {
            _client.Dispose();
            _lock.Dispose();
        }

        [Fact]
        public async Task Status_Leader()
        {
            var leader = await _client.Status.Leader();
            Assert.False(string.IsNullOrEmpty(leader));
        }

        [Fact]
        public async Task Status_Peers()
        {
            var peers = await _client.Status.Peers();
            Assert.True(peers.Length > 0);
        }
    }
}
