using System;
using System.Linq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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