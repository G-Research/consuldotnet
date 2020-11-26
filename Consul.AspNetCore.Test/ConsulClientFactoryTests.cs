using System;
using Microsoft.Extensions.Options;
using Xunit;

namespace Consul.AspNetCore.Test
{
    public class ConsulClientFactoryTests
    {
        [Fact]
        public void CreateClient_Options_Override()
        {
            var datacenter = "datacenter";
            var address = new Uri("http://address");
            var token = "token";
            var waitTime = TimeSpan.FromSeconds(30);

            var monitor = ConsulOptionsTestHelper.CreateOptionsMonitor(
                new[]
                {
                    new ConfigureOptions<ConsulClientConfiguration>(options =>
                    {
                        options.Datacenter = datacenter;
                        options.Address = address;
                        options.Token = token;
                        options.WaitTime = waitTime;
                    })
                });

            var factory = new ConsulClientFactory(monitor);

            var client = factory.CreateClient() as ConsulClient;

            var configuration = client.Config;

            Assert.Same(monitor.Get(Options.DefaultName), configuration);
            Assert.Equal(datacenter, configuration.Datacenter);
            Assert.Equal(address, configuration.Address);
            Assert.Equal(token, configuration.Token);
            Assert.Equal(waitTime, configuration.WaitTime);
        }

        [Fact]
        public void CreateClient_NamedOptions_Override()
        {
            var datacenter = "datacenter2";
            var address = new Uri("http://address2");
            var token = "token2";
            var waitTime = TimeSpan.FromSeconds(30);

            var monitor = ConsulOptionsTestHelper.CreateOptionsMonitor(
                new IConfigureOptions<ConsulClientConfiguration>[]
                {
                    new ConfigureOptions<ConsulClientConfiguration>(options =>
                    {
                        options.Datacenter = "datacenter1";
                        options.Address = new Uri("http://address1");
                        options.Token = "token1";
                        options.WaitTime = TimeSpan.FromSeconds(10);
                    }),
                    new ConfigureNamedOptions<ConsulClientConfiguration>(ConsulOptionsTestHelper.CustomOptionsName, options =>
                    {
                        options.Datacenter = datacenter;
                        options.Address = address;
                        options.Token = token;
                        options.WaitTime = waitTime;
                    })
                });

            var factory = new ConsulClientFactory(monitor);

            var client = factory.CreateClient(ConsulOptionsTestHelper.CustomOptionsName) as ConsulClient;

            var configuration = client.Config;

            Assert.Same(monitor.Get(ConsulOptionsTestHelper.CustomOptionsName), configuration);
            Assert.Equal(datacenter, configuration.Datacenter);
            Assert.Equal(address, configuration.Address);
            Assert.Equal(token, configuration.Token);
            Assert.Equal(waitTime, configuration.WaitTime);
        }
    }
}
