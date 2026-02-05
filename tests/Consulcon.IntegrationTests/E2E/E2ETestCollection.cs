namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Collection definition for E2E tests.
/// All tests in this collection share the same E2ETestFixture instance,
/// meaning the test database is created once before all tests and deleted after all tests complete.
/// </summary>
[CollectionDefinition("E2E Tests")]
public class E2ETestCollection : ICollectionFixture<E2ETestFixture>
{
    // This class has no code, it's just a marker for xUnit.
    // The fixture (E2ETestFixture) will be instantiated once per collection,
    // and shared across all test classes in the collection.
}
