using System;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class BaseFixture : IAsyncLifetime
    {
        protected ConsulClient _client;
        protected static SemanticVersion AgentVersion;

        private static readonly Lazy<Task> Ready = new Lazy<Task>(async () =>
        {
            var client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            });

            var timeout = TimeSpan.FromSeconds(60);
            var cancelToken = new CancellationTokenSource(timeout).Token;
            Exception exception = null;
            var firstIteration = true;
            while (true)
            {
                if (!firstIteration)
                    await Task.Delay(TimeSpan.FromSeconds(1), cancelToken);

                firstIteration = false;

                if (cancelToken.IsCancellationRequested)
                {
                    if (exception != null)
                    {
                        // rethrow exception preserving its stack trace
                        ExceptionDispatchInfo.Capture(exception).Throw();
                    }
                    else
                    {
                        throw new OperationCanceledException($"Consul server is still not responding after {timeout}");
                    }
                }

                try
                {
                    // single instance of test server is expected
                    var peers = await client.Status.Peers();
                    var leader = await client.Status.Leader();
                    if (peers.Length != 1 || peers.Single() != leader)
                        continue;

                    // test basic functionality
                    var sessionRequest = await client.Session.Create(new SessionEntry());
                    if (string.IsNullOrWhiteSpace(sessionRequest.Response))
                        continue;

                    await client.Session.Destroy(sessionRequest.Response);

                    var info = await client.Agent.Self();
                    AgentVersion = SemanticVersion.Parse(info.Response["Config"]["Version"]);

                    // Workaround for https://github.com/hashicorp/consul/issues/15061
                    await client.Agent.GetAgentMetrics();

                    if ((await client.Coordinate.Nodes()).Response.Length == 0)
                        continue;

                    break;
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
        });

        static BaseFixture()
        {
            // Some Consul object (e.g. semaphores) use multiple http connections,
            // but on .NETFramework the default limit is sometimes very low (2) so we need to bump it to higher value.
            // E.g. https://github.com/microsoft/referencesource/blob/5697c29004a34d80acdaf5742d7e699022c64ecd/System.Web/HttpRuntime.cs#L1200
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            // As for HTTP connections, we need multiple threads to test semaphores and locks.
            // XUnit sets the initial number of worker threads to the number of CPU cores.
            // Unfortunately, when the initial limit for the ThreadPool is too low, it introduces a risk of a deadlock-like behavior and the tests are timing out.
            ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
            if (workerThreads < 6)
            {
                workerThreads = 6;
                ThreadPool.SetMinThreads(workerThreads, completionPortThreads);
            }
        }

        public BaseFixture()
        {
            _client = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            });
        }

        public Task DisposeAsync()
        {
            _client.Dispose();
            return Task.CompletedTask;
        }

        /// https://github.com/hashicorp/consul/issues/819
        /// With parallel testing on the CI, sometimes the tests start before the consul server finished properly initializing.
        /// So before we let any test run, we try some basic functionality (like session creation) to assure that the test server is ready.
        public async Task InitializeAsync()
        {
            await Ready.Value;
        }
    }

    public class EnterpriseOnlyFact : SkippableFactAttribute
    {
        public EnterpriseOnlyFact()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RUN_CONSUL_ENTERPRISE_TESTS")))
            {
                Skip = "Skipped; this test requires a consul enterprise server to run.";
            }
        }
    }
}
