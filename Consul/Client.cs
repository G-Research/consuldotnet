// -----------------------------------------------------------------------
//  <copyright file="Client.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
//    Copyright 2020 G-Research Limited
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Consul.Filtering;

namespace Consul
{

    /// <summary>
    /// Represents the configuration options for a Consul client.
    /// </summary>
    public class ConsulClientConfiguration
    {
        private NetworkCredential _httpauth;
        private bool _disableServerCertificateValidation;
        private X509Certificate2 _clientCertificate;

        internal event EventHandler _updated;

        internal static Lazy<bool> ClientCertSupport = new Lazy<bool>(() => Type.GetType("Mono.Runtime") == null);

        internal static bool ClientCertificateSupported => ClientCertSupport.Value;

#if NETSTANDARD || NETCOREAPP
        [Obsolete("Use of DisableServerCertificateValidation should be converted to setting the HttpHandler's ServerCertificateCustomValidationCallback in the ConsulClient constructor" +
            "This property will be removed in a future release.", false)]
#else
        [Obsolete("Use of DisableServerCertificateValidation should be converted to setting the WebRequestHandler's ServerCertificateValidationCallback in the ConsulClient constructor" +
            "This property will be removed in a future release.", false)]
#endif
        internal bool DisableServerCertificateValidation
        {
            get
            {
                return _disableServerCertificateValidation;
            }
            set
            {
                _disableServerCertificateValidation = value;
                OnUpdated(new EventArgs());
            }
        }

        /// <summary>
        /// The Uri to connect to the Consul agent.
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// Namespace is the name of the namespace to send along for the request
        /// when no other Namespace is present in the QueryOptions
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Datacenter to provide with each request. If not provided, the default agent datacenter is used.
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        /// Credentials to use for access to the HTTP API.
        /// This is only needed if an authenticating service exists in front of Consul; Token is used for ACL authentication by Consul.
        /// </summary>
#if NETSTANDARD || NETCOREAPP
        [Obsolete("Use of HttpAuth should be converted to setting the HttpHandler's Credential property in the ConsulClient constructor" +
            "This property will be removed in a future release.", false)]
#else
        [Obsolete("Use of HttpAuth should be converted to setting the WebRequestHandler's Credential property in the ConsulClient constructor" +
            "This property will be removed in a future release.", false)]
#endif
        public NetworkCredential HttpAuth
        {
            internal get => _httpauth;
            set
            {
                _httpauth = value;
                OnUpdated(new EventArgs());
            }
        }

        /// <summary>
        /// TLS Client Certificate used to secure a connection to a Consul agent. Not supported on Mono.
        /// This is only needed if an authenticating service exists in front of Consul; Token is used for ACL authentication by Consul. This is not the same as RPC Encryption with TLS certificates.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">Setting this property will throw a PlatformNotSupportedException on Mono</exception>
#if __MonoCS__
        [Obsolete("Client Certificates are not implemented in Mono", true)]
#elif NETSTANDARD || NETCOREAPP
        [Obsolete("Use of ClientCertificate should be converted to adding to the HttpHandler's ClientCertificates list in the ConsulClient constructor." +
            "This property will be removed in a future release.", false)]
#else
        [Obsolete("Use of ClientCertificate should be converted to adding to the WebRequestHandler's ClientCertificates list in the ConsulClient constructor." +
            "This property will be removed in a future release.", false)]
#endif
        public X509Certificate2 ClientCertificate
        {
            internal get => _clientCertificate;
            set
            {
                if (!ClientCertificateSupported)
                {
                    throw new PlatformNotSupportedException("Client certificates are not supported on this platform");
                }
                _clientCertificate = value;
                OnUpdated(new EventArgs());
            }
        }

        /// <summary>
        /// Token is used to provide an ACL token which overrides the agent's default token. This ACL token is used for every request by
        /// clients created using this configuration.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// WaitTime limits how long a Watch will block. If not provided, the agent default values will be used.
        /// </summary>
        public TimeSpan? WaitTime { get; set; }

