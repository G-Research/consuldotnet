// -----------------------------------------------------------------------
//  <copyright file="NamespaceTest.cs" company="G-Research Limited">
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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace Consul.Test
{
    public class NamespaceTest : BaseFixture
    {
        [EnterpriseOnlyFact]
        public async Task Namespaces_CreateNamespace()
        {
            var cutOffVersion = SemanticVersion.Parse("1.7.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Namespaces` is only supported from Consul {cutOffVersion}");

            var name = "test";

            var ns = new Namespace
            {
                Name = name
            };

            var request = await _client.Namespaces.Create(ns);

            Assert.Equal(request.Response.Name, name);
        }

        [EnterpriseOnlyFact]
        public async Task Namespaces_UpdateNamespace()
        {
            var cutOffVersion = SemanticVersion.Parse("1.7.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Namespaces` is only supported from Consul {cutOffVersion}");

            var name = "test";

            var ns = new Namespace
            {
                Name = name
            };

            await _client.Namespaces.Create(ns);

            var description = "updated namespace";

            var newNamespace = new Namespace
            {
                Name = name,
                Description = description
            };

            var updateRequest = await _client.Namespaces.Update(newNamespace);

            Assert.Equal(updateRequest.Response.Description, description);
        }

        [EnterpriseOnlyFact]
        public async Task Namespaces_ReadNamespace()
        {
            var cutOffVersion = SemanticVersion.Parse("1.7.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Namespaces` is only supported from Consul {cutOffVersion}");

            var name = "test";

            var ns = new Namespace
            {
                Name = name
            };

            await _client.Namespaces.Create(ns);
            var request = await _client.Namespaces.Read(name);

            Assert.Equal(request.Response.Name, name);
        }


        [EnterpriseOnlyFact]
        public async Task Namespaces_ListNamespaces()
        {
            var cutOffVersion = SemanticVersion.Parse("1.7.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Namespaces` is only supported from Consul {cutOffVersion}");

            var testNames = new HashSet<string> { "test-a", "test-b", "test-c" };

            foreach (var name in testNames)
            {
                var ns = new Namespace
                {
                    Name = name
                };

                await _client.Namespaces.Create(ns);
            }

            var request = await _client.Namespaces.List();
            testNames.Add("default");
            Assert.True(new HashSet<string>(request.Response.Select(x => x.Name)).SetEquals(testNames));
        }

        [EnterpriseOnlyFact]
        public async Task Namespaces_DeleteNamespace()
        {
            var cutOffVersion = SemanticVersion.Parse("1.7.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Namespaces` is only supported from Consul {cutOffVersion}");

            var name = "test";

            var ns = new Namespace
            {
                Name = name
            };

            var createRequest = await _client.Namespaces.Create(ns);
            Assert.Equal(name, createRequest.Response.Name);
            Assert.Null(createRequest.Response.DeletedAt);

            await _client.Namespaces.Delete(name);

            var readRequest = await _client.Namespaces.Read(name);
            Assert.NotNull(readRequest.Response.DeletedAt);
        }

        [EnterpriseOnlyFact]
        public async Task Namespaces_KVIsolation()
        {
            var cutOffVersion = SemanticVersion.Parse("1.7.0");
            Skip.If(AgentVersion < cutOffVersion, $"Current version is {AgentVersion}, but `Namespaces` is only supported from Consul {cutOffVersion}");

            var name = "test";

            await _client.Namespaces.Create(new Namespace
            {
                Name = name
            });

            var key = "key";

            var requestPair = new KVPair
            {
                Key = key,
                Value = Encoding.UTF8.GetBytes("value")
            };

            await _client.KV.Put(requestPair, new WriteOptions { Namespace = name });
            var namespaceResponsePair = await _client.KV.Get(key, new QueryOptions { Namespace = name });
            Assert.Equal(requestPair.Value, namespaceResponsePair.Response.Value);

            var defaultNamespaceResponsePair = await _client.KV.Get(key);
            Assert.Null(defaultNamespaceResponsePair.Response?.Value);
        }
    }
}
