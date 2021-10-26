// -----------------------------------------------------------------------
//  <copyright file="TestHelper.cs" company="G-Research Limited">
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
using Microsoft.Extensions.Configuration;

namespace Consul.Test
{
    /// <summary>
    /// A collection fixture to pass config to test classes
    /// </summary>
    public static class TestHelper
    {
        public static string BindingAddress { get; }
        public static string HttpPort { get; }
        public static string MasterToken { get; }
        public static string HttpAddr { get; }
        public static Uri HttpUri { get; }

        /// <summary>
        /// Set up a helper to allow overriding verious settings when testing
        /// </summary>
        static TestHelper()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            BindingAddress = config["binding_address"];
            HttpPort = config["http_port"];
            MasterToken = config["master_token"];

            HttpAddr = $"http://{BindingAddress}:{HttpPort}";
            HttpUri = new Uri(HttpAddr);
        }
    }
}