        /// <summary>
        /// Creates a new instance of a Consul client configuration.
        /// </summary>
        /// <exception cref="ConsulConfigurationException">An error that occured while building the configuration.</exception>
        public ConsulClientConfiguration()
        {
            UriBuilder consulAddress = new UriBuilder("http://127.0.0.1:8500");
            ConfigureFromEnvironment(consulAddress);
            Address = consulAddress.Uri;

            string ns = Environment.GetEnvironmentVariable("CONSUL_NAMESPACE");
            if (!string.IsNullOrEmpty(ns))
            {
                Namespace = ns;
            }
        }

        /// <summary>
        /// Builds configuration based on environment variables.
        /// </summary>
        /// <exception cref="ConsulConfigurationException">An environment variable could not be parsed</exception>
        private void ConfigureFromEnvironment(UriBuilder consulAddress)
        {
            var envAddr = (Environment.GetEnvironmentVariable("CONSUL_HTTP_ADDR") ?? string.Empty).Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(envAddr))
            {
                if (!Uri.TryCreate(envAddr, UriKind.Absolute, out Uri uri))
                {
                    // If the URI cannot be parsed it probably lacks the schema, use http as a default
                    uri = new Uri($"http://{envAddr}");
                }

                if (!string.IsNullOrEmpty(uri.Host))
                {
                    consulAddress.Host = uri.Host;
                }

                consulAddress.Port = uri.Port;
                consulAddress.Path = uri.AbsolutePath;
            }

            var useSsl = (Environment.GetEnvironmentVariable("CONSUL_HTTP_SSL") ?? string.Empty).Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(useSsl))
            {
                try
                {
                    if (useSsl == "1" || bool.Parse(useSsl))
                    {
                        consulAddress.Scheme = "https";
                    }
                }
                catch (Exception ex)
                {
                    throw new ConsulConfigurationException("Could not parse environment variable CONSUL_HTTP_SSL", ex);
                }
            }

            var verifySsl = (Environment.GetEnvironmentVariable("CONSUL_HTTP_SSL_VERIFY") ?? string.Empty).Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(verifySsl))
            {
                try
                {
                    if (verifySsl == "0" || bool.Parse(verifySsl))
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        DisableServerCertificateValidation = true;
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
                catch (Exception ex)
                {
                    throw new ConsulConfigurationException("Could not parse environment variable CONSUL_HTTP_SSL_VERIFY", ex);
                }
            }

            var auth = Environment.GetEnvironmentVariable("CONSUL_HTTP_AUTH");
            if (!string.IsNullOrEmpty(auth))
            {
                var credential = new NetworkCredential();
                if (auth.Contains(":"))
                {
                    var split = auth.Split(':');
                    credential.UserName = split[0];
                    credential.Password = split[1];
                }
                else
                {
                    credential.UserName = auth;
                }
#pragma warning disable CS0618 // Type or member is obsolete
                HttpAuth = credential;
#pragma warning restore CS0618 // Type or member is obsolete
            }

            string token = Environment.GetEnvironmentVariable("CONSUL_HTTP_TOKEN");
            if (!string.IsNullOrEmpty(token))
            {
                Token = token;
            }
        }

        internal virtual void OnUpdated(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler handler = _updated;

            // Event will be null if there are no subscribers
            handler?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Represents a persistant connection to a Consul agent. Instances of this class should be created rarely and reused often.
    /// </summary>
    public partial class ConsulClient : IDisposable
    {
        /// <summary>
        /// This class is used to group all the configurable bits of a ConsulClient into a single pointer reference
        /// which is great for implementing reconfiguration later.
        /// </summary>
        private class ConsulClientConfigurationContainer
        {
            internal readonly bool _skipClientDispose;
            internal readonly HttpClient _httpClient;
#if NETSTANDARD || NETCOREAPP
            internal readonly HttpClientHandler _httpHandler;
#else
            internal readonly WebRequestHandler _httpHandler;
#endif
            public readonly ConsulClientConfiguration Config;

            public ConsulClientConfigurationContainer()
            {
                Config = new ConsulClientConfiguration();
#if NETSTANDARD || NETCOREAPP
                _httpHandler = new HttpClientHandler();
#else
                _httpHandler = new WebRequestHandler();
#endif
                _httpClient = new HttpClient(_httpHandler);
                _httpClient.Timeout = TimeSpan.FromMinutes(15);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            }

            #region Old style config handling
            public ConsulClientConfigurationContainer(ConsulClientConfiguration config, HttpClient client)
            {
                _skipClientDispose = true;
                Config = config;
                _httpClient = client;
            }

            public ConsulClientConfigurationContainer(ConsulClientConfiguration config)
            {
                Config = config;
#if NETSTANDARD || NETCOREAPP
                _httpHandler = new HttpClientHandler();
#else
                _httpHandler = new WebRequestHandler();
#endif
                _httpClient = new HttpClient(_httpHandler)
                {
                    Timeout = TimeSpan.FromMinutes(15)
                };

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            }
            #endregion

            #region IDisposable Support
            private bool _disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        if (!_skipClientDispose)
                        {
                            _httpClient?.Dispose();
                        }

                        _httpHandler?.Dispose();
                    }

                    _disposedValue = true;
                }
            }

            //~ConsulClient()
            //{
            //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //    Dispose(false);
            //}

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void CheckDisposed()
            {
                if (_disposedValue)
                {
                    throw new ObjectDisposedException(typeof(ConsulClientConfigurationContainer).FullName.ToString());
                }
            }
            #endregion
        }

        private ConsulClientConfigurationContainer _configContainer;

        internal HttpClient HttpClient { get { return _configContainer._httpClient; } }
