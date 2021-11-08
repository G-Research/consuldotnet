// -----------------------------------------------------------------------
//  <copyright file="UtilTest.cs" company="PlayFab Inc">
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
using Xunit;

namespace Consul.Test
{
    public class UtilTest
    {
        [Fact]
        public void TestToGoDuration()
        {
            Assert.Equal("150ms", new TimeSpan(0, 0, 0, 0, 150).ToGoDuration());
            Assert.Equal("26h3m4.005s", new TimeSpan(1, 2, 3, 4, 5).ToGoDuration());
            Assert.Equal("2h3m4.005s", new TimeSpan(0, 2, 3, 4, 5).ToGoDuration());
            Assert.Equal("3m4.005s", new TimeSpan(0, 0, 3, 4, 5).ToGoDuration());
            Assert.Equal("4.005s", new TimeSpan(0, 0, 0, 4, 5).ToGoDuration());
            Assert.Equal("5ms", new TimeSpan(0, 0, 0, 0, 5).ToGoDuration());
            Assert.Equal("1m0.001s", new TimeSpan(0, 0, 1, 0, 1).ToGoDuration());
            Assert.Equal("1h0m0.001s", new TimeSpan(0, 1, 0, 0, 1).ToGoDuration());
            Assert.Equal("0", TimeSpan.FromMilliseconds(0.1).ToGoDuration());
            Assert.Equal("0", TimeSpan.Zero.ToGoDuration());
        }

        [Fact]
        public void TestFromGoDuration()
        {
            Assert.Equal(new TimeSpan(0, 0, 0, 0, 150), Extensions.FromGoDuration("150ms"));
            Assert.Equal(new TimeSpan(1, 2, 3, 4, 5), Extensions.FromGoDuration("26h3m4.005s"));
            Assert.Equal(new TimeSpan(0, 2, 3, 4, 5), Extensions.FromGoDuration("2h3m4.005s"));
            Assert.Equal(new TimeSpan(0, 0, 3, 4, 5), Extensions.FromGoDuration("3m4.005s"));
            Assert.Equal(new TimeSpan(0, 0, 0, 4, 5), Extensions.FromGoDuration("4.005s"));
            Assert.Equal(new TimeSpan(0, 0, 0, 0, 5), Extensions.FromGoDuration("5ms"));
            Assert.Equal(new TimeSpan(0, 0, 1, 0, 1), Extensions.FromGoDuration("1m0.001s"));
            Assert.Equal(new TimeSpan(0, 1, 0, 0, 1), Extensions.FromGoDuration("1h0m0.001s"));
            Assert.Equal(TimeSpan.FromMilliseconds(0.0), Extensions.FromGoDuration("0s"));
        }
    }
}
