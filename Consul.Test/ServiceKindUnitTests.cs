// -----------------------------------------------------------------------
//  <copyright file="ServiceKindUnitTests.cs" company="G-Research Limited">
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

using System.Collections.Generic;
using Xunit;

namespace Consul.Test
{
    public class ServiceKindUnitTests
    {
        public static IList<object[]> TryParseTestCases => new List<object[]>
        {
            new object[] { null, true, null },
            new object[] { "", true, null },
            new object[] { " ", true, null },
            new object[] { "    ", true, null },
            new object[] { "invalidvalud", false, null },
            new object[] { "connect-proxy", true, ServiceKind.ConnectProxy },
            new object[] { "Connect-proxy", true, ServiceKind.ConnectProxy },
            new object[] { "mesh-gateway", true, ServiceKind.MeshGateway },
            new object[] { "Mesh-gateway", true, ServiceKind.MeshGateway },
            new object[] { "terminating-gateway", true, ServiceKind.TerminatingGateway },
            new object[] { "Terminating-gateway", true, ServiceKind.TerminatingGateway },
            new object[] { "ingress-gateway", true, ServiceKind.IngressGateway },
            new object[] { "Ingress-gateway", true, ServiceKind.IngressGateway },
        };

        [Fact]
        public void Test_Equals()
        {
            Assert.Equal(ServiceKind.IngressGateway, ServiceKind.IngressGateway);
            Assert.Equal(ServiceKind.IngressGateway, (object)"ingress-gateway");
            Assert.NotEqual(ServiceKind.IngressGateway, ServiceKind.ConnectProxy);
            Assert.NotEqual(ServiceKind.IngressGateway, (object)"connect-proxy");
        }

        [Theory]
        [MemberData(nameof(TryParseTestCases))]
        public void TryParse(string value, bool expectedPredicateResult, ServiceKind expected)
        {
            bool actualPredicateResult = ServiceKind.TryParse(value, out ServiceKind actual);

            Assert.Equal(expectedPredicateResult, actualPredicateResult);
            Assert.Equal(expected, actual);
        }
    }
}
