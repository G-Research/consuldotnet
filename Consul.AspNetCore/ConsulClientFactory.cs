using Microsoft.Extensions.Options;

namespace Consul.AspNetCore
{
	public class ConsulClientFactory : IConsulClientFactory
	{
		private readonly ConsulClientConfiguration _options;

		public ConsulClientFactory(IOptions<ConsulClientConfiguration> options)
		{
			_options = options.Value;
		}
		
		public IConsulClient CreateClient()
		{
			return new ConsulClient(_options);
		}
	}
}