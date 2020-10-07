using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Consul.AspNetCore
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Add default <see cref="IConsulClient"/>
		/// </summary>
		public static IServiceCollection AddConsul(this IServiceCollection services)
		{
			return services.AddConsul(options => {});
		}

		/// <summary>
		/// Add default <see cref="IConsulClient"/> with configured <see cref="IOptions{ConsulClientConfiguration}"/>
		/// </summary>
		public static IServiceCollection AddConsul(
			this IServiceCollection services,
			Action<ConsulClientConfiguration> options)
		{
			return services.AddConsul(Options.DefaultName, options);
		}

		/// <summary>
		/// Add named <see cref="IConsulClient"/> with configured <see cref="IOptions{ConsulClientConfiguration}"/>
		/// </summary>
		public static IServiceCollection AddConsul(
			this IServiceCollection services,
			string name,
			Action<ConsulClientConfiguration> options)
		{
			services.Configure(name, options);
			services.TryAddSingleton<IConsulClientFactory, ConsulClientFactory>();
			services.TryAddSingleton(sp => sp.GetRequiredService<IConsulClientFactory>().CreateClient(name));

			return services;
		}

		/// <summary>
		/// Register consul service with default <see cref="IConsulClient"/>
		/// </summary>
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