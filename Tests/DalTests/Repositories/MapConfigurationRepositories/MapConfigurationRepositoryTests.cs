using DAL.ApplicationStorage;
using DAL.Repositories.MapConfigurationRepositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Tests.DalTests.Repositories.MapConfigurationRepositories
{
    [Trait("Category", "Integration")]
    [Collection("Shared TestDatabaseFixture")]
    public class MapConfigurationRepositoryTests
    {
        private readonly TestDatabaseFixture _fixture;

        public MapConfigurationRepositoryTests(TestDatabaseFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task GetMapConfigurationByName_ShouldReturnMap()
        {
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<MapConfigurationRepository> logger = loggerFactory.CreateLogger<MapConfigurationRepository>();
            var repository = new MapConfigurationRepository(dbContext, logger);

            var result = await repository.GetMapConfigurationByName("test map name");
            Assert.NotNull(result.Data);
            Assert.Equal("test map name", result.Data.MapName);

            transaction.Rollback();

        }

        [Fact]
        public async Task GetMapConfigurationByName_ShouldReturnFailure_WhenMapDoesNotExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<MapConfigurationRepository> logger = loggerFactory.CreateLogger<MapConfigurationRepository>();
            var repository = new MapConfigurationRepository(dbContext, logger);

            // Act
            var result = await repository.GetMapConfigurationByName("non-existent map name");

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to get map configuration from db", result.ErrMsg);
            Assert.Null(result.Data);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetMapConfigurationByName_ShouldReturnExceptionFailure_WhenExceptionOccurs()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<MapConfigurationRepository> logger = loggerFactory.CreateLogger<MapConfigurationRepository>();
            var repository = new MapConfigurationRepository(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            // Act
            var exceptionResult = await repository.GetMapConfigurationByName("test map name");

            // Assert
            Assert.NotNull(exceptionResult);
            Assert.False(exceptionResult.IsSuccess);
            Assert.Null(exceptionResult.Data);
        }


        [Fact]
        public async Task GetMapConfigurationByName_ShouldReturnMapWithLayerConfigurations()
        {
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<MapConfigurationRepository> logger = loggerFactory.CreateLogger<MapConfigurationRepository>();
            var repository = new MapConfigurationRepository(dbContext, logger);

            var result = await repository.GetMapConfigurationByName("test map name");

            Assert.NotNull(result);
            Assert.Equal("test map name", result.Data.MapName);
            Assert.NotEmpty(result.Data.MapLayerConfigurations);

            transaction.Rollback();
        }


    }
}
