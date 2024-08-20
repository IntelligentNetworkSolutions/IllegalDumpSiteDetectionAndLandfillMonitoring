using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.DatasetTests
{
    public class DatasetViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new DatasetViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void NameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var name = "TestDatasetName";
            var viewModel = new DatasetViewModel
            {
                Name = name
            };

            // Act & Assert
            Assert.Equal(name, viewModel.Name);
        }

        [Fact]
        public void NameProperty_ShouldBeRequired()
        {
            // Arrange
            var viewModel = new DatasetViewModel();

            // Act
            var validationContext = new ValidationContext(viewModel, null, null);
            var results = new List<ValidationResult>();

            // Assert
            Assert.False(Validator.TryValidateObject(viewModel, validationContext, results, true));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(viewModel.Name)) && r.ErrorMessage.Contains("required"));
        }

        [Fact]
        public void DescriptionProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var description = "TestDatasetDescription";
            var viewModel = new DatasetViewModel
            {
                Description = description
            };

            // Act & Assert
            Assert.Equal(description, viewModel.Description);
        }

        [Fact]
        public void DescriptionProperty_ShouldBeRequired()
        {
            // Arrange
            var viewModel = new DatasetViewModel();

            // Act
            var validationContext = new ValidationContext(viewModel, null, null);
            var results = new List<ValidationResult>();

            // Assert
            Assert.False(Validator.TryValidateObject(viewModel, validationContext, results, true));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(viewModel.Description)) && r.ErrorMessage.Contains("required"));
        }

        [Fact]
        public void IsPublishedProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new DatasetViewModel
            {
                IsPublished = true
            };

            // Act & Assert
            Assert.True(viewModel.IsPublished);
        }

        [Fact]
        public void ParentDatasetIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var parentDatasetId = Guid.NewGuid();
            var viewModel = new DatasetViewModel
            {
                ParentDatasetId = parentDatasetId
            };

            // Act & Assert
            Assert.Equal(parentDatasetId, viewModel.ParentDatasetId);
        }

        [Fact]
        public void ParentDatasetProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var parentDataset = new DatasetDTO();
            var viewModel = new DatasetViewModel
            {
                ParentDataset = parentDataset
            };

            // Act & Assert
            Assert.Equal(parentDataset, viewModel.ParentDataset);
        }

        [Fact]
        public void CreatedByIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdById = "TestCreatedById";
            var viewModel = new DatasetViewModel
            {
                CreatedById = createdById
            };

            // Act & Assert
            Assert.Equal(createdById, viewModel.CreatedById);
        }

        [Fact]
        public void CreatedOnProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var viewModel = new DatasetViewModel
            {
                CreatedOn = createdOn
            };

            // Act & Assert
            Assert.Equal(createdOn, viewModel.CreatedOn);
        }

        [Fact]
        public void CreatedByProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdBy = new UserDTO();
            var viewModel = new DatasetViewModel
            {
                CreatedBy = createdBy
            };

            // Act & Assert
            Assert.Equal(createdBy, viewModel.CreatedBy);
        }

        [Fact]
        public void UpdatedByIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var updatedById = "TestUpdatedById";
            var viewModel = new DatasetViewModel
            {
                UpdatedById = updatedById
            };

            // Act & Assert
            Assert.Equal(updatedById, viewModel.UpdatedById);
        }

        [Fact]
        public void UpdatedOnProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var updatedOn = DateTime.UtcNow;
            var viewModel = new DatasetViewModel
            {
                UpdatedOn = updatedOn
            };

            // Act & Assert
            Assert.Equal(updatedOn, viewModel.UpdatedOn);
        }

        [Fact]
        public void UpdatedByProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var updatedBy = new UserDTO();
            var viewModel = new DatasetViewModel
            {
                UpdatedBy = updatedBy
            };

            // Act & Assert
            Assert.Equal(updatedBy, viewModel.UpdatedBy);
        }

        [Fact]
        public void AnnotationsPerSubclassProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var viewModel = new DatasetViewModel
            {
                AnnotationsPerSubclass = true
            };

            // Act & Assert
            Assert.True(viewModel.AnnotationsPerSubclass);
        }
    }
}