#if NETSTANDARD || NETCOREAPP
        internal HttpClientHandler HttpHandler { get { return _configContainer._httpHandler; } }
#else
        internal WebRequestHandler HttpHandler { get { return _configContainer._httpHandler; } }
#endif
        public ConsulClientConfiguration Config { get { return _configContainer.Config; } }


        #region New style config with Actions
        /// <summary>
        /// Initializes a new Consul client with a default configuration that connects to 127.0.0.1:8500.
        /// </summary>
        public ConsulClient() : this(null, null, null) { }

        /// <summary>
        /// Initializes a new Consul client with the ability to set portions of the configuration.
        /// </summary>
        /// <param name="configOverride">The Action to modify the default configuration with</param>
        public ConsulClient(Action<ConsulClientConfiguration> configOverride) : this(configOverride, null, null) { }

        /// <summary>
        /// Initializes a new Consul client with the ability to set portions of the configuration and access the underlying HttpClient for modification.
        /// The HttpClient is modified to set options like the request timeout and headers.
        /// The Timeout property also applies to all long-poll requests and should be set to a value that will encompass all successful requests.
        /// </summary>
        /// <param name="configOverride">The Action to modify the default configuration with</param>
        /// <param name="clientOverride">The Action to modify the HttpClient with</param>
        public ConsulClient(Action<ConsulClientConfiguration> configOverride, Action<HttpClient> clientOverride) : this(configOverride, clientOverride, null) { }

        /// <summary>
        /// Initializes a new Consul client with the ability to set portions of the configuration and access the underlying HttpClient and WebRequestHandler for modification.
        /// The HttpClient is modified to set options like the request timeout and headers.
        /// The WebRequestHandler is modified to set options like Proxy and Credentials.
        /// The Timeout property also applies to all long-poll requests and should be set to a value that will encompass all successful requests.
        /// </summary>
        /// <param name="configOverride">The Action to modify the default configuration with</param>
        /// <param name="clientOverride">The Action to modify the HttpClient with</param>
        /// <param name="handlerOverride">The Action to modify the WebRequestHandler with</param>
#if !(NETSTANDARD || NETCOREAPP)
        public ConsulClient(Action<ConsulClientConfiguration> configOverride, Action<HttpClient> clientOverride, Action<WebRequestHandler> handlerOverride)
#else
        public ConsulClient(Action<ConsulClientConfiguration> configOverride, Action<HttpClient> clientOverride, Action<HttpClientHandler> handlerOverride)
