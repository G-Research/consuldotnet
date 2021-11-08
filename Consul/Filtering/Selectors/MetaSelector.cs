// -----------------------------------------------------------------------
//  <copyright file="MetaSelector.cs" company="G-Research Limited">
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

namespace Consul.Filtering
{
    public sealed class MetaSelector : Selector, IEmptyApplicableConstraint
    {
        private static readonly string Self = "Meta";

        public string Prefix { get; }

        public MetaSelector() : this(null)
        {
        }

        public MetaSelector(string prefix)
        {
            Prefix = prefix;
        }

        public override string Encode() => Combine(Prefix, Self);

        public ServiceMetaEntrySelector this[string name] => new ServiceMetaEntrySelector(Encode(), name);

        public Filter IsEmpty() => Filters.Empty(this);
    }
}
