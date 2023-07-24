// -----------------------------------------------------------------------
//  <copyright file="FilterTests.cs" company="G-Research Limited">
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
using System.Threading.Tasks;
using Consul.Filtering;
using Xunit;
using S = Consul.Filtering.Selectors;

namespace Consul.Test
{
    public class FilterTests : BaseFixture
    {
        private static void CheckEncoded(string expected, IEncodable expression) =>
            Assert.Equal(expected, expression.Encode());

        private class TestFilter : Filter
        {
            public static TestFilter Instance { get; } = new TestFilter();

            public override string Encode() => "Test";
        }

        private class TestSelector : Selector, IEqualsApplicableConstraint, IContainsApplicableConstraint
        {
            public static TestSelector Instance { get; } = new TestSelector();

            public override string Encode() => "Test";
        }

        private static class ServiceConstants
        {
            public static readonly string ServiceId = "serviceId";
            public static readonly string Address = "1.1.1.1";
            public static readonly int Port = 1234;
            public static readonly string ServiceName = "serviceName";
            public static readonly string MetaName = "Environment";
            public static readonly string MetaValue = "dev1";
            public static readonly string Tag = "well-behaved";
            public static readonly Dictionary<string, string> Meta = new Dictionary<string, string> { { MetaName, MetaValue } };
        }

        private async Task<ServiceEntry[]> CreateAndFindServiceByFilterOnAgent(Filter filter)
        {
            await _client.Agent.ServiceRegister(new AgentServiceRegistration
            {
                ID = ServiceConstants.ServiceId,
                Address = ServiceConstants.Address,
                Port = ServiceConstants.Port,
                Meta = ServiceConstants.Meta,
                Name = ServiceConstants.ServiceName,
                Tags = new[] { ServiceConstants.Tag },
                EnableTagOverride = false,
            });

            var queryResult = await _client.Health.Service(ServiceConstants.ServiceName, null, passingOnly: false, q: QueryOptions.Default, filter: filter);

            await _client.Agent.ServiceDeregister(ServiceConstants.ServiceId);

            return queryResult.Response;
        }

        [Fact]
        public async Task Filter_Service_PerfectMatch()
        {
            var filter = S.Service.Id == ServiceConstants.ServiceId
                & !S.Service.Meta.IsEmpty()
                & S.Service.Meta[ServiceConstants.MetaName] == ServiceConstants.MetaValue
                & !S.Service.Tags.IsEmpty()
                & S.Service.Tags.Contains(ServiceConstants.Tag);

            var result = await CreateAndFindServiceByFilterOnAgent(filter);

            Assert.Single(result);
        }

