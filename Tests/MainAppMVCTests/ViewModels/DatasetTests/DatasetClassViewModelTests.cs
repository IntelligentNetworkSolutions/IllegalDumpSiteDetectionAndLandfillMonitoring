using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.DatasetTests
{
    public class DatasetClassViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new DatasetClassViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void ParentClassIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var parentClassId = Guid.NewGuid();
            var viewModel = new DatasetClassViewModel
            {
                ParentClassId = parentClassId
            };

            // Act & Assert
            Assert.Equal(parentClassId, viewModel.ParentClassId);
        }

        [Fact]
        public void ParentClassProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var parentClass = new DatasetClassDTO();
            var viewModel = new DatasetClassViewModel
            {
                ParentClass = parentClass
            };

            // Act & Assert
            Assert.Equal(parentClass, viewModel.ParentClass);
        }

        [Fact]
        public void ClassNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var className = "TestClassName";
            var viewModel = new DatasetClassViewModel
            {
                ClassName = className
            };

            // Act & Assert
            Assert.Equal(className, viewModel.ClassName);
        }

        [Fact]
        public void CreatedByIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdById = "TestCreatedById";
            var viewModel = new DatasetClassViewModel
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
            var viewModel = new DatasetClassViewModel
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
            var viewModel = new DatasetClassViewModel
            {
                CreatedBy = createdBy
            };

            // Act & Assert
            Assert.Equal(createdBy, viewModel.CreatedBy);
        }

        [Fact]
        public void DatasetsProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var datasets = new List<Dataset_DatasetClassViewModel>
            {
                new Dataset_DatasetClassViewModel()
            };
            var viewModel = new DatasetClassViewModel
            {
                Datasets = datasets
            };

            // Act & Assert
            Assert.Equal(datasets, viewModel.Datasets);
        }
    }
}
