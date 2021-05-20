using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    public class BaseFixture : IAsyncLifetime
    {
        protected ConsulClient _client;

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
            bool ready = false;

            while (!ready)
            {
                try
                {
                    var leader = await _client.Status.Leader();
                    if (!string.IsNullOrEmpty(leader))
                    {
                        ready = true;
                    }
                }
                catch
                {
                }
            }
        }
    }
}
