using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Consul.AspNetCore.Test
{
    public static class ConsulOptionsTestHelper
    {
        public const string CustomOptionsName = "custom";

        public static OptionsMonitor<ConsulClientConfiguration> CreateOptionsMonitor(
            IEnumerable<IConfigureOptions<ConsulClientConfiguration>> configureOptions)
        {
            return new OptionsMonitor<ConsulClientConfiguration>(
                new OptionsFactory<ConsulClientConfiguration>(
                    configureOptions,
                    Array.Empty<IPostConfigureOptions<ConsulClientConfiguration>>(),
                    Array.Empty<IValidateOptions<ConsulClientConfiguration>>()),
                Array.Empty<IOptionsChangeTokenSource<ConsulClientConfiguration>>(),
                new OptionsCache<ConsulClientConfiguration>());
        }
    }
}
