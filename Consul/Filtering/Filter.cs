// -----------------------------------------------------------------------
//  <copyright file="Filter.cs" company="G-Research Limited">
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
    // todo generalize to allow usage for nodes filtering (marker class as a generic parameter?)
    public abstract class Filter : IEncodable
    {
        public abstract string Encode();

        public static Filter operator &(Filter left, Filter right) =>
            Filters.And(left, right);

        public static Filter operator |(Filter left, Filter right) =>
            Filters.Or(left, right);

        public static Filter operator !(Filter filter) =>
            Filters.Not(filter);

        protected static string Quote(string s) => '"' + s.Replace(@"\", @"\\").Replace(@"""", @"\""") + '"'; // todo escape better?
    }

    internal sealed class AndFilter : Filter
    {
        public Filter LeftFilter { get; set; }
        public Filter RightFilter { get; set; }

        public override string Encode() => $"({LeftFilter.Encode()} and {RightFilter.Encode()})";
    }

    internal sealed class OrFilter : Filter
    {
        public Filter LeftFilter { get; set; }
        public Filter RightFilter { get; set; }

        public override string Encode() => $"({LeftFilter.Encode()} or {RightFilter.Encode()})";
    }

    internal sealed class NotFilter : Filter
    {
        public Filter Filter { get; set; }

        public override string Encode() => $"not {Filter.Encode()}";
    }

    internal sealed class EmptyFilter<TSelector> : Filter
        where TSelector : Selector, IEmptyApplicableConstraint
    {
        public TSelector Selector { get; set; }

        public override string Encode() => $"{Selector.Encode()} is empty";
    }

    internal sealed class ContainsFilter<TSelector> : Filter
        where TSelector : Selector, IContainsApplicableConstraint
    {
        public TSelector Selector { get; set; }
        public string Value { get; set; }

        public override string Encode() => $"{Selector.Encode()} contains {Quote(Value)}";
    }

    internal sealed class EqualsFilter<TSelector> : Filter
        where TSelector : Selector, IEqualsApplicableConstraint
    {
        public TSelector Selector { get; set; }
        public string Value { get; set; }

        public override string Encode() => $"{Selector.Encode()} == {Quote(Value)}";
    }

    internal sealed class NotEqualsFilter<TSelector> : Filter
        where TSelector : Selector, IEqualsApplicableConstraint
    {
        public TSelector Selector { get; set; }
        public string Value { get; set; }

        public override string Encode() => $"{Selector.Encode()} != {Quote(Value)}";
    }
}
