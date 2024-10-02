using Entities;
using Entities.LegalLandfillsManagementEntites;

namespace Tests.EntitiesTests.LegalLandfillManagementTests
{
    public class LegalLandfillWasteImportTests
    {
        [Fact]
        public void LegalLandfillWasteImport_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var legalLandfillWasteImport = new LegalLandfillWasteImport();

            // Assert
            Assert.True(legalLandfillWasteImport is BaseEntity<Guid>);
        }

        [Fact]
        public void ImportedOn_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var importedOn = new DateTime(2023, 9, 1);

            // Act
            legalLandfillWasteImport.ImportedOn = importedOn;

            // Assert
            Assert.Equal(importedOn, legalLandfillWasteImport.ImportedOn);
        }

        [Fact]
        public void ImportExportStatus_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var importExportStatus = 1;

            // Act
            legalLandfillWasteImport.ImportExportStatus = importExportStatus;

            // Assert
            Assert.Equal(importExportStatus, legalLandfillWasteImport.ImportExportStatus);
        }

        [Fact]
        public void Capacity_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var capacity = 100.2;

            // Act
            legalLandfillWasteImport.Capacity = capacity;

            // Assert
            Assert.Equal(capacity, legalLandfillWasteImport.Capacity);
        }

        [Fact]
        public void Weight_ShouldBeSettableAndGettable()
        {
            //Arange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var weight = 100.2;

            // Act
            legalLandfillWasteImport.Weight = weight;

            // Assert
            Assert.Equal(weight, legalLandfillWasteImport.Weight);
        }

        [Fact]
        public void CreateById_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            string createById = "test-user-id";

            // Act
            legalLandfillWasteImport.CreatedById = createById;

            // Assert
            Assert.Equal(createById, legalLandfillWasteImport.CreatedById);
        }

        [Fact]
        public void CreatedOn_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var createdOn = new DateTime(2024, 9, 6, 12, 0, 0);

            // Act
            legalLandfillWasteImport.CreatedOn = createdOn;

            // Assert
            Assert.Equal(createdOn, legalLandfillWasteImport.CreatedOn);
        }

        [Fact]
        public void LegalLandfillTruckId_ShouldBeSettableAndGettable()
        {
            // Arange
            var legalLandwillWasteImport = new LegalLandfillWasteImport();
            var legalLandfillTruckId = new Guid();

            // Act
            legalLandwillWasteImport.LegalLandfillTruckId = legalLandfillTruckId;

            // Assert
            Assert.Equal(legalLandfillTruckId, legalLandwillWasteImport.LegalLandfillTruckId);
        }

        [Fact]
        public void LegalLandfillTruck_ShouldBeNullByDefault()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();

            // Act&Assert
            Assert.Null(legalLandfillWasteImport.LegalLandfillTruck);
        }

        [Fact]
        public void LegalLandfillTruck_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var legalLandfillTruck = new LegalLandfillTruck();

            // Act
            legalLandfillWasteImport.LegalLandfillTruck = legalLandfillTruck;

            // Assert
            Assert.Equal(legalLandfillTruck, legalLandfillWasteImport.LegalLandfillTruck);
        }

        [Fact]
        public void LegalLandfillId_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var legalLandfillId = new Guid();

            // Act
            legalLandfillWasteImport.LegalLandfillId = legalLandfillId;

            // Assert
            Assert.Equal(legalLandfillId, legalLandfillWasteImport.LegalLandfillId);
        }

        [Fact]
        public void LegalLandfill_ShouldBeNullByDefault()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();

            // Act&Assert
            Assert.Null(legalLandfillWasteImport.LegalLandfill);
        }

        [Fact]
        public void LegalLandfill_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var legalLandfill = new LegalLandfill();

            // Act
            legalLandfillWasteImport.LegalLandfill = legalLandfill;

            // Assert
            Assert.Equal(legalLandfill, legalLandfillWasteImport.LegalLandfill);
        }

        [Fact]
        public void LegalLandfillWasteTypeId_ShouldBeSettableAndGettable()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var legalLandfillWasteTypeId = new Guid();

            // Act
            legalLandfillWasteImport.LegalLandfillWasteTypeId = legalLandfillWasteTypeId;

            // Arrange
            Assert.Equal(legalLandfillWasteTypeId, legalLandfillWasteImport.LegalLandfillWasteTypeId);
        }

        [Fact]
        public void LegalLandfillWasteType_ShouldBeNullByDefault()
        {
            // Arrange
            var legalLandfillWasteImport = new LegalLandfillWasteImport();
            var legalLandfillWasteType = new LegalLandfillWasteType();

            // Act
            legalLandfillWasteImport.LegalLandfillWasteType = legalLandfillWasteType;

            // Assert
            Assert.Equal(legalLandfillWasteType, legalLandfillWasteImport.LegalLandfillWasteType);
        }

    }
}
