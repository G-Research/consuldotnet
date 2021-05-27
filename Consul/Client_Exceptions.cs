// -----------------------------------------------------------------------
//  <copyright file="Client_Exceptions.cs" company="G-Research Limited">
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
#if !(NETSTANDARD || NETCOREAPP)
    using System.Security.Permissions;
    using System.Runtime.Serialization;
#endif

namespace Consul
{
    /// <summary>
    /// Represents errors that occur while sending data to or fetching data from the Consul agent.
    /// </summary>
#if !(NETSTANDARD || NETCOREAPP)
    [Serializable]
#endif
    public class ConsulRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public ConsulRequestException() { }
        public ConsulRequestException(string message, HttpStatusCode statusCode) : base(message) { StatusCode = statusCode; }
        public ConsulRequestException(string message, HttpStatusCode statusCode, Exception inner) : base(message, inner) { StatusCode = statusCode; }
#if !(NETSTANDARD || NETCOREAPP)
        protected ConsulRequestException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("StatusCode", StatusCode);
        }
#endif
    }

    /// <summary>
    /// Represents errors that occur during initalization of the Consul client's configuration.
    /// </summary>
#if !(NETSTANDARD || NETCOREAPP)
    [Serializable]
#endif
    public class ConsulConfigurationException : Exception
    {
        public ConsulConfigurationException() { }
        public ConsulConfigurationException(string message) : base(message) { }
        public ConsulConfigurationException(string message, Exception inner) : base(message, inner) { }
#if !(NETSTANDARD || NETCOREAPP)
        protected ConsulConfigurationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
    }
}
