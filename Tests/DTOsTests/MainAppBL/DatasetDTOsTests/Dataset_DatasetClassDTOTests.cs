using DTOs.MainApp.BL.DatasetDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DatasetDTOsTests
{
    public class Dataset_DatasetClassDTOTests
    {
        [Fact]
        public void Dataset_DatasetClassDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new DatasetDTO();
            var datasetClassId = Guid.NewGuid();
            var datasetClass = new DatasetClassDTO();

            // Act
            var dto = new Dataset_DatasetClassDTO
            {
                DatasetId = datasetId,
                Dataset = dataset,
                DatasetClassId = datasetClassId,
                DatasetClass = datasetClass
            };

            // Assert
            Assert.Equal(datasetId, dto.DatasetId);
            Assert.Equal(dataset, dto.Dataset);
            Assert.Equal(datasetClassId, dto.DatasetClassId);
            Assert.Equal(datasetClass, dto.DatasetClass);
        }

        [Fact]
        public void Dataset_DatasetClassDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new Dataset_DatasetClassDTO();

            // Assert
            Assert.Null(dto.DatasetId);
            Assert.Null(dto.Dataset);
            Assert.Null(dto.DatasetClassId);
            Assert.Null(dto.DatasetClass);
        }

        [Fact]
        public void Dataset_DatasetClassDTO_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var dto = new Dataset_DatasetClassDTO();
            var datasetId = Guid.NewGuid();
            var dataset = new DatasetDTO();
            var datasetClassId = Guid.NewGuid();
            var datasetClass = new DatasetClassDTO();

            // Act
            dto.DatasetId = datasetId;
            dto.Dataset = dataset;
            dto.DatasetClassId = datasetClassId;
            dto.DatasetClass = datasetClass;

            // Assert
            Assert.Equal(datasetId, dto.DatasetId);
            Assert.Equal(dataset, dto.Dataset);
            Assert.Equal(datasetClassId, dto.DatasetClassId);
            Assert.Equal(datasetClass, dto.DatasetClass);
        }
    }
}
