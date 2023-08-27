using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Consul
{
    public class Namespace
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public NamespaceACLConfig ACLs { get; set; }
        public Dictionary<string, string> Meta { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ulong CreateIndex { get; set; }
        public ulong ModifyIndex { get; set; }

        public Namespace(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }

    public class NamespaceACLConfig
    {
        public List<AuthMethodEntry> PolicyDefaults { get; set; }
        public List<AuthMethodEntry> RoleDefaults { get; set; }
    }

    public class Namespaces : INamespacesEndpoint
    {
        private readonly ConsulClient _client;

        internal Namespaces(ConsulClient c)
        {
            _client = c;
        }

        public async Task<WriteResult<Namespace>> Create(Namespace ns, WriteOptions q, CancellationToken ct = default)
        {
            var res = await _client.Put("v1/namespace", ns, q).Execute(ct).ConfigureAwait(false);
            return new WriteResult<Namespace>(res);
        }

        public async Task<WriteResult<Namespace>> Create(Namespace ns, CancellationToken ct = default)
        {
            var res = await _client.Put("v1/namespace", ns, WriteOptions.Default).Execute(ct).ConfigureAwait(false);
            return new WriteResult<Namespace>(res);
        }

        public async Task<WriteResult<Namespace>> Update(Namespace ns, WriteOptions q, CancellationToken ct = default)
        {
            var res = await _client.Put($"v1/namespace/{ns.Name}", ns, q).Execute(ct).ConfigureAwait(false);
            return new WriteResult<Namespace>(res);
        }

        public async Task<WriteResult<Namespace>> Update(Namespace ns, CancellationToken ct = default)
        {
            var res = await _client.Put($"v1/namespace/{ns.Name}", ns, WriteOptions.Default).Execute(ct).ConfigureAwait(false);
            return new WriteResult<Namespace>(res);
        }

        public Task<QueryResult<Namespace>> Read(string name, QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<Namespace>($"v1/namespace/{name}", q).Execute(ct);
        }

        public Task<QueryResult<Namespace>> Read(string name, CancellationToken ct = default)
        {
            return _client.Get<Namespace>($"v1/namespace/{name}", QueryOptions.Default).Execute(ct);
        }

        public Task<QueryResult<Namespace[]>> List(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<Namespace[]>($"v1/namespaces", q).Execute(ct);
        }

        public Task<QueryResult<Namespace[]>> List(CancellationToken ct = default)
        {
            return _client.Get<Namespace[]>($"v1/namespaces", QueryOptions.Default).Execute(ct);
        }

        public Task<WriteResult> Delete(string name, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Delete($"v1/namespace/{name}", q).Execute(ct);
        }

        public Task<WriteResult> Delete(string name, CancellationToken ct = default)
        {
            return _client.Delete($"v1/namespace/{name}", WriteOptions.Default).Execute(ct);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Namespaces> _namespaces;

        /// <summary>
        /// Namespaces returns a handle to the namespaces endpoint
        /// </summary>
        public INamespacesEndpoint Namespaces => _namespaces.Value;
    }
}
