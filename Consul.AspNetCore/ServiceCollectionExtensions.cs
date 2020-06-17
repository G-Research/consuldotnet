using System;
using Microsoft.Extensions.DependencyInjection;

namespace Consul.AspNetCore
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddConsul(
			this IServiceCollection services,
			Action<ConsulClientConfiguration> options)
		{
			return services.AddSingleton<IConsulClient>(sp => new ConsulClient(options));
		}
	}
}