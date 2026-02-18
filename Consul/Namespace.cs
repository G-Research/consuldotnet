
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consul
{
    public class Namespace
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NamespaceACLConfig ACLs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Meta { get; set; }
    }

    public class NamespaceResponse : Namespace
    {
        public ulong CreateIndex { get; set; }
        public ulong ModifyIndex { get; set; }
        public DateTime? DeletedAt { get; set; }
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

        public async Task<WriteResult<NamespaceResponse>> Create(Namespace ns, WriteOptions q, CancellationToken ct = default)
        {
            var res = await _client.Put<Namespace, NamespaceResponse>("v1/namespace", ns, q).Execute(ct).ConfigureAwait(false);
            return new WriteResult<NamespaceResponse>(res, res.Response);
        }

        public async Task<WriteResult<NamespaceResponse>> Create(Namespace ns, CancellationToken ct)
        {
            return await Create(ns, WriteOptions.Default, ct).ConfigureAwait(false);
        }

        public async Task<WriteResult<NamespaceResponse>> Create(Namespace ns)
        {
            return await Create(ns, WriteOptions.Default, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<WriteResult<NamespaceResponse>> Update(Namespace ns, WriteOptions q, CancellationToken ct = default)
        {
            var res = await _client.Put<Namespace, NamespaceResponse>($"v1/namespace/{ns.Name}", ns, q).Execute(ct).ConfigureAwait(false);
            return new WriteResult<NamespaceResponse>(res, res.Response);
        }

        public async Task<WriteResult<NamespaceResponse>> Update(Namespace ns, CancellationToken ct)
        {
            return await Update(ns, WriteOptions.Default, ct).ConfigureAwait(false);
        }

        public async Task<WriteResult<NamespaceResponse>> Update(Namespace ns)
        {
            return await Update(ns, WriteOptions.Default, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<QueryResult<NamespaceResponse>> Read(string name, QueryOptions q, CancellationToken ct = default)
        {
            return await _client.Get<NamespaceResponse>($"v1/namespace/{name}", q).Execute(ct).ConfigureAwait(false);
        }

        public async Task<QueryResult<NamespaceResponse>> Read(string name, CancellationToken ct)
        {
            return await Read(name, QueryOptions.Default, ct).ConfigureAwait(false);
        }

        public async Task<QueryResult<NamespaceResponse>> Read(string name)
        {
            return await Read(name, QueryOptions.Default, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<QueryResult<NamespaceResponse[]>> List(QueryOptions q, CancellationToken ct = default)
        {
            return await _client.Get<NamespaceResponse[]>($"v1/namespaces", q).Execute(ct).ConfigureAwait(false);
        }

        public async Task<QueryResult<NamespaceResponse[]>> List(CancellationToken ct)
        {
            return await List(QueryOptions.Default, ct).ConfigureAwait(false);
        }

        public async Task<QueryResult<NamespaceResponse[]>> List()
        {
            return await List(QueryOptions.Default, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<WriteResult> Delete(string name, WriteOptions q, CancellationToken ct = default)
        {
            return await _client.Delete($"v1/namespace/{name}", q).Execute(ct).ConfigureAwait(false);
        }

        public async Task<WriteResult> Delete(string name, CancellationToken ct)
        {
            return await Delete(name, WriteOptions.Default, ct).ConfigureAwait(false);
        }

        public async Task<WriteResult> Delete(string name)
        {
            return await Delete(name, WriteOptions.Default, CancellationToken.None).ConfigureAwait(false);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        /// <summary>
        /// Namespaces returns a handle to the namespaces endpoint
        /// </summary>
        public INamespacesEndpoint Namespaces { get; private set; }
    }
}
