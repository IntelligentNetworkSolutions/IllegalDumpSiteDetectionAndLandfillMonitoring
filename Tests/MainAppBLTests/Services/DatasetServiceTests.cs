using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.ObjectDetection.API.CocoFormatDTOs;
using Entities;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.BL.Services.DatasetServices;
using Microsoft.Extensions.Logging;
using Moq;
using SD;
using System.Linq.Expressions;

namespace Tests.MainAppBLTests.Services
{
    public class DatasetServiceTests
    {
        private readonly Mock<IDatasetClassesRepository> _mockDatasetClassesRepository;
        private readonly Mock<IDatasetsRepository> _mockDatasetRepository;
        private readonly Mock<IDataset_DatasetClassRepository> _mockDatasetDatasetClassRepository;
        private readonly Mock<IDatasetImagesRepository> _mockDatasetImagesRepository;
        private readonly Mock<IImageAnnotationsRepository> _mockImageAnnotationsRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<ICocoUtilsService> _mockCocoUtilsService;
        private readonly DatasetService _datasetService;
        private readonly Mock<ILogger<DatasetService>> _mockLogger;

        public DatasetServiceTests()
        {
            _mockDatasetClassesRepository = new Mock<IDatasetClassesRepository>();
            _mockDatasetRepository = new Mock<IDatasetsRepository>();
            _mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            _mockDatasetImagesRepository = new Mock<IDatasetImagesRepository>();
            _mockImageAnnotationsRepository = new Mock<IImageAnnotationsRepository>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockMapper = new Mock<IMapper>();
            _mockCocoUtilsService = new Mock<ICocoUtilsService>();
            _mockLogger = new Mock<ILogger<DatasetService>>();

            //var mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            //var mockDatasetImagesRepository = new Mock<IDatasetImagesRepository>();
            //var mockImageAnnotationsRepository = new Mock<IImageAnnotationsRepository>();
            //var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            _datasetService = new DatasetService(
                _mockDatasetRepository.Object,
                _mockDatasetClassesRepository.Object,
                _mockDatasetDatasetClassRepository.Object,
                _mockDatasetImagesRepository.Object,
                _mockImageAnnotationsRepository.Object,
                _mockAppSettingsAccessor.Object,
                _mockMapper.Object,
                _mockCocoUtilsService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task FillDatasetDto_ShouldFillDto_WhenDtoIdIsEmpty()
        {
            // Arrange
            var dto = new CreateDatasetDTO { Id = Guid.Empty };
            var datasetClasses = new List<DatasetClass> { new DatasetClass { Id = Guid.NewGuid() } };
            var datasetClassDtos = new List<DatasetClassDTO> { new DatasetClassDTO { Id = Guid.NewGuid() } };

            var resultDto = ResultDTO<IEnumerable<DatasetClass>>.Ok(datasetClasses);

            _mockDatasetClassesRepository
                .Setup(repo => repo.GetAll(null, null, false, "ParentClass,Datasets", null))
                .ReturnsAsync(resultDto);

            _mockMapper
                .Setup(mapper => mapper.Map<List<DatasetClassDTO>>(It.IsAny<IEnumerable<DatasetClass>>()))
                .Returns(datasetClassDtos);

            // Act
            var result = await _datasetService.FillDatasetDto(dto);

            // Assert
            Assert.NotNull(result.AllDatasetClasses);
            Assert.Equal(datasetClassDtos.Count, result.AllDatasetClasses.Count);
        }

        [Fact]
        public async Task GetDatasetById_ShouldReturnDatasetDto_WhenDatasetFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset { Id = datasetId };
            var datasetDto = new DatasetDTO { Id = datasetId };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, It.IsAny<bool>(), "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockMapper
                .Setup(mapper => mapper.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(datasetDto);

            // Act
            var result = await _datasetService.GetDatasetById(datasetId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(datasetId, result.Id);
        }

        [Fact]
        public async Task GetDatasetById_ShouldThrowException_WhenDatasetNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, It.IsAny<bool>(), "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Object not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.GetDatasetById(datasetId));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task GetDatasetById_ShouldThrowException_WhenMappingFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset { Id = datasetId };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, It.IsAny<bool>(), "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockMapper
                .Setup(mapper => mapper.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns((DatasetDTO)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.GetDatasetById(datasetId));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task CreateDataset_ShouldReturnDatasetDto_WhenCreationIsSuccessful()
        {
            // Arrange
            var datasetDto = new DatasetDTO { Id = Guid.NewGuid() };
            var dataset = new Dataset { Id = datasetDto.Id };
            var createdDatasetDto = new DatasetDTO { Id = datasetDto.Id };

            _mockMapper
                .Setup(mapper => mapper.Map<Dataset>(It.IsAny<DatasetDTO>()))
                .Returns(dataset);

            _mockDatasetRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<Dataset>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockMapper
                .Setup(mapper => mapper.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(createdDatasetDto);

            // Act
            var result = await _datasetService.CreateDataset(datasetDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(datasetDto.Id, result.Id);
        }

        [Fact]
        public async Task CreateDataset_ShouldThrowException_WhenCreationFails()
        {
            // Arrange
            var datasetDto = new DatasetDTO { Id = Guid.NewGuid() };
            var dataset = new Dataset { Id = datasetDto.Id };

            _mockMapper
                .Setup(mapper => mapper.Map<Dataset>(It.IsAny<DatasetDTO>()))
                .Returns(dataset);

            _mockDatasetRepository
                .Setup(repo => repo.CreateAndReturnEntity(It.IsAny<Dataset>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultDTO<Dataset>.Fail("Object not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.CreateDataset(datasetDto));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task CreateDataset_ShouldThrowException_WhenInitialMappingFails()
        {
            // Arrange
            var datasetDto = new DatasetDTO { Id = Guid.NewGuid() };

            _mockMapper
                .Setup(mapper => mapper.Map<Dataset>(It.IsAny<DatasetDTO>()))
                .Returns((Dataset)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.CreateDataset(datasetDto));
            Assert.Equal("Object not found", exception.Message);
        }


        [Fact]
        public async Task AddDatasetClassForDataset_ShouldThrowException_WhenDatasetNotFound()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var userId = "testUser";

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, It.IsAny<bool>(), "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Fail("Object not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.AddDatasetClassForDataset(selectedClassId, datasetId, userId));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task AddDatasetClassForDataset_ShouldHandleException_WhenDatasetUpdateFails()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var userId = "testUser";

            var dataset = new Dataset { Id = datasetId, CreatedById = userId, UpdatedById = userId };
            var datasetResult = ResultDTO<Dataset>.Ok(dataset);

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, It.IsAny<bool>(), "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(datasetResult);

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.Create(It.IsAny<Dataset_DatasetClass>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetRepository
                .Setup(repo => repo.Update(It.IsAny<Dataset>(), true, default))
                .ThrowsAsync(new Exception("Update failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _datasetService.AddDatasetClassForDataset(selectedClassId, datasetId, userId));
        }

        [Fact]
        public async Task AddDatasetClassForDataset_ShouldReturnFailure_WhenClassAdditionFails()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var userId = "user-id";

            var datasetDb = new Dataset
            {
                Id = datasetId,
                UpdatedById = null,
                CreatedById = "creator-id"
            };

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.Create(It.IsAny<Dataset_DatasetClass>(), true, default))
                .ReturnsAsync(ResultDTO.Fail("Dataset class was not added"));


            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(datasetDb));

            // Act
            var result = await _datasetService.AddDatasetClassForDataset(selectedClassId, datasetId, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
            Assert.Equal("Dataset class was not added", result.ErrMsg);
        }

        [Fact]
        public async Task AddDatasetClassForDataset_ShouldReturnSuccess_WhenClassIsAddedSuccessfully()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var userId = "user-id";

            var datasetDb = new Dataset
            {
                Id = datasetId,
                UpdatedById = null,
                CreatedById = "creator-id"
            };

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.Create(It.IsAny<Dataset_DatasetClass>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(datasetDb));

            // Act
            var result = await _datasetService.AddDatasetClassForDataset(selectedClassId, datasetId, userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data);
            Assert.Null(result.ErrMsg);
        }


        [Fact]
        public async Task AddInheritedParentClasses_ShouldThrowException_WhenParentClassesNotFound()
        {
            // Arrange
            var insertedDatasetId = Guid.NewGuid();
            var parentDatasetId = Guid.NewGuid();

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, It.IsAny<bool>(), null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Fail("Object not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.AddInheritedParentClasses(insertedDatasetId, parentDatasetId));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task SetAnnotationsPerSubclass_ShouldReturnFailureResult_WhenDatasetNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var annotationsPerSubclass = true;
            var userId = "testUser";

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Object not found"));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await _datasetService.SetAnnotationsPerSubclass(datasetId, annotationsPerSubclass, userId));

            // Assert
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task DeleteDataset_ShouldReturnFailure_WhenDatasetNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, true, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Object not found"));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.DeleteDataset(datasetId));

            // Assert
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task DeleteDatasetClassForDataset_ShouldReturnFailure_WhenDatasetClassNotFound()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var userId = "testUser";

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetFirstOrDefault(x => x.DatasetId == datasetId && x.DatasetClassId == selectedClassId, false, null))
                .ReturnsAsync(ResultDTO<Dataset_DatasetClass?>.Fail("Object not found"));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.DeleteDatasetClassForDataset(selectedClassId, datasetId, userId));

            // Assert
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task GetAllDatasets_ShouldReturnListOfDatasetDtos_WhenDatasetsExist()
        {
            // Arrange
            var datasets = new List<Dataset>
                {
                    new Dataset { Id = Guid.NewGuid() },
                    new Dataset { Id = Guid.NewGuid() }
                };
            var datasetDtos = datasets.Select(d => new DatasetDTO { Id = d.Id }).ToList();

            _mockDatasetRepository
                .Setup(repo => repo.GetAll(null, null, false, "CreatedBy,UpdatedBy,ParentDataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset>>.Ok(datasets));

            _mockMapper
                .Setup(mapper => mapper.Map<List<DatasetDTO>>(It.IsAny<IEnumerable<Dataset>>()))
                .Returns(datasetDtos);

            // Act
            var result = await _datasetService.GetAllDatasets();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(datasetDtos.Count, result.Count);
            Assert.Equal(datasetDtos[0].Id, result[0].Id);
            Assert.Equal(datasetDtos[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetAllDatasets_ShouldThrowException_WhenNoDatasetsFound()
        {
            // Arrange
            _mockDatasetRepository
                .Setup(repo => repo.GetAll(null, null, false, "CreatedBy,UpdatedBy,ParentDataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset>>.Fail("Object not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.GetAllDatasets());
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task GetAllDatasets_ShouldThrowException_WhenMappingFails()
        {
            // Arrange
            var datasets = new List<Dataset>
                {
                    new Dataset { Id = Guid.NewGuid() },
                    new Dataset { Id = Guid.NewGuid() }
                };

            _mockDatasetRepository
                .Setup(repo => repo.GetAll(null, null, false, "CreatedBy,UpdatedBy,ParentDataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset>>.Ok(datasets));

            _mockMapper
                .Setup(mapper => mapper.Map<List<DatasetDTO>>(It.IsAny<IEnumerable<Dataset>>()))
                .Returns((List<DatasetDTO>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.GetAllDatasets());
            Assert.Equal("Dataset list not found", exception.Message);
        }

        [Fact]
        public async Task GetAllPublishedDatasets_ShouldReturnListOfPublishedDatasetDtos_WhenPublishedDatasetsExist()
        {
            // Arrange
            var datasets = new List<Dataset>
                {
                    new Dataset { Id = Guid.NewGuid(), IsPublished = true },
                    new Dataset { Id = Guid.NewGuid(), IsPublished = true }
                };
            var datasetDtos = datasets.Select(d => new DatasetDTO { Id = d.Id }).ToList();

            _mockDatasetRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset, bool>>>(), null, false, "CreatedBy,ParentDataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset>>.Ok(datasets));

            _mockMapper
                .Setup(mapper => mapper.Map<List<DatasetDTO>>(It.IsAny<IEnumerable<Dataset>>()))
                .Returns(datasetDtos);

            // Act
            var result = await _datasetService.GetAllPublishedDatasets();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(datasetDtos.Count, result.Data.Count);
            Assert.Equal(datasetDtos[0].Id, result.Data[0].Id);
            Assert.Equal(datasetDtos[1].Id, result.Data[1].Id);
        }

        [Fact]
        public async Task GetAllPublishedDatasets_ShouldReturnFailResult_WhenNoPublishedDatasetsFound()
        {
            // Arrange
            _mockDatasetRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset, bool>>>(), null, false, "CreatedBy,ParentDataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset>>.Fail("Training runs not found"));

            // Act
            var result = await _datasetService.GetAllPublishedDatasets();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Training runs not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllPublishedDatasets_ShouldReturnExceptionFail_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockDatasetRepository
                .Setup(repo => repo.GetAll(
                    It.IsAny<Expression<Func<Dataset, bool>>>(), It.IsAny<Func<IQueryable<Dataset>, IOrderedQueryable<Dataset>>>(), false, "CreatedBy,ParentDataset", null))
                .ThrowsAsync(new Exception("Database connection failed"));


            // Act
            var result = await _datasetService.GetAllPublishedDatasets();

            // Assert
            Assert.False(result.IsSuccess);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
        #region Dataset
        [Fact]
        public async Task GetDatasetDTOFullyIncluded_ShouldReturnDatasetDTO_WhenDatasetExists()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset { Id = datasetId };
            var datasetDto = new DatasetDTO { Id = datasetId };

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(
                    datasetId,
                    It.IsAny<bool>(),
                    It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockMapper
                .Setup(mapper => mapper.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(datasetDto);

            // Act
            var result = await _datasetService.GetDatasetDTOFullyIncluded(datasetId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(datasetId, result.Data.Id);
        }

        [Fact]
        public async Task GetDatasetDTOFullyIncluded_ShouldReturnFail_WhenDatasetNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(
                    datasetId,
                    It.IsAny<bool>(),
                    It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Dataset not found"));

            // Act
            var result = await _datasetService.GetDatasetDTOFullyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Dataset not found", result.ErrMsg);
        }

        [Fact]
        public async Task GetDatasetDTOFullyIncluded_ShouldReturnFail_WhenRepositoryFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(
                    datasetId,
                    It.IsAny<bool>(),
                    It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Repository error"));

            // Act
            var result = await _datasetService.GetDatasetDTOFullyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetDatasetDTOFullyIncluded_ShouldReturnFail_WhenMappingFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset { Id = datasetId };

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(
                    datasetId,
                    It.IsAny<bool>(),
                    It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockMapper
                .Setup(mapper => mapper.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns((DatasetDTO)null);

            // Act
            var result = await _datasetService.GetDatasetDTOFullyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Dataset Mapping failed, for id: {datasetId}", result.ErrMsg);
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldReturnEditDatasetDTO_WhenSuccessful()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            SetupSuccessfulMocksWithData(datasetId);

            // Act
            var result = await _datasetService.GetObjectForEditDataset(datasetId, null, null, null, null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<EditDatasetDTO>(result);
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldThrowException_WhenDatasetDatasetClassesIsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync((ResultDTO<IEnumerable<Dataset_DatasetClass>>?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _datasetService.GetObjectForEditDataset(datasetId, null, null, null, null, 1, 10));
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldThrowException_WhenDatasetClassesIsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockDatasetClassesRepository
                .Setup(repo => repo.GetAll(null, null, false, "ParentClass,Datasets", null))
                .ReturnsAsync((ResultDTO<IEnumerable<DatasetClass>>?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _datasetService.GetObjectForEditDataset(datasetId, null, null, null, null, 1, 10));
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldThrowException_WhenCurrentDatasetIsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(
                    datasetId,
                    false,
                    It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync((ResultDTO<Dataset?>?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _datasetService.GetObjectForEditDataset(datasetId, null, null, null, null, 1, 10));
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldThrowException_WhenNumberOfImagesToPublishIsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockAppSettingsAccessor
                .Setup(accessor => accessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100))
                .ReturnsAsync((ResultDTO<int>?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _datasetService.GetObjectForEditDataset(datasetId, null, null, null, null, 1, 10));
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldThrowException_WhenNumberOfClassesToPublishIsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockAppSettingsAccessor
                .Setup(accessor => accessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1))
                .ReturnsAsync((ResultDTO<int>?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _datasetService.GetObjectForEditDataset(datasetId, null, null, null, null, 1, 10));
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldThrowException_WhenImageResultIsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            SetupSuccessfulMocks(datasetId);
            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, "ImageAnnotations", null))
                .ReturnsAsync((ResultDTO<IEnumerable<DatasetImage>>?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _datasetService.GetObjectForEditDataset(datasetId, null, null, null, null, 1, 10));
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldFilterImages_ByImageName()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var searchByImageName = "test-image";
            SetupSuccessfulMocks(datasetId);

            var images = new List<DatasetImage>
            {
                new DatasetImage { Name = "test-image" },
                new DatasetImage { Name = "other-image" }
            };

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, "ImageAnnotations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(images));

            // Act
            var result = await _datasetService.GetObjectForEditDataset(datasetId, searchByImageName, null, null, null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.All(result.ListOfDatasetImages, image => Assert.Equal("test-image", image.Name));
        }


        [Fact]
        public async Task GetObjectForEditDataset_ShouldFilterImages_ByIsEnabled()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var searchByIsEnabledImage = true;
            SetupSuccessfulMocksWithData(datasetId);

            var images = new List<DatasetImage>
                {
                    new DatasetImage { IsEnabled = true },
                    new DatasetImage { IsEnabled = false }
                };

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, "ImageAnnotations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(images));

            // Act
            var result = await _datasetService.GetObjectForEditDataset(datasetId, null, searchByIsEnabledImage, null, null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.All(result.ListOfDatasetImages, image => Assert.True(image.IsEnabled));
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldFilterImages_ByIsAnnotated()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var searchByIsAnnotatedImage = true;
            SetupSuccessfulMocksWithData(datasetId);

            var images = new List<DatasetImage>
                {
                    new DatasetImage
                    {
                        Id = Guid.NewGuid(),
                        ImageAnnotations = new List<ImageAnnotation>
                        {
                            new ImageAnnotation { Id = Guid.NewGuid(), DatasetImageId = Guid.NewGuid() }
                        }
                    },
                    new DatasetImage
                    {
                        Id = Guid.NewGuid(),
                        ImageAnnotations = new List<ImageAnnotation>()
                    }
                };

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, "ImageAnnotations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(images));

            // Act
            var result = await _datasetService.GetObjectForEditDataset(datasetId, null, searchByIsAnnotatedImage, null, null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.NumberOfAnnotatedImages);
        }

        [Fact]
        public async Task GetObjectForEditDataset_ShouldReturnCorrectEditDatasetDTO_NoFilters()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            SetupSuccessfulMocksWithData(datasetId); // Setup the mocks with required data

            // Act
            var result = await _datasetService.GetObjectForEditDataset(datasetId, null, null, null, null, 1, 10);

            // Assert
            Assert.NotNull(result);

            // Validate properties of EditDatasetDTO
            Assert.Equal(1, result.NumberOfDatasetClasses);
            Assert.Equal("MappedDataset", result.CurrentDataset.Name);
            Assert.Equal(2, result.NumberOfDatasetImages);
            Assert.Equal(1, result.NumberOfClassesNeededToPublishDataset);
            Assert.Equal(100, result.NumberOfImagesNeededToPublishDataset);
            Assert.Equal(1, result.NumberOfEnabledImages);
            Assert.Equal(2, result.NumberOfAnnotatedImages);
            Assert.False(result.AllEnabledImagesHaveAnnotations);
            Assert.NotEmpty(result.ListOfAllDatasetImagesUnFiltered);

            Assert.Equal("MappedClass", result.UninsertedDatasetRootClasses.First().ClassName);
            Assert.Single(result.UninsertedDatasetSubclasses);
        }


        private void SetupSuccessfulMocks(Guid datasetId)
        {
            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            _mockDatasetClassesRepository
                .Setup(repo => repo.GetAll(null, null, false, "ParentClass,Datasets", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>()));

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(
                    datasetId,
                    false,
                    It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(new Dataset { Id = datasetId }));

            _mockAppSettingsAccessor
                .Setup(accessor => accessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100))
                .ReturnsAsync(ResultDTO<int>.Ok(100));

            _mockAppSettingsAccessor
                .Setup(accessor => accessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, "ImageAnnotations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(new List<DatasetImage>()));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(new List<ImageAnnotation>()));

            _mockMapper
                .Setup(mapper => mapper.Map<List<DatasetClassDTO>>(It.IsAny<List<DatasetClass>>()))
                .Returns(new List<DatasetClassDTO>());

            _mockMapper
                .Setup(mapper => mapper.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(new DatasetDTO());

            _mockMapper
                .Setup(mapper => mapper.Map<List<DatasetImageDTO>>(It.IsAny<List<DatasetImage>>()))
                .Returns(new List<DatasetImageDTO>());
        }

        private void SetupSuccessfulMocksWithData(Guid datasetId)
        {
            // Mock Dataset-DatasetClass relationships with sample data
            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>
                {
                new Dataset_DatasetClass
                {
                    DatasetId = datasetId,
                    DatasetClassId = Guid.NewGuid(),
                    DatasetClass = new DatasetClass { Id = Guid.NewGuid(), ClassName = "Class1" },
                    Dataset = new Dataset // Ensure this is not null
                    {
                        Id = datasetId,
                        ParentDatasetId = Guid.NewGuid(), // Set this to a valid GUID or null
                        DatasetClasses = new List<Dataset_DatasetClass>(),
                        DatasetImages = new List<DatasetImage>()
                    }
                }
                }));


            // Mock DatasetClasses with sample classes
            _mockDatasetClassesRepository
                .Setup(repo => repo.GetAll(null, null, false, "ParentClass,Datasets", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>
                {
            new DatasetClass { Id = Guid.NewGuid(), ClassName = "ParentClass", ParentClassId = null },
            new DatasetClass { Id = Guid.NewGuid(), ClassName = "SubClass", ParentClassId = Guid.NewGuid() }
                }));

            // Mock Dataset entity with associated sample data
            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(
                    datasetId,
                    false,
                    It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(new Dataset
                {
                    Id = datasetId,
                    Name = "Test Dataset",
                    CreatedBy = new ApplicationUser { Id = "creator-id", UserName = "Creator" },
                    UpdatedBy = new ApplicationUser { Id = "updater-id", UserName = "Updater" },
                    ParentDataset = new Dataset { Id = Guid.NewGuid(), Name = "ParentDataset" },
                    DatasetClasses = new List<Dataset_DatasetClass>
                    {
                        new Dataset_DatasetClass
                        {
                            DatasetClass = new DatasetClass { Id = Guid.NewGuid(), ClassName  = "Class1" }
                        }
                                },
                    DatasetImages = new List<DatasetImage>
                                {
                        new DatasetImage
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Image",
                            IsEnabled = true,
                            ImageAnnotations = new List<ImageAnnotation>
                            {
                                new ImageAnnotation
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedBy = new ApplicationUser { Id = "annotator-id", UserName = "Annotator" },
                                    DatasetImageId = Guid.NewGuid()
                                }
                            }
                        }
                    }
                }));


            // Mock application settings for required images and classes to publish
            _mockAppSettingsAccessor
                .Setup(accessor => accessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100))
                .ReturnsAsync(ResultDTO<int>.Ok(100));

            _mockAppSettingsAccessor
                .Setup(accessor => accessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            // Mock DatasetImages with sample image data
            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, "ImageAnnotations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(new List<DatasetImage>
                {
            new DatasetImage { Id = Guid.NewGuid(), Name = "test-image", IsEnabled = true, ImageAnnotations = new List<ImageAnnotation>() },
            new DatasetImage { Id = Guid.NewGuid(), Name = "other-image", IsEnabled = false, ImageAnnotations = new List<ImageAnnotation>() }
                }));

            // Mock ImageAnnotations
            _mockImageAnnotationsRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(new List<ImageAnnotation>
                {
            new ImageAnnotation { Id = Guid.NewGuid(), DatasetImageId = Guid.NewGuid() }
                }));

            // Mock mappings
            _mockMapper
                .Setup(mapper => mapper.Map<List<DatasetClassDTO>>(It.IsAny<List<DatasetClass>>()))
                .Returns(new List<DatasetClassDTO> { new DatasetClassDTO { Id = Guid.NewGuid(), ClassName = "MappedClass" } });

            _mockMapper
                .Setup(mapper => mapper.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(new DatasetDTO { Id = datasetId, Name = "MappedDataset" });

            _mockMapper
                .Setup(mapper => mapper.Map<List<DatasetImageDTO>>(It.IsAny<List<DatasetImage>>()))
                .Returns(new List<DatasetImageDTO>
                {
            new DatasetImageDTO { Id = Guid.NewGuid(), Name = "MappedImage", IsEnabled = true },
            new DatasetImageDTO { Id = Guid.NewGuid(), Name = "MappedImage2", IsEnabled = false }
                });
        }

        [Fact]
        public async Task AddInheritedParentClasses_ShouldReturnSuccess_WhenClassesAreAddedSuccessfully()
        {
            // Arrange
            var insertedDatasetId = Guid.NewGuid();
            var parentDatasetId = Guid.NewGuid();

            var parentClassesList = new List<Dataset_DatasetClass>
            {
                new Dataset_DatasetClass { DatasetClassId = Guid.NewGuid() },
                new Dataset_DatasetClass { DatasetClassId = Guid.NewGuid() }
            };

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(parentClassesList));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.CreateRange(It.IsAny<List<Dataset_DatasetClass>>(), true, default))
                .ReturnsAsync(ResultDTO.Ok()); // Simulating success without specifying an integer

            // Act
            var result = await _datasetService.AddInheritedParentClasses(insertedDatasetId, parentDatasetId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data); // Ensure the data indicates success
            Assert.Null(result.ErrMsg); // Ensure there's no error message
        }



        [Fact]
        public async Task AddInheritedParentClasses_ShouldReturnFailure_WhenClassesAdditionFails()
        {
            // Arrange
            var insertedDatasetId = Guid.NewGuid();
            var parentDatasetId = Guid.NewGuid();
            var parentClassesList = new List<Dataset_DatasetClass>
                {
                    new Dataset_DatasetClass { DatasetClassId = Guid.NewGuid(), DatasetId = parentDatasetId }
                };

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(parentClassesList));
            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.CreateRange(It.IsAny<List<Dataset_DatasetClass>>(), true, default))
                .ReturnsAsync(ResultDTO.Fail("Dataset classes were not added"));

            // Act
            var result = await _datasetService.AddInheritedParentClasses(insertedDatasetId, parentDatasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
            Assert.Equal("Dataset classes were not added", result.ErrMsg);
        }


        [Fact]
        public async Task AddInheritedParentClasses_ShouldThrowException_WhenParentClassesIdsNotFound()
        {
            // Arrange
            var insertedDatasetId = Guid.NewGuid();
            var parentDatasetId = Guid.NewGuid();

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await _datasetService.AddInheritedParentClasses(insertedDatasetId, parentDatasetId));

            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task PublishDataset_ShouldReturnSuccess_WhenAllConditionsAreMet()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";

            var dataset = new Dataset { Id = datasetId, IsPublished = false };
            var datasetClasses = new List<Dataset_DatasetClass>
            {
                new Dataset_DatasetClass { DatasetId = datasetId, DatasetClass = new DatasetClass() }
            };
            var datasetImages = new List<DatasetImage>
            {
                new DatasetImage { Id = Guid.NewGuid(), IsEnabled = true, DatasetId = datasetId }
            };
            var imageAnnotations = new List<ImageAnnotation>
            {
                new ImageAnnotation { DatasetImageId = datasetImages.First().Id }
            };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetClasses));

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, "ImageAnnotations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(datasetImages));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(imageAnnotations));

            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            _mockDatasetRepository
                .Setup(repo => repo.Update(It.IsAny<Dataset>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _datasetService.PublishDataset(datasetId, userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data);
        }

        [Fact]
        public async Task PublishDataset_ShouldThrowException_WhenDatasetNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, null))
                .ReturnsAsync((ResultDTO<Dataset>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.PublishDataset(datasetId, userId));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task PublishDataset_ShouldThrowException_WhenDatasetClassesNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";
            var dataset = new Dataset { Id = datasetId, IsPublished = false };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, null))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync((ResultDTO<IEnumerable<Dataset_DatasetClass>>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.PublishDataset(datasetId, userId));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task PublishDataset_ShouldThrowException_WhenDatasetImagesNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";
            var dataset = new Dataset { Id = datasetId, IsPublished = false };
            var datasetClasses = new List<Dataset_DatasetClass>
    {
        new Dataset_DatasetClass { DatasetId = datasetId, DatasetClass = new DatasetClass() }
    };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, null))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetClasses));

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync((ResultDTO<IEnumerable<DatasetImage>>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.PublishDataset(datasetId, userId));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task PublishDataset_ShouldThrowException_WhenImageAnnotationsNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";
            var dataset = new Dataset { Id = datasetId, IsPublished = false };
            var datasetClasses = new List<Dataset_DatasetClass>
    {
        new Dataset_DatasetClass { DatasetId = datasetId, DatasetClass = new DatasetClass() }
    };
            var datasetImages = new List<DatasetImage>
    {
        new DatasetImage { Id = Guid.NewGuid(), IsEnabled = true }
    };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, null))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetClasses));

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(datasetImages));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync((ResultDTO<IEnumerable<ImageAnnotation>>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.PublishDataset(datasetId, userId));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task PublishDataset_ShouldReturnFailure_WhenInsufficientClassesOrImages()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";
            var dataset = new Dataset { Id = datasetId, IsPublished = false };

            var datasetClasses = new List<Dataset_DatasetClass>(); // No classes
            var datasetImages = new List<DatasetImage>
    {
        new DatasetImage { Id = Guid.NewGuid(), IsEnabled = true, DatasetId = datasetId } // Image associated with the dataset
    };

            // Set up the mock for the dataset repository
            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset)); // Ensure it returns the dataset

            // Mocking dataset classes repository
            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetClasses)); // No classes

            // Mocking dataset images repository
            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, "ImageAnnotations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(datasetImages)); // Return dataset images

            // Mocking image annotations repository
            _mockImageAnnotationsRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(new List<ImageAnnotation>())); // No annotations

            // Setting up application settings for the number of images and classes required
            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100))
                .ReturnsAsync(ResultDTO<int>.Ok(5)); // Require more images

            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            // Act
            var result = await _datasetService.PublishDataset(datasetId, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
        }



        [Fact]
        public async Task PublishDataset_ShouldReturnFailure_WhenUpdateFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";
            var dataset = new Dataset { Id = datasetId, IsPublished = false };
            var datasetClasses = new List<Dataset_DatasetClass>
                {
                    new Dataset_DatasetClass { DatasetId = datasetId, DatasetClass = new DatasetClass() }
                };
            var datasetImages = new List<DatasetImage>
                {
                    new DatasetImage { Id = Guid.NewGuid(), IsEnabled = true, DatasetId = datasetId }
                };
            var imageAnnotations = new List<ImageAnnotation>
                {
                    new ImageAnnotation { DatasetImageId = datasetImages.First().Id }
                };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(null, null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetClasses));

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, "ImageAnnotations", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(datasetImages));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<ImageAnnotation>>.Ok(imageAnnotations));

            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            _mockAppSettingsAccessor
                .Setup(x => x.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            _mockDatasetRepository
                .Setup(repo => repo.Update(It.IsAny<Dataset>(), true, default))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _datasetService.PublishDataset(datasetId, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(3, result.Data);
            Assert.Equal("Update failed", result.ErrMsg);
        }

        [Fact]
        public async Task SetAnnotationsPerSubclass_ShouldReturnSuccess_WhenUpdateIsSuccessful()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";
            var dataset = new Dataset { Id = datasetId, AnnotationsPerSubclass = false };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetRepository
                .Setup(repo => repo.Update(dataset, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _datasetService.SetAnnotationsPerSubclass(datasetId, true, userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data);
        }

        [Fact]
        public async Task SetAnnotationsPerSubclass_ShouldReturnFailure_WhenDatasetNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Fail("Object not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.SetAnnotationsPerSubclass(datasetId, true, userId));
            Assert.Equal("Object not found", exception.Message);

        }

        [Fact]
        public async Task SetAnnotationsPerSubclass_ShouldReturnFailure_WhenUpdateFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var userId = "test-user-id";
            var dataset = new Dataset { Id = datasetId, AnnotationsPerSubclass = false };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetRepository
                .Setup(repo => repo.Update(dataset, true, default))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _datasetService.SetAnnotationsPerSubclass(datasetId, true, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
        }

        [Fact]
        public async Task EnableAllImagesInDataset_ShouldReturnSuccess_WhenImagesEnabledSuccessfully()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage { Id = Guid.NewGuid(), IsEnabled = false },
            new DatasetImage { Id = Guid.NewGuid(), IsEnabled = false }
        }
            };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "DatasetImages"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetRepository
                .Setup(repo => repo.Update(dataset, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _datasetService.EnableAllImagesInDataset(datasetId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EnableAllImagesInDataset_ShouldReturnFailure_WhenDatasetNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "DatasetImages"))
                .ReturnsAsync(ResultDTO<Dataset>.Fail("Dataset not found"));

            // Act
            var result = await _datasetService.EnableAllImagesInDataset(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting Dataset", result.ErrMsg);
        }

        [Fact]
        public async Task EnableAllImagesInDataset_ShouldReturnFailure_WhenUpdateFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage { Id = Guid.NewGuid(), IsEnabled = false }
        }
            };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "DatasetImages"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetRepository
                .Setup(repo => repo.Update(dataset, true, default))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _datasetService.EnableAllImagesInDataset(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error updating Dataset", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetClassForDataset_ShouldThrowException_WhenDatasetClassNotFound()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), false, null))
                .ReturnsAsync(ResultDTO<Dataset_DatasetClass?>.Fail("Object not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.DeleteDatasetClassForDataset(selectedClassId, datasetId, "test-user-id"));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task DeleteDatasetClassForDataset_ShouldThrowException_WhenDatasetNotFound()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), false, null))
                .ReturnsAsync(ResultDTO<Dataset_DatasetClass>.Ok(new Dataset_DatasetClass { DatasetId = datasetId, DatasetClassId = selectedClassId }));

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, It.IsAny<bool>(), "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Object not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.DeleteDatasetClassForDataset(selectedClassId, datasetId, "test-user-id"));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task DeleteDatasetClassForDataset_ShouldThrowException_WhenDatasetClassDbNotFound()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), false, null))
                .ReturnsAsync(ResultDTO<Dataset_DatasetClass?>.Ok(null)); // Simulate not found

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.DeleteDatasetClassForDataset(selectedClassId, datasetId, "test-user-id"));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task DeleteDatasetClassForDataset_ShouldReturnFailure_WhenDatasetClassDeletionFails()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetClass = new Dataset_DatasetClass { DatasetId = datasetId, DatasetClassId = selectedClassId };
            var dataset = new Dataset { Id = datasetId };

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), false, null))
                .ReturnsAsync(ResultDTO<Dataset_DatasetClass>.Ok(datasetClass));

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, It.IsAny<bool>(), "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.Delete(datasetClass, true, default))
                .ReturnsAsync(ResultDTO.Fail("Dataset class was not deleted"));

            // Act
            var result = await _datasetService.DeleteDatasetClassForDataset(selectedClassId, datasetId, "test-user-id");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
            Assert.Equal("Dataset class was not deleted", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetClassForDataset_ShouldReturnSuccess_WhenDeletionIsSuccessful()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var datasetClass = new Dataset_DatasetClass { DatasetId = datasetId, DatasetClassId = selectedClassId };
            var dataset = new Dataset { Id = datasetId };

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), false, null))
                .ReturnsAsync(ResultDTO<Dataset_DatasetClass>.Ok(datasetClass));

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, It.IsAny<bool>(), "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset>.Ok(dataset));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.Delete(datasetClass, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetRepository
                .Setup(repo => repo.Update(It.IsAny<Dataset>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _datasetService.DeleteDatasetClassForDataset(selectedClassId, datasetId, "test-user-id");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data);
        }

        [Fact]
        public async Task DeleteDatasetCompletelyIncluded_ShouldReturnFailure_WhenDatasetIdIsEmpty()
        {
            // Arrange
            var datasetId = Guid.Empty;

            // Act
            var result = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Dataset Id", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetCompletelyIncluded_ShouldReturnFailure_WhenDatasetNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, true, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Error getting Dataset"));

            // Act
            var result = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting Dataset", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetCompletelyIncluded_ShouldReturnFailure_WhenDatasetIsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, true, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(null));

            // Act
            var result = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting Dataset", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetCompletelyIncluded_ShouldReturnFailure_WhenImageAnnotationsDeletionFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                DatasetClasses = new List<Dataset_DatasetClass>(),
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage
            {
                ImageAnnotations = new List<ImageAnnotation>
                {
                    new ImageAnnotation
                    {
                        Id = Guid.NewGuid()
                    }
                }
            }
        }
            };

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, true, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<IEnumerable<ImageAnnotation>>(), false, default))
                .ReturnsAsync(ResultDTO.Fail("Error deleting annotations"));

            // Act
            var result = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error deleting annotations", result.ErrMsg);
        }


        [Fact]
        public async Task DeleteDatasetCompletelyIncluded_ShouldReturnFailure_WhenImagesDeletionFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                DatasetClasses = new List<Dataset_DatasetClass>(),
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage
            {
                ImageAnnotations = new List<ImageAnnotation>()
            }
        }
            };

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, true, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<ImageAnnotation>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetImagesRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<DatasetImage>>(), false, default))
                .ReturnsAsync(ResultDTO.Fail("Error deleting images"));

            // Act
            var result = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error deleting images", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetCompletelyIncluded_ShouldReturnFailure_WhenDatasetClassesDeletionFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                DatasetClasses = new List<Dataset_DatasetClass>
        {
            new Dataset_DatasetClass()
        },
                DatasetImages = new List<DatasetImage>()
            };

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, true, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<ImageAnnotation>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetImagesRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<DatasetImage>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<Dataset_DatasetClass>>(), false, default))
                .ReturnsAsync(ResultDTO.Fail("Error deleting dataset classes"));

            // Act
            var result = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error deleting dataset classes", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetCompletelyIncluded_ShouldReturnFailure_WhenDatasetDeletionFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                DatasetClasses = new List<Dataset_DatasetClass>(),
                DatasetImages = new List<DatasetImage>()
            };

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, true, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<ImageAnnotation>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetImagesRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<DatasetImage>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<Dataset_DatasetClass>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetRepository
                .Setup(repo => repo.Delete(It.IsAny<Dataset>(), false, default))
                .ReturnsAsync(ResultDTO.Fail("Error deleting dataset"));

            // Act
            var result = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error deleting dataset", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetCompletelyIncluded_ShouldReturnFailure_WhenSaveChangesFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                DatasetClasses = new List<Dataset_DatasetClass>(),
                DatasetImages = new List<DatasetImage>()
            };


            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, true, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<ImageAnnotation>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetImagesRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<DatasetImage>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<Dataset_DatasetClass>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetRepository
                .Setup(repo => repo.Delete(It.IsAny<Dataset>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetRepository
                .Setup(repo => repo.SaveChangesAsync(default))
                .ReturnsAsync(ResultDTO<int>.Fail("Error saving changes"));

            // Act
            var result = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error saving changes", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetCompletelyIncluded_ShouldReturnSuccess_WhenDeletionIsSuccessful()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                DatasetClasses = new List<Dataset_DatasetClass>(),
                DatasetImages = new List<DatasetImage>()
            };

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, true, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockImageAnnotationsRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<ImageAnnotation>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetImagesRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<DatasetImage>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<ICollection<Dataset_DatasetClass>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetRepository
                .Setup(repo => repo.Delete(It.IsAny<Dataset>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetRepository
                .Setup(repo => repo.SaveChangesAsync(default))
                .ReturnsAsync(ResultDTO<int>.Ok(1));

            // Act
            var result = await _datasetService.DeleteDatasetCompletelyIncluded(datasetId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteDataset_ShouldReturnSuccess_WhenDatasetIsDeleted()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset { Id = datasetId, ParentDatasetId = null };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockDatasetRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset, bool>>>(), null, false, "CreatedBy", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset>>.Ok(new List<Dataset>()));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(new List<DatasetImage>()));

            _mockDatasetRepository
                .Setup(repo => repo.Delete(dataset, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _datasetService.DeleteDataset(datasetId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data);
        }

        [Fact]
        public async Task DeleteDataset_ShouldReturnFailure_WhenDatasetHasChildDatasets()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset { Id = datasetId, ParentDatasetId = null };
            var childDataset = new Dataset { ParentDatasetId = datasetId };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockDatasetRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset, bool>>>(), null, false, "CreatedBy", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset>>.Ok(new List<Dataset> { childDataset }));

            // Act
            var result = await _datasetService.DeleteDataset(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
            Assert.Equal("This dataset can not be deleted because there are subdatasets. Delete first the subdatasets!", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDataset_ShouldReturnFailure_WhenListOfAllDatasetsNotFound()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset { Id = datasetId, ParentDatasetId = null };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockDatasetRepository
                .Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset>>.Fail("Object not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.DeleteDataset(datasetId));
            Assert.Equal("Object not found", exception.Message);
        }

        [Fact]
        public async Task DeleteDataset_ShouldReturnFailure_WhenDatasetClassDeletionFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset { Id = datasetId, ParentDatasetId = null };
            var datasetClass = new Dataset_DatasetClass { DatasetId = datasetId };

            _mockDatasetRepository
                .Setup(repo => repo.GetById(datasetId, false, "CreatedBy,UpdatedBy,ParentDataset"))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

            _mockDatasetRepository
                .Setup(repo => repo.GetAll(null, null, false, "CreatedBy", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset>>.Ok(new List<Dataset>()));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass> { datasetClass }));

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.DeleteRange(It.IsAny<IEnumerable<Dataset_DatasetClass>>(), false, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetImagesRepository
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetImage, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetImage>>.Ok(new List<DatasetImage>()));

            _mockDatasetRepository
                .Setup(repo => repo.Delete(dataset, true, default))
                .ReturnsAsync(ResultDTO.Fail("Error deleting dataset"));

            // Act
            var result = await _datasetService.DeleteDataset(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(3, result.Data);
            Assert.Equal("Error deleting dataset", result.ErrMsg);
        }

        #endregion

        [Fact]
        public async Task ExportDatasetAsCOCOFormat_ShouldReturnFailure_WhenDatasetIdIsEmpty()
        {
            // Arrange
            var datasetId = Guid.Empty;

            // Act
            var result = await _datasetService.ExportDatasetAsCOCOFormat(datasetId, "option", "location", false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Dataset Id", result.ErrMsg);
        }

        [Fact]
        public async Task ExportDatasetAsCOCOFormat_ShouldReturnFailure_WhenGetByIdIncludeThenAllFails()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, false, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Fail("Error retrieving dataset"));

            // Act
            var result = await _datasetService.ExportDatasetAsCOCOFormat(datasetId, "option", "location", false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error retrieving dataset", result.ErrMsg);
        }

        [Fact]
        public async Task ExportDatasetAsCOCOFormat_ShouldReturnFailure_WhenDatasetIsNull()
        {
            // Arrange
            var datasetId = Guid.NewGuid();

            _mockDatasetRepository
                .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, false, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
                .ReturnsAsync(ResultDTO<Dataset?>.Ok(null));

            // Act
            var result = await _datasetService.ExportDatasetAsCOCOFormat(datasetId, "option", "location", false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting Dataset", result.ErrMsg);
        }

        //[Fact]
        //public async Task ExportDatasetAsCOCOFormat_ShouldReturnSuccess_WhenExportedWithoutSplit()
        //{
        //    // Arrange
        //    var datasetId = Guid.NewGuid();
        //    var dataset = new Dataset
        //    {
        //        Id = datasetId,
        //        DatasetImages =
        //        {
        //            new DatasetImage { Id = Guid.NewGuid(), DatasetId = datasetId ,ImageAnnotations = new List<ImageAnnotation> { new ImageAnnotation { Id = Guid.NewGuid() } } },
        //            new DatasetImage { Id = Guid.NewGuid(), DatasetId = datasetId ,ImageAnnotations = new List<ImageAnnotation> { new ImageAnnotation { Id = Guid.NewGuid() } } },
        //        }
        //    };

        //    _mockDatasetRepository
        //        .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, false, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
        //        .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

        //    // Act
        //    var result = await _datasetService.ExportDatasetAsCOCOFormat(datasetId, "option", "location", false);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //    Assert.Equal("Exported path", result.Data);
        //}

        //[Fact]
        //public async Task ExportDatasetAsCOCOFormat_ShouldReturnSuccess_WhenExportedWithSplit()
        //{
        //    // Arrange
        //    var datasetId = Guid.NewGuid();
        //    var dataset = new Dataset
        //    {
        //        Id = datasetId,
        //        DatasetImages =
        //        {
        //            new DatasetImage { Id = Guid.NewGuid(), DatasetId = datasetId ,ImageAnnotations = new List<ImageAnnotation> { new ImageAnnotation { Id = Guid.NewGuid() } } },
        //            new DatasetImage { Id = Guid.NewGuid(), DatasetId = datasetId ,ImageAnnotations = new List<ImageAnnotation> { new ImageAnnotation { Id = Guid.NewGuid() } } }
        //        }
        //    };

        //    _mockDatasetRepository
        //        .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, false, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
        //        .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

        //    // Act
        //    var result = await _datasetService.ExportDatasetAsCOCOFormat(datasetId, "option", "location", true);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //    Assert.Equal("Exported split path", result.Data);
        //}

        //[Fact]
        //public async Task ExportDatasetAsCOCOFormat_ShouldReturnFailure_WhenConversionFails()
        //{
        //    // Arrange
        //    var datasetId = Guid.NewGuid();
        //    var dataset = new Dataset
        //    {
        //        Id = datasetId,
        //        DatasetImages =
        //        {
        //            new DatasetImage { Id = Guid.NewGuid(), DatasetId = datasetId ,ImageAnnotations = new List<ImageAnnotation> { new ImageAnnotation { Id = Guid.NewGuid() } } },
        //            new DatasetImage { Id = Guid.NewGuid(), DatasetId = datasetId ,ImageAnnotations = new List<ImageAnnotation> { new ImageAnnotation { Id = Guid.NewGuid() } } },
        //        }
        //    };

        //    _mockDatasetRepository
        //        .Setup(repo => repo.GetByIdIncludeThenAll(datasetId, false, It.IsAny<(Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]>()))
        //        .ReturnsAsync(ResultDTO<Dataset?>.Ok(dataset));

        //    // Act
        //    var result = await _datasetService.ExportDatasetAsCOCOFormat(datasetId, "option", "location", false);

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Equal("Conversion failed", result.ErrMsg);
        //}

        [Fact]
        public async Task DeleteAllFilesInDatasetDirectoryAtWwwRoot_ShouldReturnFail_WhenPathIsNullOrEmpty()
        {
            // Act
            var result = await _datasetService.DeleteAllFilesInDatasetDirectoryAtWwwRoot(string.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Directory Path", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteAllFilesInDatasetDirectoryAtWwwRoot_ShouldDeleteFiles_WhenDirectoryIsValid()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), "TestDeleteDirectory");
            Directory.CreateDirectory(tempDirectory);
            var filePath1 = Path.Combine(tempDirectory, "file1.txt");
            var filePath2 = Path.Combine(tempDirectory, "file2.txt");
            File.WriteAllText(filePath1, "Test content 1");
            File.WriteAllText(filePath2, "Test content 2");

            // Act
            var result = await _datasetService.DeleteAllFilesInDatasetDirectoryAtWwwRoot(tempDirectory);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(File.Exists(filePath1)); // Check if the file was deleted
            Assert.False(File.Exists(filePath2)); // Check if the file was deleted

            // Clean up
            Directory.Delete(tempDirectory, true); // Delete the temporary directory
        }

        [Fact]
        public async Task DeleteAllFilesInDatasetDirectoryAtWwwRoot_ShouldReturnExceptionFail_WhenIOExceptionThrown()
        {
            // Arrange
            var invalidDirectoryPath = Path.Combine(Path.GetTempPath(), "NonExistentDirectory");

            // Act
            var result = await _datasetService.DeleteAllFilesInDatasetDirectoryAtWwwRoot(invalidDirectoryPath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Could not find a part of the path", result.ErrMsg); // Check for a part of the actual error message
        }

        [Fact]
        public async Task CopyImageFromSourcePathToDestinationPath_ShouldCopyFile_WhenDestinationFileDoesNotExist()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), "TestDatasetService");
            Directory.CreateDirectory(tempDirectory); // Create a temporary directory

            var sourceFilePath = Path.Combine(tempDirectory, "source.jpg");
            var destinationFilePath = Path.Combine(tempDirectory, "destination.jpg");

            // Create a source file for testing
            await File.WriteAllTextAsync(sourceFilePath, "Test content");

            // Act
            var result = _datasetService.CopyImageFromSourcePathToDestinationPath(sourceFilePath, destinationFilePath);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(File.Exists(destinationFilePath)); // Verify the file was copied

            // Clean up
            File.Delete(sourceFilePath);
            File.Delete(destinationFilePath);
            Directory.Delete(tempDirectory, true); // Delete the temporary directory
        }

        [Fact]
        public async Task CopyImageFromSourcePathToDestinationPath_ShouldDeleteAndCopyFile_WhenDestinationFileExists()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), "TestDatasetService");
            Directory.CreateDirectory(tempDirectory); // Create a temporary directory

            var sourceFilePath = Path.Combine(tempDirectory, "source.jpg");
            var destinationFilePath = Path.Combine(tempDirectory, "destination.jpg");

            // Create source and destination files for testing
            await File.WriteAllTextAsync(sourceFilePath, "Test content");
            await File.WriteAllTextAsync(destinationFilePath, "Old content");

            // Act
            var result = _datasetService.CopyImageFromSourcePathToDestinationPath(sourceFilePath, destinationFilePath);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(File.Exists(destinationFilePath)); // Verify the file was copied
            Assert.False(File.Exists(Path.Combine(tempDirectory, "old_content.jpg"))); // Check the old file is deleted

            // Clean up
            File.Delete(sourceFilePath);
            File.Delete(destinationFilePath);
            Directory.Delete(tempDirectory, true); // Delete the temporary directory
        }

        [Fact]
        public async Task CopyImageFromSourcePathToDestinationPath_ShouldReturnExceptionFail_WhenIOExceptionThrown()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), "TestDatasetService");
            Directory.CreateDirectory(tempDirectory); // Create a temporary directory

            var sourceFilePath = Path.Combine(tempDirectory, "non_existent.jpg");
            var destinationFilePath = Path.Combine(tempDirectory, "destination.jpg");

            // Act
            var result = _datasetService.CopyImageFromSourcePathToDestinationPath(sourceFilePath, destinationFilePath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Could not find file", result.ErrMsg); // Check for a part of the actual error message

            // Clean up
            Directory.Delete(tempDirectory, true); // Delete the temporary directory
        }

        [Fact]
        public async Task GetDatasetImagesDirectoryRelativePathByDatasetId_ShouldReturnPath_WhenSettingIsRetrievedSuccessfully()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var expectedPath = "Base/Path/" + datasetId.ToString();
            _mockAppSettingsAccessor
                .Setup(m => m.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages"))
                .ReturnsAsync(ResultDTO<string>.Ok("Base/Path"));

            // Act
            var result = await _datasetService.GetDatasetImagesDirectoryRelativePathByDatasetId(datasetId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetDatasetImagesDirectoryRelativePathByDatasetId_ShouldReturnFail_WhenSettingCannotBeRetrieved()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockAppSettingsAccessor
                .Setup(m => m.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages"))
                .ReturnsAsync(ResultDTO<string>.Fail("Error retrieving setting"));

            // Act
            var result = await _datasetService.GetDatasetImagesDirectoryRelativePathByDatasetId(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting base Dataset Images Directory", result.ErrMsg);
        }

        [Fact]
        public async Task GetDatasetImagesDirectoryRelativePathByDatasetId_ShouldReturnFail_WhenRetrievedPathIsNullOrEmpty()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            _mockAppSettingsAccessor
                .Setup(m => m.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages"))
                .ReturnsAsync(ResultDTO<string>.Ok(null)); // Simulate null

            // Act
            var result = await _datasetService.GetDatasetImagesDirectoryRelativePathByDatasetId(datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting base Dataset Images Directory", result.ErrMsg);
        }

        [Fact]
        public async Task GetDatasetImagesDirectoryAbsolutePathByDatasetId_ShouldReturnFail_WhenWwwRootIsNullOrEmpty()
        {
            // Act
            var result = await _datasetService.GetDatasetImagesDirectoryAbsolutePathByDatasetId(string.Empty, Guid.NewGuid());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid wwwRoot Path", result.ErrMsg);
        }

        [Fact]
        public async Task GetDatasetImagesDirectoryAbsolutePathByDatasetId_ShouldReturnFail_WhenRelativePathIsInvalid()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), "TestGetDatasetImagesDirectoryAbsolutePath");
            Directory.CreateDirectory(tempDirectory);

            var datasetId = Guid.NewGuid();
            var wwwRoot = tempDirectory; // Use temp directory

            _mockAppSettingsAccessor
                .Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages"))
                .ReturnsAsync(ResultDTO<string>.Ok("Images"));

            // Act
            var result = await _datasetService.GetDatasetImagesDirectoryAbsolutePathByDatasetId(wwwRoot, datasetId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Path.Combine(wwwRoot, "Images", datasetId.ToString()), result.Data);

            // Clean up
            Directory.Delete(tempDirectory, true);
        }

        [Fact]
        public async Task GetDatasetImagesDirectoryAbsolutePathByDatasetId_ShouldReturnFail_WhenGettingRelativePathFails()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), "TestGetDatasetImagesDirectoryAbsolutePath");
            Directory.CreateDirectory(tempDirectory);

            var datasetId = Guid.NewGuid();
            var wwwRoot = tempDirectory; // Use temp directory

            _mockAppSettingsAccessor
                .Setup(a => a.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages"))
                .ReturnsAsync(ResultDTO<string>.Fail("Error getting base Dataset Images Directory"));

            // Act
            var result = await _datasetService.GetDatasetImagesDirectoryAbsolutePathByDatasetId(wwwRoot, datasetId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error getting base Dataset Images Directory", result.ErrMsg);

            // Clean up
            Directory.Delete(tempDirectory, true);
        }

        [Fact]
        public async Task GetDatasetImageRelativePathByDatasetIdAndFileName_ShouldReturnFail_WhenFileNameIsNullOrEmpty()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var fileName = string.Empty; // Invalid file name

            // Act
            var result = await _datasetService.GetDatasetImageRelativePathByDatasetIdAndFileName(datasetId, fileName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid File Name", result.ErrMsg);
        }

        [Fact]
        public async Task GetDatasetImageAbsolutePathByDatasetIdAndFileName_ShouldReturnFail_WhenWwwRootIsNullOrEmpty()
        {
            // Arrange
            var datasetId = Guid.NewGuid();
            var fileName = "image.jpg";
            var wwwRoot = string.Empty; // Invalid wwwRoot

            // Act
            var result = await _datasetService.GetDatasetImageAbsolutePathByDatasetIdAndFileName(wwwRoot, datasetId, fileName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid wwwRoot Path", result.ErrMsg);
        }


        //[Fact]
        //public async Task ImportDatasetCocoFormatedAtDirectoryPath_ShouldReturnSuccess_WhenDataIsValid()
        //{
        //    // Arrange
        //    string datasetName = "Test Dataset";
        //    string cocoDirPath = "/path/to/coco";
        //    string userId = "test-user-id";
        //    string saveDir = null;

        //    var cocoDataset = new CocoDatasetDTO
        //    {
        //        Info = new CocoInfoDTO { Description = "Test Coco Dataset" },
        //        Categories = new List<CocoCategoryDTO>
        //            {
        //                new CocoCategoryDTO { Id = 1, Name = "Category1" }
        //            },
        //        Images = new List<CocoImageDTO>
        //            {
        //                new CocoImageDTO { Id = 1, FileName = "image1.jpg", Height = 1, Width = 1 }
        //            },
        //        Annotations = new List<CocoAnnotationDTO>
        //            {
        //                new CocoAnnotationDTO { Id = 1, CategoryId = 1, ImageId = 1, Bbox = new List<float>{ 1, 2, 3 } }
        //}
        //    };

        //    _mockCocoUtilsService
        //        .Setup(service => service.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(cocoDirPath, false))
        //        .ReturnsAsync(ResultDTO<CocoDatasetDTO>.Ok(cocoDataset));

        //    _mockDatasetClassesRepository
        //        .Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<DatasetClass, bool>>>(), false, null))
        //        .ReturnsAsync(ResultDTO<DatasetClass>.Ok(new DatasetClass { Id = Guid.NewGuid(), ClassName = "Category1" }));

        //    _mockDatasetRepository
        //        .Setup(repo => repo.Create(It.IsAny<Dataset>(), false, default))
        //        .ReturnsAsync(ResultDTO.Ok());

        //    _mockMapper
        //        .Setup(m => m.Map<DatasetDTO>(It.IsAny<Dataset>()))
        //        .Returns(new DatasetDTO());

        //    // Act
        //    var result = await _datasetService.ImportDatasetCocoFormatedAtDirectoryPath(datasetName, cocoDirPath, userId, saveDir, false);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //}


        [Fact]
        public async Task ImportDatasetCocoFormatedAtDirectoryPath_ShouldReturnFail_WhenCocoDatasetRetrievalFails()
        {
            // Arrange
            string datasetName = "Test Dataset";
            string cocoDirPath = "/path/to/coco";
            string userId = "test-user-id";

            _mockCocoUtilsService
                .Setup(service => service.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(cocoDirPath, false))
                .ReturnsAsync(ResultDTO<CocoDatasetDTO>.Fail("Error retrieving Coco dataset"));

            // Act
            var result = await _datasetService.ImportDatasetCocoFormatedAtDirectoryPath(datasetName, cocoDirPath, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error retrieving Coco dataset", result.ErrMsg);
        }

        [Fact]
        public async Task ImportDatasetCocoFormatedAtDirectoryPath_ShouldReturnFail_WhenUserIdIsInvalid()
        {
            // Arrange
            string datasetName = "Test Dataset";
            string cocoDirPath = "/path/to/coco";
            string userId = ""; // Invalid user ID
            var cocoDataset = new CocoDatasetDTO
            {
                Info = new CocoInfoDTO { Description = "Test Coco Dataset" },
                Categories = new List<CocoCategoryDTO> { new CocoCategoryDTO { Id = 1, Name = "Category1" } },
                Images = new List<CocoImageDTO> { new CocoImageDTO { Id = 1, FileName = "image1.jpg", Height = 1, Width = 1 } },
                Annotations = new List<CocoAnnotationDTO> { new CocoAnnotationDTO { Id = 1, CategoryId = 1, ImageId = 1, Bbox = new List<float> { 0, 0, 100, 100 } } }
            };

            _mockCocoUtilsService
                .Setup(service => service.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(cocoDirPath, false))
                .ReturnsAsync(ResultDTO<CocoDatasetDTO>.Ok(cocoDataset));
            // Act
            var result = await _datasetService.ImportDatasetCocoFormatedAtDirectoryPath(datasetName, cocoDirPath, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid User Id", result.ErrMsg);
        }

        [Fact]
        public async Task ImportDatasetCocoFormatedAtDirectoryPath_ShouldReturnExceptionFail_WhenExceptionIsThrown()
        {
            // Arrange
            string datasetName = "Test Dataset";
            string cocoDirPath = "/path/to/coco";
            string userId = "test-user-id";

            _mockCocoUtilsService
                .Setup(service => service.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(cocoDirPath, false))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _datasetService.ImportDatasetCocoFormatedAtDirectoryPath(datasetName, cocoDirPath, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Unexpected error", result.ErrMsg);
        }
        [Fact]
        public async Task ImportDatasetCocoFormatedAtDirectoryPath_ShouldReturnSuccess_WhenValidDatasetIsImported()
        {
            // Arrange
            string datasetName = "Test Dataset";
            string cocoDirPath = "/path/to/coco";
            string userId = "test-user-id";

            var cocoDataset = new CocoDatasetDTO
            {
                Info = new CocoInfoDTO { Description = "Test Coco Dataset" },
                Categories = new List<CocoCategoryDTO> { new CocoCategoryDTO { Id = 1, Name = "Category1" } },
                Images = new List<CocoImageDTO> { new CocoImageDTO { Id = 1, FileName = "image1.jpg", Height = 100, Width = 100 } },
                Annotations = new List<CocoAnnotationDTO> { new CocoAnnotationDTO { Id = 1, CategoryId = 1, ImageId = 1, Bbox = new List<float> { 0, 0, 100, 100 } } }
            };

            _mockCocoUtilsService
                .Setup(service => service.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(cocoDirPath, false))
                .ReturnsAsync(ResultDTO<CocoDatasetDTO>.Ok(cocoDataset));

            _mockDatasetClassesRepository
                .Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<DatasetClass, bool>>>(), false, null))
                .ReturnsAsync(ResultDTO<DatasetClass>.Fail("Not found"));

            _mockDatasetClassesRepository
                .Setup(repo => repo.Create(It.IsAny<DatasetClass>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockDatasetRepository
                .Setup(repo => repo.Create(It.IsAny<Dataset>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            _mockAppSettingsAccessor
                .Setup(m => m.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages"))
                .ReturnsAsync(ResultDTO<string>.Ok("Base/Path"));

            // Mock the setting for "DatasetThumbnailsFolder"
            _mockAppSettingsAccessor
                .Setup(m => m.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails"))
                .ReturnsAsync(ResultDTO<string>.Ok("Base/Thumbnails"));
            // Mock the mapper to return a fully initialized DatasetDTO
            _mockMapper
                .Setup(m => m.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(new DatasetDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Mapped Dataset",
                    Description = "This is a mapped dataset.",
                    IsPublished = true,
                    ParentDatasetId = null,
                    ParentDataset = null,
                    CreatedById = "user-id",
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = null,
                    UpdatedById = null,
                    UpdatedOn = null,
                    UpdatedBy = null,
                    AnnotationsPerSubclass = true,
                    DatasetClasses = new List<Dataset_DatasetClassDTO>(),
                    DatasetImages = new List<DatasetImageDTO>()
                });


            // Act
            var result = await _datasetService.ImportDatasetCocoFormatedAtDirectoryPath(datasetName, cocoDirPath, userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }


        [Fact]
        public void GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity_ShouldReturnExpectedResult()
        {
            var datasetId = Guid.NewGuid();

            // Arrange
            var dataset = new Dataset
            {
                Id = datasetId,
                Name = "Test Dataset",
                Description = "This is a test dataset.",
                IsPublished = true,
                CreatedById = "user-id",
                AnnotationsPerSubclass = false,
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage { Id = Guid.NewGuid(), FileName = "Image1.jpg", ImagePath = "path/to/Image1.jpg" }
            // Add more DatasetImage objects as necessary
        },
                DatasetClasses = new List<Dataset_DatasetClass>
        {
            new Dataset_DatasetClass
            {
                DatasetId = datasetId,
                DatasetClass = new DatasetClass
                {
                    Id = Guid.NewGuid(),
                    ClassName = "Class1"
                },
                DatasetClassValue = 1 // Set a valid class value
            }
        }
            };

            string exportOption = "AllImages";

            // Mock the mapping from Dataset to DatasetDTO
            var datasetDTO = new DatasetDTO
            {
                Id = datasetId,
                Name = dataset.Name,
                Description = dataset.Description,
                IsPublished = dataset.IsPublished,
                // Add other necessary properties
            };

            _mockMapper
                .Setup(m => m.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(datasetDTO); // Mock the return value for mapping

            _mockMapper
                .Setup(m => m.Map<List<DatasetImageDTO>>(It.IsAny<List<DatasetImage>>()))
                .Returns(new List<DatasetImageDTO>
                {
            new DatasetImageDTO { IdInt = 0, FileName = "Image1.jpg", ImagePath = "path/to/Image1.jpg" }
                });

            _mockMapper
                .Setup(m => m.Map<List<ImageAnnotationDTO>>(It.IsAny<List<ImageAnnotation>>()))
                .Returns(new List<ImageAnnotationDTO> { /* Add expected ImageAnnotationDTOs if necessary */ });

            // Act
            var result = _datasetService.GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity(dataset, exportOption, true, false);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(dataset.DatasetImages.Count, result.Data.DatasetImages.Count);
            // Add more assertions as necessary to validate the properties of the returned DatasetFullIncludeDTO
        }



        [Fact]
        public void GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity_ShouldReturnExpectedResult_WithAnnotatedImages()
        {
            var datasetId = Guid.NewGuid();

            // Arrange
            var dataset = new Dataset
            {
                Id = datasetId,
                Name = "Test Dataset",
                Description = "This is a test dataset.",
                IsPublished = true,
                CreatedById = "user-id",
                AnnotationsPerSubclass = false,
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage { Id = Guid.NewGuid(), FileName = "Image1.jpg", ImagePath = "path/to/Image1.jpg" }
            // Add more DatasetImage objects as necessary
        },
                DatasetClasses = new List<Dataset_DatasetClass>
        {
            new Dataset_DatasetClass
            {
                DatasetId = datasetId,
                DatasetClass = new DatasetClass
                {
                    Id = Guid.NewGuid(),
                    ClassName = "Class1"
                },
                DatasetClassValue = 1 // Set a valid class value
            }
        }
            };

            string exportOption = "AnnotatedImages";

            // Mock the mapping from Dataset to DatasetDTO
            var datasetDTO = new DatasetDTO
            {
                Id = datasetId,
                Name = dataset.Name,
                Description = dataset.Description,
                IsPublished = dataset.IsPublished,
                // Add other necessary properties
            };

            _mockMapper
                .Setup(m => m.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(datasetDTO); // Mock the return value for mapping

            _mockMapper
                .Setup(m => m.Map<List<DatasetImageDTO>>(It.IsAny<List<DatasetImage>>()))
                .Returns(new List<DatasetImageDTO>
                {
            new DatasetImageDTO { IdInt = 0, FileName = "Image1.jpg", ImagePath = "path/to/Image1.jpg" }
                });

            _mockMapper
                .Setup(m => m.Map<List<ImageAnnotationDTO>>(It.IsAny<List<ImageAnnotation>>()))
                .Returns(new List<ImageAnnotationDTO> { /* Add expected ImageAnnotationDTOs if necessary */ });

            // Act
            var result = _datasetService.GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity(dataset, exportOption, true, false);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(dataset.DatasetImages.Count, result.Data.DatasetImages.Count);
            // Add more assertions as necessary to validate the properties of the returned DatasetFullIncludeDTO
        }

        [Fact]
        public void GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity_ShouldReturnExpectedResult_WithUnannotatedImages()
        {
            var datasetId = Guid.NewGuid();

            // Arrange
            var dataset = new Dataset
            {
                Id = datasetId,
                Name = "Test Dataset",
                Description = "This is a test dataset.",
                IsPublished = true,
                CreatedById = "user-id",
                AnnotationsPerSubclass = false,
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage { Id = Guid.NewGuid(), FileName = "Image1.jpg", ImagePath = "path/to/Image1.jpg" }
            // Add more DatasetImage objects as necessary
        },
                DatasetClasses = new List<Dataset_DatasetClass>
        {
            new Dataset_DatasetClass
            {
                DatasetId = datasetId,
                DatasetClass = new DatasetClass
                {
                    Id = Guid.NewGuid(),
                    ClassName = "Class1"
                },
                DatasetClassValue = 1 // Set a valid class value
            }
        }
            };

            string exportOption = "";

            // Mock the mapping from Dataset to DatasetDTO
            var datasetDTO = new DatasetDTO
            {
                Id = datasetId,
                Name = dataset.Name,
                Description = dataset.Description,
                IsPublished = dataset.IsPublished,
                // Add other necessary properties
            };

            _mockMapper
                .Setup(m => m.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(datasetDTO); // Mock the return value for mapping

            _mockMapper
                .Setup(m => m.Map<List<DatasetImageDTO>>(It.IsAny<List<DatasetImage>>()))
                .Returns(new List<DatasetImageDTO>
                {
            new DatasetImageDTO { IdInt = 0, FileName = "Image1.jpg", ImagePath = "path/to/Image1.jpg" }
                });

            _mockMapper
                .Setup(m => m.Map<List<ImageAnnotationDTO>>(It.IsAny<List<ImageAnnotation>>()))
                .Returns(new List<ImageAnnotationDTO> { /* Add expected ImageAnnotationDTOs if necessary */ });

            // Act
            var result = _datasetService.GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity(dataset, exportOption, false, false);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(dataset.DatasetImages.Count, result.Data.DatasetImages.Count);
            // Add more assertions as necessary to validate the properties of the returned DatasetFullIncludeDTO
        }

        [Fact]
        public void GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity_ShouldReturnExpectedResult_WithEnabledImages()
        {
            var datasetId = Guid.NewGuid();

            // Arrange
            var dataset = new Dataset
            {
                Id = datasetId,
                Name = "Test Dataset",
                Description = "This is a test dataset.",
                IsPublished = true,
                CreatedById = "user-id",
                AnnotationsPerSubclass = false,
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage { Id = Guid.NewGuid(), FileName = "Image1.jpg", ImagePath = "path/to/Image1.jpg" }
            // Add more DatasetImage objects as necessary
        },
                DatasetClasses = new List<Dataset_DatasetClass>
        {
            new Dataset_DatasetClass
            {
                DatasetId = datasetId,
                DatasetClass = new DatasetClass
                {
                    Id = Guid.NewGuid(),
                    ClassName = "Class1"
                },
                DatasetClassValue = 1
            }
        }
            };

            string exportOption = "EnabledImages";

            // Mock the mapping from Dataset to DatasetDTO
            var datasetDTO = new DatasetDTO
            {
                Id = datasetId,
                Name = dataset.Name,
                Description = dataset.Description,
                IsPublished = dataset.IsPublished,
                // Add other necessary properties
            };

            _mockMapper
                .Setup(m => m.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(datasetDTO); // Mock the return value for mapping

            _mockMapper
                .Setup(m => m.Map<List<DatasetImageDTO>>(It.IsAny<List<DatasetImage>>()))
                .Returns(new List<DatasetImageDTO>
                {
            new DatasetImageDTO { IdInt = 0, FileName = "Image1.jpg", ImagePath = "path/to/Image1.jpg" }
                });

            _mockMapper
                .Setup(m => m.Map<List<ImageAnnotationDTO>>(It.IsAny<List<ImageAnnotation>>()))
                .Returns(new List<ImageAnnotationDTO> { /* Add expected ImageAnnotationDTOs if necessary */ });

            // Act
            var result = _datasetService.GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity(dataset, exportOption, false, true);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(dataset.DatasetImages.Count, result.Data.DatasetImages.Count);
            // Add more assertions as necessary to validate the properties of the returned DatasetFullIncludeDTO
        }

        [Fact]
        public async Task ConvertDatasetEntityToCocoDatasetWithAssignedIdIntsAsSplitDataset_ShouldReturnZipFilePath_WhenSuccessful()
        {
            // Create a temporary test directory
            var _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            // Prepare the image directory and file
            var unitTestImgPath = Path.Combine(Path.GetFullPath("wwwroot"), "unit-test-img");
            Directory.CreateDirectory(unitTestImgPath);

            // Create a sample image file for testing
            var _imagePath = Path.Combine(unitTestImgPath, "00000000-0000-0000-0000-000000000000.jpg");
            File.WriteAllText(_imagePath, "This is a test image file.");
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                Name = "Test Dataset",
                Description = "This is a test dataset.",
                IsPublished = true,
                CreatedById = "user-id",
                AnnotationsPerSubclass = false,
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage { Id = Guid.NewGuid(), FileName = "00000000-0000-0000-0000-000000000000.jpg", ImagePath = unitTestImgPath } // Adjusted image path
        },
                DatasetClasses = new List<Dataset_DatasetClass>
        {
            new Dataset_DatasetClass
            {
                DatasetId = datasetId,
                DatasetClass = new DatasetClass
                {
                    Id = Guid.NewGuid(),
                    ClassName = "Class1"
                },
                DatasetClassValue = 1 // Set a valid class value
            }
        }
            };

            string exportOption = "AllImages";

            // Mock the mapping from Dataset to DatasetDTO
            var datasetDTO = new DatasetDTO
            {
                Id = datasetId,
                Name = dataset.Name,
                Description = dataset.Description,
                IsPublished = dataset.IsPublished,
                // Add other necessary properties
            };

            _mockMapper
                .Setup(m => m.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(datasetDTO); // Mock the return value for mapping

            _mockMapper
                .Setup(m => m.Map<List<DatasetImageDTO>>(It.IsAny<List<DatasetImage>>()))
                .Returns(new List<DatasetImageDTO>
                {
            new DatasetImageDTO { IdInt = 0, FileName = "00000000-0000-0000-0000-000000000000.jpg", ImagePath = unitTestImgPath } // Adjusted image path
                });

            _mockMapper
                .Setup(m => m.Map<List<ImageAnnotationDTO>>(It.IsAny<List<ImageAnnotation>>()))
                .Returns(new List<ImageAnnotationDTO> { /* Add expected ImageAnnotationDTOs if necessary */ });

            // Create a temporary download location
            string tempDownloadPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDownloadPath); // Ensure the temp directory exists
            string downloadLocation = Path.Combine(tempDownloadPath, "dataset.zip"); // Define a zip file path

            var datasetFullIncludeDTO = new DatasetFullIncludeDTO(
                dataset: datasetDTO,
                datasetClassForDataset: new List<DatasetClassForDatasetDTO>
                {
            new DatasetClassForDatasetDTO
            {
                ClassName = "Class1",
                ClassValue = 1,
                DatasetClassId = Guid.NewGuid(),
                DatasetDatasetClassId = Guid.NewGuid()
            }
                },
                datasetImages: new List<DatasetImageDTO>
                {
            new DatasetImageDTO { IdInt = 0, FileName = "00000000-0000-0000-0000-000000000000.jpg", ImagePath = unitTestImgPath }
                },
                imageAnnotations: new List<ImageAnnotationDTO>
                {
                    // Add your image annotations here, if any
                }
            );

            _mockMapper
                .Setup(m => m.Map<DatasetFullIncludeDTO>(It.IsAny<Dataset>()))
                .Returns(datasetFullIncludeDTO);

            // Act
            var result = await _datasetService.ConvertDatasetEntityToCocoDatasetWithAssignedIdIntsAsSplitDataset(dataset, exportOption, downloadLocation);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.EndsWith(".zip")); // Check if it's a zip file path
            Assert.True(File.Exists(result.Data)); // Check if the file was created

            // Cleanup the created file if necessary
            File.Delete(result.Data);
            // Delete directories recursively
            Directory.Delete(_testDirectory, true);
            Directory.Delete(unitTestImgPath, true);
            Directory.Delete(tempDownloadPath, true);
        }

        [Fact]
        public async Task ConvertDatasetEntityToCocoDatasetWithAssignedIdInts_ShouldReturnZipFilePath_WhenSuccessful()
        {
            // Create a temporary test directory
            var _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            // Prepare the image directory and file
            var unitTestImgPath = Path.Combine(Path.GetFullPath("wwwroot"), "unit-test-img");
            Directory.CreateDirectory(unitTestImgPath);

            // Create a sample image file for testing
            var _imagePath = Path.Combine(unitTestImgPath, "00000000-0000-0000-0000-000000000000.jpg");
            File.WriteAllText(_imagePath, "This is a test image file.");
            // Arrange
            var datasetId = Guid.NewGuid();
            var dataset = new Dataset
            {
                Id = datasetId,
                Name = "Test Dataset",
                Description = "This is a test dataset.",
                IsPublished = true,
                CreatedById = "user-id",
                AnnotationsPerSubclass = false,
                DatasetImages = new List<DatasetImage>
        {
            new DatasetImage { Id = Guid.NewGuid(), FileName = "00000000-0000-0000-0000-000000000000.jpg", ImagePath = unitTestImgPath } // Adjusted image path
        },
                DatasetClasses = new List<Dataset_DatasetClass>
        {
            new Dataset_DatasetClass
            {
                DatasetId = datasetId,
                DatasetClass = new DatasetClass
                {
                    Id = Guid.NewGuid(),
                    ClassName = "Class1"
                },
                DatasetClassValue = 1 // Set a valid class value
            }
        }
            };

            string exportOption = "AllImages";

            // Mock the mapping from Dataset to DatasetDTO
            var datasetDTO = new DatasetDTO
            {
                Id = datasetId,
                Name = dataset.Name,
                Description = dataset.Description,
                IsPublished = dataset.IsPublished,
                // Add other necessary properties
            };

            _mockMapper
                .Setup(m => m.Map<DatasetDTO>(It.IsAny<Dataset>()))
                .Returns(datasetDTO); // Mock the return value for mapping

            _mockMapper
                .Setup(m => m.Map<List<DatasetImageDTO>>(It.IsAny<List<DatasetImage>>()))
                .Returns(new List<DatasetImageDTO>
                {
            new DatasetImageDTO { IdInt = 0, FileName = "00000000-0000-0000-0000-000000000000.jpg", ImagePath = unitTestImgPath } // Adjusted image path
                });

            _mockMapper
                .Setup(m => m.Map<List<ImageAnnotationDTO>>(It.IsAny<List<ImageAnnotation>>()))
                .Returns(new List<ImageAnnotationDTO> { /* Add expected ImageAnnotationDTOs if necessary */ });

            // Create a temporary download location
            string tempDownloadPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDownloadPath); // Ensure the temp directory exists
            string downloadLocation = Path.Combine(tempDownloadPath, "dataset.zip"); // Define a zip file path

            var datasetFullIncludeDTO = new DatasetFullIncludeDTO(
                dataset: datasetDTO,
                datasetClassForDataset: new List<DatasetClassForDatasetDTO>
                {
            new DatasetClassForDatasetDTO
            {
                ClassName = "Class1",
                ClassValue = 1,
                DatasetClassId = Guid.NewGuid(),
                DatasetDatasetClassId = Guid.NewGuid()
            }
                },
                datasetImages: new List<DatasetImageDTO>
                {
            new DatasetImageDTO { IdInt = 0, FileName = "00000000-0000-0000-0000-000000000000.jpg", ImagePath = unitTestImgPath }
                },
                imageAnnotations: new List<ImageAnnotationDTO>
                {
                    // Add your image annotations here, if any
                }
            );

            _mockMapper
                .Setup(m => m.Map<DatasetFullIncludeDTO>(It.IsAny<Dataset>()))
                .Returns(datasetFullIncludeDTO);

            // Act
            var result = await _datasetService.ConvertDatasetEntityToCocoDatasetWithAssignedIdInts(dataset, exportOption, downloadLocation);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            //Assert.True(result.Data.EndsWith(".zip"));
            //Assert.True(File.Exists(result.Data));

        }



    }


}
