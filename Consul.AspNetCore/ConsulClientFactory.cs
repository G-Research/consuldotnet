using Microsoft.Extensions.Options;

namespace Consul.AspNetCore
{
	public class ConsulClientFactory : IConsulClientFactory
	{
		private readonly IOptionsMonitor<ConsulClientConfiguration> _monitor;

		public ConsulClientFactory(IOptionsMonitor<ConsulClientConfiguration> monitor)
		{
			_monitor = monitor;
		}

		public IConsulClient CreateClient()
		{
			return CreateClient(Options.DefaultName);
		}

		public IConsulClient CreateClient(string name)
		{
			var options = _monitor.Get(name);

			return new ConsulClient(options);
		}
	}
}