// -----------------------------------------------------------------------
//  <copyright file="ClientTest.cs" company="PlayFab Inc">
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class ClientTest : IDisposable
    {
        private ConsulClient _client;

        public ClientTest()
        {
            _client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            });
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        [Fact]
        public void Client_DefaultConfig_env()
        {
            const string addr = "1.2.3.4:5678";
            const string token = "abcd1234";
            const string auth = "username:password";
            Environment.SetEnvironmentVariable("CONSUL_HTTP_ADDR", addr);
            Environment.SetEnvironmentVariable("CONSUL_HTTP_TOKEN", token);
            Environment.SetEnvironmentVariable("CONSUL_HTTP_AUTH", auth);
            Environment.SetEnvironmentVariable("CONSUL_HTTP_SSL", "1");
            Environment.SetEnvironmentVariable("CONSUL_HTTP_SSL_VERIFY", "0");

            var client = new ConsulClient();
            var config = client.Config;

            Assert.Equal(addr, string.Format("{0}:{1}", config.Address.Host, config.Address.Port));
            Assert.Equal(token, config.Token);
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.NotNull(config.HttpAuth);
            Assert.Equal("username", config.HttpAuth.UserName);
            Assert.Equal("password", config.HttpAuth.Password);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.Equal("https", config.Address.Scheme);

            Environment.SetEnvironmentVariable("CONSUL_HTTP_ADDR", string.Empty);
            Environment.SetEnvironmentVariable("CONSUL_HTTP_TOKEN", string.Empty);
            Environment.SetEnvironmentVariable("CONSUL_HTTP_AUTH", string.Empty);
            Environment.SetEnvironmentVariable("CONSUL_HTTP_SSL", string.Empty);
            Environment.SetEnvironmentVariable("CONSUL_HTTP_SSL_VERIFY", string.Empty);


#if !CORECLR
            Assert.True((client.HttpHandler as WebRequestHandler).ServerCertificateValidationCallback(null, null, null,
                SslPolicyErrors.RemoteCertificateChainErrors));
            ServicePointManager.ServerCertificateValidationCallback = null;
#else
            Assert.True((client.HttpHandler as HttpClientHandler).ServerCertificateCustomValidationCallback(null, null, null,
                SslPolicyErrors.RemoteCertificateChainErrors));
#endif

            Assert.NotNull(client);
        }

        [Fact]
        public async Task Client_SetQueryOptions()
        {
            var opts = new QueryOptions()
            {
                Datacenter = "foo",
                Consistency = ConsistencyMode.Consistent,
                WaitIndex = 1000,
                WaitTime = new TimeSpan(0, 0, 100),
                Token = "12345"
            };
            var request = _client.Get<KVPair>("/v1/kv/foo", opts);

            await Assert.ThrowsAsync<ConsulRequestException>(async () => await request.Execute(CancellationToken.None));

            Assert.Equal("foo", request.Params["dc"]);
            Assert.True(request.Params.ContainsKey("consistent"));
            Assert.Equal("1000", request.Params["index"]);
            Assert.Equal("1m40s", request.Params["wait"]);
        }

        [Fact]
        public async Task Client_SetClientOptions()
        {
            using (var client = new ConsulClient(c =>
            {
                c.Address = TestHelper.HttpUri;
                c.Datacenter = "foo";
                c.WaitTime = new TimeSpan(0, 0, 100);
                c.Token = "12345";
            }))
            {
                var request = client.Get<KVPair>("/v1/kv/foo");

                await Assert.ThrowsAsync<ConsulRequestException>(async () =>
                    await request.Execute(CancellationToken.None));

                Assert.Equal("foo", request.Params["dc"]);
                Assert.Equal("1m40s", request.Params["wait"]);
            }
        }
        [Fact]
        public async Task Client_SetWriteOptions()
        {
            var opts = new WriteOptions()
            {
                Datacenter = "foo",
                Token = "12345"
            };

            var request = _client.Put("/v1/kv/foo", new KVPair("kv/foo"), opts);

            await Assert.ThrowsAsync<ConsulRequestException>(async () => await request.Execute(CancellationToken.None));

            Assert.Equal("foo", request.Params["dc"]);
        }

        [Fact]
        public async Task Client_CustomHttpClient()
        {
            using (var hc = new HttpClient())
            {
                hc.Timeout = TimeSpan.FromDays(10);
                hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var config = new ConsulClientConfiguration();
                config.Address = TestHelper.HttpUri;
                config.Token = TestHelper.MasterToken;

#pragma warning disable CS0618 // Type or member is obsolete
                using (var client = new ConsulClient(config, hc))
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    await client.KV.Put(new KVPair("customhttpclient") { Value = System.Text.Encoding.UTF8.GetBytes("hello world") });
                    Assert.Equal(TimeSpan.FromDays(10), client.HttpClient.Timeout);
                    Assert.Contains(new MediaTypeWithQualityHeaderValue("application/json"), client.HttpClient.DefaultRequestHeaders.Accept);
                }

                var getRawClientResponse = await hc.GetAsync(TestHelper.HttpAddr + "/v1/kv/customhttpclient?raw");
                Assert.Equal("hello world", await getRawClientResponse.Content.ReadAsStringAsync()); ;
            }
        }

        [Fact]
        public async Task Client_DisposeBehavior()
        {
            var opts = new WriteOptions()
            {
                Datacenter = "foo",
                Token = "12345"
            };

            _client.Dispose();

            var request = _client.Put("/v1/kv/foo", new KVPair("kv/foo"), opts);

            await Assert.ThrowsAsync<ObjectDisposedException>(() => request.Execute(CancellationToken.None));
        }

        [Fact]
        public async Task Client_ReuseAndUpdateConfig()
        {
            var config = new ConsulClientConfiguration();
            config.Address = TestHelper.HttpUri;
            config.Token = TestHelper.MasterToken;

#pragma warning disable CS0618 // Type or member is obsolete
            using (var client = new ConsulClient(config))
#pragma warning restore CS0618 // Type or member is obsolete
            {
#pragma warning disable CS0618 // Type or member is obsolete
                config.DisableServerCertificateValidation = true;
#pragma warning restore CS0618 // Type or member is obsolete
                await client.KV.Put(new KVPair("kv/reuseconfig") { Flags = 1000 });
                Assert.Equal<ulong>(1000, (await client.KV.Get("kv/reuseconfig")).Response.Flags);
#if !CORECLR
                Assert.True(client.HttpHandler.ServerCertificateValidationCallback(null, null, null,
                    SslPolicyErrors.RemoteCertificateChainErrors));
#endif
            }

#pragma warning disable CS0618 // Type or member is obsolete
            using (var client = new ConsulClient(config))
#pragma warning restore CS0618 // Type or member is obsolete
            {
#pragma warning disable CS0618 // Type or member is obsolete
                config.DisableServerCertificateValidation = false;
#pragma warning restore CS0618 // Type or member is obsolete
                await client.KV.Put(new KVPair("kv/reuseconfig") { Flags = 2000 });
                Assert.Equal<ulong>(2000, (await client.KV.Get("kv/reuseconfig")).Response.Flags);
#if !CORECLR
                Assert.Null(client.HttpHandler.ServerCertificateValidationCallback);
#endif
            }

#pragma warning disable CS0618 // Type or member is obsolete
            using (var client = new ConsulClient(config))
#pragma warning restore CS0618 // Type or member is obsolete
            {
                await client.KV.Delete("kv/reuseconfig");
            }
        }

        [Fact]
        public void Client_Constructors()
        {
            Action<ConsulClientConfiguration> cfgAction2 = (cfg) => { cfg.Token = "yep"; };
            Action<ConsulClientConfiguration> cfgAction = (cfg) => { cfg.Datacenter = "foo"; cfgAction2(cfg); };

            using (var c = new ConsulClient())
            {
                Assert.NotNull(c.Config);
                Assert.NotNull(c.HttpHandler);
                Assert.NotNull(c.HttpClient);
            }

            using (var c = new ConsulClient(cfgAction))
            {
                Assert.NotNull(c.Config);
                Assert.NotNull(c.HttpHandler);
                Assert.NotNull(c.HttpClient);
                Assert.Equal("foo", c.Config.Datacenter);
            }

            using (var c = new ConsulClient(cfgAction,
                (client) => { client.Timeout = TimeSpan.FromSeconds(30); }))
            {
                Assert.NotNull(c.Config);
                Assert.NotNull(c.HttpHandler);
                Assert.NotNull(c.HttpClient);
                Assert.Equal("foo", c.Config.Datacenter);
                Assert.Equal(TimeSpan.FromSeconds(30), c.HttpClient.Timeout);
            }

#if !CORECLR
            using (var c = new ConsulClient(cfgAction,
                (client) => { client.Timeout = TimeSpan.FromSeconds(30); },
                (handler) => { handler.Proxy = new WebProxy("127.0.0.1", 8080); }))
            {
                Assert.NotNull(c.Config);
                Assert.NotNull(c.HttpHandler);
                Assert.NotNull(c.HttpClient);
                Assert.Equal("foo", c.Config.Datacenter);
                Assert.Equal(TimeSpan.FromSeconds(30), c.HttpClient.Timeout);
                Assert.NotNull(c.HttpHandler.Proxy);
            }
#endif
        }
    }
}
