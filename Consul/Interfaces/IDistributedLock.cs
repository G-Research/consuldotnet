// -----------------------------------------------------------------------
//  <copyright file="IDistributedLock.cs" company="PlayFab Inc">
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
    /// The interface for the Distributed Lock API Endpoints
    /// </summary>
    public interface IDistributedLock
    {
        bool IsHeld { get; }

        Task<CancellationToken> Acquire(CancellationToken ct = default(CancellationToken));
        Task Destroy(CancellationToken ct = default(CancellationToken));
        Task Release(CancellationToken ct = default(CancellationToken));
    }
}
