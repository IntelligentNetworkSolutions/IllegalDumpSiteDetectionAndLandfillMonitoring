using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Services.DatasetServices;
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
    public class DatasetImagesServiceTests
    {
        private readonly Mock<IDatasetsRepository> _mockDatasetsRepository;
        private readonly Mock<IDatasetClassesRepository> _mockDatasetClassesRepository;
        private readonly Mock<IDataset_DatasetClassRepository> _mockDatasetDatasetClassRepository;
        private readonly Mock<IDatasetImagesRepository> _mockDatasetImagesRepository;
        private readonly Mock<IImageAnnotationsRepository> _mockImageAnnotationsRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DatasetImagesService _service;

        public DatasetImagesServiceTests()
        {
            _mockDatasetsRepository = new Mock<IDatasetsRepository>();
            _mockDatasetClassesRepository = new Mock<IDatasetClassesRepository>();
            _mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            _mockDatasetImagesRepository = new Mock<IDatasetImagesRepository>();
            _mockImageAnnotationsRepository = new Mock<IImageAnnotationsRepository>();
            _mockMapper = new Mock<IMapper>();

            _service = new DatasetImagesService(
                _mockDatasetsRepository.Object,
                _mockDatasetClassesRepository.Object,
                _mockDatasetDatasetClassRepository.Object,
                _mockDatasetImagesRepository.Object,
                _mockImageAnnotationsRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetDatasetImageById_Returns_Valid_DatasetImageDTO()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetImageDto = new DatasetImageDTO
            {
                Id = datasetImageId,
                FileName = "TestImage.jpg",
                Name = "Test Image",
                ImagePath = "/images/test.jpg",
                ThumbnailPath = "/thumbnails/test.jpg",
                IsEnabled = true,
                DatasetId = Guid.NewGuid(),
                CreatedById = "user1",
                CreatedOn = DateTime.UtcNow.AddDays(-1),
                UpdatedById = "user2",
                UpdatedOn = DateTime.UtcNow
            };

            var datasetImageEntity = new DatasetImage
            {
                Id = datasetImageDto.Id,
                FileName = datasetImageDto.FileName,
                Name = datasetImageDto.Name,
                ImagePath = datasetImageDto.ImagePath,
                ThumbnailPath = datasetImageDto.ThumbnailPath,
                IsEnabled = datasetImageDto.IsEnabled,
                DatasetId = datasetImageDto.DatasetId,
                CreatedById = datasetImageDto.CreatedById,
                CreatedOn = datasetImageDto.CreatedOn,
                UpdatedById = datasetImageDto.UpdatedById,
                UpdatedOn = datasetImageDto.UpdatedOn
            };

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<DatasetImageDTO>(It.IsAny<DatasetImage>())).Returns(datasetImageDto);

            var mockRepository = new Mock<IDatasetImagesRepository>();
            mockRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()))
              .ReturnsAsync((Guid id, bool track, string? includeProperties) =>
                  ResultDTO<DatasetImage?>.Ok(id == datasetImageId ? datasetImageEntity : null));


            var service = new DatasetImagesService(null, null, null, mockRepository.Object, null, mockMapper.Object);

            // Act
            var result = await service.GetDatasetImageById(datasetImageId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(datasetImageDto.Id, result.Id);
            Assert.Equal(datasetImageDto.FileName, result.FileName);
            Assert.Equal(datasetImageDto.Name, result.Name);
            Assert.Equal(datasetImageDto.ImagePath, result.ImagePath);
            Assert.Equal(datasetImageDto.ThumbnailPath, result.ThumbnailPath);
            Assert.Equal(datasetImageDto.IsEnabled, result.IsEnabled);
            Assert.Equal(datasetImageDto.DatasetId, result.DatasetId);
            Assert.Equal(datasetImageDto.CreatedById, result.CreatedById);
            Assert.Equal(datasetImageDto.CreatedOn, result.CreatedOn);
            Assert.Equal(datasetImageDto.UpdatedById, result.UpdatedById);
            Assert.Equal(datasetImageDto.UpdatedOn, result.UpdatedOn);
        }

        [Fact]
        public async Task GetImagesForDataset_ExceptionThrown_ThrowsException()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var exceptionMessage = "Error retrieving data";

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, null, null))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.GetImagesForDataset(datasetId));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task GetImagesForDataset_ReturnsImagesForDataset()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var datasetImages = new List<DatasetImage>
            {
                new DatasetImage { Id = Guid.NewGuid(), DatasetId = datasetId, FileName = "Image1" },
                new DatasetImage { Id = Guid.NewGuid(), DatasetId = datasetId, FileName = "Image2" }
            };
            var resultData = ResultDTO<IEnumerable<DatasetImage>>.Ok(datasetImages);

            _mockDatasetImagesRepository
            .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, null, null))
            .ReturnsAsync(resultData);

            var datasetImageDTOs = new List<DatasetImageDTO>
            {
                new DatasetImageDTO { Id = datasetImages[0].Id, FileName = "Image1" },
                new DatasetImageDTO { Id = datasetImages[1].Id, FileName = "Image2" }
            };

            _mockMapper
                .Setup(m => m.Map<List<DatasetImageDTO>>(datasetImages))
                .Returns(datasetImageDTOs);

            // Act
            var result = await _service.GetImagesForDataset(datasetId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Image1", result[0].FileName);
            Assert.Equal("Image2", result[1].FileName);
        }

        [Fact]
        public async Task AddDatasetImage_NullDatasetId_ThrowsException()
        {
            var datasetImageDto = new DatasetImageDTO();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _service.AddDatasetImage(datasetImageDto));
        }

        [Fact]
        public async Task AddDatasetImage_ExceptionDuringProcess_ThrowsException()
        {
            // Arrange
            var datasetImageDto = new DatasetImageDTO { DatasetId = Guid.NewGuid() };

            _mockDatasetsRepository
                .Setup(repo => repo.GetById(It.IsAny<Guid>(), true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ThrowsAsync(new Exception("Error retrieving dataset"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _service.AddDatasetImage(datasetImageDto));
        }

        [Fact]
        public async Task EditDatasetImage_ExceptionDuringProcess_ThrowsException()
        {
            // Arrange
            var editDatasetImageDTO = new EditDatasetImageDTO
            {
                Id = Guid.NewGuid(),
                DatasetId = Guid.NewGuid(),
                UpdatedById = "userId"
            };

            _mockImageAnnotationsRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<ImageAnnotation, bool>>>(), null, false, null, null))
                .ThrowsAsync(new Exception("Error retrieving annotations"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _service.EditDatasetImage(editDatasetImageDTO));
        }

        [Fact]
        public async Task DeleteDatasetImage_ValidId_ReturnsSuccess()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(It.IsAny<Guid>(), false, null))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(new DatasetImage()));

            _mockDatasetImagesRepository
            .Setup(repo => repo.Delete(It.IsAny<DatasetImage>(), true, default))
            .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteDatasetImage(datasetImageId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data); 
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetImage_InvalidId_ThrowsException()
        {
            // Arrange
            var invalidDatasetImageId = Guid.NewGuid();

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(It.IsAny<Guid>(), false, null))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(null));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _service.DeleteDatasetImage(invalidDatasetImageId));
        }

        [Fact]
        public async Task DeleteDatasetImage_ImageNotFound_ThrowsException()
        {
            // Arrange
            var invalidDatasetImageId = Guid.NewGuid();

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(It.IsAny<Guid>(), false, null))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(null));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.DeleteDatasetImage(invalidDatasetImageId));
            Assert.Equal("Object not found", exception.Message);
        }

       

    }
}