#endif
        {
            var ctr = new ConsulClientConfigurationContainer();

            configOverride?.Invoke(ctr.Config);
            ApplyConfig(ctr.Config, ctr._httpHandler, ctr._httpClient);
            handlerOverride?.Invoke(ctr._httpHandler);
            clientOverride?.Invoke(ctr._httpClient);

            _configContainer = ctr;

            InitializeEndpoints();
        }
        #endregion

        #region Old style config
        /// <summary>
        /// Initializes a new Consul client with the configuration specified.
        /// </summary>
        /// <param name="config">A Consul client configuration</param>
        public ConsulClient(ConsulClientConfiguration config)
        {
            config._updated += HandleConfigUpdateEvent;
            var ctr = new ConsulClientConfigurationContainer(config);
            ApplyConfig(ctr.Config, ctr._httpHandler, ctr._httpClient);

            _configContainer = ctr;
            InitializeEndpoints();
        }

        /// <summary>
        /// Initializes a new Consul client with the configuration specified and a custom HttpClient, which is useful for setting proxies/custom timeouts.
        /// The HttpClient must accept the "application/json" content type and the Timeout property should be set to at least 15 minutes to allow for blocking queries.
        /// </summary>
        /// <param name="config">A Consul client configuration</param>
        /// <param name="client">A custom HttpClient. The lifetime, including disposal, of this HttpClient is not handled by ConsulClient</param>
        public ConsulClient(ConsulClientConfiguration config, HttpClient client)
        {
            var ctr = new ConsulClientConfigurationContainer(config, client);
            if (!ctr._httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")))
            {
                throw new ArgumentException("HttpClient must accept the application/json content type", nameof(client));
            }
            _configContainer = ctr;
            InitializeEndpoints();
        }
        #endregion

        private void InitializeEndpoints()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            _acl = new Lazy<ACL>(() => new ACL(this));
#pragma warning restore CS0618 // Type or member is obsolete
            _agent = new Lazy<Agent>(() => new Agent(this));
            _catalog = new Lazy<Catalog>(() => new Catalog(this));
            _coordinate = new Lazy<Coordinate>(() => new Coordinate(this));
            _configuration = new Lazy<Configuration>(() => new Configuration(this));
            _event = new Lazy<Event>(() => new Event(this));
            _health = new Lazy<Health>(() => new Health(this));
            _kv = new Lazy<KV>(() => new KV(this));
            _operator = new Lazy<Operator>(() => new Operator(this));
            _policy = new Lazy<Policy>(() => new Policy(this));
            _preparedquery = new Lazy<PreparedQuery>(() => new PreparedQuery(this));
            _raw = new Lazy<Raw>(() => new Raw(this));
            _role = new Lazy<Role>(() => new Role(this));
            _session = new Lazy<Session>(() => new Session(this));
            _snapshot = new Lazy<Snapshot>(() => new Snapshot(this));
            _status = new Lazy<Status>(() => new Status(this));
            _token = new Lazy<Token>(() => new Token(this));
            _aclReplication = new Lazy<ACLReplication>(() => new ACLReplication(this));
            _authMethod = new Lazy<AuthMethod>(() => new AuthMethod(this));
            _namespaces = new Lazy<Namespaces>(() => new Namespaces(this));
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Config._updated -= HandleConfigUpdateEvent;
                    if (_configContainer != null)
                    {
                        _configContainer.Dispose();
                    }
                }

                _disposedValue = true;
            }
        }

        //~ConsulClient()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(false);
        //}

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void CheckDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(typeof(ConsulClient).FullName.ToString());
            }
        }
        #endregion

        void HandleConfigUpdateEvent(object sender, EventArgs e)
        {
            ApplyConfig(sender as ConsulClientConfiguration, HttpHandler, HttpClient);

        }
#if !(NETSTANDARD || NETCOREAPP)
        void ApplyConfig(ConsulClientConfiguration config, WebRequestHandler handler, HttpClient client)
#else
        void ApplyConfig(ConsulClientConfiguration config, HttpClientHandler handler, HttpClient client)
#endif
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (config.HttpAuth != null)
#pragma warning restore CS0618 // Type or member is obsolete
            {
#pragma warning disable CS0618 // Type or member is obsolete
                handler.Credentials = config.HttpAuth;
#pragma warning restore CS0618 // Type or member is obsolete
            }
