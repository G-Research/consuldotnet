// -----------------------------------------------------------------------
//  <copyright file="ACL.cs" company="PlayFab Inc">
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
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consul
{
    /// <summary>
    /// [Deprecated] The type of ACL token, which sets the permissions ceiling
    /// </summary>
    [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
    public class ACLType : IEquatable<ACLType>
    {
        public string Type { get; private set; }

        /// <summary>
        /// [Deprecated] Token type which cannot modify ACL rules
        /// </summary>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public static ACLType Client
        {
            get { return new ACLType() { Type = "client" }; }
        }

        /// <summary>
        /// [Deprecated] Token type which is allowed to perform all actions
        /// </summary>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public static ACLType Management
        {
            get { return new ACLType() { Type = "management" }; }
        }

        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public bool Equals(ACLType other)
        {
            if (other == null)
            {
                return false;
            }
            return Type.Equals(other.Type);
        }

#pragma warning disable CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public override bool Equals(object other)
#pragma warning restore CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        {
            return other != null &&
                   GetType() == other.GetType() &&
                   Equals((ACLType)other);
        }

#pragma warning disable CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public override int GetHashCode()
#pragma warning restore CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        {
            return Type.GetHashCode();
        }
    }

    [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
    public class ACLTypeConverter : JsonConverter
    {
#pragma warning disable CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
#pragma warning restore CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        {
            serializer.Serialize(writer, ((ACLType)value).Type);
        }

#pragma warning disable CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
#pragma warning restore CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        {
            var type = (string)serializer.Deserialize(reader, typeof(string));
            switch (type)
            {
                case "client":
                    return ACLType.Client;
                case "management":
                    return ACLType.Management;
                default:
                    throw new ArgumentOutOfRangeException("serializer", type,
                        "Unknown ACL token type value found during deserialization");
            }
        }

