using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Services;
using Moq;
using SD;
using System.Linq.Expressions;

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

        [Fact]
        public async Task BulkUpdateImageAnnotations_UpdatesExistingAnnotations()
        {
            // Arrange
            var imageAnnotationsRepositoryMock = new Mock<IImageAnnotationsRepository>();
            var datasetImagesRepositoryMock = new Mock<IDatasetImagesRepository>();
            var mapperMock = new Mock<IMapper>();

            var service = new ImageAnnotationsService(imageAnnotationsRepositoryMock.Object, datasetImagesRepositoryMock.Object, mapperMock.Object);

            var existingAnnotationId = Guid.NewGuid();
            var datasetImageId = Guid.NewGuid();

            var editImageAnnotationsDto = new EditImageAnnotationsDTO
            {
                DatasetImageId = datasetImageId,
                ImageAnnotations = new List<ImageAnnotationDTO>
        {
            new ImageAnnotationDTO { Id = existingAnnotationId },
            new ImageAnnotationDTO { Id = null }
        }
            };

            var existingAnnotations = new List<ImageAnnotation>
    {
        new ImageAnnotation
        {
            Id = existingAnnotationId,
            DatasetImageId = datasetImageId,
            CreatedById = Guid.NewGuid().ToString(),
            CreatedOn = DateTime.UtcNow.AddDays(-1)
        }
    };

            imageAnnotationsRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<ImageAnnotation, bool>>>(), null, false, null, null))
                                          .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(existingAnnotations));

            mapperMock.Setup(m => m.Map<ImageAnnotation>(It.IsAny<ImageAnnotationDTO>()))
                       .Returns((ImageAnnotationDTO dto) => new ImageAnnotation
                       {
                           Id = dto.Id ?? Guid.NewGuid(),
                           UpdatedOn = DateTime.UtcNow
                       });

            imageAnnotationsRepositoryMock.Setup(repo => repo.BulkUpdateImageAnnotations(It.IsAny<List<ImageAnnotation>>(),
                                                                                        It.IsAny<List<ImageAnnotation>>(),
                                                                                        It.IsAny<List<ImageAnnotation>>(),
                                                                                        null))
                                         .ReturnsAsync(true); // Simulating a successful update

            // Act
            var result = await service.BulkUpdateImageAnnotations(editImageAnnotationsDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }


        [Fact]
        public async Task BulkUpdateImageAnnotations_InsertsNewAnnotations()
        {
            // Arrange
            var imageAnnotationsRepositoryMock = new Mock<IImageAnnotationsRepository>();
            var datasetImagesRepositoryMock = new Mock<IDatasetImagesRepository>();
            var mapperMock = new Mock<IMapper>();

            var service = new ImageAnnotationsService(imageAnnotationsRepositoryMock.Object, datasetImagesRepositoryMock.Object, mapperMock.Object);

            var datasetImageId = Guid.NewGuid();
            var editImageAnnotationsDto = new EditImageAnnotationsDTO
            {
                DatasetImageId = datasetImageId,
                ImageAnnotations = new List<ImageAnnotationDTO>
        {
            new ImageAnnotationDTO { Id = null },
            new ImageAnnotationDTO { Id = Guid.NewGuid() }
        }
            };

            imageAnnotationsRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<ImageAnnotation, bool>>>(), null, false, null, null))
                                          .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(new List<ImageAnnotation>()));

            mapperMock.Setup(m => m.Map<ImageAnnotation>(It.IsAny<ImageAnnotationDTO>()))
                       .Returns((ImageAnnotationDTO dto) => new ImageAnnotation
                       {
                           Id = Guid.NewGuid(),
                           CreatedOn = DateTime.UtcNow
                       });

            imageAnnotationsRepositoryMock.Setup(repo => repo.BulkUpdateImageAnnotations(
                    It.IsAny<List<ImageAnnotation>>(),
                    It.IsAny<List<ImageAnnotation>>(),
                    It.IsAny<List<ImageAnnotation>>(),
                    null
                ))
                .ReturnsAsync(true);

            // Act
            var result = await service.BulkUpdateImageAnnotations(editImageAnnotationsDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }



    }
}
