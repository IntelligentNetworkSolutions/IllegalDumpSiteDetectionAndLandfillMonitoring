using Entities;
using Entities.DatasetEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.DatasetEntittes
{
    public class DatasetTests
    {   
        [Fact]
        public void Dataset_Should_Set_Name_And_Description()
        {
            // Arrange
            var name = "Test Dataset";
            var description = "This is a test dataset.";
            var dataset = new Dataset();

            // Act
            dataset.Name = name;
            dataset.Description = description;

            // Assert
            Assert.Equal(name, dataset.Name);
            Assert.Equal(description, dataset.Description);
        }

        [Fact]
        public void Dataset_Should_Set_IsPublished()
        {
            // Arrange
            var dataset = new Dataset();

            // Act
            dataset.IsPublished = true;

            // Assert
            Assert.True(dataset.IsPublished);
        }

        [Fact]
        public void Dataset_Should_Set_ParentDataset()
        {
            // Arrange
            var parentDatasetId = Guid.NewGuid();
            var parentDataset = new Dataset { Name = "Parent Dataset", Description = "Parent Description" };
            var dataset = new Dataset();

            // Act
            dataset.ParentDatasetId = parentDatasetId;
            dataset.ParentDataset = parentDataset;

            // Assert
            Assert.Equal(parentDatasetId, dataset.ParentDatasetId);
            Assert.Equal(parentDataset, dataset.ParentDataset);
        }

        [Fact]
        public void Dataset_Should_Set_CreatedBy()
        {
            // Arrange
            var createdById = "user123";
            var createdBy = new ApplicationUser { Id = createdById, UserName = "testuser" };
            var dataset = new Dataset();

            // Act
            dataset.CreatedById = createdById;
            dataset.CreatedBy = createdBy;

            // Assert
            Assert.Equal(createdById, dataset.CreatedById);
            Assert.Equal(createdBy, dataset.CreatedBy);
        }

        [Fact]
        public void Dataset_Should_Set_UpdatedBy()
        {
            // Arrange
            var updatedById = "user456";
            var updatedBy = new ApplicationUser { Id = updatedById, UserName = "updateduser" };
            var updatedOn = DateTime.UtcNow;
            var dataset = new Dataset();

            // Act
            dataset.UpdatedById = updatedById;
            dataset.UpdatedBy = updatedBy;
            dataset.UpdatedOn = updatedOn;

            // Assert
            Assert.Equal(updatedById, dataset.UpdatedById);
            Assert.Equal(updatedBy, dataset.UpdatedBy);
            Assert.Equal(updatedOn, dataset.UpdatedOn);
        }

        [Fact]
        public void Dataset_Should_Set_AnnotationsPerSubclass()
        {
            // Arrange
            var dataset = new Dataset();

            // Act
            dataset.AnnotationsPerSubclass = true;

            // Assert
            Assert.True(dataset.AnnotationsPerSubclass.Value);
        }

      
    }
}
