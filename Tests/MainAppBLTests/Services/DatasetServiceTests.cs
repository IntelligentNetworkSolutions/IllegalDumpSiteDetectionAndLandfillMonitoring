using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DAL.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities;
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
    public class DatasetServiceTests
    {
        private readonly Mock<IDatasetClassesRepository> _mockDatasetClassesRepository;
        private readonly Mock<IDatasetsRepository> _mockDatasetRepository;
        private readonly Mock<IDataset_DatasetClassRepository> _mockDatasetDatasetClassRepository;
        private readonly Mock<IDatasetImagesRepository> _mockDatasetImagesRepository;
        private readonly Mock<IImageAnnotationsRepository> _mockImageAnnotationsRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly DatasetService _datasetService;

        public DatasetServiceTests()
        {
            _mockDatasetClassesRepository = new Mock<IDatasetClassesRepository>();
            _mockDatasetRepository = new Mock<IDatasetsRepository>();
            _mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            _mockDatasetImagesRepository = new Mock<IDatasetImagesRepository>();
            _mockImageAnnotationsRepository = new Mock<IImageAnnotationsRepository>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockMapper = new Mock<IMapper>();
                        
            var mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            var mockDatasetImagesRepository = new Mock<IDatasetImagesRepository>();
            var mockImageAnnotationsRepository = new Mock<IImageAnnotationsRepository>();
            var mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();

            _datasetService = new DatasetService(
                _mockDatasetRepository.Object,
                _mockDatasetClassesRepository.Object,
                mockDatasetDatasetClassRepository.Object,
                mockDatasetImagesRepository.Object,
                mockImageAnnotationsRepository.Object,
                _mockAppSettingsAccessor.Object,
                _mockMapper.Object
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
                .Setup(repo => repo.GetFirstOrDefault(x => x.DatasetId == datasetId && x.DatasetClassId == selectedClassId, false,null))
                .ReturnsAsync(ResultDTO<Dataset_DatasetClass?>.Fail("Dataset not found"));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.DeleteDatasetClassForDataset(selectedClassId, datasetId, userId));

            // Assert
            Assert.Equal("Dataset not found", exception.Message);
        }

        [Fact]
        public async Task DeleteDatasetClassForDataset_ShouldThrowException_WhenDatasetClassNotFound()
        {
            // Arrange
            var selectedClassId = Guid.NewGuid();
            var datasetId = Guid.NewGuid();
            var userId = "testUser";

            _mockDatasetDatasetClassRepository
                .Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<Dataset_DatasetClass>.Fail("Dataset class not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _datasetService.DeleteDatasetClassForDataset(selectedClassId, datasetId, userId));
            Assert.Equal("Dataset not found", exception.Message);
        }

        


    }
}
