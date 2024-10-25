using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL.DatasetDTOs;

namespace Tests.DTOsTests.MainAppBL.DatasetDTOsTests
{
    public class DatasetFullIncludeDTOTests
    {
        [Fact]
        public void Constructor_WithAllParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            DatasetDTO dataset = new() { Id = Guid.NewGuid(), Name = "Test Dataset" };
            List<DatasetClassForDatasetDTO> datasetClassForDataset = 
                [ new() { DatasetClassId = Guid.NewGuid(), DatasetDatasetClassId = Guid.NewGuid(), ClassName = "Class 1", ClassValue = 1 }];
            List<DatasetImageDTO> datasetImages = 
                [ new() { Id = Guid.NewGuid(), FileName = "image1.jpg" } ];
            List<ImageAnnotationDTO> imageAnnotations = 
                [ new() { Id = Guid.NewGuid(), DatasetImageId = datasetImages[0].Id } ];

            // Act
            DatasetFullIncludeDTO dto = new DatasetFullIncludeDTO(dataset, datasetClassForDataset, datasetImages, imageAnnotations);

            // Assert
            Assert.Equal(dataset, dto.Dataset);
            Assert.Equal(datasetClassForDataset, dto.DatasetClassForDataset);
            Assert.Equal(datasetImages, dto.DatasetImages);
            Assert.Equal(imageAnnotations, dto.ImageAnnotations);
        }

        [Fact]
        public void Constructor_WithDatasetDTOOnly_InitializesEmptyLists()
        {
            // Arrange
            DatasetDTO dataset = new DatasetDTO { Id = Guid.NewGuid(), Name = "Test Dataset" };

            // Act
            DatasetFullIncludeDTO dto = new DatasetFullIncludeDTO(dataset);

            // Assert
            Assert.Equal(dataset, dto.Dataset);
            Assert.Empty(dto.DatasetClassForDataset);
            Assert.Empty(dto.DatasetImages);
            Assert.Empty(dto.ImageAnnotations);
        }

        [Fact]
        public void Constructor_WithNullDataset_ThrowsArgumentNullException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => new DatasetFullIncludeDTO(null));
        }

        [Fact]
        public void Constructor_WithNullLists_DoesNotThrowException()
        {
            // Arrange
            DatasetDTO dataset = new DatasetDTO { Id = Guid.NewGuid(), Name = "Test Dataset" };

            // Act & Assert
            Exception exception = Record.Exception(() => new DatasetFullIncludeDTO(dataset, null, null, null));

            Assert.Null(exception);
        }

        [Fact]
        public void Equality_TwoDifferentDTOs_AreNotEqual()
        {
            // Arrange
            var dto1 = new DatasetFullIncludeDTO(new DatasetDTO { Id = Guid.NewGuid(), Name = "Dataset 1" });
            var dto2 = new DatasetFullIncludeDTO(new DatasetDTO { Id = Guid.NewGuid(), Name = "Dataset 2" });

            // Act & Assert
            Assert.NotEqual(dto1, dto2);
        }
    }
}
