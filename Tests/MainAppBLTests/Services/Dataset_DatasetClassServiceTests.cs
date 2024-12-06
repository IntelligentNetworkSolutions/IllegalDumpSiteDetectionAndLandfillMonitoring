using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Services.DatasetServices;
using Moq;
using SD;
using System.Linq.Expressions;

namespace Tests.MainAppBLTests.Services
{
    public class Dataset_DatasetClassServiceTests
    {
        private readonly Mock<IDataset_DatasetClassRepository> _datasetDatasetClassRepositoryMock;
        private readonly IMapper _mapper;
        private readonly Dataset_DatasetClassService _service;

        public Dataset_DatasetClassServiceTests()
        {
            _datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dataset_DatasetClass, Dataset_DatasetClassDTO>();
            });

            _mapper = config.CreateMapper();
            _service = new Dataset_DatasetClassService(_datasetDatasetClassRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task GetDataset_DatasetClassByClassId_ReturnsMappedList()
        {
            var classId = Guid.NewGuid();
            var datasetClasses = new List<Dataset_DatasetClass>
            {
                new Dataset_DatasetClass { DatasetId = Guid.NewGuid(), DatasetClassId = classId, DatasetClassValue = 1 },
                new Dataset_DatasetClass { DatasetId = Guid.NewGuid(), DatasetClassId = classId, DatasetClassValue = 2 }
            };

            var resultDto = ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetClasses);

            _datasetDatasetClassRepositoryMock
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, null, null))
                .ReturnsAsync(resultDto);

            var result = await _service.GetDataset_DatasetClassByClassId(classId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count);
            Assert.All(result.Data, item => Assert.Equal(classId, item.DatasetClassId));
        }

        [Fact]
        public async Task GetDataset_DatasetClassByClassId_ObjectNotFound_ReturnsFailResult()
        {
            // Arrange
            var classId = Guid.NewGuid();

            var resultDto = ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(null);
            _datasetDatasetClassRepositoryMock
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, null, null))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _service.GetDataset_DatasetClassByClassId(classId);

            // Assert
            Assert.False(result.IsSuccess, "Expected result to indicate failure.");
            Assert.Equal("DatasetClass is null", result.ErrMsg);
            Assert.Null(result.Data);
        }


        [Fact]
        public async Task GetDataset_DatasetClassByClassId_RepositoryThrowsException_ThrowsException()
        {
            var classId = Guid.NewGuid();
            var expectedExceptionMessage = "Repository exception";

            _datasetDatasetClassRepositoryMock
                .Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, null, null))
                .ThrowsAsync(new Exception(expectedExceptionMessage));

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetDataset_DatasetClassByClassId(classId));
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }


    }
}
