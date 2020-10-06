using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Consul.AspNetCore
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddConsul(this IServiceCollection services)
		{
			return services.AddConsul(options => {});
		}

		public static IServiceCollection AddConsul(
			this IServiceCollection services,
			Action<ConsulClientConfiguration> options)
		{
			services.Configure(options)
				.AddSingleton<IConsulClientFactory, ConsulClientFactory>()
				.TryAddSingleton(sp => sp.GetRequiredService<IConsulClientFactory>().CreateClient());

			return services;
		}

		public static IServiceCollection AddConsulServiceRegistration(
			this IServiceCollection services,
			Action<AgentServiceRegistration> options)
		{
			var registration = new AgentServiceRegistration();

			options.Invoke(registration);

			return services
				.AddSingleton(registration)
				.AddHostedService<AgentServiceRegistrationHostedService>();
		}
	}
}