// -----------------------------------------------------------------------
//  <copyright file="TimeoutUtils.cs" company="G-Research Limited">
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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    /// <summary>
    /// Utilities for using timeouts in tests
    /// </summary>
    internal static class TimeoutUtils
    {
        /// <summary>
        /// Waits for a condition to become true and throws an exception if it is still false after reaching the timeout
        /// </summary>
        /// <param name="condition">Condition to wait for</param>
        /// <param name="failureMessage">Message shown when the condition does not become true</param>
        public static async Task WaitFor(Func<bool> condition, string failureMessage)
        {
            var cancellationToken = new CancellationTokenSource(DefaultTimeout).Token;
            while (!condition())
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            Assert.True(condition(), failureMessage);
        }

        /// <summary>
        /// Waits for the given task to complete and throws an exception if it doesn't complete within the timeout
        /// </summary>
        /// <param name="task">The task to wait for</param>
        public static async Task WithTimeout(Task task)
        {
            var timeoutTask = Task.Delay(DefaultTimeout);
            var completedTask = await Task.WhenAny(new[] { task, timeoutTask });
            if (completedTask == timeoutTask)
            {
                throw new OperationCanceledException("Timeout waiting for task to complete");
            }
            // Make sure we await the task so that any exceptions are thrown if it faulted.
            await task;
        }

        // The default timeout should allow plenty of time for tests to complete when running on a CI host under load.
        // It isn't intended to be used as a performance check but rather to catch when a test is hanging.
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(5);
    }
}
