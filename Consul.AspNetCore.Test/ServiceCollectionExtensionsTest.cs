using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

			var descriptor = Assert.Single(_services);

			Assert.Equal(typeof(IConsulClient), descriptor.ServiceType);
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

			Assert.Collection(
				_services,
				consul => Assert.Equal(typeof(IConsulClient), consul.ServiceType),
				agent => Assert.Equal(typeof(AgentServiceRegistration), agent.ServiceType),
				hostedService =>
				{
					Assert.Equal(typeof(IHostedService), hostedService.ServiceType);
					Assert.Equal(typeof(AgentServiceRegistrationHostedService), hostedService.ImplementationType);
				});
		}
	}
}