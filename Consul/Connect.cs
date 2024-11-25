// -----------------------------------------------------------------------
//  <copyright file="Connect.cs" company="G-Research Limited">
//    Copyright 2020 G-Research Limited
//
//    Licensed under the Apache License, Version 2.0 (the "License"),
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
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
using System.Text;
using Consul.Interfaces;

namespace Consul
{
    public class Connect : IConnectEndpoint
    {
        private readonly ConsulClient _client;

        internal Connect(ConsulClient c)
        {
            _client = c;
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Connect> _connect;

        /// <summary>
        /// Connect returns a handle to the Connect endpoints
        /// </summary>
        public IConnectEndpoint Connect => _connect.Value;
    }
}
