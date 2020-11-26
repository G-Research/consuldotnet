namespace Consul.AspNetCore
{
    public interface IConsulClientFactory
    {
        IConsulClient CreateClient();
        IConsulClient CreateClient(string name);
    }
}
