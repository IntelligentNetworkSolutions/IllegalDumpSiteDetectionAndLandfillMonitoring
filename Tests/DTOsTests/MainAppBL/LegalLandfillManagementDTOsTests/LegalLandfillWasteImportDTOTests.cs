using DTOs.MainApp.BL;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;

namespace Tests.DTOsTests.MainAppBL.LegalLandfillManagementDTOsTests
{
    public class LegalLandfillWasteImportDTOTests
    {
        [Fact]
        public void DefaultConstructor_InitializesProperties()
        {
            // Arrange
            var wasteImportDTO = new LegalLandfillWasteImportDTO();

            // Act
            var id = wasteImportDTO.Id;
            var importedOn = wasteImportDTO.ImportedOn;
            var importExportStatus = wasteImportDTO.ImportExportStatus;
            var capacity = wasteImportDTO.Capacity;
            var weight = wasteImportDTO.Weight;
            var createdById = wasteImportDTO.CreatedById;
            var createdOn = wasteImportDTO.CreatedOn;
            var createdBy = wasteImportDTO.CreatedBy;
            var truckId = wasteImportDTO.LegalLandfillTruckId;
            var truck = wasteImportDTO.LegalLandfillTruck;
            var landfillId = wasteImportDTO.LegalLandfillId;
            var landfill = wasteImportDTO.LegalLandfill;
            var wasteTypeId = wasteImportDTO.LegalLandfillWasteTypeId;
            var wasteType = wasteImportDTO.LegalLandfillWasteType;

            // Assert
            Assert.Equal(default, id);
            Assert.Equal(default, importedOn);
            Assert.Equal(0, importExportStatus);
            Assert.Null(capacity);
            Assert.Null(weight);
            Assert.Null(createdById);
            Assert.Equal(DateTime.UtcNow.Date, createdOn.Date);
            Assert.Null(createdBy);
            Assert.Equal(default, truckId);
            Assert.Null(truck);
            Assert.Equal(default, landfillId);
            Assert.Null(landfill);
            Assert.Equal(default, wasteTypeId);
            Assert.Null(wasteType);
        }

        [Fact]
        public void Properties_CanBeAssignedAndRetrieved()
        {
            // Arrange
            var wasteImportDTO = new LegalLandfillWasteImportDTO();
            var id = Guid.NewGuid();
            var importedOn = DateTime.UtcNow;
            var importExportStatus = 1;
            var capacity = 15000.5;
            var weight = 7500.0;
            var createdById = "user123";
            var createdOn = DateTime.UtcNow;
            var createdBy = new UserDTO
            {
                Id = "testuserid",
                UserName = "testuser",
                Email = "testuser@example.com"
            };
            var truckId = Guid.NewGuid();
            var truck = new LegalLandfillTruckDTO
            {
                Id = truckId,
                Name = "Test Truck"
            };
            var landfillId = Guid.NewGuid();
            var landfill = new LegalLandfillDTO
            {
                Id = landfillId,
                Name = "Test Landfill"
            };
            var wasteTypeId = Guid.NewGuid();
            var wasteType = new LegalLandfillWasteTypeDTO
            {
                Id = wasteTypeId,
                Name = "Test Waste Type"
            };

            // Act
            wasteImportDTO.Id = id;
            wasteImportDTO.ImportedOn = importedOn;
            wasteImportDTO.ImportExportStatus = importExportStatus;
            wasteImportDTO.Capacity = capacity;
            wasteImportDTO.Weight = weight;
            wasteImportDTO.CreatedById = createdById;
            wasteImportDTO.CreatedOn = createdOn;
            wasteImportDTO.CreatedBy = createdBy;
            wasteImportDTO.LegalLandfillTruckId = truckId;
            wasteImportDTO.LegalLandfillTruck = truck;
            wasteImportDTO.LegalLandfillId = landfillId;
            wasteImportDTO.LegalLandfill = landfill;
            wasteImportDTO.LegalLandfillWasteTypeId = wasteTypeId;
            wasteImportDTO.LegalLandfillWasteType = wasteType;

            // Assert
            Assert.Equal(id, wasteImportDTO.Id);
            Assert.Equal(importedOn, wasteImportDTO.ImportedOn);
            Assert.Equal(importExportStatus, wasteImportDTO.ImportExportStatus);
            Assert.Equal(capacity, wasteImportDTO.Capacity);
            Assert.Equal(weight, wasteImportDTO.Weight);
            Assert.Equal(createdById, wasteImportDTO.CreatedById);
            Assert.Equal(createdOn, wasteImportDTO.CreatedOn);
            Assert.Equal(createdBy, wasteImportDTO.CreatedBy);
            Assert.Equal(truckId, wasteImportDTO.LegalLandfillTruckId);
            Assert.Equal(truck, wasteImportDTO.LegalLandfillTruck);
            Assert.Equal(landfillId, wasteImportDTO.LegalLandfillId);
            Assert.Equal(landfill, wasteImportDTO.LegalLandfill);
            Assert.Equal(wasteTypeId, wasteImportDTO.LegalLandfillWasteTypeId);
            Assert.Equal(wasteType, wasteImportDTO.LegalLandfillWasteType);
        }
    }
}
