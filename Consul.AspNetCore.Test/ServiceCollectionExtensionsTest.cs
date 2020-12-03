using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace Consul.AspNetCore.Test
{
    public class ServiceCollectionExtensionsTest
    {
        private readonly IServiceCollection _services;

        public ServiceCollectionExtensionsTest()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddConsul_Client_Registered()
        {
            _services.AddConsul();

            var descriptor = Assert.Single(_services.Where(x => x.ServiceType == typeof(IConsulClient)));

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Null(descriptor.ImplementationInstance);
            Assert.NotNull(descriptor.ImplementationFactory);
        }

        [Fact]
        public void AddConsul_Options_Override()
        {
            var datacenter = "datacenter";
            var address = new Uri("http://address");
            var token = "token";
            var waitTime = TimeSpan.FromSeconds(30);

            var serviceProvider = _services
                .AddConsul(options =>
                {
                    options.Datacenter = datacenter;
                    options.Address = address;
                    options.Token = token;
                    options.WaitTime = waitTime;
                })
                .BuildServiceProvider();

            var consulClient = serviceProvider.GetRequiredService<IConsulClient>() as ConsulClient;

            var configuration = consulClient.Config;

            Assert.Equal(datacenter, configuration.Datacenter);
            Assert.Equal(address, configuration.Address);
            Assert.Equal(token, configuration.Token);
            Assert.Equal(waitTime, configuration.WaitTime);
        }

        [Fact]
        public void AddConsul_Named_Options_Override()
        {
            var datacenter1 = "datacenter1";
            var address1 = new Uri("http://address1");
            var token1 = "token1";
            var waitTime1 = TimeSpan.FromSeconds(10);

            var datacenter2 = "datacenter2";
            var address2 = new Uri("http://address2");
            var token2 = "token2";
            var waitTime2 = TimeSpan.FromSeconds(30);

            var serviceProvider = _services
                .AddConsul(ConsulOptionsTestHelper.CustomOptionsName, options =>
                {
                    options.Datacenter = datacenter1;
                    options.Address = address1;
                    options.Token = token1;
                    options.WaitTime = waitTime1;
                })
                .AddConsul(options =>
                {
                    options.Datacenter = datacenter2;
                    options.Address = address2;
                    options.Token = token2;
                    options.WaitTime = waitTime2;
                })
                .BuildServiceProvider();

            var consulClient1 = serviceProvider.GetRequiredService<IConsulClient>() as ConsulClient;

            var configuration1 = consulClient1.Config;
            Assert.Equal(datacenter1, configuration1.Datacenter);
            Assert.Equal(address1, configuration1.Address);
            Assert.Equal(token1, configuration1.Token);
            Assert.Equal(waitTime1, configuration1.WaitTime);

            var consulClient2 = serviceProvider
                .GetRequiredService<IConsulClientFactory>()
                .CreateClient(Options.DefaultName) as ConsulClient;

            var configuration2 = consulClient2.Config;
            Assert.Equal(datacenter2, configuration2.Datacenter);
            Assert.Equal(address2, configuration2.Address);
            Assert.Equal(token2, configuration2.Token);
            Assert.Equal(waitTime2, configuration2.WaitTime);
        }

        [Fact]
        public void AddConsulRegistration_HostedService()
        {
            _services.AddConsul()
                .AddConsulServiceRegistration(options =>
                {
                    options.ID = "id";
                    options.Name = "name";
                });

            Assert.Single(_services.Where(x => x.ServiceType == typeof(IConfigureOptions<ConsulClientConfiguration>)));
            Assert.Single(_services.Where(x => x.ServiceType == typeof(IConsulClient)));
            Assert.Single(_services.Where(x => x.ServiceType == typeof(AgentServiceRegistration)));
            var hostedService = Assert.Single(_services.Where(x => x.ServiceType == typeof(IHostedService)));
            Assert.Equal(typeof(AgentServiceRegistrationHostedService), hostedService.ImplementationType);
        }
    }
}
