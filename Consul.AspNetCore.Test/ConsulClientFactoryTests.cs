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

			var options = new ConsulClientConfiguration
			{
				Datacenter = datacenter,
				Address = address,
				Token = token,
				WaitTime = waitTime
			};

			var factory = new ConsulClientFactory(
				new OptionsWrapper<ConsulClientConfiguration>(options));

			var client = factory.CreateClient() as ConsulClient;

			var configuration = client.Config;

			Assert.Same(options, configuration);
			Assert.Equal(datacenter, configuration.Datacenter);
			Assert.Equal(address, configuration.Address);
			Assert.Equal(token, configuration.Token);
			Assert.Equal(waitTime, configuration.WaitTime);
		}
	}
}