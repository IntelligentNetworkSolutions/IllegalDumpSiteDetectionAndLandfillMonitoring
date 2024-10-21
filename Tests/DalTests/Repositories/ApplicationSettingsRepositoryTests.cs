using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tests.DalTests.Repositories
{
    [Trait("Category", "Integration")]
    [Collection("Shared TestDatabaseFixture")]
    public class ApplicationSettingsRepositoryTests
    {
        private readonly TestDatabaseFixture _fixture;

        public ApplicationSettingsRepositoryTests(TestDatabaseFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task CreateApplicationSetting_ShouldCreateNewSetting()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ApplicationSettingsRepository repository = new ApplicationSettingsRepository(dbContext);
            ApplicationSettings newSetting = new ApplicationSettings { Key = "TestKey", Value = "TestValue", Description = "TestDesc" };

            // Act
            bool result = await repository.CreateApplicationSetting(newSetting);

            // Assert
            Assert.True(result);
            ApplicationSettings? createdSetting = await dbContext.ApplicationSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Key == "TestKey");
            Assert.NotNull(createdSetting);
            Assert.Equal("TestValue", createdSetting.Value);
            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteApplicationSettingByKey_ShouldDeleteExistingSetting()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ApplicationSettingsRepository repository = new ApplicationSettingsRepository(dbContext);
            ApplicationSettings setting = new ApplicationSettings { Key = "TestKey", Value = "TestValue", Description = "TestDesc" };
            dbContext.ApplicationSettings.Add(setting);
            await dbContext.SaveChangesAsync();

            // Act
            bool? result = await repository.DeleteApplicationSettingByKey("TestKey");

            // Assert
            Assert.True(result);
            ApplicationSettings? deletedSetting = await dbContext.ApplicationSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Key == "TestKey");
            Assert.Null(deletedSetting);
            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteApplicationSettingByKey_ShouldReturnNullForNonExistentKey()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ApplicationSettingsRepository repository = new ApplicationSettingsRepository(dbContext);

            // Act
            bool? result = await repository.DeleteApplicationSettingByKey("NonExistentKey");

            // Assert
            Assert.Null(result);
            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateApplicationSetting_ShouldUpdateExistingSetting()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ApplicationSettingsRepository repository = new ApplicationSettingsRepository(dbContext);
            ApplicationSettings setting = new ApplicationSettings { Key = "TestKey", Value = "OldValue", Description = "TestDesc" };
            dbContext.ApplicationSettings.Add(setting);
            await dbContext.SaveChangesAsync();

            // Act
            setting.Value = "NewValue";
            bool result = await repository.UpdateApplicationSetting(setting);

            // Assert
            Assert.True(result);
            ApplicationSettings? updatedSetting = await dbContext.ApplicationSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Key == "TestKey");
            Assert.NotNull(updatedSetting);
            Assert.Equal("NewValue", updatedSetting.Value);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetAllApplicationSettingsAsList_ShouldReturnAllSettings()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ApplicationSettingsRepository repository = new ApplicationSettingsRepository(dbContext);
            List<ApplicationSettings> settings =
                [ new ApplicationSettings { Key = "Key1", Value = "Value1", Description = "TestDesc" }, 
                    new ApplicationSettings { Key = "Key2", Value = "Value2", Description = "TestDesc" } ];
            dbContext.ApplicationSettings.AddRange(settings);
            await dbContext.SaveChangesAsync();

            // Act
            List<ApplicationSettings> result = await repository.GetAllApplicationSettingsAsList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Key == "Key1" && s.Value == "Value1");
            Assert.Contains(result, s => s.Key == "Key2" && s.Value == "Value2");
            transaction.Rollback();
        }

        [Fact]
        public async Task GetAllApplicationSettingsAsQueryable_ShouldReturnQueryable()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ApplicationSettingsRepository repository = new ApplicationSettingsRepository(dbContext);
            List<ApplicationSettings> settings =
                [ new ApplicationSettings { Key = "Key1", Value = "Value1", Description = "TestDesc" }, 
                    new ApplicationSettings { Key = "Key2", Value = "Value2", Description = "TestDesc" } ];
            dbContext.ApplicationSettings.AddRange(settings);
            await dbContext.SaveChangesAsync();

            // Act
            IQueryable<ApplicationSettings> result = await repository.GetAllApplicationSettingsAsQueryable();

            // Assert
            Assert.IsAssignableFrom<IQueryable<ApplicationSettings>>(result);
            Assert.Equal(2, await result.CountAsync());
            transaction.Rollback();
        }

        [Fact]
        public async Task GetAllApplicationSettingsKeysAsList_ShouldReturnAllKeys()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ApplicationSettingsRepository repository = new ApplicationSettingsRepository(dbContext);
            List<ApplicationSettings> settings =
                [ new ApplicationSettings { Key = "Key1", Value = "Value1", Description = "TestDesc" }, 
                    new ApplicationSettings { Key = "Key2", Value = "Value2", Description = "TestDesc" } ];
            dbContext.ApplicationSettings.AddRange(settings);
            await dbContext.SaveChangesAsync();

            // Act
            List<string> result = await repository.GetAllApplicationSettingsKeysAsList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains("Key1", result);
            Assert.Contains("Key2", result);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetApplicationSettingByKey_ShouldReturnSettingForExistingKey()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ApplicationSettingsRepository repository = new ApplicationSettingsRepository(dbContext);
            ApplicationSettings setting = new ApplicationSettings { Key = "TestKey", Value = "TestValue", Description = "TestDesc" };
            dbContext.ApplicationSettings.Add(setting);
            await dbContext.SaveChangesAsync();

            // Act
            ApplicationSettings? result = await repository.GetApplicationSettingByKey("TestKey");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestValue", result.Value);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetApplicationSettingByKey_ShouldReturnNullForNonExistentKey()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ApplicationSettingsRepository repository = new ApplicationSettingsRepository(dbContext);

            // Act
            ApplicationSettings? result = await repository.GetApplicationSettingByKey("NonExistentKey");

            // Assert
            Assert.Null(result);
            transaction.Rollback();
        }
    }
}
