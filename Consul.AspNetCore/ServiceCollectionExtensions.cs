using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Consul.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add default <see cref="IConsulClient"/>.
        /// First client can be accessed from DI, to create multiple named clients use <see cref="IConsulClientFactory"/>
        /// </summary>
        public static IServiceCollection AddConsul(this IServiceCollection services)
        {
            return services.AddConsul(options => { });
        }

        /// <summary>
        /// Add default <see cref="IConsulClient"/> with configured <see cref="IOptions{ConsulClientConfiguration}"/>.
        /// First client can be accessed from DI, to create multiple named clients use <see cref="IConsulClientFactory"/>
        /// </summary>
        public static IServiceCollection AddConsul(
            this IServiceCollection services,
            Action<ConsulClientConfiguration> configure)
        {
            return services.AddConsul(Options.DefaultName, configure);
        }

        /// <summary>
        /// Add named <see cref="IConsulClient"/> with configured <see cref="IOptions{ConsulClientConfiguration}"/>.
        /// First client can be accessed from DI, to create multiple named clients use <see cref="IConsulClientFactory"/>
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
        /// Register consul service with default <see cref="IConsulClient"/>.
        /// First client can be accessed from DI, to create multiple named clients use <see cref="IConsulClientFactory"/>
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
