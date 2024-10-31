using DAL.ApplicationStorage.SeedDatabase.TestSeedData;
using DAL.Repositories.DatasetRepositories;
using DTOs.Helpers;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Tests.DalTests.Repositories.DatasetRepositories
{
    [Trait("Category", "Integration")]
    [Collection("Shared TestDatabaseFixture")]
    public class ImageAnnotationsRepositoryTests
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly WKTReader _wktReader;
        private readonly GeometryFactory _geometryFactory;

        public ImageAnnotationsRepositoryTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            _geometryFactory = new GeometryFactory();
            _wktReader = new WKTReader(_geometryFactory);
        }

        private ImageAnnotation CreateTestAnnotation(Guid id, Guid imageId, Guid classId, string userId)
        {
            return new ImageAnnotation
            {
                Id = id,
                Geom = GeoJsonHelpers.ConvertBoundingBoxToPolygon([150, 250, 100, 150]),
                DatasetImageId = imageId,
                DatasetClassId = classId,
                IsEnabled = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task BulkUpdateImageAnnotations_SuccessfullyHandlesAllOperations()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            _fixture.SeedDatabase(dbContext);
            ImageAnnotationsRepository repository = new ImageAnnotationsRepository(dbContext);

            // Seed some initial data
            var datasetImageId = DatasetImagesSeedData.FirstDatasetFirstImage.Id;
            var datasetClassId = DatasetClassesSeedData.FirstDatasetClass.Id;
            var userId = UserSeedData.FirstUser.Id;

            // Create initial annotations
            var existingAnnotation1 = dbContext.ImageAnnotations.Find(ImageAnnotationsSeedData.FirstImageFirstAnnotationFirstClass.Id)!;
            var delAnn = CreateTestAnnotation(Guid.NewGuid(), datasetImageId, datasetClassId, userId);
            dbContext.ImageAnnotations.Add(delAnn);
            dbContext.SaveChanges();
            // Prepare lists for bulk update
            List<ImageAnnotation> insertList = [ CreateTestAnnotation(Guid.NewGuid(), datasetImageId, datasetClassId, userId) ];
            List<ImageAnnotation> updateList = [];
            List<ImageAnnotation> deleteList = [delAnn];

            // Act
            var result = await repository.BulkUpdateImageAnnotations(insertList, updateList, deleteList, transaction);

            // Assert
            Assert.True(result);

            // Verify inserts
            var newAnnotation = await dbContext.ImageAnnotations.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == insertList.First().Id);
            Assert.NotNull(newAnnotation);

            // Verify deletes
            var deletedAnnotation = await dbContext.ImageAnnotations.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == delAnn.Id);
            Assert.Null(deletedAnnotation);

            transaction.Rollback();
        }

        [Fact]
        public async Task BulkUpdateImageAnnotations_HandlesEmptyLists()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            ImageAnnotationsRepository repository = new ImageAnnotationsRepository(dbContext);

            // Act
            var result = await repository.BulkUpdateImageAnnotations(null, null, null, transaction);

            // Assert
            Assert.True(result);
            transaction.Rollback();
        }

        [Fact]
        public async Task BulkUpdateImageAnnotations_HandlesLargeDatasets()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            _fixture.SeedDatabase(dbContext);
            ImageAnnotationsRepository repository = new ImageAnnotationsRepository(dbContext);

            var datasetImageId = DatasetImagesSeedData.FirstDatasetFirstImage.Id;
            var datasetClassId = DatasetClassesSeedData.FirstDatasetClass.Id;
            var userId = UserSeedData.FirstUser.Id;

            // Create large lists of annotations
            var insertList = new List<ImageAnnotation>();
            var updateList = new List<ImageAnnotation>();
            var deleteList = new List<ImageAnnotation>();

            // Add 100 annotations to each list
            for (int i = 0; i < 100; i++)
            {
                if (i < 33)
                    insertList.Add(CreateTestAnnotation(Guid.NewGuid(), datasetImageId, datasetClassId, userId));
                else if (i < 66)
                    updateList.Add(CreateTestAnnotation(Guid.NewGuid(), datasetImageId, datasetClassId, userId));
                else
                    deleteList.Add(CreateTestAnnotation(Guid.NewGuid(), datasetImageId, datasetClassId, userId));
            }

            // Add update and delete items to database first
            await dbContext.ImageAnnotations.AddRangeAsync(updateList.Concat(deleteList));
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.BulkUpdateImageAnnotations(insertList, updateList, deleteList, transaction);

            // Assert
            Assert.True(result);

            transaction.Rollback();
        }

        [Fact]
        public async Task BulkUpdateImageAnnotations_ThrowsError()
        {
            // Arrange
            using var dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            _fixture.SeedDatabase(dbContext);
            ImageAnnotationsRepository repository = new ImageAnnotationsRepository(dbContext);

            var datasetImageId = DatasetImagesSeedData.FirstDatasetFirstImage.Id;
            var datasetClassId = DatasetClassesSeedData.FirstDatasetClass.Id;
            var userId = UserSeedData.FirstUser.Id;

            // Create large lists of annotations
            var insertList = new List<ImageAnnotation>();
            var updateList = new List<ImageAnnotation>();
            var deleteList = new List<ImageAnnotation>();

            // Add 100 annotations to each list
            for (int i = 0; i < 100; i++)
            {
                if (i < 33)
                    insertList.Add(CreateTestAnnotation(Guid.NewGuid(), datasetImageId, datasetClassId, userId));
                else if (i < 66)
                    updateList.Add(CreateTestAnnotation(Guid.NewGuid(), datasetImageId, datasetClassId, userId));
                else
                    deleteList.Add(CreateTestAnnotation(Guid.NewGuid(), datasetImageId, datasetClassId, userId));
            }

            // Add update and delete items to database first
            await dbContext.ImageAnnotations.AddRangeAsync(updateList.Concat(deleteList));
            await dbContext.SaveChangesAsync();

            transaction.Rollback();
            dbContext.Dispose();

            // Act
            await Assert.ThrowsAsync<ObjectDisposedException>(async () 
                => await repository.BulkUpdateImageAnnotations(insertList, updateList, deleteList, transaction));
        }
    }
}
