using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Consul.AspNetCore
{
	public class AgentServiceRegistrationHostedService : IHostedService
	{
		private readonly IConsulClient consulClient;
		private readonly AgentServiceRegistration registration;

		public AgentServiceRegistrationHostedService(
			IConsulClient consulClient,
			AgentServiceRegistration registration)
		{
			this.consulClient = consulClient;
			this.registration = registration;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			return consulClient.Agent.ServiceRegister(registration, cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return consulClient.Agent.ServiceDeregister(registration.ID, cancellationToken);
		}
	}
}