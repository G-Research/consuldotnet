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
		/// Add url based <see cref="IConsulClient"/>
		/// </summary>
		/// <example>http://token@host:port/datacenter</example>
		public static IServiceCollection AddConsul(
			this IServiceCollection services,
			Uri url,
			Action<ConsulClientConfiguration> configure = null)
		{
			return services.AddConsul(Options.DefaultName, url, configure);
		}

		/// <summary>
		/// Add default <see cref="IConsulClient"/> with configured <see cref="IOptions{ConsulClientConfiguration}"/>
		/// </summary>
		public static IServiceCollection AddConsul(
			this IServiceCollection services,
			Action<ConsulClientConfiguration> configure)
		{
			return services.AddConsul(Options.DefaultName, configure);
		}

		/// <summary>
		/// Add named url based <see cref="IConsulClient"/>
		/// </summary>
		/// <example>http://token@host:port/datacenter</example>
		public static IServiceCollection AddConsul(
			this IServiceCollection services,
			string name,
			Uri url,
			Action<ConsulClientConfiguration> configure = null)
		{
			return services.AddConsul(name, options =>
			{
				options.Address = new Uri($"{url.Scheme}://{url.Authority}");
				options.Token = url.UserInfo;
				options.Datacenter = url.AbsolutePath.TrimStart('/');

				configure?.Invoke(options);
			});
		}
		
		/// <summary>
		/// Add named <see cref="IConsulClient"/> with configured <see cref="IOptions{ConsulClientConfiguration}"/>
		/// </summary>
		public static IServiceCollection AddConsul(
			this IServiceCollection services,
			string name,
			Action<ConsulClientConfiguration> configure)
		{
			services.Configure(name, configure);
			services.TryAddSingleton<IConsulClientFactory, ConsulClientFactory>();
			services.TryAddSingleton(sp => sp.GetRequiredService<IConsulClientFactory>().CreateClient(name));

            return services;
        }

		/// <summary>
		/// Register consul service with default <see cref="IConsulClient"/>
		/// </summary>
		public static IServiceCollection AddConsulServiceRegistration(
			this IServiceCollection services,
			Action<AgentServiceRegistration> configure)
		{
			var registration = new AgentServiceRegistration();

			configure.Invoke(registration);

            return services
                .AddSingleton(registration)
                .AddHostedService<AgentServiceRegistrationHostedService>();
        }
    }
}
