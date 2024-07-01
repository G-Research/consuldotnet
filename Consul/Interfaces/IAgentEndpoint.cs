// -----------------------------------------------------------------------
//  <copyright file="IAgentEndpoint.cs" company="PlayFab Inc">
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Consul.Filtering;

namespace Consul
{
    /// <summary>
    /// The interface for the Agent API Endpoints
    /// </summary>
    public interface IAgentEndpoint
    {
        Task<WriteResult> CheckDeregister(string checkID, CancellationToken ct = default);
        Task<WriteResult> CheckRegister(AgentCheckRegistration check, CancellationToken ct = default);
        Task<QueryResult<Dictionary<string, AgentCheck>>> Checks(CancellationToken ct = default);
        Task<QueryResult<Dictionary<string, AgentCheck>>> Checks(Filter filter, CancellationToken ct = default);
        Task<WriteResult> DisableNodeMaintenance(CancellationToken ct = default);
        Task<WriteResult> DisableServiceMaintenance(string serviceID, CancellationToken ct = default);
        Task<WriteResult> EnableNodeMaintenance(string reason, CancellationToken ct = default);
        Task<WriteResult> EnableServiceMaintenance(string serviceID, string reason, CancellationToken ct = default);
        Task FailTTL(string checkID, string note, CancellationToken ct = default);
        Task<WriteResult> ForceLeave(string node, CancellationToken ct = default);
        Task<WriteResult> Join(string addr, bool wan, CancellationToken ct = default);
        Task<QueryResult<AgentMember[]>> Members(bool wan, CancellationToken ct = default);
        [Obsolete("This property will be removed in a future release. Replace uses of it with a call to GetNodeName()")]
        string NodeName { get; }
        Task<string> GetNodeName(CancellationToken ct = default);
        Task PassTTL(string checkID, string note, CancellationToken ct = default);
        Task<QueryResult<Dictionary<string, Dictionary<string, dynamic>>>> Self(CancellationToken ct = default);
        Task<WriteResult> ServiceDeregister(string serviceID, CancellationToken ct = default);
        Task<WriteResult> ServiceRegister(AgentServiceRegistration service, CancellationToken ct = default);
        Task<WriteResult> ServiceRegister(AgentServiceRegistration service, bool replaceExistingChecks, CancellationToken ct = default);
        Task<QueryResult<Dictionary<string, AgentService>>> Services(CancellationToken ct = default);
        Task<QueryResult<Dictionary<string, AgentService>>> Services(Filter filter, CancellationToken ct = default);
        Task<WriteResult> UpdateTTL(string checkID, string output, TTLStatus status, CancellationToken ct = default);
        Task WarnTTL(string checkID, string note, CancellationToken ct = default);
        Task<Agent.LogStream> Monitor(LogLevel level = default, CancellationToken ct = default);
        Task<Agent.LogStream> MonitorJSON(LogLevel level = default, CancellationToken ct = default);
        Task<QueryResult<LocalServiceHealth[]>> GetLocalServiceHealth(string serviceName, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<LocalServiceHealth[]>> GetLocalServiceHealth(string serviceName, CancellationToken ct = default);
        Task<QueryResult<string>> GetWorstLocalServiceHealth(string serviceName, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<string>> GetWorstLocalServiceHealth(string serviceName, CancellationToken ct = default);
        Task<QueryResult<LocalServiceHealth>> GetLocalServiceHealthByID(string serviceID, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<LocalServiceHealth>> GetLocalServiceHealthByID(string serviceID, CancellationToken ct = default);
        Task<QueryResult<Metrics>> GetAgentMetrics(CancellationToken ct = default);
        Task<WriteResult<AgentAuthorizeResponse>> ConnectAuthorize(AgentAuthorizeParameters parameters, CancellationToken ct = default);
        Task<WriteResult<AgentAuthorizeResponse>> ConnectAuthorize(AgentAuthorizeParameters parameters, WriteOptions w, CancellationToken ct = default);
        Task<QueryResult<CARoots>> GetCARoots(CancellationToken ct = default);
        Task<QueryResult<CARoots>> GetCARoots(QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<CALeaf>> GetCALeaf(string serviceId, CancellationToken ct = default);
        Task<QueryResult<CALeaf>> GetCALeaf(string serviceId, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<AgentVersion>> GetAgentVersion(CancellationToken ct = default);
        Task<WriteResult> Reload(CancellationToken ct = default);
        [Obsolete]
        Task<WriteResult> Reload(string node, CancellationToken ct = default);
        Task<QueryResult<AgentHostInfo>> GetAgentHostInfo(CancellationToken ct = default);
        Task<WriteResult> Leave(string node, CancellationToken ct = default);
        Task<QueryResult<ServiceConfiguration>> GetServiceConfiguration(string serviceID, QueryOptions q, CancellationToken ct = default);
        Task<QueryResult<ServiceConfiguration>> GetServiceConfiguration(string serviceID, CancellationToken ct = default);
    }
}