        [Fact]
        public async Task Filter_Service_OtherId()
        {
            var filter = S.Service.Id == "other id";

            var result = await CreateAndFindServiceByFilterOnAgent(filter);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Filter_Service_EmptyMeta()
        {
            var filter = S.Service.Meta.IsEmpty();

            var result = await CreateAndFindServiceByFilterOnAgent(filter);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Filter_Service_ContainsOtherMeta()
        {
            var filter = S.Service.Meta[ServiceConstants.MetaName] == "other value";

            var result = await CreateAndFindServiceByFilterOnAgent(filter);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Filter_Service_EmptyTags()
        {
            var filter = S.Service.Tags.IsEmpty();

            var result = await CreateAndFindServiceByFilterOnAgent(filter);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Filter_Service_ContainsOtherTag()
        {
            var filter = S.Service.Tags.Contains("other tag");

            var result = await CreateAndFindServiceByFilterOnAgent(filter);

            Assert.Empty(result);
        }

        [Fact]
        public void Filter_CompositeTest()
        {
            var filter = S.Service.Meta.IsEmpty() | S.Service.Meta["instance_type"] == "t2.medium" | S.Service.Tags.Contains("primary") & S.Service.Id == "53A4BE6C";

            var expected = @"((Service.Meta is empty or Service.Meta.instance_type == ""t2.medium"") or (Service.Tags contains ""primary"" and Service.ID == ""53A4BE6C""))";

            CheckEncoded(expected, filter);
        }

        [Fact]
        public void Filter_MetaSelector()
        {
            CheckEncoded("Meta", new MetaSelector(null));
        }

        [Fact]
        public void Filter_MetaSelector_IsEmpty()
        {
            CheckEncoded("Meta is empty", new MetaSelector(null).IsEmpty());
        }

        [Fact]
        public void Filter_MetaSelector_Indexate()
        {
            CheckEncoded("Meta.field", new MetaSelector(null)["field"]);
        }

        [Fact]
        public void Filter_Selectors_Service()
        {
            CheckEncoded("Service", S.Service);
        }

        [Fact]
        public void Filter_ServiceSelector_Equals()
        {
            CheckEncoded("Service == \"serviceName\"", S.Service == ServiceConstants.ServiceName);
        }

        [Fact]
        public void Filter_ServiceSelector_NotEquals()
        {
            CheckEncoded("Service != \"serviceName\"", S.Service != ServiceConstants.ServiceName);
        }

        [Fact]
        public void Filter_ServiceMetaEntrySelector()
        {
            CheckEncoded("A.B", new ServiceMetaEntrySelector("A", "B"));
        }

        [Fact]
        public void Filter_ServiceMetaEntrySelector_Contains()
        {
            CheckEncoded("A.B contains \"C\"", new ServiceMetaEntrySelector("A", "B").Contains("C"));
        }

        [Fact]
        public void Filter_ServiceMetaEntrySelector_Eq()
        {
            CheckEncoded("A.B == \"C\"", new ServiceMetaEntrySelector("A", "B") == "C");
        }

        [Fact]
        public void Filter_ServiceMetaEntrySelector_NotEq()
        {
            CheckEncoded("A.B != \"C\"", new ServiceMetaEntrySelector("A", "B") != "C");
        }

        [Fact]
        public void Filter_ServiceSelector_Id()
        {
            CheckEncoded("Service.ID", new ServiceSelector().Id);
        }

        [Fact]
        public void Filter_ServiceSelector_Tags()
        {
            CheckEncoded("Service.Tags", new ServiceSelector().Tags);
        }

        [Fact]
        public void Filter_ServiceSelector_Meta()
        {
            CheckEncoded("Service.Meta", new ServiceSelector().Meta);
        }

        [Fact]
        public void Filter_StringFieldSelector()
        {
            CheckEncoded("A.B", new StringFieldSelector("A", "B"));
        }

        [Fact]
        public void Filter_StringFieldSelector_IsEmpty()
        {
            CheckEncoded("A.B is empty", new StringFieldSelector("A", "B").IsEmpty());
        }

        [Fact]
        public void Filter_StringFieldSelector_Contains()
        {
            CheckEncoded("A.B contains \"C\"", new StringFieldSelector("A", "B").Contains("C"));
        }

        [Fact]
        public void Filter_StringFieldSelector_Eq()
        {
            CheckEncoded("A.B == \"C\"", new StringFieldSelector("A", "B") == "C");
        }

        [Fact]
        public void Filter_StringFieldSelector_NotEq()
        {
            CheckEncoded("A.B != \"C\"", new StringFieldSelector("A", "B") != "C");
        }

        [Fact]
        public void Filter_TagsSelector()
        {
            CheckEncoded("A.Tags", new TagsSelector("A"));
        }

        [Fact]
        public void Filter_TagsSelector_IsEmpty()
        {
            CheckEncoded("A.Tags is empty", new TagsSelector("A").IsEmpty());
        }

        [Fact]
        public void Filter_TagsSelector_Contains()
        {
            CheckEncoded("A.Tags contains \"B\"", new TagsSelector("A").Contains("B"));
        }

        [Fact]
        public void Filter_Filter_And()
        {
            CheckEncoded("(Test and Test)", TestFilter.Instance & TestFilter.Instance);
        }

        [Fact]
        public void Filter_Filter_Or()
        {
            CheckEncoded("(Test or Test)", TestFilter.Instance | TestFilter.Instance);
        }

        [Fact]
        public void Filter_Filter_Not()
        {
            CheckEncoded("not Test", !TestFilter.Instance);
        }

        [Fact]
        public void Filter_Filters_And()
        {
            CheckEncoded("(Test and Test)", Filters.And(TestFilter.Instance, TestFilter.Instance));
        }

        [Fact]
        public void Filter_Filters_Or()
        {
            CheckEncoded("(Test or Test)", Filters.Or(TestFilter.Instance, TestFilter.Instance));
        }

        [Fact]
        public void Filter_Filters_Not()
        {
            CheckEncoded("not Test", Filters.Not(TestFilter.Instance));
        }

        [Fact]
        public void Filter_Filters_Contains()
        {
            CheckEncoded("Test contains \"A\"", Filters.Contains(TestSelector.Instance, "A"));
        }

        [Fact]
        public void Filter_Filters_Eq()
        {
            CheckEncoded("Test == \"A\"", Filters.Eq(TestSelector.Instance, "A"));
        }

        [Fact]
        public void Filter_Filters_NotEq()
        {
            CheckEncoded("Test != \"A\"", Filters.NotEq(TestSelector.Instance, "A"));
        }
    }
}
