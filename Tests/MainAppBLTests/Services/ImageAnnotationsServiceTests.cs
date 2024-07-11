using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Services;
using Moq;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Services
{
    public class ImageAnnotationsServiceTests
    {
        [Fact]
        public async Task BulkUpdateImageAnnotations_Failure_NoExistingAnnotations()
        {
            // Arrange
            var imageAnnotationsRepositoryMock = new Mock<IImageAnnotationsRepository>();
            var datasetImagesRepositoryMock = new Mock<IDatasetImagesRepository>();
            var mapperMock = new Mock<IMapper>();

            var service = new ImageAnnotationsService(imageAnnotationsRepositoryMock.Object, datasetImagesRepositoryMock.Object, mapperMock.Object);

            var editImageAnnotationsDto = new EditImageAnnotationsDTO
            {
                DatasetImageId = Guid.NewGuid(),
                ImageAnnotations = new List<ImageAnnotationDTO>
                {
                    new ImageAnnotationDTO { Id = Guid.NewGuid() },
                    new ImageAnnotationDTO { Id = Guid.NewGuid() }
                }
            };

            var existingAnnotations = Enumerable.Empty<ImageAnnotation>();

            imageAnnotationsRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<ImageAnnotation, bool>>>(), null, false, null, null))
                                          .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(existingAnnotations));

            // Act
            var result = await service.BulkUpdateImageAnnotations(editImageAnnotationsDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.False(result.Data);
        }

        [Fact]
        public async Task GetImageAnnotationsByImageId_ExceptionThrown()
        {
            // Arrange
            var imageAnnotationsRepositoryMock = new Mock<IImageAnnotationsRepository>();
            var mapperMock = new Mock<IMapper>();

            var service = new ImageAnnotationsService(imageAnnotationsRepositoryMock.Object, null, mapperMock.Object);

            var datasetImageId = Guid.NewGuid();

            imageAnnotationsRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<ImageAnnotation, bool>>>(), null, false, null, null))
                                          .ThrowsAsync(new Exception("An error occurred while fetching annotations."));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetImageAnnotationsByImageId(datasetImageId));
        }

        [Fact]
        public async Task GetImageAnnotationsByImageId_NullResult()
        {
            // Arrange
            var imageAnnotationsRepositoryMock = new Mock<IImageAnnotationsRepository>();
            var mapperMock = new Mock<IMapper>();

            var service = new ImageAnnotationsService(imageAnnotationsRepositoryMock.Object, null, mapperMock.Object);

            var datasetImageId = Guid.NewGuid();

            imageAnnotationsRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<ImageAnnotation, bool>>>(), null, false, null, null))
                                          .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(null));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetImageAnnotationsByImageId(datasetImageId));
        }

        [Fact]
        public async Task GetImageAnnotationsByImageId_EmptyResult()
        {
            // Arrange
            var imageAnnotationsRepositoryMock = new Mock<IImageAnnotationsRepository>();
            var mapperMock = new Mock<IMapper>();

            var service = new ImageAnnotationsService(imageAnnotationsRepositoryMock.Object, null, mapperMock.Object);

            var datasetImageId = Guid.NewGuid();
            var annotations = Enumerable.Empty<ImageAnnotation>();

            imageAnnotationsRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<ImageAnnotation, bool>>>(), null, false, null, null))
                                          .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(annotations));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetImageAnnotationsByImageId(datasetImageId));
        }

        [Fact]
        public async Task GetImageAnnotationsByImageId_Success()
        {
            // Arrange
            var imageAnnotationsRepositoryMock = new Mock<IImageAnnotationsRepository>();
            var mapperMock = new Mock<IMapper>();

            var service = new ImageAnnotationsService(imageAnnotationsRepositoryMock.Object, null, mapperMock.Object);

            var datasetImageId = Guid.NewGuid();
            var annotations = new List<ImageAnnotation>
            {
                new ImageAnnotation { Id = Guid.NewGuid() },
                new ImageAnnotation { Id = Guid.NewGuid()}
            };

            imageAnnotationsRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<ImageAnnotation, bool>>>(), null, false, null, null))
                                          .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(annotations));

            mapperMock.Setup(mapper => mapper.Map<List<ImageAnnotationDTO>>(annotations))
                       .Returns(new List<ImageAnnotationDTO>());

            // Act
            var result = await service.GetImageAnnotationsByImageId(datasetImageId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
      
        [Fact]
        public async Task BulkUpdateImageAnnotations_Exception()
        {
            // Arrange
            var imageAnnotationsRepositoryMock = new Mock<IImageAnnotationsRepository>();
            var datasetImagesRepositoryMock = new Mock<IDatasetImagesRepository>();
            var mapperMock = new Mock<IMapper>();

            var service = new ImageAnnotationsService(imageAnnotationsRepositoryMock.Object, datasetImagesRepositoryMock.Object, mapperMock.Object);

            var editImageAnnotationsDto = new EditImageAnnotationsDTO
            {
                DatasetImageId = Guid.NewGuid(),
                ImageAnnotations = new List<ImageAnnotationDTO>
                {
                    new ImageAnnotationDTO { Id = Guid.NewGuid() }
                }
            };

            imageAnnotationsRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<ImageAnnotation, bool>>>(), null, false, null, null))
                                          .ThrowsAsync(new Exception("An error occurred while fetching existing annotations."));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.BulkUpdateImageAnnotations(editImageAnnotationsDto));
        }

      

    }
}