#if !__MonoCS__
            if (ConsulClientConfiguration.ClientCertificateSupported)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (config.ClientCertificate != null)
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
#pragma warning disable CS0618 // Type or member is obsolete
                    handler.ClientCertificates.Add(config.ClientCertificate);
#pragma warning restore CS0618 // Type or member is obsolete
                }
                else
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ClientCertificates.Clear();
                }
            }
#endif
#if !(NETSTANDARD || NETCOREAPP)
#pragma warning disable CS0618 // Type or member is obsolete
            if (config.DisableServerCertificateValidation)
#pragma warning restore CS0618 // Type or member is obsolete
            {
#pragma warning disable CA5359 // The ServerCertificateValidationCallback is set to a function that accepts any server certificate...
                handler.ServerCertificateValidationCallback += (certSender, cert, chain, sslPolicyErrors) => { return true; };
#pragma warning restore CA5359
            }
            else
            {
                handler.ServerCertificateValidationCallback = null;
            }
#else
#pragma warning disable CS0618 // Type or member is obsolete
            if (config.DisableServerCertificateValidation)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                handler.ServerCertificateCustomValidationCallback += (certSender, cert, chain, sslPolicyErrors) => { return true; };
            }
            else
            {
                handler.ServerCertificateCustomValidationCallback = null;
            }
#endif

            if (!string.IsNullOrEmpty(config.Token))
            {
                if (client.DefaultRequestHeaders.Contains("X-Consul-Token"))
                {
                    client.DefaultRequestHeaders.Remove("X-Consul-Token");
                }
                client.DefaultRequestHeaders.Add("X-Consul-Token", config.Token);
            }
        }

        internal GetRequest<TOut> Get<TOut>(string path, QueryOptions opts = null, IEncodable filter = null)
        {
            return new GetRequest<TOut>(this, path, opts, filter);
        }

        internal GetRequest Get(string path, QueryOptions opts = null)
        {
            return new GetRequest(this, path, opts);
        }

        internal DeleteReturnRequest<TOut> DeleteReturning<TOut>(string path, WriteOptions opts = null)
        {
            return new DeleteReturnRequest<TOut>(this, path, opts);
        }

        internal DeleteRequest Delete(string path, WriteOptions opts = null)
        {
            return new DeleteRequest(this, path, opts);
        }

        internal DeleteAcceptingRequest<TIn> DeleteAccepting<TIn>(string path, TIn body, WriteOptions opts)
        {
            return new DeleteAcceptingRequest<TIn>(this, path, body, opts);
        }

        internal PutReturningRequest<TOut> PutReturning<TOut>(string path, WriteOptions opts = null)
        {
            return new PutReturningRequest<TOut>(this, path, opts);
        }

        internal PutRequest<TIn> Put<TIn>(string path, TIn body, WriteOptions opts)
        {
            return new PutRequest<TIn>(this, path, body, opts);
        }

        internal PutNothingRequest PutNothing(string path, WriteOptions opts = null)
        {
            return new PutNothingRequest(this, path, opts);
        }

        internal PutRequest<TIn, TOut> Put<TIn, TOut>(string path, TIn body, WriteOptions opts)
        {
            return new PutRequest<TIn, TOut>(this, path, body, opts);
        }

        internal PostReturningRequest<TOut> PostReturning<TOut>(string path, WriteOptions opts = null)
        {
            return new PostReturningRequest<TOut>(this, path, opts);
        }

        internal PostRequest<TIn> Post<TIn>(string path, TIn body, WriteOptions opts)
        {
            return new PostRequest<TIn>(this, path, body, opts);
        }

        internal PostRequest<TIn, TOut> Post<TIn, TOut>(string path, TIn body, WriteOptions opts)
        {
            return new PostRequest<TIn, TOut>(this, path, body, opts);
        }

        internal PostRequest Post(string path, string body, WriteOptions opts = null)
        {
            return new PostRequest(this, path, body, opts);
        }
    }
}