#pragma warning disable CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public override bool CanConvert(Type objectType)
#pragma warning restore CS0809 // Obsolete member 'ACLType.Equals(object)' overrides non-obsolete member
        {
            if (objectType == typeof(ACLType))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// [Deprecated] ACLEntry is used to represent an ACL entry (Legacy Token)
    /// </summary>
    [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
    public class ACLEntry
    {
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public ulong CreateIndex { get; set; }
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public ulong ModifyIndex { get; set; }
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public string ID { get; set; }
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public string Name { get; set; }

        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        [JsonConverter(typeof(ACLTypeConverter))]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ACLType Type { get; set; }

        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public string Rules { get; set; }

        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public bool ShouldSerializeCreateIndex()
        {
            return false;
        }

        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public bool ShouldSerializeModifyIndex()
        {
            return false;
        }

        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public ACLEntry()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public ACLEntry(string name, string rules)
            : this(string.Empty, name, rules)
        {
        }

        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public ACLEntry(string id, string name, string rules)
        {
            Type = ACLType.Client;
            ID = id;
            Name = name;
            Rules = rules;
        }
    }

    /// <summary>
    /// [Deprecated] ACL can be used to query the ACL endpoints
    /// </summary>
    [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
    public class ACL : IACLEndpoint
    {
        private readonly ConsulClient _client;

        internal ACL(ConsulClient c)
        {
            _client = c;
        }

        private class ACLCreationResult
        {
            [JsonProperty]
            internal string ID { get; set; }
        }

        /// <summary>
        /// [Deprecated] Create is used to generate a new token with the given parameters
        /// </summary>
        /// <param name="acl">The ACL entry to create</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A write result containing the newly created ACL token</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<WriteResult<string>> Create(ACLEntry acl, CancellationToken ct = default)
        {
            return Create(acl, WriteOptions.Default, ct);
        }

        /// <summary>
        /// [Deprecated] Create is used to generate a new token with the given parameters
        /// </summary>
        /// <param name="acl">The ACL entry to create</param>
        /// <param name="q">Customized write options</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A write result containing the newly created ACL token</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public async Task<WriteResult<string>> Create(ACLEntry acl, WriteOptions q, CancellationToken ct = default)
        {
            var res = await _client.Put<ACLEntry, ACLCreationResult>("/v1/acl/create", acl, q).Execute(ct).ConfigureAwait(false);
            return new WriteResult<string>(res, res.Response.ID);
        }

        /// <summary>
        /// [Deprecated] Update is used to update the rules of an existing token
        /// </summary>
        /// <param name="acl">The ACL entry to update</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<WriteResult> Update(ACLEntry acl, CancellationToken ct = default)
        {
            return Update(acl, WriteOptions.Default, ct);
        }

        /// <summary>
        /// [Deprecated] Update is used to update the rules of an existing token
        /// </summary>
        /// <param name="acl">The ACL entry to update</param>
        /// <param name="q">Customized write options</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<WriteResult> Update(ACLEntry acl, WriteOptions q, CancellationToken ct = default)
        {
            return _client.Put("/v1/acl/update", acl, q).Execute(ct);
        }

        /// <summary>
        /// [Deprecated] Destroy is used to destroy a given ACL token ID
        /// </summary>
        /// <param name="id">The ACL ID to destroy</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<WriteResult<bool>> Destroy(string id, CancellationToken ct = default)
        {
            return Destroy(id, WriteOptions.Default, ct);
        }

        /// <summary>
        /// [Deprecated] Destroy is used to destroy a given ACL token ID
        /// </summary>
        /// <param name="id">The ACL ID to destroy</param>
        /// <param name="q">Customized write options</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>An empty write result</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<WriteResult<bool>> Destroy(string id, WriteOptions q, CancellationToken ct = default)
        {
            return _client.PutReturning<bool>(string.Format("/v1/acl/destroy/{0}", id), q).Execute(ct);
        }

        /// <summary>
        /// [Deprecated] Clone is used to return a new token cloned from an existing one
        /// </summary>
        /// <param name="id">The ACL ID to clone</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A write result containing the newly created ACL token</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<WriteResult<string>> Clone(string id, CancellationToken ct = default)
        {
            return Clone(id, WriteOptions.Default, ct);
        }

        /// <summary>
        /// [Deprecated] Clone is used to return a new token cloned from an existing one
        /// </summary>
        /// <param name="id">The ACL ID to clone</param>
        /// <param name="q">Customized write options</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A write result containing the newly created ACL token</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public async Task<WriteResult<string>> Clone(string id, WriteOptions q, CancellationToken ct = default)
        {
            var res = await _client.PutReturning<ACLCreationResult>(string.Format("/v1/acl/clone/{0}", id), q).Execute(ct).ConfigureAwait(false);
            return new WriteResult<string>(res, res.Response.ID);
        }

        /// <summary>
        /// [Deprecated] Info is used to query for information about an ACL token
        /// </summary>
        /// <param name="id">The ACL ID to request information about</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A query result containing the ACL entry matching the provided ID, or a query result with a null response if no token matched the provided ID</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<QueryResult<ACLEntry>> Info(string id, CancellationToken ct = default)
        {
            return Info(id, QueryOptions.Default, ct);
        }

        /// <summary>
        /// [Deprecated] Info is used to query for information about an ACL token
        /// </summary>
        /// <param name="id">The ACL ID to request information about</param>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A query result containing the ACL entry matching the provided ID, or a query result with a null response if no token matched the provided ID</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public async Task<QueryResult<ACLEntry>> Info(string id, QueryOptions q, CancellationToken ct = default)
        {
            var res = await _client.Get<ACLEntry[]>(string.Format("/v1/acl/info/{0}", id), q).Execute(ct).ConfigureAwait(false);
            return new QueryResult<ACLEntry>(res, res.Response != null && res.Response.Length > 0 ? res.Response[0] : null);
        }

        /// <summary>
        /// [Deprecated] List is used to get all the ACL tokens
        /// </summary>
        /// <returns>A write result containing the list of all ACLs</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<QueryResult<ACLEntry[]>> List(CancellationToken ct = default)
        {
            return List(QueryOptions.Default, ct);
        }

        /// <summary>
        /// [Deprecated] List is used to get all the ACL tokens
        /// </summary>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A write result containing the list of all ACLs</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<QueryResult<ACLEntry[]>> List(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<ACLEntry[]>("/v1/acl/list", q).Execute(ct);
        }

        /// <summary>
        /// [Deprecated] TranslateRules will translate legacy rule syntax to latest syntax
        /// </summary>
        /// <param name="rules">The legacy rule(s) to translate</param>
        /// <param name="ct">>Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A string containing the translated rule(s)</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<WriteResult<string>> TranslateRules(string rules, CancellationToken ct = default)
        {
            return TranslateRules(rules, WriteOptions.Default, ct);
        }

        /// <summary>
        /// [Deprecated] TranslateRules will translate legacy rule syntax to latest syntax
        /// </summary>
        /// <param name="rules">The legacy rule(s) to translate</param>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">>Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A string containing the translated rule(s)</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public async Task<WriteResult<string>> TranslateRules(string rules, WriteOptions q, CancellationToken ct = default)
        {

            var res = await _client.Post($"/v1/acl/rules/translate", rules, q).Execute(ct).ConfigureAwait(false);
            return new WriteResult<string>(res, res.Response);
        }

        /// <summary>
        /// [Deprecated] TranslateLegacyTokenRules will translate legacy rule syntax on a legacy token in to the latest syntax
        /// </summary>
        /// <param name="id">The legacy token ID whos rule(s) need translated</param>
        /// <param name="ct">>Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A string containing the translated rule(s)</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public Task<QueryResult<string>> TranslateLegacyTokenRules(string id, CancellationToken ct = default)
        {
            return TranslateLegacyTokenRules(id, QueryOptions.Default, ct);
        }

        /// <summary>
        /// [Deprecated] TranslateLegacyTokenRules will translate legacy rule syntax on a legacy token in to the latest syntax
        /// </summary>
        /// <param name="id">The legacy token ID whos rule(s) need translated</param>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">>Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the request is cancelled before completing</param>
        /// <returns>A string containing the translated rule(s)</returns>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public async Task<QueryResult<string>> TranslateLegacyTokenRules(string id, QueryOptions q, CancellationToken ct = default)
        {
            var res = await _client.Get($"/v1/acl/rules/translate/{id}", q).Execute(ct).ConfigureAwait(false);
            return new QueryResult<string>(res, res.Response);
        }
    }

    public partial class ConsulClient : IConsulClient
    {
#pragma warning disable CS0618 // Type or member is obsolete
        private Lazy<ACL> _acl;
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>
        /// [Deprecated] ACL returns a handle to the ACL endpoints
        /// </summary>
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        public IACLEndpoint ACL
        {
            get { return _acl.Value; }
        }
    }
}
