// -----------------------------------------------------------------------
//  <copyright file="LockTest.cs" company="PlayFab Inc">
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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Consul.Test
{
    // These tests are slow, so we put them into separate collection so they can run in parallel to other tests.
    [Trait("speed", "slow")]
    [Collection("LockTest")]
    public class LockTest : BaseFixture
    {
        [Fact]
        public async Task Lock_AcquireRelease()
        {
            const string keyName = "test/lock/acquirerelease";
            var lockKey = _client.CreateLock(keyName);

            await Assert.ThrowsAsync<LockNotHeldException>(async () =>
                await lockKey.Release());

            await lockKey.Acquire(CancellationToken.None);

            await Assert.ThrowsAsync<LockHeldException>(async () =>
                await lockKey.Acquire(CancellationToken.None));

            Assert.True(lockKey.IsHeld);

            await lockKey.Release();

            await Assert.ThrowsAsync<LockNotHeldException>(async () =>
                await lockKey.Release());

            Assert.False(lockKey.IsHeld);
        }

        [Fact]
        public void Lock_RetryRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LockOptions("test/lock/retryrange")
            {
                LockRetryTime = TimeSpan.Zero
            });
        }

        [Fact]
        public void Lock_MultithreadedRelease()
        {
            const int numTasks = 100000;
            const string keyName = "test/lock/acquirerelease";
            var lockKey = _client.CreateLock(keyName);
            Task[] tasks = new Task[numTasks];

            for (int i = 0; i < numTasks; i++)
            {
                tasks[i] = lockKey.Acquire(CancellationToken.None);
            }

            var aggregateException = Assert.Throws<AggregateException>(() => Task.WaitAll(tasks));
            Assert.Equal(numTasks - 1, aggregateException.InnerExceptions.Count);

            Assert.True(lockKey.IsHeld);

            for (int i = 0; i < numTasks; i++)
            {
                tasks[i] = lockKey.Release(CancellationToken.None);
            }

            aggregateException = Assert.Throws<AggregateException>(() => Task.WaitAll(tasks));
            Assert.Equal(numTasks - 1, aggregateException.InnerExceptions.Count);
        }

        [Fact]
        public async Task Lock_OneShot()
        {
            const string keyName = "test/lock/oneshot";
            var lockOptions = new LockOptions(keyName)
            {
                LockTryOnce = true
            };

            Assert.Equal(Lock.DefaultLockWaitTime, lockOptions.LockWaitTime);

            lockOptions.LockWaitTime = TimeSpan.FromMilliseconds(1000);

            var lockKey = _client.CreateLock(lockOptions);

            await lockKey.Acquire(CancellationToken.None);

            var contender = _client.CreateLock(new LockOptions(keyName)
            {
                LockTryOnce = true,
                LockWaitTime = TimeSpan.FromMilliseconds(1000)
            });

            var stopwatch = Stopwatch.StartNew();

            Assert.True(lockKey.IsHeld);
            Assert.False(contender.IsHeld);

            await TimeoutUtils.WithTimeout(
                Assert.ThrowsAsync<LockMaxAttemptsReachedException>(async () => await contender.Acquire()));

            Assert.False(stopwatch.ElapsedMilliseconds < lockOptions.LockWaitTime.TotalMilliseconds);
            Assert.False(contender.IsHeld, "Contender should have failed to acquire");

            Assert.True(lockKey.IsHeld);
            Assert.False(contender.IsHeld);

            await lockKey.Release();
            Assert.False(lockKey.IsHeld);
            Assert.False(contender.IsHeld);

            while (contender.IsHeld == false)
            {
                try
                {
                    await contender.Acquire();
                    Assert.False(lockKey.IsHeld);
                    Assert.True(contender.IsHeld);
                }
                catch (LockMaxAttemptsReachedException)
                {
                    // Ignore because lock delay might be in effect.
                }
            }

            await contender.Release();
            await contender.Destroy();
        }

        [Fact]
        public async Task Lock_EphemeralAcquireRelease()
        {
            const string keyName = "test/lock/ephemerallock";
            var sessionId = await _client.Session.Create(new SessionEntry { Behavior = SessionBehavior.Delete });

            var l = await _client.AcquireLock(new LockOptions(keyName) { Session = sessionId.Response }, CancellationToken.None);

            Assert.True(l.IsHeld);
            await _client.Session.Destroy(sessionId.Response);

            await TimeoutUtils.WaitFor(() => !l.IsHeld, "Expected lock to be lost when session destroyed");

            Assert.Null((await _client.KV.Get(keyName)).Response);
        }

        [Fact]
        public async Task Lock_Disposable()
        {
            const string keyName = "test/lock/disposable";

            var l = await _client.AcquireLock(keyName);
            try
            {
                Assert.True(l.IsHeld);
            }
            finally
            {
                await l.Release();
            }
        }

        [Fact]
        public async Task Lock_ExecuteAction()
        {
            const string keyName = "test/lock/action";
            var actionExecuted = false;

            await _client.ExecuteLocked(keyName, () => actionExecuted = true);

            Assert.True(actionExecuted);
        }

        [Fact]
        public async Task Lock_AcquireWaitRelease()
        {
            const string keyName = "test/lock/acquirewaitrelease";

            var lockOptions = new LockOptions(keyName)
            {
                SessionName = "test_locksession",
                SessionTTL = TimeSpan.FromSeconds(10)
            };

            var l = _client.CreateLock(lockOptions);

            await l.Acquire(CancellationToken.None);

            Assert.True(l.IsHeld);

            // Wait for multiple renewal cycles to ensure the semaphore session stays renewed.
            for (int i = 0; i < 60; i++)
            {
                await Task.Delay(1000);
                Assert.True(l.IsHeld);
            }

            Assert.True(l.IsHeld);

            await l.Release();

            Assert.False(l.IsHeld);

            await l.Destroy();
        }

        [Fact]
        public async Task Lock_ContendWait()
        {
            const string keyName = "test/lock/contendwait";
            const int contenderPool = 3;

            var acquired = new System.Collections.Concurrent.ConcurrentDictionary<int, bool>();

            var tasks = new List<Task>();
            for (var i = 0; i < contenderPool; i++)
            {
                var v = i;
                acquired[v] = false;
                tasks.Add(Task.Run(async () =>
                {
                    var lockKey = _client.CreateLock(keyName);
                    await lockKey.Acquire(CancellationToken.None);
                    acquired[v] = lockKey.IsHeld;
                    if (lockKey.IsHeld)
                    {
                        await Task.Delay(1000);
                        await lockKey.Release();
                    }
                }));
            }

            await TimeoutUtils.WithTimeout(Task.WhenAll(tasks));

            for (var i = 0; i < contenderPool; i++)
            {
                Assert.True(acquired[i], "Contender " + i.ToString() + " did not acquire the lock");
            }
        }

        [Fact]
        public async Task Lock_ContendFast()
        {
            const string keyName = "test/lock/contendfast";
            const int contenderPool = 10;

            var acquired = new System.Collections.Concurrent.ConcurrentDictionary<int, bool>();

            var tasks = new List<Task>();
            for (var i = 0; i < contenderPool; i++)
            {
                var v = i;
                tasks.Add(Task.Run(async () =>
                {
                    var lockKey = _client.CreateLock(keyName);
                    await lockKey.Acquire(CancellationToken.None);
                    Assert.True(acquired.TryAdd(v, lockKey.IsHeld));
                    if (lockKey.IsHeld)
                    {
                        await lockKey.Release();
                    }
                }));
            }

            await TimeoutUtils.WithTimeout(Task.WhenAll(tasks));

            for (var i = 0; i < contenderPool; i++)
            {
                if (acquired.ContainsKey(i))
                {
                    Assert.True(acquired[i]);
                }
                else
                {
                    Assert.True(false, "Contender " + i.ToString() + " did not acquire the lock");
                }
            }
        }

        [Fact]
        public async Task Lock_Contend_LockDelay()
        {
            const string keyName = "test/lock/contendlockdelay";

            const int contenderPool = 3;

            var acquired = new System.Collections.Concurrent.ConcurrentDictionary<int, bool>();

            var tasks = new List<Task>();
            for (var i = 0; i < contenderPool; i++)
            {
                var v = i;
                tasks.Add(Task.Run(async () =>
                {
                    var lockKey = (Lock)_client.CreateLock(keyName);
                    await lockKey.Acquire(CancellationToken.None);
                    if (lockKey.IsHeld)
                    {
                        Assert.True(acquired.TryAdd(v, lockKey.IsHeld));
                        await _client.Session.Destroy(lockKey.LockSession);
                    }
                }));
            }

            // (contenderPool - 1) tasks will need to wait for the default session lock delay
            // before the lock can be acquired.
            await TimeoutUtils.WithTimeout(Task.WhenAll(tasks));

            for (var i = 0; i < contenderPool; i++)
            {
                bool didContend = false;
                if (acquired.TryGetValue(i, out didContend))
                {
                    Assert.True(didContend);
                }
                else
                {
                    Assert.True(false, "Contender " + i.ToString() + " did not acquire the lock");
                }
            }
        }

        [Fact]
        public async Task Lock_Destroy()
        {
            const string keyName = "test/lock/destroy";

            var lockKey = _client.CreateLock(keyName);

            try
            {
                await lockKey.Acquire(CancellationToken.None);

                Assert.True(lockKey.IsHeld);

                await Assert.ThrowsAsync<LockHeldException>(async () =>
                    await lockKey.Destroy());

                await lockKey.Release();

                Assert.False(lockKey.IsHeld);

                var lockKey2 = _client.CreateLock(keyName);

                await lockKey2.Acquire(CancellationToken.None);

                Assert.True(lockKey2.IsHeld);

                await Assert.ThrowsAsync<LockInUseException>(async () =>
                    await lockKey.Destroy());

                await lockKey2.Release();

                Assert.False(lockKey2.IsHeld);

                await lockKey.Destroy();
                await lockKey2.Destroy();
            }
            finally
            {
                try
                {
                    await lockKey.Release();
                }
                catch (LockNotHeldException)
                {
                    // Exception expected if above checks all pass
                }
            }
        }

        [Fact]
        public void Lock_RunAction()
        {
            const string keyName = "test/lock/runaction";
            var firstTaskRan = false;
            var secondTaskRan = false;

            Task.WaitAll(Task.Run(async () =>
            {
                await _client.ExecuteLocked(keyName, () =>
                {
                    // Only executes if the lock is held
                    firstTaskRan = true;
                });
            }),
            Task.Run(async () =>
            {
                await _client.ExecuteLocked(keyName, () =>
                {
                    // Only executes if the lock is held
                    secondTaskRan = true;
                });
            }));

            Assert.True(firstTaskRan);
            Assert.True(secondTaskRan);
        }

        [Fact]
        public async Task Lock_ReclaimLock()
        {
            const string keyName = "test/lock/reclaim";

            var sessionRequest = await _client.Session.Create();
            var sessionId = sessionRequest.Response;
            try
            {
                var lock1 = _client.CreateLock(new LockOptions(keyName)
                {
                    Session = sessionId
                });

                var lock2 = _client.CreateLock(new LockOptions(keyName)
                {
                    Session = sessionId
                });

                try
                {
                    await lock1.Acquire(CancellationToken.None);
                    Assert.True(lock1.IsHeld);

                    await TimeoutUtils.WithTimeout(lock2.Acquire(CancellationToken.None));
                    Assert.True(lock2.IsHeld);
                }
                finally
                {
                    await lock1.Release();
                }

                await TimeoutUtils.WaitFor(() => !lock1.IsHeld, "Lock 1 is still held");

                // By releasing lock1, lock2 should also eventually be released as it is for the same session
                await TimeoutUtils.WaitFor(() => !lock2.IsHeld, "Lock 2 is still held");
            }
            finally
            {
                var destroyResponse = await _client.Session.Destroy(sessionId);
                Assert.True(destroyResponse.Response, "Failed to destroy session");
            }
        }

        [Fact]
        public async Task Lock_SemaphoreConflict()
        {
            const string keyName = "test/lock/semaphoreconflict";

            var semaphore = _client.Semaphore(keyName, 2);

            await semaphore.Acquire(CancellationToken.None);

            Assert.True(semaphore.IsHeld);

            var lockKey = _client.CreateLock(keyName + "/.lock");

            await Assert.ThrowsAsync<LockConflictException>(async () =>
                await lockKey.Acquire(CancellationToken.None));

            await Assert.ThrowsAsync<LockConflictException>(async () =>
                await lockKey.Destroy());

            await semaphore.Release();
            await semaphore.Destroy();
        }

        [Fact]
        public async Task Lock_ForceInvalidate()
        {
            const string keyName = "test/lock/forceinvalidate";

            var lockKey = (Lock)_client.CreateLock(keyName);
            try
            {
                await lockKey.Acquire(CancellationToken.None);

                Assert.True(lockKey.IsHeld);

                var checker = TimeoutUtils.WaitFor(
                    () => !lockKey.IsHeld,
                    "Expected session destroy to release lock");

                await _client.Session.Destroy(lockKey.LockSession);

                await checker;
            }
            finally
            {
                try
                {
                    await lockKey.Release();
                    await lockKey.Destroy();
                }
                catch (LockNotHeldException)
                {
                    // Exception expected if above checks all pass
                }
            }
        }

        [Fact]
        public async Task Lock_DeleteKey()
        {
            const string keyName = "test/lock/deletekey";

            var lockKey = (Lock)_client.CreateLock(keyName);
            try
            {
                await lockKey.Acquire(CancellationToken.None);

                Assert.True(lockKey.IsHeld);

                var checker = TimeoutUtils.WaitFor(() => !lockKey.IsHeld, "Expected key delete to release lock");

                await _client.KV.Delete(lockKey.Opts.Key);

                await checker;
            }
            finally
            {
                try
                {
                    await lockKey.Release();
                    await lockKey.Destroy();
                }
                catch (LockNotHeldException)
                {
                    // Exception expected if above checks all pass
                }
            }
        }

        [Fact]
        public async Task Lock_TryAcquireOnceWithLockDelayZeroWaitTime_NoRetryWait()
        {
            const string keyName = "test/lock/acquireoncewithlockdelay_noretrywait";

            var lockKey = (Lock)_client.CreateLock(keyName);
            await lockKey.Acquire(CancellationToken.None);
            Assert.True(lockKey.IsHeld);
            await _client.Session.Destroy(lockKey.LockSession);

            var oneShotLockOptions = new LockOptions(keyName)
            {
                LockTryOnce = true,
                LockWaitTime = TimeSpan.Zero,
                LockRetryTime = TimeSpan.FromSeconds(15)
            };

            var oneShotLock = (Lock)_client.CreateLock(oneShotLockOptions);

            var stopwatch = Stopwatch.StartNew();
            await Assert.ThrowsAsync<LockMaxAttemptsReachedException>(
                async () => await oneShotLock.Acquire(CancellationToken.None));
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            Assert.False(oneShotLock.IsHeld);
            // https://github.com/dotnet/runtime/issues/45585
            Assert.True(elapsedMilliseconds < oneShotLockOptions.LockRetryTime.TotalMilliseconds * 0.9);
        }

        [Fact]
        public async Task Lock_TryAcquireOnceWithLockDelayNonZeroWaitTime_EnsureRetryWait()
        {
            const string keyName = "test/lock/acquireoncewithlockdelay_ensureretrywait";

            var lockKey = (Lock)_client.CreateLock(keyName);
            await lockKey.Acquire(CancellationToken.None);
            Assert.True(lockKey.IsHeld);
            await _client.Session.Destroy(lockKey.LockSession);

            var oneShotLockOptions = new LockOptions(keyName)
            {
                LockTryOnce = true,
                LockWaitTime = TimeSpan.FromSeconds(1),
                LockRetryTime = TimeSpan.FromSeconds(5)
            };

            var oneShotLock = (Lock)_client.CreateLock(oneShotLockOptions);

            var stopwatch = Stopwatch.StartNew();
            await Assert.ThrowsAsync<LockMaxAttemptsReachedException>(
                async () => await oneShotLock.Acquire(CancellationToken.None));
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            Assert.False(oneShotLock.IsHeld);
            // https://github.com/dotnet/runtime/issues/45585
            Assert.True(elapsedMilliseconds > oneShotLockOptions.LockRetryTime.TotalMilliseconds * 0.9);
        }
    }
}
