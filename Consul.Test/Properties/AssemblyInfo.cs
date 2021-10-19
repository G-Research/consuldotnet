using Xunit;


// Tests within the same test class (or same test collection) will not run in parallel against each other.
// By default, each test class is a unique test collection, but we put all test classes that are not decorated with
// CollectionAttribute into the assembly-level collection instead.
// This is to limit the number of tests that are going to be run in parallel so we can reduce the number
// of concurrent http connections that are required to complete all tests.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
