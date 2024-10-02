# Guide: Adding Integration Tests for Illegal Dump Site Detection Project  

## Table of Contents  

1. [Introduction](#introduction)  
2. [Setup](#setup)  
   - [Environment Variables](#environment-variables)  
   - [TestDatabaseFixture](#testdatabasefixture)  
3. [Writing Integration Tests](#writing-integration-tests)  
   - [Test Class Setup](#test-class-setup)  
   - [Individual Test Structure](#individual-test-structure)  
4. [Best Practices](#best-practices)  
5. [Example: Adding a New Integration Test](#example-adding-a-new-integration-test)  
6. [Troubleshooting](#troubleshooting)  

## Introduction  

This guide outlines the process of adding integration tests to the Illegal Dump Site Detection and Landfill Monitoring project. These tests ensure that various components of the system work correctly together, particularly focusing on database operations and repository patterns.  

## Setup  

### Environment Variables  

Before running the tests, ensure that the `TestConnectionString` environment variable is set. If not set, the tests will use a default connection string:  

```
Host=localhost;Port=5434;Database=waste_detection_test_v10;Username=postgres;Password=admin;Pooling=true;
```

You can set the environment variable in your CI/CD pipeline or locally before running the tests.  

### TestDatabaseFixture  

The `TestDatabaseFixture` class is crucial for setting up the test database. It handles:  

1. Creating a database context  
2. Applying migrations  
3. Seeding test data  

Key points:  

- It implements `IDisposable` to clean up resources after tests.  
- It provides a `CreateDbContext()` method to create new context instances for tests.  
- The `SeedDatabase()` method populates the database with test data.  

## Writing Integration Tests  

### Test Class Setup  

1. Create a new test class in the appropriate namespace (e.g., `Tests.DalTests.Repositories`).  
2. Inherit from `IClassFixture<TestDatabaseFixture>` to use the shared database fixture.  
3. Add the `[Trait("Category", "Integration")]` attribute to categorize the tests.  

Example:  

```csharp
namespace Tests.DalTests.Repositories
{
    [Trait("Category", "Integration")]
    public class NewRepositoryTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _fixture;

        public NewRepositoryTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        // Tests go here
    }
}
```

### Individual Test Structure  

Each test method should follow this general structure:  

1. Arrange: Set up the test data and context  
2. Act: Perform the operation being tested  
3. Assert: Verify the results  
4. Rollback: Ensure the database is left in its original state  

Example structure:  

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    using ApplicationDbContext dbContext = _fixture.CreateDbContext();
    dbContext.AuditDisabled = true;
    using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
    _fixture.SeedDatabase(dbContext);
    
    // Additional setup if needed
    
    // Act
    // Perform the operation being tested
    
    // Assert
    // Verify the results
    
    // Rollback
    transaction.Rollback();
}
```

## Best Practices  

1. Use descriptive test names that explain the scenario and expected behavior.  
2. Isolate each test by using transactions and rolling back changes.  
3. Disable auditing for tests to avoid unnecessary complications.  
4. Use `async/await` for asynchronous operations.  
5. Test both successful and failure scenarios.  
6. Use meaningful assertions that clearly indicate what's being tested.  

## Example: Adding a New Integration Test  

Let's add a new test for the `DatasetsRepository` to check if we can retrieve datasets by a specific creator:  

```csharp
[Fact]
public async Task GetAll_ShouldReturnDatasetsForSpecificCreator_WhenFilterProvided()
{
    // Arrange
    using ApplicationDbContext dbContext = _fixture.CreateDbContext();
    dbContext.AuditDisabled = true;
    using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
    _fixture.SeedDatabase(dbContext);
    DatasetsRepository repository = new DatasetsRepository(dbContext);
    string creatorId = UserSeedData.FirstUser.Id;

    // Act
    ResultDTO<IEnumerable<Dataset>> result = await repository.GetAll(
        filter: d => d.CreatedById == creatorId
    );

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Data);
    Assert.All(result.Data, d => Assert.Equal(creatorId, d.CreatedById));

    // Rollback
    transaction.Rollback();
}
```

## Troubleshooting  

1. **Database connection issues**: Ensure the connection string is correct and the database server is running.  
2. **Failed migrations**: Check if all migrations are up to date and can be applied to a clean database.  
3. **Inconsistent test data**: Verify that the `SeedDatabase` method is correctly populating all required data.  
4. **Slow tests**: Consider using in-memory databases for faster execution, but be aware of potential differences from the actual database.  
5. **Flaky tests**: Ensure proper isolation between tests and avoid dependencies on external systems when possible.  

By following this guide, you can effectively add and maintain integration tests for the Illegal Dump Site Detection and Landfill Monitoring project, ensuring the reliability and correctness of your data access layer and repository implementations.  
