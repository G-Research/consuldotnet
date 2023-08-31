// -----------------------------------------------------------------------
//  <copyright file="IACLEndpoint.cs" company="PlayFab Inc">
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

namespace Consul
{
    /// <summary>
    /// The interface for the Legacy ACL System API Endpoints
    /// </summary>
    [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
    public interface IACLEndpoint
    {
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult<string>> Clone(string id, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult<string>> Clone(string id, WriteOptions q, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult<string>> Create(ACLEntry acl, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult<string>> Create(ACLEntry acl, WriteOptions q, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult<bool>> Destroy(string id, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult<bool>> Destroy(string id, WriteOptions q, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<QueryResult<ACLEntry>> Info(string id, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<QueryResult<ACLEntry>> Info(string id, QueryOptions q, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<QueryResult<ACLEntry[]>> List(CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<QueryResult<ACLEntry[]>> List(QueryOptions q, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult> Update(ACLEntry acl, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult> Update(ACLEntry acl, WriteOptions q, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult<string>> TranslateRules(string rules, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<WriteResult<string>> TranslateRules(string rules, WriteOptions q, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<QueryResult<string>> TranslateLegacyTokenRules(string id, CancellationToken ct = default);
        [Obsolete("The Legacy ACL system has been deprecated, please use Token, Role and Policy instead.")]
        Task<QueryResult<string>> TranslateLegacyTokenRules(string id, QueryOptions q, CancellationToken ct = default);
    }
}
