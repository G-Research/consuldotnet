// -----------------------------------------------------------------------
//  <copyright file="SemaphoreTest.cs" company="PlayFab Inc">
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    // These tests are slow, so we put them into separate collection so they can run in parallel to other tests.
    [Trait("speed", "slow")]
    [Collection("SemaphoreTest")]
    public class SemaphoreTest : BaseFixture
    {
        const int DefaultSessionTTLSeconds = 10;
        const int LockWaitTimeSeconds = 15;

        [Fact]
        public async Task Semaphore_BadLimit()
        {
            const string keyName = "test/semaphore/badlimit";

            Assert.Throws<ArgumentOutOfRangeException>(() => _client.Semaphore(keyName, 0));

            var semaphore1 = _client.Semaphore(keyName, 1);
            await semaphore1.Acquire(CancellationToken.None);

            var semaphore2 = _client.Semaphore(keyName, 2);
            var ex = await Assert.ThrowsAsync<SemaphoreLimitConflictException>(async () => await semaphore2.Acquire(CancellationToken.None));
            Assert.Equal(1, ex.RemoteLimit);
            Assert.Equal(2, ex.LocalLimit);

            await semaphore1.Release();
            await semaphore1.Destroy();

            Assert.False(semaphore1.IsHeld);
        }

        [Fact]
        public async Task Semaphore_AcquireRelease()
        {
            const string keyName = "test/semaphore/acquirerelease";

            var semaphore = _client.Semaphore(keyName, 2);

            await Assert.ThrowsAsync<SemaphoreNotHeldException>(async () => await semaphore.Release());

            await semaphore.Acquire(CancellationToken.None);

            Assert.True(semaphore.IsHeld);

            await Assert.ThrowsAsync<SemaphoreHeldException>(async () => await semaphore.Acquire(CancellationToken.None));

            Assert.True(semaphore.IsHeld);

            await semaphore.Release();

            await Assert.ThrowsAsync<SemaphoreNotHeldException>(async () => await semaphore.Release());

            Assert.False(semaphore.IsHeld);
        }

        [Fact]
        public async Task Semaphore_OneShot()
        {
            const string keyName = "test/semaphore/oneshot";
            TimeSpan waitTime = TimeSpan.FromMilliseconds(3000);

            var semaphoreOptions = new SemaphoreOptions(keyName, 2)
            {
                SemaphoreTryOnce = true
            };

            semaphoreOptions.SemaphoreWaitTime = waitTime;

            var semaphoreKey = _client.Semaphore(semaphoreOptions);

            await semaphoreKey.Acquire(CancellationToken.None);
            Assert.True(semaphoreKey.IsHeld);

            var another = _client.Semaphore(new SemaphoreOptions(keyName, 2)
            {
                SemaphoreTryOnce = true,
                SemaphoreWaitTime = waitTime
            });

            await another.Acquire();
            Assert.True(another.IsHeld);
            Assert.True(semaphoreKey.IsHeld);

            var contender = _client.Semaphore(new SemaphoreOptions(keyName, 2)
            {
                SemaphoreTryOnce = true,
                SemaphoreWaitTime = waitTime
            });

            var stopwatch = Stopwatch.StartNew();

            await TimeoutUtils.WithTimeout(
                Assert.ThrowsAsync<SemaphoreMaxAttemptsReachedException>(async () => await contender.Acquire()));

            Assert.False(contender.IsHeld, "Contender should have failed to acquire");
            Assert.False(stopwatch.ElapsedMilliseconds < semaphoreOptions.SemaphoreWaitTime.TotalMilliseconds);

            Assert.False(contender.IsHeld);
            Assert.True(another.IsHeld);
            Assert.True(semaphoreKey.IsHeld);
            await semaphoreKey.Release();
            await another.Release();
            await contender.Destroy();
        }

        [Fact]
        public async Task Semaphore_AcquireSemaphore()
        {
            const string keyName = "test/semaphore/disposable";
            var semaphore = await _client.AcquireSemaphore(keyName, 2);

            try
            {
                Assert.True(semaphore.IsHeld);
            }
            finally
            {
                await semaphore.Release();
            }
        }

        [Fact]
        public async Task Semaphore_ExecuteAction()
        {
            const string keyName = "test/semaphore/action";
            var actionExecuted = false;

            await _client.ExecuteInSemaphore(keyName, 2, () => actionExecuted = true);

            Assert.True(actionExecuted);
        }

        [Fact]
        public async Task Semaphore_AcquireWaitRelease()
        {
            const string keyName = "test/semaphore/acquirewaitrelease";

            var semaphoreOptions = new SemaphoreOptions(keyName, 1)
            {
                SessionName = "test_semaphoresession",
                SessionTTL = TimeSpan.FromSeconds(10),
                MonitorRetries = 10
            };

            var semaphore = _client.Semaphore(semaphoreOptions);

            await semaphore.Acquire(CancellationToken.None);

            Assert.True(semaphore.IsHeld);

            // Wait for multiple renewal cycles to ensure the semaphore session stays renewed.
            for (int i = 0; i < 60; i++)
            {
                await Task.Delay(1000);
                Assert.True(semaphore.IsHeld);
            }

            Assert.True(semaphore.IsHeld);

            await semaphore.Release();

            Assert.False(semaphore.IsHeld);

            await semaphore.Destroy();
        }

        [Fact]
        public async Task Semaphore_ContendWait()
        {
            const string keyName = "test/semaphore/contend";
            const int contenderPool = 4;

            var acquired = new System.Collections.Concurrent.ConcurrentDictionary<int, bool>();

            var tasks = new List<Task>();
            for (var i = 0; i < contenderPool; i++)
            {
                var v = i;
                tasks.Add(Task.Run(async () =>
                {
                    var semaphore = _client.Semaphore(keyName, 2);
                    await semaphore.Acquire(CancellationToken.None);
                    acquired[v] = semaphore.IsHeld;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await semaphore.Release();
                }));
            }

            await TimeoutUtils.WithTimeout(Task.WhenAll(tasks));

            for (var i = 0; i < contenderPool; i++)
            {
                Assert.True(acquired.ContainsKey(i), $"Contender {i} did not acquire the lock");
                Assert.True(acquired[i], $"IsHeld was false for contender {i}");
            }
        }

        [Fact]
        public async Task Semaphore_ContendFast()
        {
            const string keyName = "test/semaphore/contend";
            const int contenderPool = 15;

            var acquired = new System.Collections.Concurrent.ConcurrentDictionary<int, bool>();

            var tasks = new List<Task>();
            for (var i = 0; i < contenderPool; i++)
            {
                var v = i;
                tasks.Add(Task.Run(async () =>
                {
                    var semaphore = _client.Semaphore(keyName, 2);
                    await semaphore.Acquire(CancellationToken.None);
                    acquired[v] = semaphore.IsHeld;
                    await semaphore.Release();
                }));
            }

            await TimeoutUtils.WithTimeout(Task.WhenAll(tasks));

            for (var i = 0; i < contenderPool; i++)
            {
                Assert.True(acquired.ContainsKey(i), $"Contender {i} did not acquire the lock");
                Assert.True(acquired[i], $"IsHeld was false for contender {i}");
            }
        }

        [Fact]
        public async Task Semaphore_Destroy()
        {
            const string keyName = "test/semaphore/destroy";

            var semaphore1 = _client.Semaphore(keyName, 2);
            var semaphore2 = _client.Semaphore(keyName, 2);
            try
            {
                await semaphore1.Acquire(CancellationToken.None);
                Assert.True(semaphore1.IsHeld);
                await semaphore2.Acquire(CancellationToken.None);
                Assert.True(semaphore2.IsHeld);

                await Assert.ThrowsAsync<SemaphoreHeldException>(async () => await semaphore1.Destroy());

                await semaphore1.Release();
                Assert.False(semaphore1.IsHeld);

                await Assert.ThrowsAsync<SemaphoreInUseException>(async () => await semaphore1.Destroy());

                await semaphore2.Release();
                Assert.False(semaphore2.IsHeld);
                await semaphore1.Destroy();
                await semaphore2.Destroy();
            }
            finally
            {
                try
                {
                    await semaphore1.Release();
                }
                catch (SemaphoreNotHeldException)
                {
                    // Exception expected if checks pass
                }
                try
                {
                    await semaphore2.Release();
                }
                catch (SemaphoreNotHeldException)
                {
                    // Exception expected if checks pass
                }
            }
        }

        [Fact]
        public async Task Semaphore_DeleteKey()
        {
            const string keyName = "test/semaphore/deletekey";

            var semaphore = _client.Semaphore(keyName, 2);

            try
            {
                await semaphore.Acquire(CancellationToken.None);

                Assert.True(semaphore.IsHeld);

                var req = await _client.KV.DeleteTree(keyName);
                Assert.True(req.Response);

                await TimeoutUtils.WaitFor(() => !semaphore.IsHeld, "Expected deleting tree to release semaphore");
            }
            finally
            {
                try
                {
                    await semaphore.Release();
                }
                catch (SemaphoreNotHeldException)
                {
                    // Exception expected if checks pass
                }
            }
        }

        [Fact]
        public async Task Semaphore_Conflict()
        {
            const string keyName = "test/semaphore/conflict";

            var semaphoreLock = _client.CreateLock(keyName + "/.lock");

            await semaphoreLock.Acquire(CancellationToken.None);

            Assert.True(semaphoreLock.IsHeld);

            var semaphore = _client.Semaphore(keyName, 2);

            await Assert.ThrowsAsync<SemaphoreConflictException>(async () => await semaphore.Acquire(CancellationToken.None));

            await Assert.ThrowsAsync<SemaphoreConflictException>(async () => await semaphore.Destroy());

            await semaphoreLock.Release();

            Assert.False(semaphoreLock.IsHeld);

            await semaphoreLock.Destroy();
        }

        [Fact]
        public async Task Passing_Cancelled_CancellationToken_Should_Throw_LockNotHeldException()
        {
            const string keyName = "service/myApp/leader";

            var distributedLock = _client.CreateLock(new LockOptions(keyName)
            {
                SessionTTL = TimeSpan.FromSeconds(DefaultSessionTTLSeconds),
                LockWaitTime = TimeSpan.FromSeconds(LockWaitTimeSeconds)
            });

            var cts = new CancellationTokenSource();

            cts.Cancel();

            await Assert.ThrowsAsync<LockNotHeldException>(async () => await distributedLock.Acquire(cts.Token));
        }

        [Fact]
        public async Task Cancelling_A_Token_When_Acquiring_A_Lock_Should_Throw_TaskCanceledException()
        {
            var keyName = Path.GetRandomFileName();

            var masterClient = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            });

            var distributedLock2 = masterClient.CreateLock(new LockOptions(keyName)
            {
                SessionTTL = TimeSpan.FromSeconds(DefaultSessionTTLSeconds),
                LockWaitTime = TimeSpan.FromSeconds(LockWaitTimeSeconds)
            });

            var distributedLock = _client.CreateLock(new LockOptions(keyName)
            {
                SessionTTL = TimeSpan.FromSeconds(DefaultSessionTTLSeconds),
                LockWaitTime = TimeSpan.FromSeconds(LockWaitTimeSeconds)
            });

            await distributedLock2.Acquire(); // Become "Master" with another instance first

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            await Assert.ThrowsAsync<TaskCanceledException>(async () => await distributedLock.Acquire(cts.Token));

            await distributedLock2.Release();
            await distributedLock2.Destroy();
            masterClient.Dispose();
        }

        [Fact]
        public async Task Cancelling_A_Token_When_Acquiring_A_Lock_Respects_The_Token()
        {
            var keyName = Path.GetRandomFileName();

            var masterInstanceClient = new ConsulClient(c =>
            {
                c.Token = TestHelper.MasterToken;
                c.Address = TestHelper.HttpUri;
            });

            // Arrange
            var distributedLock2 = masterInstanceClient.CreateLock(new LockOptions(keyName)
            {
                SessionTTL = TimeSpan.FromSeconds(DefaultSessionTTLSeconds),
                LockWaitTime = TimeSpan.FromSeconds(LockWaitTimeSeconds)
            });
            var distributedLock = _client.CreateLock(new LockOptions(keyName)
            {
                SessionTTL = TimeSpan.FromSeconds(DefaultSessionTTLSeconds),
                LockWaitTime = TimeSpan.FromSeconds(LockWaitTimeSeconds)
            });
            var cancellationOperationTimer = new Stopwatch();

            // Act
            await distributedLock2.Acquire(); // Become "Master" with another instance first
            cancellationOperationTimer.Start();

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            try
            {
                await distributedLock.Acquire(cts.Token);
            }
            catch (Exception) { }
            cancellationOperationTimer.Stop();

            // Assert
            var stopTimeMs = cancellationOperationTimer.ElapsedMilliseconds;
            var lockWaitTimeMs = TimeSpan.FromSeconds(LockWaitTimeSeconds).TotalMilliseconds;
            Assert.True(stopTimeMs < lockWaitTimeMs);

            // cleanup
            await distributedLock2.Release();
            if (distributedLock.IsHeld)
                await distributedLock2.Release();
            await distributedLock2.Destroy();

            masterInstanceClient.Dispose();
        }
    }
}
