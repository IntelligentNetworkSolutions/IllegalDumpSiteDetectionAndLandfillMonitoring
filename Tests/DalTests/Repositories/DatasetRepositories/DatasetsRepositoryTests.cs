using DAL.ApplicationStorage;
using DAL.ApplicationStorage.SeedDatabase.TestSeedData;
using DAL.Repositories.DatasetRepositories;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SD;
using System.Linq.Expressions;

namespace Tests.DalTests.Repositories.DatasetRepositories
{
    [Trait("Category", "Integration")]
    [Collection("Shared TestDatabaseFixture")]
    public class DatasetsRepositoryTests
    {
        private readonly TestDatabaseFixture _fixture;

        public DatasetsRepositoryTests(TestDatabaseFixture fixture) => _fixture = fixture;

        #region Create
        [Fact]
        public async Task Create_ShouldAddDataset_WhenDatasetIsValid()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            Dataset newDataset = new Dataset
            {
                Id = Guid.NewGuid(),
                Name = "Test Dataset",
                Description = "Testing DAL",
                CreatedById = UserSeedData.FirstUser.Id
            };

            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).Create(newDataset);

            // Assert
            Assert.True(result.IsSuccess);
            Dataset? dbDataset = await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == newDataset.Id);
            Assert.NotNull(dbDataset);
            Assert.Equal("Test Dataset", dbDataset.Name);
            transaction.Rollback();
        }

        [Fact]
        public async Task Create_ShouldFail_WhenEntityIsNull()
        {
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).Create(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid entity, is null", result.ErrMsg);
            transaction.Rollback();
        }

        [Fact]
        public async Task Create_ShouldFail_WhenSaveChangesFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            // Simulate failure by adding same Id (assuming the save will fail)
            Dataset newDataset = new()
            {
                Id = DatasetsSeedData.FirstDataset.Id,
                Name = "Test Dataset",
                Description = "Testing DAL",
                CreatedById = UserSeedData.FirstUser.Id
            };

            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).Create(newDataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.False(string.IsNullOrEmpty(result.ErrMsg));
            transaction.Rollback();
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            Dataset invalidDataset = new Dataset { Id = Guid.Empty, Name = null }; // Invalid dataset to trigger exception

            // Act
            ResultDTO result = await repository.Create(invalidDataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
            transaction.Rollback();
        }

        [Fact]
        public async Task CreateAndReturnEntity_ShouldCreateAndReturnEntity_WhenEntityIsValid()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            Dataset newDataset = new Dataset
            {
                Id = Guid.NewGuid(),
                Name = "Test Dataset",
                Description = "Testing DAL",
                CreatedById = UserSeedData.FirstUser.Id
            };

            // Act
            ResultDTO<Dataset> result = await new DatasetsRepository(dbContext).CreateAndReturnEntity(newDataset);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Test Dataset", result.Data.Name);

            Dataset? dbDataset = await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == newDataset.Id);
            Assert.NotNull(dbDataset);
            Assert.Equal(newDataset.Id, dbDataset.Id);
            transaction.Rollback();
        }

        [Fact]
        public async Task CreateAndReturnEntity_ShouldFail_WhenEntityIsNull()
        {
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            // Act
            ResultDTO<Dataset> result = await new DatasetsRepository(dbContext).CreateAndReturnEntity(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid entity, is null", result.ErrMsg);
            transaction.Rollback();
        }

        [Fact]
        public async Task CreateAndReturnEntity_ShouldFail_WhenSaveChangesFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            // Simulate failure by adding same Id (assuming the save will fail)
            Dataset newDataset = new()
            {
                Id = DatasetsSeedData.FirstDataset.Id,
                Name = "Test Dataset",
                Description = "Testing DAL",
                CreatedById = UserSeedData.FirstUser.Id
            };

            // Act
            ResultDTO<Dataset> result = await new DatasetsRepository(dbContext).CreateAndReturnEntity(newDataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.False(string.IsNullOrEmpty(result.ErrMsg));
            transaction.Rollback();
        }

        [Fact]
        public async Task CreateAndReturnEntity_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            Dataset invalidDataset = new Dataset { Id = Guid.Empty, Name = null }; // Invalid dataset to trigger exception

            // Act
            ResultDTO<Dataset> result = await repository.CreateAndReturnEntity(invalidDataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
            transaction.Rollback();
        }

        [Fact]
        public async Task CreateRange_ShouldAddAllEntities_WhenEntitiesAreValid()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            List<Dataset> datasets = [ new Dataset { Id = Guid.NewGuid(), Name = "Test Dataset 1",
                                            Description = "Testing DAL 1", CreatedById = UserSeedData.FirstUser.Id },
                                        new Dataset { Id = Guid.NewGuid(), Name = "Test Dataset 2",
                                            Description = "Testing DAL 2", CreatedById = UserSeedData.FirstUser.Id }
                                        ];

            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).CreateRange(datasets);

            // Assert
            Assert.True(result.IsSuccess);

            Dataset? dbDataset1 = await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == datasets[0].Id);
            Dataset? dbDataset2 = await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == datasets[1].Id);

            Assert.NotNull(dbDataset1);
            Assert.NotNull(dbDataset2);
            transaction.Rollback();
        }

        [Fact]
        public async Task CreateRange_ShouldFail_WhenEntitiesAreNullOrEmpty()
        {
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            // Act with null entities
            ResultDTO resultWithNull = await new DatasetsRepository(dbContext).CreateRange(null);

            // Assert for null
            Assert.False(resultWithNull.IsSuccess);
            Assert.Equal("Invalid entities, enumerable is null or empty", resultWithNull.ErrMsg);

            // Act with empty list
            ResultDTO resultWithEmpty = await new DatasetsRepository(dbContext).CreateRange([]);

            // Assert for empty list
            Assert.False(resultWithEmpty.IsSuccess);
            //Assert.Equal("Invalid entities, enumerable is null or empty", resultWithEmpty.ErrMsg);
            transaction.Rollback();
        }

        [Fact]
        public async Task CreateRange_ShouldFail_WhenSaveChangesFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            // Simulate failure by adding same Id (assuming the save will fail)
            List<Dataset> datasets = [ new Dataset { Id = DatasetsSeedData.FirstDataset.Id, Name = "Test Dataset 1",
                                            Description = "Testing DAL 1", CreatedById = UserSeedData.FirstUser.Id },
                                        new Dataset { Id = DatasetsSeedData.SecondDataset.Id, Name = "Test Dataset 2",
                                            Description = "Testing DAL 2", CreatedById = UserSeedData.FirstUser.Id }
                                        ];

            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).CreateRange(datasets);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.False(string.IsNullOrEmpty(result.ErrMsg));
            transaction.Rollback();
        }

        [Fact]
        public async Task CreateRange_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            List<Dataset> invalidDatasets = [new Dataset { Id = Guid.Empty, Name = null }, new Dataset { Id = Guid.Empty, Name = null }];

            // Act
            ResultDTO result = await repository.CreateRange(invalidDatasets);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
            transaction.Rollback();
        }

        #endregion

        #region Update
        [Fact]
        public async Task Update_ShouldModifyDataset_WhenDatasetExists()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            DatasetsRepository repository = new DatasetsRepository(dbContext);
            _fixture.SeedDatabase(dbContext);
            Dataset updatedDataset = new()
            {
                Id = Guid.NewGuid(),
                Name = "Updated Dataset",
                Description = "Testing DAL",
                CreatedById = UserSeedData.FirstUser.Id
            };
            await dbContext.Datasets.AddAsync(updatedDataset);
            await dbContext.SaveChangesAsync();

            // Act
            ResultDTO result = await repository.Update(updatedDataset);

            // Assert
            Assert.True(result.IsSuccess);
            Dataset? dbDataset = await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == updatedDataset.Id);
            Assert.NotNull(dbDataset);
            Assert.Equal("Updated Dataset", dbDataset.Name);
            transaction.Rollback();
        }

        [Fact]
        public async Task Update_ShouldFail_WhenEntityIsNull()
        {
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).Update(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid entity, is null", result.ErrMsg);
            transaction.Rollback();
        }

        [Fact]
        public async Task Update_ShouldFail_WhenSaveChangesFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            Dataset updatedDataset = new()
            {
                Id = DatasetsSeedData.FirstDataset.Id,
                ParentDatasetId = DatasetsSeedData.FirstDataset.ParentDatasetId,
                Name = null,
                UpdatedById = DatasetsSeedData.FirstDataset.UpdatedById,
                Description = DatasetsSeedData.FirstDataset.Description,
                IsPublished = DatasetsSeedData.FirstDataset.IsPublished,
                CreatedOn = DatasetsSeedData.FirstDataset.CreatedOn,
                CreatedById = DatasetsSeedData.FirstDataset.CreatedById,
            };

            //updatedDataset.Name = null;

            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).Update(updatedDataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(string.IsNullOrEmpty(result.ErrMsg) == false);
            transaction.Rollback();
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            Dataset invalidDataset = new Dataset { Id = DatasetsSeedData.FirstDataset.Id, Name = null }; // Invalid update to trigger exception

            // Act
            ResultDTO result = await repository.Update(invalidDataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateAndReturnEntity_ShouldUpdateAndReturnDataset_WhenDatasetExists()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            Dataset updatedDataset = new()
            {
                Id = Guid.NewGuid(),
                Name = "Updated Dataset",
                Description = "Testing DAL",
                CreatedById = UserSeedData.FirstUser.Id
            };
            await dbContext.Datasets.AddAsync(updatedDataset);
            await dbContext.SaveChangesAsync();
            // Act
            ResultDTO<Dataset> result = await repository.UpdateAndReturnEntity(updatedDataset);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Updated Dataset", result.Data.Name);
            Dataset? dbDataset = await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == result.Data.Id);
            Assert.Equal("Updated Dataset", dbDataset.Name);
            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateAndReturnEntity_ShouldFail_WhenEntityIsNull()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);

            // Act
            ResultDTO<Dataset> result = await repository.UpdateAndReturnEntity(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid entity, is null", result.ErrMsg);
            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateAndReturnEntity_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            Dataset invalidDataset = new Dataset { Id = DatasetsSeedData.FirstDataset.Id, Name = null }; // Invalid update to trigger exception

            // Act
            ResultDTO<Dataset> result = await repository.UpdateAndReturnEntity(invalidDataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateRange_ShouldUpdateMultipleDatasets_WhenDatasetsExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);

            List<Dataset> datasets = [ new Dataset { Id = Guid.NewGuid(), Name = "Updated First Dataset",
                                                        Description = "Testing DAL", CreatedById = UserSeedData.FirstUser.Id },
                                        new Dataset { Id = Guid.NewGuid(), Name = "Updated Second Dataset",
                                                        Description = "Testing DAL", CreatedById = UserSeedData.FirstUser.Id}
                                        ];
            await dbContext.Datasets.AddRangeAsync(datasets);
            await dbContext.SaveChangesAsync();

            // Act
            ResultDTO result = await repository.UpdateRange(datasets);

            // Assert
            Assert.True(result.IsSuccess);
            Dataset? updatedFirstDataset =
                await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == datasets[0].Id);
            Dataset? updatedSecondDataset =
                await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == datasets[1].Id);
            Assert.Equal("Updated First Dataset", updatedFirstDataset.Name);
            Assert.Equal("Updated Second Dataset", updatedSecondDataset.Name);
            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateRange_ShouldFail_WhenEntitiesAreNullOrEmpty()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);

            // Act
            ResultDTO result = await repository.UpdateRange(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid entities, enumerable is null or empty", result.ErrMsg);
            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateRange_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            List<Dataset> invalidDatasets = [ new Dataset { Id = DatasetsSeedData.FirstDataset.Id, Name = null },
                                                new Dataset { Id = DatasetsSeedData.SecondDataset.Id, Name = null }];

            // Act
            ResultDTO result = await repository.UpdateRange(invalidDatasets);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
            transaction.Rollback();
        }
        #endregion

        #region Delete
        [Fact]
        public async Task Delete_ShouldRemoveDataset_WhenDatasetExists()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).Delete(DatasetsSeedData.FirstDataset);

            // Assert
            Assert.True(result.IsSuccess);
            Dataset? dbDataset = await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == DatasetsSeedData.FirstDataset.Id);
            Assert.Null(dbDataset);
            transaction.Rollback();
        }

        [Fact]
        public async Task Delete_ShouldFail_WhenEntityIsNull()
        {
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).Delete(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid entity, is null", result.ErrMsg);
            transaction.Rollback();
        }

        [Fact]
        public async Task Delete_ShouldFail_WhenSaveChangesFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            // Simulate failure by deleting random Id (assuming the save will fail)
            Dataset newDataset = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Dataset",
                Description = "Testing DAL",
                CreatedById = UserSeedData.FirstUser.Id
            };

            // Act
            ResultDTO result = await new DatasetsRepository(dbContext).Delete(newDataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.False(string.IsNullOrEmpty(result.ErrMsg));
            transaction.Rollback();
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            Dataset nonExistentDataset = new() { Id = Guid.NewGuid() }; // Non-existent dataset to trigger exception

            // Act
            ResultDTO result = await repository.Delete(nonExistentDataset);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteRange_ShouldDeleteMultipleDatasets_WhenDatasetsExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            List<Dataset> datasetsToDelete = [DatasetsSeedData.FirstDataset, DatasetsSeedData.SecondDataset];

            // Act
            ResultDTO result = await repository.DeleteRange(datasetsToDelete);

            // Assert
            Assert.True(result.IsSuccess);
            Dataset? deletedFirstDataset = await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == DatasetsSeedData.FirstDataset.Id);
            Dataset? deletedSecondDataset = await dbContext.Datasets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == DatasetsSeedData.SecondDataset.Id);
            Assert.Null(deletedFirstDataset);
            Assert.Null(deletedSecondDataset);
            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteRange_ShouldFail_WhenEntitiesAreNullOrEmpty()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);

            // Act
            ResultDTO result = await repository.DeleteRange(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid entities, enumerable is null or empty", result.ErrMsg);
            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteRange_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);
            List<Dataset> nonExistentDatasets = [new Dataset { Id = Guid.NewGuid() }, new Dataset { Id = Guid.NewGuid() }];

            // Act
            ResultDTO result = await repository.DeleteRange(nonExistentDatasets);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
            transaction.Rollback();
        }
        #endregion

        #region Read
        #region Get by Id
        [Fact]
        public async Task GetById_ShouldReturnDataset_WhenDatasetExists()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            // Act
            ResultDTO<Dataset?> result = await new DatasetsRepository(dbContext).GetById(DatasetsSeedData.FirstDataset.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(DatasetsSeedData.FirstDataset.Id, result.Data!.Id);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetById_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            transaction.Rollback();
            dbContext.Dispose(); // Dispose context to cause exception

            // Act
            ResultDTO<Dataset?> result = await repository.GetById(Guid.NewGuid());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
        }
        #endregion

        #region Get All
        [Fact]
        public async Task GetAll_ShouldReturnAllDatasets_WhenDatasetsExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            // Act
            ResultDTO<IEnumerable<Dataset>> result = await new DatasetsRepository(dbContext).GetAll();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.FirstOrDefault(d => d.Id == DatasetsSeedData.FirstDataset.Id));
            Assert.NotNull(result.Data.FirstOrDefault(d => d.Id == DatasetsSeedData.SecondDataset.Id));
            transaction.Rollback();
        }

        [Fact]
        public async Task GetAll_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);
            transaction.Rollback();
            dbContext.Dispose(); // Dispose context to cause exception

            // Act
            ResultDTO<IEnumerable<Dataset>> result = await repository.GetAll();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);

        }
        #endregion

        #region GetFirstOrDefault Tests
        [Fact]
        public async Task GetFirstOrDefault_ShouldReturnFirstDataset_WhenNoFilterProvided()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);

            // Act
            ResultDTO<Dataset?> result = await repository.GetFirstOrDefault();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(DatasetsSeedData.FirstDataset.Id, result.Data.Id);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetFirstOrDefault_ShouldReturnFilteredDataset_WhenFilterProvided()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);
            Expression<Func<Dataset, bool>> filter = d => d.Name == DatasetsSeedData.SecondDataset.Name;

            // Act
            ResultDTO<Dataset?> result = await repository.GetFirstOrDefault(filter);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(DatasetsSeedData.SecondDataset.Id, result.Data.Id);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetFirstOrDefault_ShouldReturnNull_WhenNoMatchingDatasetExists()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);
            Expression<Func<Dataset, bool>> filter = d => d.Name == "Non-existent Dataset";

            // Act
            ResultDTO<Dataset?> result = await repository.GetFirstOrDefault(filter);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Data);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetFirstOrDefault_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            transaction.Rollback();
            dbContext.Dispose(); // Dispose context to cause exception

            // Act
            ResultDTO<Dataset?> result = await repository.GetFirstOrDefault(x => x.Id == Guid.NewGuid());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
        }
        #endregion

        #region GetByIdInclude Tests
        [Fact]
        public async Task GetByIdInclude_ShouldReturnDatasetWithIncludedProperties_WhenDatasetExists()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);
            Expression<Func<Dataset, object>>[] includeProperties = { d => d.DatasetImages };

            // Act
            ResultDTO<Dataset?> result = await repository.GetByIdInclude(DatasetsSeedData.FirstDataset.Id, includeProperties: includeProperties);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(DatasetsSeedData.FirstDataset.Id, result.Data.Id);
            Assert.NotNull(result.Data.DatasetImages);
            Assert.NotEmpty(result.Data.DatasetImages);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetByIdInclude_ShouldReturnNull_WhenDatasetDoesNotExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);
            Expression<Func<Dataset, object>>[] includeProperties = { d => d.DatasetImages };

            // Act
            ResultDTO<Dataset?> result = await repository.GetByIdInclude(Guid.NewGuid(), includeProperties: includeProperties);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Data);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetByIdInclude_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            Expression<Func<Dataset, object>>[] includeProperties = { d => d.DatasetImages };
            transaction.Rollback();

            // Act
            dbContext.Dispose(); // Dispose context to cause exception
            ResultDTO<Dataset?> result = await repository.GetByIdInclude(Guid.NewGuid(), includeProperties: includeProperties);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
        }
        #endregion

        #region GetByIdIncludeThenAll Tests
        [Fact]
        public async Task GetByIdIncludeThenAll_ShouldReturnDatasetWithNestedIncludes_WhenDatasetExists()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);
            (Expression<Func<Dataset, object>> Include, Expression<Func<object, object>>[]? ThenInclude)[] includeProperties =
            {
                (d => d.DatasetImages, new Expression<Func<object, object>>[] { i => ((DatasetImage)i).ImageAnnotations })
            };

            // Act
            ResultDTO<Dataset?> result = await repository.GetByIdIncludeThenAll(DatasetsSeedData.FirstDataset.Id, includeProperties: includeProperties);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(DatasetsSeedData.FirstDataset.Id, result.Data.Id);
            Assert.NotNull(result.Data.DatasetImages);
            Assert.NotEmpty(result.Data.DatasetImages);
            Assert.NotNull(result.Data.DatasetImages.First().ImageAnnotations);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetByIdIncludeThenAll_ShouldReturnNull_WhenDatasetDoesNotExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new DatasetsRepository(dbContext);
            (Expression<Func<Dataset, object>> Include, Expression<Func<object, object>>[]? ThenInclude)[] includeProperties =
            {
                (d => d.DatasetImages, new Expression<Func<object, object>>[] { i => ((DatasetImage)i).ImageAnnotations })
            };

            // Act
            ResultDTO<Dataset?> result = await repository.GetByIdIncludeThenAll(Guid.NewGuid(), includeProperties: includeProperties);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Data);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetByIdIncludeThenAll_ShouldThrowException_WhenDatabaseOperationFails()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            DatasetsRepository repository = new(dbContext);
            (Expression<Func<Dataset, object>> Include, Expression<Func<object, object>>[]? ThenInclude)[] includeProperties =
            {
                (d => d.DatasetImages, new Expression<Func<object, object>>[] { i => ((DatasetImage)i).ImageAnnotations })
            };
            transaction.Rollback();

            // Act
            dbContext.Dispose(); // Dispose context to cause exception
            ResultDTO<Dataset?> result = await repository.GetByIdIncludeThenAll(Guid.NewGuid(), includeProperties: includeProperties);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.NotNull(result.ExObj);
        }
        #endregion
        #endregion
    }
}
