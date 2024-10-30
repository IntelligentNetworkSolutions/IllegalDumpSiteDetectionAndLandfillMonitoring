using DTOs.MainApp.BL;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;

namespace Tests.MainAppMVCTests.ViewModels.LegalLandfillManagementTests
{
    public class LegalLandfillWasteImportViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void ImportedOnProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var importedOn = DateTime.UtcNow.AddDays(-1);
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                ImportedOn = importedOn
            };

            // Act & Assert
            Assert.Equal(importedOn, viewModel.ImportedOn);
        }

        [Fact]
        public void ImportedOnProperty_ShouldDefaultToUtcNow()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel();
            viewModel.ImportedOn = DateTime.UtcNow;

            // Act
            var now = DateTime.UtcNow;

            // Assert
            Assert.True((now - viewModel.ImportedOn).TotalSeconds < 1);
        }

        [Fact]
        public void ImportExportStatusProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var status = 1;
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                ImportExportStatus = status
            };

            // Act & Assert
            Assert.Equal(status, viewModel.ImportExportStatus);
        }

        [Fact]
        public void CapacityProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var capacity = 500.75;
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                Capacity = capacity
            };

            // Act & Assert
            Assert.Equal(capacity, viewModel.Capacity);
        }

        [Fact]
        public void IsEnabledProperty_ShouldDefaultToFalse()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel();

            // Act & Assert
            Assert.False(viewModel.IsEnabled);
        }

        [Fact]
        public void IsEnabledProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel { IsEnabled = true };

            // Act & Assert
            Assert.True(viewModel.IsEnabled);
        }


        [Fact]
        public void WeightProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var weight = 1200.50;
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                Weight = weight
            };

            // Act & Assert
            Assert.Equal(weight, viewModel.Weight);
        }

        [Fact]
        public void CreatedByIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdById = "user123";
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                CreatedById = createdById
            };

            // Act & Assert
            Assert.Equal(createdById, viewModel.CreatedById);
        }

        [Fact]
        public void CreatedOnProperty_ShouldDefaultToUtcNow()
        {
            // Arrange
            var viewModel = new LegalLandfillWasteImportViewModel();
            viewModel.CreatedOn = DateTime.UtcNow;

            // Act
            var now = DateTime.UtcNow;

            // Assert
            Assert.True((now - viewModel.CreatedOn).TotalSeconds < 1);
        }

        [Fact]
        public void CreatedByProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdBy = new UserDTO { Id = "user123", UserName = "testuser" };
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                CreatedBy = createdBy
            };

            // Act & Assert
            Assert.Equal(createdBy, viewModel.CreatedBy);
        }

        [Fact]
        public void LegalLandfillTruckIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var truckId = Guid.NewGuid();
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                LegalLandfillTruckId = truckId
            };

            // Act & Assert
            Assert.Equal(truckId, viewModel.LegalLandfillTruckId);
        }

        [Fact]
        public void LegalLandfillTruckProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var truck = new LegalLandfillTruckDTO { Id = Guid.NewGuid(), Name = "Truck A" };
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                LegalLandfillTruck = truck
            };

            // Act & Assert
            Assert.Equal(truck, viewModel.LegalLandfillTruck);
        }

        [Fact]
        public void LegalLandfillIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var landfillId = Guid.NewGuid();
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                LegalLandfillId = landfillId
            };

            // Act & Assert
            Assert.Equal(landfillId, viewModel.LegalLandfillId);
        }

        [Fact]
        public void LegalLandfillProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var landfill = new LegalLandfillDTO { Id = Guid.NewGuid(), Name = "Landfill A" };
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                LegalLandfill = landfill
            };

            // Act & Assert
            Assert.Equal(landfill, viewModel.LegalLandfill);
        }

        [Fact]
        public void LegalLandfillWasteTypeIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var wasteTypeId = Guid.NewGuid();
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                LegalLandfillWasteTypeId = wasteTypeId
            };

            // Act & Assert
            Assert.Equal(wasteTypeId, viewModel.LegalLandfillWasteTypeId);
        }

        [Fact]
        public void LegalLandfillWasteTypeProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var wasteType = new LegalLandfillWasteTypeDTO { Id = Guid.NewGuid(), Name = "Waste Type A" };
            var viewModel = new LegalLandfillWasteImportViewModel
            {
                LegalLandfillWasteType = wasteType
            };

            // Act & Assert
            Assert.Equal(wasteType, viewModel.LegalLandfillWasteType);
        }
    }

}
