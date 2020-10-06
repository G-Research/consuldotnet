namespace Consul.AspNetCore
{
	public interface IConsulClientFactory
	{
		IConsulClient CreateClient();
	}
}