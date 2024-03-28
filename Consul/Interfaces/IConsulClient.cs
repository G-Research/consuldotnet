// -----------------------------------------------------------------------
//  <copyright file="IConsulClient.cs" company="PlayFab Inc">
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
using Consul.Interfaces;

namespace Consul
{
    /// <summary>
    /// The interface for the Consul Client
    /// </summary>
    public interface IConsulClient : IDisposable
    {
#pragma warning disable CS0618 // Type or member is obsolete
        IACLEndpoint ACL { get; }
#pragma warning restore CS0618 // Type or member is obsolete
        IPolicyEndpoint Policy { get; }
        IRoleEndpoint Role { get; }
        ITokenEndpoint Token { get; }
        Task<IDistributedLock> AcquireLock(LockOptions opts, CancellationToken ct = default);
        Task<IDistributedLock> AcquireLock(string key, CancellationToken ct = default);
        Task<IDistributedSemaphore> AcquireSemaphore(SemaphoreOptions opts, CancellationToken ct = default);
        Task<IDistributedSemaphore> AcquireSemaphore(string prefix, int limit, CancellationToken ct = default);
        IAgentEndpoint Agent { get; }
        ICatalogEndpoint Catalog { get; }
        IConfigurationEndpoint Configuration { get; }
        IDistributedLock CreateLock(LockOptions opts);
        IDistributedLock CreateLock(string key);
        IEventEndpoint Event { get; }
        Task ExecuteInSemaphore(SemaphoreOptions opts, Action a, CancellationToken ct = default);
        Task ExecuteInSemaphore(string prefix, int limit, Action a, CancellationToken ct = default);
        Task ExecuteLocked(LockOptions opts, Action action, CancellationToken ct = default);
        [Obsolete("This method will be removed in a future release. Replace calls with the method signature ExecuteLocked(LockOptions, Action, CancellationToken)")]
        Task ExecuteLocked(LockOptions opts, CancellationToken ct, Action action);
        Task ExecuteLocked(string key, Action action, CancellationToken ct = default);
        [Obsolete("This method will be removed in a future release. Replace calls with the method signature ExecuteLocked(string, Action, CancellationToken)")]
        Task ExecuteLocked(string key, CancellationToken ct, Action action);
        IHealthEndpoint Health { get; }
        IKVEndpoint KV { get; }
        IRawEndpoint Raw { get; }
        IDistributedSemaphore Semaphore(SemaphoreOptions opts);
        IDistributedSemaphore Semaphore(string prefix, int limit);
        ISessionEndpoint Session { get; }
        IStatusEndpoint Status { get; }
        IOperatorEndpoint Operator { get; }
        IPreparedQueryEndpoint PreparedQuery { get; }
        ICoordinateEndpoint Coordinate { get; }
        ISnapshotEndpoint Snapshot { get; }
    }
}
