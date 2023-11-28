# Changelog

## Unreleased

## 1.7.14.1
* Add ServiceTags to the health check endpoint (#244)
* Write correct value for KV.Release (#237)
* Introduce Namespace (#229)
* Add support for logjson (#224)
* Add support for agent cache (#225)
* CI: Use enterprise build (#183)
* Adding more filtering features (#219)
* Basic Connect support for service registration on the agent (#216)

## 1.6.10.9
* Adding properties: AliasService, AliasNode to AgentServiceCheck class for support: (#202)
* Update project url (#212)
* Update CI, drop tests for old frameworks, add net7.0 (#213)

## 1.6.10.8
* Fix to correctly handle query TTL (#187)
* Add `CheckId` to the `AgentServiceCheck` and mark the `ID` as obsolete (#185)

## 1.6.10.7
* Add support for `replace-existing-checks` parameter of service registration endpoint that allows to replace existing checks when re-registering a service (#155)
* Update Newtonsoft.Json reference to version 13.0.1 (#160)

## 1.6.10.6
* Enforce ConfigureAwait(false) on the whole library (#148)
* Resolve synchronous access deadlock by adding ‘ConfigureAwait(false)’ (#146)

## 1.6.10.5
* Check for LockTryOnce before delay (#21)
* Add support for building and testing with .NET 6 (#135)

## 1.6.10.4
* Added the `TaggedAddresses` field to the `AgentService`, `CatalogService` and `AgentServiceRegistration` structures (#111)
* Fix the equality check (#123)
* Added filtering abilities into Agent.Services (#128)
* Fixed serialization of TimeSpan to GoDuration (#129)

## 1.6.10.3
* Fix DLL versioning showing 0.0.0.0 (#112)

## 1.6.10.2
* Add support for net5.0 (#85).
* Use ILRepack instead of ILMerge so .NETFramework builds on all platforms (#92)
* Many CI and build improvements (#101, #96, #89, #100, #95):
  * Unit tests are now run against several versions of the Consul server (1.6.10, 1.7.14, 1.8.14, 1.9.8, 1.10.1, as of this writing).
  * Mono on Unix is used for test runs of target `net461`.
* Initial release of Consul.AspNetCore after retrieving ownership. The versioning of both Consul.AspNetCore and Consul packages are aligned for simplicity.

## 1.6.10.1
* Add GRPC and GRPCUseTLS to the agent service check parameters (#22)
* Compatibility with Consul 1.7.x - RoleLink & PolicyLink (#31)
* Added the possibility to specify more http request options and meta-data when registering service checks (#39)
  It is now possible to specify the http Header(s) (`Headers`), `Method` and `Body` to be used for a given service check.
  A given service check might also now have an identifier (`ID`), `Name` and a description (`Notes`) associated.
* Added the `Type` field to the `AgentCheck` and `HealthCheck` structures (#42)
* Fix issue #24 removing constructor's Obsolete attribute for HttpClient injection (#32)
* Added asp net core integration - Consul.AspNetCore (#17)(#45)
* Fixed lock and semaphore timeouts logic (#64)
* Added service and node filtering abilities into Health.Service (#70)
* Fixed NRE in Lock (#66)
* Lock: Forward user cancellation token downstream (#67)
* Pass WriteOptions parameter to correct parameter of Put(...) (#76)(#77)
* Update consul to v1.6.10 (#78)

## 1.6.1.1
* Fix issue #9 preventing use of the library with .NET Framework

## Forked from PlayFab Inc by G-Research (1.6.1)
* Upgrade to .NET 4.6.1 and .NET Core 2.0
* Add missing code documentation
* General cleanup
* Make unit tests run on .NET framework
* Added new Token, Policy and Role API Entry points to reflect the ACL
changes made in Consul 1.4.0 and 1.5.0
* Aligned to Consul API 1.6.1

## Major Changes between 0.6.4.7 and 0.7.0
* The method of configuring the ConsulClient has been reworked. It now
  uses `Action`s to configure the options inside itself - e.g.
  `var client = new ConsulClient((cfg) => { cfg.Datacenter = "us-west-2"; }`
  See the file `Consul.Test/ClientTest.cs` and the test method
  `Client_Constructors()` for more examples. The old method will work
  but has been made Obsolete.
* The `ExecuteAbortableLocked` method has been removed.
* Requests to use the `X-Consul-Token` header instead of the `?token=`
  query parameter. This may break use of ACLs with very old Consuls (<
  0.6.0). Please file an issue if this breaks for you.

## 2017-01-17
* Fixed up a few leaking tasks in Sessions, Locks and Semaphores, as well as
  properly waiting for the previously-leaked tasks to complete.
  Also made some tasks Long Running in the TPL sense.

## 2017-01-03 (0.7.2.0)
* New APIs ported:
  * Snapshot API. Requires >= Consul 0.7.1 to use.
  * Keyring API. Requires >= Consul 0.7.2 to use.
  * Agent Leave/Reload APIs. Requires >= Consul 0.7.2 to use.
* Added `TLSSkipVerify` and `DeregisterCriticalServiceAfter` to the
  AgentServiceCheck class. Requires >= Consul 0.7.0 to use.
* Added `Health.AggregatedStatus()` extension method to any
  `IEnumerable<HealthCheck>` to roll up a set of checks. Can be used
  with any version of Consul.
* Renamed `CheckStatus` to `HealthStatus`. This affects the
  `AgentServiceCheck` and `AgentCheck` classes.
* Changed the `Health.HealthCheck` `Status` field to be of type
  `HealthStatus` instead of `string`.

## 2016-12-29 (0.7.0.5)
* Added missing nuget info to project.json. Thanks @latop2604!

## 2016-12-20
* Added a couple missing interfaces to IConsulClient. Thanks
  @chrishoffman!

## 2016-11-02
* Cleaned up a few minor logic errors and made disposal of internally-used
  `CancellationTokenSources` a lot more explicit instead of leaking them.

## 2016-10-10
* Revert inadvertent upgrade to .NET 4.5.1.

## 2016-09-21
* Fixed a variety of bugs that could cause sessions to be destroyed if provided
  to a Lock or Semaphore via a `LockOptions` or `SemaphoreOptions` object.

## 2016-08-17
* Ported in changes from the Consul Go API for 0.7.0. Most of these
  require 0.7.0 servers/agents. The changes are:
  * Atomic transactions for the KV store
  * Only retry locks/semaphores on Consul errors, not on all errors
  * Add the `Near` property to Prepared Queries
  * Add Query Templates to Prepared Queries, with regex filtering
  * Change all requests to use the `X-Consul-Token` header instead of
    the `?token=` query parameter.
  * Add the ability to deregister a service that has been critical for
    an arbitrary period of time.
  * Signal WAN address translation and add the ability to look up
    the WAN and LAN addresses if address translation is being used.
  * Added Operator API to allow Raft editing.

## 2016-08-03
* Added the ability to set `LockOpts.LockRetryTime`. Thanks @pfrejlich!

## 2016-07-10
* Add an optional CancellationToken parameter to every method that ends up
  doing an HTTP request. Some of these can create an unstable Consul state
  (e.g.  allowing the release of a distribted Semaphore to be canceled) but in
  many cases they should only be used if the call can possibly fail and a
  secondary timeout is needed.

## 2016-07-07
* Add .NET Core port and build process thanks to work by @akatz0813.
* Converted all Locks and Semaphores to be totally `async` thanks to
  work by @mjgoethe.
* Entirely removed the method `ExecuteAbortableLocked` and all
  functionality around aborting a thread based on a Consul lock.
* Reworked configuration of the `ConsulClient` to use `Action<T>` to
  configure options.

## 2016-06-10
* Correct the behavior of `LockTryOnce/SemaphoreTryOnce` so that it now
  properly waits one multiple of the WaitTime before exiting in case of it
  already being held.

## 2016-05-27
* Disable Client Certificates on Mono since the certificate handler is
  not implemented at the Mono library level.

## 2016-05-24
* Added missing CancellationToken overrides to allow long polling for
  `Catalog.Node()` and `Catalog.Service()`.

## 2016-05-16
* Fixed configuration reuse between multiple clients so multiple
  `ConsulClient`s that exist one after the other that both reference the same
  configuration do not spuriously dispose of part of the
  `ConsulClientConfiguration`.

## 2016-04-29
* The `Newtonsoft.Json` DLL is now ILMerged into the `Consul` DLL, so
  there should be no more issues with mismatched JSON.NET versions in
  user projects. Thanks @grounded042!

## 2016-03-23
* Fix a bug where setting `ConsulClientConfiguration.WaitTime` would cause 400
  Bad Request responses. Also converted `QueryOptions.WaitTime` to a nullable
  timespan to match the `ConsulClientConfiguration` property of the same
  name/purpose.

## 2016-03-19
* Fix a bug where the StatusCode was not being set correctly on some result
  types.

## 2016-03-16
* Port in Consul 0.6.4 API since 0.6.4 is now released, which was just an
  update to `UpdateTTL` and a rename of some of the strings.
* Moved all the helper objects (`TTLStatus`, `CheckStatus.Passing`, etc.) to
  reference `static readonly` instances to cut down on allocation and ease
  comparison.
* Marked AbortableLock obselete since `Thread.Abort` doesn't exist in DNXCore
  and it's contrary to the Task philosophy to abort threads.

## 2016-03-15
* Add missing `IDisposable` to `IConsulClient`.
* Made the setters for `CreateIndex` and `ModifyIndex` on `ACLEntry`, `KVPair`
  and `SessionEntry` public to allow for easier unit testing.
* Ported the `EnableTagOverride` feature from
  https://github.com/hashicorp/consul/commit/afdeb2f1fc189c5a9e6440c27c1918e7b09c2cdc

## 2016-03-10
* Added a `ConsulClient(ConsulClientConfiguration, HttpClient)` constructor
  that allows a user to pass in a custom HttpClient to set a custom proxy
  setting/timeout.

## 2016-03-04
* Fixed double-encoding of `UpdateTTL` `note` argument.

## 2016-02-24
* Removed use of PushStreamContent to fix Mono problems.

## 2016-02-10
* Added the ability to use Client Certificates to authenticate a client against
  a Consul agent endpoint that is protected by some other service. See the
  `ClientCertificate` property of `ConsulClientConfiguration`. Thanks @AndyRB!
* Fixed a possible deadlock in the Session `RenewPeriodic` method.

## 2016-02-09
* Implemented the IDisposable Pattern for the `ConsulClient` class.
  `ConsulClient` objects should now have `Dispose()` called on them to properly
  clean up. Thanks @TMaster!
* Cleaned up the Prepared Queries endpoint stack.
* Fixed a timing bug in one of the client execute calls.
* Added Docker checks
* Added the ability for Semaphores and Locks to ride out brief periods of
  failure using the `MonitorRetries` and `MonitorRetryTime` fields in
  `LockOptions` and `SemaphoreOptions` classes.
* Added the ability for Semaphores and Locks to have configurable `WaitTime`
  values, as well as to operate in `TryOnce` mode, which means it attempts to
  acquire once and throws an exception if the acquisition was not successful.
  To use these, set the `LockWaitTime` and `LockTryOnce` fields on the
  `LockOptions` class and the `SemaphoreWaitTime` and `SemaphoreTryOnce` fields
  on the `SemaphoreOptions` class.

## 2016-02-07
* Reduce the callstack and task overhead by returning the originating
  Task where possible. Thanks @TMaster!

## 2016-01-12
* Rewrote entire API to be `async`.
* Added Prepared Queries from Consul 0.6.0.

## 2015-11-21
* Reworked the entire Client class to use `System.Net.HttpClient` as its
  underpinning rather than `WebRequest`/`WebResponse`.
* Moved all tests to Xunit.
* Converted all uses of `System.Web.HttpUtility.UrlPathEncode` to
  `System.Uri.EscapeDataString`.

## 2015-11-09

* Added coordinates API. *WARNING*: No check is done to see if the API
  is available. If used against a 0.5.2 agent, an exception will be
  thrown because the agent does not understand the coordinate URL.
* Fixed bug in tests for session renewal.

## 2015-11-03

* Fixed a bug where the node name was not deserialized when using the
  `Catalog.Nodes()` endpoint. Thanks @lockwobr!
* Fixed a bug where a zero timespan could not be specified for Lock
  Delays, TTLs or Check Intervals. Thanks @eugenyshchelkanov!

## 2015-10-24

* Port in changes from hashicorp/consul master:
  * Add TCP check type
  * BEHAVIOR CHANGE: Changed Session.Renew() to now throw a
    SessionExpiredException when the session does not exist on the
    Consul server
  * BEHAVIOR CHANGE: Changed all the KV write methods (Put, Delete,
    DeleteTree, DeleteCAS, CAS, Release, Acquire) to throw an
    InvalidKeyPairException if the key path or prefix begins with a `/`
    character.
* Fixed documentation typos.

## 2015-08-27

* Convert all uses of
  [System.Web.HttpUtility.UrlEncode](https://msdn.microsoft.com/en-us/library/system.web.httputility.urlencode)
  and corresponding UrlDecode calls to
  [UrlPathEncode](https://msdn.microsoft.com/en-us/library/system.web.httputility.urlpathencode)/Decode.
  This is was because UrlEncode encodes spaces as a `+` symbol rather
  than the hex `%20` as expected.

## 2015-08-26

* Fix a NullReferenceException when the Consul connection is down and
  the WebException returned has an empty response.

## 2015-07-25

* BREAKING CHANGE: Renamed `Client` class to `ConsulClient` and `Config`
  to `ConsulClientConfiguration` to reduce confusion.
* Completed major rework of the Client class to remove unneeded type
  parameters from various internal calls
* Added interfaces to all the endpoint classes so that test mocking is
  possible.
