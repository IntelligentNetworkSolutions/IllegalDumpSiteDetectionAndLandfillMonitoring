using DAL.ApplicationStorage;
using DAL.ApplicationStorage.SeedDatabase.TestSeedData;
using Microsoft.EntityFrameworkCore;

public class TestDatabaseFixture : IDisposable
{
    private readonly string _connectionString;
    public ApplicationDbContextFactory DbContextFactory { get; }
    public ApplicationDbContext DbContext { get; private set; }

    public TestDatabaseFixture()
    {
        string? testConnectionEnv = Environment.GetEnvironmentVariable("TestConnectionString");
        _connectionString = string.IsNullOrEmpty(testConnectionEnv)
            ? $"Host=localhost;Port=5434;Database=waste_detection_tests_v1;Username=postgres;Password=postgres;Pooling=true;"
            : testConnectionEnv;

        DbContextFactory = new ApplicationDbContextFactory();

        // Use the factory to create the context for migrations and database setup
        DbContext = CreateDbContext();
        DbContext.AuditDisabled = true;
        if (DbContext.Database.GetPendingMigrations().Count() > 0)
            DbContext.Database.Migrate();
    }

    public ApplicationDbContext CreateDbContext()
    {
        return DbContextFactory.CreateDbContext(new string[] { _connectionString });
    }

    public void SeedDatabase(ApplicationDbContext? dbContext = null)
    {
        try
        {
            UserSeedData.SeedData(dbContext is null ? DbContext : dbContext);
            DatasetsSeedData.SeedData(dbContext is null ? DbContext : dbContext);
            DatasetClassesSeedData.SeedData(dbContext is null ? DbContext : dbContext);
            DatasetDatasetClassesSeedData.SeedData(dbContext is null ? DbContext : dbContext);
            DatasetImagesSeedData.SeedData(dbContext is null ? DbContext : dbContext);
            ImageAnnotationsSeedData.SeedData(dbContext is null ? DbContext : dbContext);

            if (dbContext is null)
                DbContext.SaveChanges();
            else
                dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while seeding the database.", ex);
        }
    }
    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}