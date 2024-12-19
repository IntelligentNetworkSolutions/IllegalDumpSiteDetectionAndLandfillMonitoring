using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities;
using Entities.DatasetEntities;
using MainApp.BL.Services.DatasetServices;
using Moq;
using SD;
using System.Linq.Expressions;

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
            Assert.Equal(datasetImageDto.Id, result.Data.Id);
            Assert.Equal(datasetImageDto.FileName, result.Data.FileName);
            Assert.Equal(datasetImageDto.Name, result.Data.Name);
            Assert.Equal(datasetImageDto.ImagePath, result.Data.ImagePath);
            Assert.Equal(datasetImageDto.ThumbnailPath, result.Data.ThumbnailPath);
            Assert.Equal(datasetImageDto.IsEnabled, result.Data.IsEnabled);
            Assert.Equal(datasetImageDto.DatasetId, result.Data.DatasetId);
            Assert.Equal(datasetImageDto.CreatedById, result.Data.CreatedById);
            Assert.Equal(datasetImageDto.CreatedOn, result.Data.CreatedOn);
            Assert.Equal(datasetImageDto.UpdatedById, result.Data.UpdatedById);
            Assert.Equal(datasetImageDto.UpdatedOn, result.Data.UpdatedOn);
        }

        [Fact]
        public async Task GetDatasetImageById_Returns_Fail_When_Repository_Fails()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var errorMessage = "Failed to retrieve dataset image";
            var mockRepository = new Mock<IDatasetImagesRepository>();
            mockRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Fail(errorMessage));

            var service = new DatasetImagesService(null, null, null, mockRepository.Object, null, null);

            // Act
            var result = await service.GetDatasetImageById(datasetImageId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetDatasetImageById_Returns_Fail_When_Mapping_Fails()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetImageEntity = new DatasetImage();
            var mockRepository = new Mock<IDatasetImagesRepository>();
            mockRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(datasetImageEntity));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<DatasetImageDTO>(It.IsAny<DatasetImage>())).Returns((DatasetImageDTO)null);

            var service = new DatasetImagesService(null, null, null, mockRepository.Object, null, mockMapper.Object);

            // Act
            var result = await service.GetDatasetImageById(datasetImageId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping failed for this dataset image", result.ErrMsg);
        }

        [Fact]
        public async Task GetImagesForDataset_Returns_Fail_When_Repository_Fails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var errorMessage = "Failed to retrieve dataset images";
            var mockRepository = new Mock<IDatasetImagesRepository>();
            mockRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Fail(errorMessage));

            var service = new DatasetImagesService(null, null, null, mockRepository.Object, null, null);

            // Act
            var result = await service.GetImagesForDataset(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.ErrMsg);
        }

        [Fact]
        public async Task GetImagesForDataset_Returns_Fail_When_Mapping_Fails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var datasetImages = new List<DatasetImage> { new DatasetImage() };
            var mockRepository = new Mock<IDatasetImagesRepository>();
            mockRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(datasetImages));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<List<DatasetImageDTO>>(It.IsAny<IEnumerable<DatasetImage>>())).Returns((List<DatasetImageDTO>)null);

            var service = new DatasetImagesService(null, null, null, mockRepository.Object, null, mockMapper.Object);

            // Act
            var result = await service.GetImagesForDataset(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping failed", result.ErrMsg);
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
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("Image1", result.Data[0].FileName);
            Assert.Equal("Image2", result.Data[1].FileName);
        }

        [Fact]
        public async Task AddDatasetImage_NullDatasetId_ReturnsErrorMEssage()
        {
            // Arrange
            DatasetImageDTO dto = new DatasetImageDTO();

            // Act
            var result = await _service.AddDatasetImage(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Dataset id is null", result.ErrMsg);
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
        public async Task AddDatasetImage_ReturnsError_WhenDatasetNotFound()
        {
            // Arrange
            var datasetImageDto = new DatasetImageDTO
            {
                DatasetId = Guid.NewGuid(),
                // ... other properties
            };

            _mockDatasetsRepository.Setup(repo => repo.GetById(datasetImageDto.DatasetId.Value, true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(null));

            // Act
            var result = await _service.AddDatasetImage(datasetImageDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Guid.Empty, result.Data);
            Assert.Equal("Could not retrive dataset.", result.ErrMsg);
        }

        [Fact]
        public async Task AddDatasetImage_ReturnsError_WhenImageAdditionFails()
        {
            // Arrange
            var datasetImageDto = new DatasetImageDTO
            {
                DatasetId = Guid.NewGuid(),
                // ... other properties
            };

            var dataset = new Dataset { Id = datasetImageDto.DatasetId.Value };

            _mockDatasetsRepository.Setup(repo => repo.GetById(datasetImageDto.DatasetId.Value, true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            var datasetImage = new DatasetImage();
            _mockMapper.Setup(m => m.Map<DatasetImage>(datasetImageDto)).Returns(datasetImage);

            _mockDatasetImagesRepository.Setup(repo => repo.CreateAndReturnEntity(datasetImage, true, default))
                .ReturnsAsync(ResultDTO<DatasetImage>.Fail("Failed to add image"));

            // Act
            var result = await _service.AddDatasetImage(datasetImageDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Guid.Empty, result.Data);
            Assert.Equal("Failed to add image", result.ErrMsg);
        }


        [Fact]
        public async Task AddDatasetImage_ValidDatasetImage_ReturnsSuccess()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var datasetImageDto = new DatasetImageDTO { DatasetId = datasetId, UpdatedById = Guid.NewGuid().ToString() };
            var dataset = new Dataset { Id = datasetId };
            var datasetImage = new DatasetImage { Id = Guid.NewGuid() };

            _mockDatasetsRepository
                .Setup(repo => repo.GetById(datasetId, true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockDatasetImagesRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<DatasetImage>(), true, default))
                .ReturnsAsync(ResultDTO<DatasetImage>.Ok(datasetImage));

            // Act
            var result = await _service.AddDatasetImage(datasetImageDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(datasetImage.Id, result.Data);
        }

        [Fact]
        public async Task AddDatasetImage_DatasetNotFound_ReturnsError()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var datasetImageDto = new DatasetImageDTO { DatasetId = datasetId };

            _mockDatasetsRepository
                .Setup(repo => repo.GetById(datasetId, true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Dataset not found"));

            // Act
            var result = await _service.AddDatasetImage(datasetImageDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Dataset not found", result.ErrMsg);
        }

        [Fact]
        public async Task AddDatasetImage_FailedDatasetImageCreation_ReturnsError()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            var datasetImageDto = new DatasetImageDTO { DatasetId = datasetId };
            var dataset = new Dataset { Id = datasetId };

            _mockDatasetsRepository
                .Setup(repo => repo.GetById(datasetId, true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockDatasetImagesRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<DatasetImage>(), true, default))
                .ReturnsAsync(ResultDTO<DatasetImage>.Fail("Creation Failed"));

            // Act
            var result = await _service.AddDatasetImage(datasetImageDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation Failed", result.ErrMsg);
        }

        [Fact]
        public async Task AddDatasetImage_UpdateFails_ThrowsException()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            var datasetImageDto = new DatasetImageDTO { DatasetId = datasetId, UpdatedById = Guid.NewGuid().ToString() };
            var dataset = new Dataset { Id = datasetId };

            _mockDatasetsRepository
                .Setup(repo => repo.GetById(datasetId, true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockDatasetImagesRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<DatasetImage>(), true, default))
                .ReturnsAsync(ResultDTO<DatasetImage>.Ok(new DatasetImage()));

            _mockDatasetsRepository
                .Setup(repo => repo.Update(It.IsAny<Dataset>(), true, default))
                .ThrowsAsync(new Exception("Update failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _service.AddDatasetImage(datasetImageDto));
        }

        [Fact]
        public async Task AddDatasetImage_EmptyImageId_ReturnsFailure()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            var datasetImageDto = new DatasetImageDTO { DatasetId = datasetId, UpdatedById = Guid.NewGuid().ToString() };
            var dataset = new Dataset { Id = datasetId };

            _mockDatasetsRepository
                .Setup(repo => repo.GetById(datasetId, true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockDatasetImagesRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<DatasetImage>(), true, default))
                .ReturnsAsync(ResultDTO<DatasetImage>.Ok(new DatasetImage { Id = Guid.Empty }));

            // Act
            var result = await _service.AddDatasetImage(datasetImageDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Dataset image was not added", result.ErrMsg);
        }


        [Fact]
        public async Task EditDatasetImage_ShouldReturnSuccess_WhenAllOperationsSucceed()
        {
            // Arrange
            EditDatasetImageDTO editDto = new EditDatasetImageDTO
            {
                DatasetId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                IsEnabled = false,
                UpdatedById = Guid.NewGuid().ToString()
            };
            Dataset dataset = new Dataset
            {
                Id = editDto.DatasetId,
                CreatedBy = new ApplicationUser(),
                UpdatedBy = new ApplicationUser() { Id = editDto.UpdatedById },
                ParentDataset = new Dataset()
            };
            DatasetImage datasetImage = new DatasetImage { Id = editDto.Id, ImageAnnotations = new List<ImageAnnotation>() };
            DatasetImage updatedDatasetImage = new DatasetImage { Id = editDto.Id, ImageAnnotations = new List<ImageAnnotation>() };

            _mockDatasetsRepository.Setup(x => x.GetByIdInclude(editDto.DatasetId, true, It.IsAny<Expression<Func<Dataset, object>>[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));
            _mockDatasetImagesRepository.Setup(x => x.GetByIdInclude(editDto.Id, true, It.IsAny<Expression<Func<DatasetImage, object>>[]>()))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(datasetImage));
            _mockMapper.Setup(x => x.Map(editDto, datasetImage)).Returns(datasetImage);
            _mockDatasetImagesRepository.Setup(x => x.Update(datasetImage, true, default)).ReturnsAsync(ResultDTO.Ok());
            _mockDatasetsRepository.Setup(x => x.Update(dataset, true, default)).ReturnsAsync(ResultDTO.Ok());

            // Act
            ResultDTO<int> result = await _service.EditDatasetImage(editDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data);
        }



        [Fact]
        public async Task EditDatasetImage_ShouldReturnFail_WhenDatasetNotFound()
        {
            // Arrange
            EditDatasetImageDTO editDto = new EditDatasetImageDTO { DatasetId = Guid.NewGuid() };
            _mockDatasetsRepository.Setup(x => x.GetByIdInclude(editDto.DatasetId, true, It.IsAny<Expression<Func<Dataset, object>>[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Dataset not found"));

            // Act
            ResultDTO<int> result = await _service.EditDatasetImage(editDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Dataset not found", result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetImage_ShouldReturnFail_WhenDatasetImageIsNotFound()
        {

            // Act
            ResultDTO<int> result = await _service.EditDatasetImage(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Null EditDatasetImageDTO", result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetImage_ShouldReturnFail_WhenDatasetImageNotFound()
        {
            // Arrange
            EditDatasetImageDTO editDto = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Id = Guid.NewGuid() };
            Dataset dataset = new Dataset { Id = editDto.DatasetId };
            _mockDatasetsRepository.Setup(x => x.GetByIdInclude(editDto.DatasetId, true, It.IsAny<Expression<Func<Dataset, object>>[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));
            _mockDatasetImagesRepository.Setup(x => x.GetByIdInclude(editDto.Id, true, It.IsAny<Expression<Func<DatasetImage, object>>[]>()))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Fail("Dataset Image not found"));

            // Act
            ResultDTO<int> result = await _service.EditDatasetImage(editDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Dataset Image not found", result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetImage_ShouldReturnFail_WhenUpdateDatasetImageFails()
        {
            // Arrange
            EditDatasetImageDTO editDto = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Id = Guid.NewGuid() };
            Dataset dataset = new Dataset { Id = editDto.DatasetId };
            DatasetImage datasetImage = new DatasetImage { Id = editDto.Id };
            _mockDatasetsRepository.Setup(x => x.GetByIdInclude(editDto.DatasetId, true, It.IsAny<Expression<Func<Dataset, object>>[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));
            _mockDatasetImagesRepository.Setup(x => x.GetByIdInclude(editDto.Id, true, It.IsAny<Expression<Func<DatasetImage, object>>[]>()))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(datasetImage));
            _mockDatasetImagesRepository.Setup(x => x.Update(It.IsAny<DatasetImage>(), true, default))
                .ReturnsAsync(ResultDTO.Fail("Update DatasetImage failed"));

            // Act
            ResultDTO<int> result = await _service.EditDatasetImage(editDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update DatasetImage failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetImage_ShouldReturnFail_WhenUpdateDatasetFails()
        {
            // Arrange
            EditDatasetImageDTO editDto = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Id = Guid.NewGuid() };
            Dataset dataset = new Dataset { Id = editDto.DatasetId };
            DatasetImage datasetImage = new DatasetImage { Id = editDto.Id };
            _mockDatasetsRepository.Setup(x => x.GetByIdInclude(editDto.DatasetId, true, It.IsAny<Expression<Func<Dataset, object>>[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));
            _mockDatasetImagesRepository.Setup(x => x.GetByIdInclude(editDto.Id, true, It.IsAny<Expression<Func<DatasetImage, object>>[]>()))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(datasetImage));
            _mockDatasetImagesRepository.Setup(x => x.Update(It.IsAny<DatasetImage>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());
            _mockDatasetsRepository.Setup(x => x.Update(It.IsAny<Dataset>(), true, default))
                .ReturnsAsync(ResultDTO.Fail("Update Dataset failed"));

            // Act
            ResultDTO<int> result = await _service.EditDatasetImage(editDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update Dataset failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetImage_ShouldReturnFail_WhenEnablingImageWithoutAnnotations()
        {
            // Arrange
            EditDatasetImageDTO editDto = new EditDatasetImageDTO { DatasetId = Guid.NewGuid(), Id = Guid.NewGuid(), IsEnabled = true };
            Dataset dataset = new Dataset { Id = editDto.DatasetId };
            DatasetImage datasetImage = new DatasetImage { Id = editDto.Id, ImageAnnotations = [] };
            _mockDatasetsRepository.Setup(x => x.GetByIdInclude(editDto.DatasetId, true, It.IsAny<Expression<Func<Dataset, object>>[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));
            _mockDatasetImagesRepository.Setup(x => x.GetByIdInclude(editDto.Id, true, It.IsAny<Expression<Func<DatasetImage, object>>[]>()))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(datasetImage));

            // Act
            ResultDTO<int> result = await _service.EditDatasetImage(editDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("You can not enable this image because there are not annotations!", result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetImage_ShouldReturnFail_WhenDatasetDataIsNull()
        {
            // Arrange
            EditDatasetImageDTO editDto = new EditDatasetImageDTO
            {
                DatasetId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                IsEnabled = false,
                UpdatedById = Guid.NewGuid().ToString()
            };

            _mockDatasetsRepository.Setup(x => x.GetByIdInclude(editDto.DatasetId, true, It.IsAny<Expression<Func<Dataset, object>>[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(null));

            // Act
            ResultDTO<int> result = await _service.EditDatasetImage(editDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting Dataset", result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetImage_ShouldReturnFail_WhenDatasetImageDataIsNull()
        {
            // Arrange
            EditDatasetImageDTO editDto = new EditDatasetImageDTO
            {
                DatasetId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                IsEnabled = false,
                UpdatedById = Guid.NewGuid().ToString()
            };
            Dataset dataset = new Dataset
            {
                Id = editDto.DatasetId,
                CreatedBy = new ApplicationUser(),
                UpdatedBy = new ApplicationUser(),
                ParentDataset = new Dataset()
            };

            _mockDatasetsRepository.Setup(x => x.GetByIdInclude(editDto.DatasetId, true, It.IsAny<Expression<Func<Dataset, object>>[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));
            _mockDatasetImagesRepository.Setup(x => x.GetByIdInclude(editDto.Id, true, It.IsAny<Expression<Func<DatasetImage, object>>[]>()))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(null));

            // Act
            ResultDTO<int> result = await _service.EditDatasetImage(editDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting Dataset Image", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetImage_ValidId_ReturnsSuccess()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(It.IsAny<Guid>(), false, "ImageAnnotations"))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(new DatasetImage()));

            _mockDatasetImagesRepository
            .Setup(repo => repo.Delete(It.IsAny<DatasetImage>(), true, default))
            .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteDatasetImage(datasetImageId, false);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetImage_ImageNotFound_ReturnsError()
        {
            // Arrange
            var invalidDatasetImageId = Guid.NewGuid();

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(It.IsAny<Guid>(), false, "ImageAnnotations"))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Fail("Image not found"));

            // Act 
            var result = await _service.DeleteDatasetImage(invalidDatasetImageId, false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Image not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetImage_ValidId_WithAnnotations_ReturnsSuccess()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetImage = new DatasetImage
            {
                Id = datasetImageId,
                ImageAnnotations = new List<ImageAnnotation> { new ImageAnnotation() }
            };
            //var includeProperties = "ImageAnnotations";
            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(datasetImageId, false, "ImageAnnotations"))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(datasetImage));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.DeleteRange(datasetImage.ImageAnnotations, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetImagesRepository
                .Setup(repo => repo.Delete(datasetImage, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteDatasetImage(datasetImageId, true);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetImage_ValidId_AnnotationsDeletionFails_ReturnsError()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetImage = new DatasetImage
            {
                ImageAnnotations = new List<ImageAnnotation> { new ImageAnnotation() }
            };

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(datasetImageId, false, "ImageAnnotations"))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(datasetImage));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.DeleteRange(datasetImage.ImageAnnotations, true, default))
                .Throws(new Exception("Error deleting annotations"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.DeleteDatasetImage(datasetImageId, true));
            Assert.Equal("Error deleting annotations", exception.Message);
        }
        [Fact]
        public async Task DeleteDatasetImage_WhenImageDeletionFails_ReturnsError()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetImage = new DatasetImage
            {
                ImageAnnotations = new List<ImageAnnotation>()
            };

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(datasetImageId, false, "ImageAnnotations"))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(datasetImage));

            _mockDatasetImagesRepository
                .Setup(repo => repo.Delete(datasetImage, true, default))
                .ReturnsAsync(ResultDTO.Fail("Failed to delete image"));

            // Act
            var result = await _service.DeleteDatasetImage(datasetImageId, false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
            Assert.Equal("Dataset image was not deleted", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetImage_WhenAnnotationDeletionFails_ReturnsError()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();
            var datasetImage = new DatasetImage
            {
                ImageAnnotations = new List<ImageAnnotation> { new ImageAnnotation() }
            };

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(datasetImageId, false, "ImageAnnotations"))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(datasetImage));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.DeleteRange(datasetImage.ImageAnnotations, true, default))
                .ReturnsAsync(ResultDTO.Fail("Failed to delete annotations"));

            // Act
            var result = await _service.DeleteDatasetImage(datasetImageId, true);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
            Assert.Equal("Failed to delete annotations", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetImage_WhenDatasetImageIsNull_ReturnsError()
        {
            // Arrange
            var datasetImageId = Guid.NewGuid();

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetById(datasetImageId, false, "ImageAnnotations"))
                .ReturnsAsync(ResultDTO<DatasetImage?>.Ok(null));

            // Act
            var result = await _service.DeleteDatasetImage(datasetImageId, false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
            Assert.Equal("Dataset image is null.", result.ErrMsg);
        }


    }
}
