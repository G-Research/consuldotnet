// -----------------------------------------------------------------------
//  <copyright file="StringFieldSelector.cs" company="G-Research Limited">
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

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
namespace Consul.Filtering
{
    public sealed class StringFieldSelector : Selector, IEqualsApplicableConstraint, IEmptyApplicableConstraint, IContainsApplicableConstraint
    {
        public string Prefix { get; }
        public string Name { get; }

        public StringFieldSelector(string name) : this(null, name)
        {
        }

        public StringFieldSelector(string prefix, string name)
        {
            Prefix = prefix;
            Name = name;
        }

        public override string Encode() => Combine(Prefix, Name);

        public Filter IsEmpty() => Filters.Empty(this);
        public Filter Contains(string value) => Filters.Contains(this, value);
        public static Filter operator ==(StringFieldSelector selector, string value) => Filters.Eq(selector, value);
        public static Filter operator !=(StringFieldSelector selector, string value) => Filters.NotEq(selector, value);
    }
}
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
