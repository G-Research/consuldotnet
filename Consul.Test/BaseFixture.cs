using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class BaseFixture : IAsyncLifetime
    {
        protected ConsulClient _client;
        private static bool _ready;

        static BaseFixture()
        {
            // Some Consul object (e.g. semaphores) use multiple http connections,
            // but on .NETFramework the default limit is sometimes very low (2) so we need to bump it to higher value.
            // E.g. https://github.com/microsoft/referencesource/blob/5697c29004a34d80acdaf5742d7e699022c64ecd/System.Web/HttpRuntime.cs#L1200
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        public BaseFixture()
        {
            _client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            });
        }

        public Task DisposeAsync()
        {
            _client.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// With parallel testing on the CI, sometimes the tests start before the consul server finished properly initializing
        /// This aims to workaround it in a not so elegant way. https://github.com/hashicorp/consul/issues/819
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            if (_ready) return;

            var cancelToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

            await Task.Run(async () =>
            {
                while (!_ready)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    try
                    {
                        var leader = await _client.Status.Leader();
                        if (!string.IsNullOrEmpty(leader))
                        {
                            _ready = true;
                        }
                    }
                    catch
                    {
                    }
                }
            }, cancelToken);
        }
    }
}
