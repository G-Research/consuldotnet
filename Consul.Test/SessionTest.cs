// -----------------------------------------------------------------------
//  <copyright file="SessionTest.cs" company="PlayFab Inc">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class SessionTest : BaseFixture
    {
        [Fact]
        public async Task Session_CreateDestroy()
        {
            var sessionRequest = await _client.Session.Create();
            var id = sessionRequest.Response;
            Assert.NotEqual(TimeSpan.Zero, sessionRequest.RequestTime);
            Assert.False(string.IsNullOrEmpty(sessionRequest.Response));

            var destroyRequest = await _client.Session.Destroy(id);
            Assert.True(destroyRequest.Response);
        }

        [Fact]
        public async Task Session_CreateNoChecksDestroy()
        {
            var sessionRequest = await _client.Session.CreateNoChecks();

            var id = sessionRequest.Response;
            Assert.NotEqual(TimeSpan.Zero, sessionRequest.RequestTime);
            Assert.False(string.IsNullOrEmpty(sessionRequest.Response));

            var destroyRequest = await _client.Session.Destroy(id);
            Assert.True(destroyRequest.Response);
        }

        [Fact]
        public async Task Session_CreateRenewDestroy()
        {
            var sessionRequest = await _client.Session.Create(new SessionEntry() { TTL = TimeSpan.FromSeconds(10) });

            var id = sessionRequest.Response;
            Assert.NotEqual(TimeSpan.Zero, sessionRequest.RequestTime);
            Assert.False(string.IsNullOrEmpty(sessionRequest.Response));

            var renewRequest = await _client.Session.Renew(id);
            Assert.NotEqual(TimeSpan.Zero, renewRequest.RequestTime);
            Assert.NotNull(renewRequest.Response.ID);
            Assert.Equal(sessionRequest.Response, renewRequest.Response.ID);
            Assert.True(renewRequest.Response.TTL.HasValue);
            Assert.Equal(renewRequest.Response.TTL.Value.TotalSeconds, TimeSpan.FromSeconds(10).TotalSeconds);

            var destroyRequest = await _client.Session.Destroy(id);
            Assert.True(destroyRequest.Response);
        }

        [Fact]
        public async Task Session_CreateRenewDestroyRenew()
        {
            var sessionRequest = await _client.Session.Create(new SessionEntry() { TTL = TimeSpan.FromSeconds(10) });

            var id = sessionRequest.Response;
            Assert.NotEqual(TimeSpan.Zero, sessionRequest.RequestTime);
            Assert.False(string.IsNullOrEmpty(sessionRequest.Response));

            var renewRequest = await _client.Session.Renew(id);
            Assert.NotEqual(TimeSpan.Zero, renewRequest.RequestTime);
            Assert.NotNull(renewRequest.Response.ID);
            Assert.Equal(sessionRequest.Response, renewRequest.Response.ID);
            Assert.Equal(renewRequest.Response.TTL.Value.TotalSeconds, TimeSpan.FromSeconds(10).TotalSeconds);

            var destroyRequest = await _client.Session.Destroy(id);
            Assert.True(destroyRequest.Response);

            try
            {
                renewRequest = await _client.Session.Renew(id);
                Assert.True(false, "Session still exists");
            }
            catch (SessionExpiredException ex)
            {
                Assert.IsType<SessionExpiredException>(ex);
            }
        }

        [Fact]
        public async Task Session_Create_RenewPeriodic_Destroy()
        {
            var sessionRequest = await _client.Session.Create(new SessionEntry() { TTL = TimeSpan.FromSeconds(10) });

            var id = sessionRequest.Response;
            Assert.NotEqual(TimeSpan.Zero, sessionRequest.RequestTime);
            Assert.False(string.IsNullOrEmpty(sessionRequest.Response));

            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;

            var renewTask = _client.Session.RenewPeriodic(TimeSpan.FromSeconds(1), id, WriteOptions.Default, ct);

            var infoRequest = await _client.Session.Info(id);
            Assert.True(infoRequest.LastIndex > 0);
            Assert.True(infoRequest.KnownLeader);

            Assert.Equal(id, infoRequest.Response.ID);

            var destroyResponse = await _client.Session.Destroy(id);
            Assert.True(destroyResponse.Response);

            try
            {
                renewTask.Wait(10000);
                Assert.True(false, "timedout: missing session did not terminate renewal loop");
            }
            catch (AggregateException ae)
            {
                Assert.IsType<SessionExpiredException>(ae.InnerExceptions[0]);
            }
        }

        [Fact]
        public async Task Session_Create_RenewPeriodic_TTLExpire()
        {
            var initialTTL = TimeSpan.FromSeconds(10);
            var sessionRequest = await _client.Session.Create(new SessionEntry() { TTL = initialTTL });

            var id = sessionRequest.Response;
            Assert.NotEqual(TimeSpan.Zero, sessionRequest.RequestTime);
            Assert.False(string.IsNullOrEmpty(sessionRequest.Response));

            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;

            try
            {
                var destroyResponse = await _client.Session.Destroy(id);
                var renewTask = _client.Session.RenewPeriodic(initialTTL, id, WriteOptions.Default, ct);
                Assert.True(destroyResponse.Response);
                renewTask.Wait(4 * (int)initialTTL.TotalMilliseconds);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    Assert.IsType<SessionExpiredException>(e);
                }
                return;
            }
            catch (SessionExpiredException ex)
            {
                Assert.IsType<SessionExpiredException>(ex);
                return;
            }
            Assert.True(false, "timed out: missing session did not terminate renewal loop");
        }

        [Fact]
        public async Task Session_Info()
        {
            var sessionRequest = await _client.Session.Create();

            var id = sessionRequest.Response;

            Assert.NotEqual(TimeSpan.Zero, sessionRequest.RequestTime);
            Assert.False(string.IsNullOrEmpty(sessionRequest.Response));

            var infoRequest = await _client.Session.Info(id);
            Assert.True(infoRequest.LastIndex > 0);
            Assert.True(infoRequest.KnownLeader);

            Assert.Equal(id, infoRequest.Response.ID);

            Assert.True(string.IsNullOrEmpty(infoRequest.Response.Name));
            Assert.False(string.IsNullOrEmpty(infoRequest.Response.Node));
            Assert.True(infoRequest.Response.CreateIndex > 0);
            Assert.Equal(infoRequest.Response.Behavior, SessionBehavior.Release);

            var destroyRequest = await _client.Session.Destroy(id);
            Assert.True(destroyRequest.Response);
        }

        [Fact]
        public async Task Session_Node()
        {
            var sessionRequest = await _client.Session.Create();

            var id = sessionRequest.Response;
            try
            {
                var infoRequest = await _client.Session.Info(id);

                var nodeRequest = await _client.Session.Node(infoRequest.Response.Node);

                Assert.Contains(sessionRequest.Response, nodeRequest.Response.Select(s => s.ID));
                Assert.NotEqual((ulong)0, nodeRequest.LastIndex);
                Assert.True(nodeRequest.KnownLeader);
            }
            finally
            {
                var destroyRequest = await _client.Session.Destroy(id);
                Assert.True(destroyRequest.Response);
            }
        }

        [Fact]
        public async Task Session_List()
        {
            var sessionRequest = await _client.Session.Create();

            var id = sessionRequest.Response;

            try
            {
                var listRequest = await _client.Session.List();

                Assert.Contains(sessionRequest.Response, listRequest.Response.Select(s => s.ID));
                Assert.NotEqual((ulong)0, listRequest.LastIndex);
                Assert.True(listRequest.KnownLeader);
            }
            finally
            {
                var destroyRequest = await _client.Session.Destroy(id);
                Assert.True(destroyRequest.Response);
            }
        }
    }
}
