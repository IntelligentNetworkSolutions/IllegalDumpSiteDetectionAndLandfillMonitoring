using AutoMapper;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Services.LegalLandfillManagementServices;
using Microsoft.Extensions.Logging;
using Moq;
using SD;
using System.Linq.Expressions;

namespace Tests.MainAppBLTests.Services
{
    public class LegalLandfillTruckServiceTests
    {
        private readonly Mock<ILegalLandfillTruckRepository> _mockTruckRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<LegalLandfillTruckService>> _mockLogger;
        private readonly Mock<ILegalLandfillWasteImportRepository> _mockWasteImportRepository;
        private readonly LegalLandfillTruckService _service;

        public LegalLandfillTruckServiceTests()
        {
            _mockTruckRepository = new Mock<ILegalLandfillTruckRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<LegalLandfillTruckService>>();
            _mockWasteImportRepository = new Mock<ILegalLandfillWasteImportRepository>();

            _service = new LegalLandfillTruckService(
                _mockTruckRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockWasteImportRepository.Object
            );
        }

        [Fact]
        public async Task CreateLegalLandfillTruck_ShouldCreateTruck()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO();
            var entity = new LegalLandfillTruck();
            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto)).Returns(entity);
            _mockTruckRepository.Setup(r => r.Create(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.CreateLegalLandfillTruck(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateLegalLandfillTruck_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO();
            var entity = new LegalLandfillTruck();
            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto)).Returns(entity);
            _mockTruckRepository.Setup(r => r.Create(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Creation failed"));

            // Act
            var result = await _service.CreateLegalLandfillTruck(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation failed", result.ErrMsg);
        }

        [Fact]
        public async Task CreateLegalLandfillTruck_WhenMappingFails_ShouldReturnExceptionFail()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO();
            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto))
                .Throws(new AutoMapperMappingException("Mapping failed"));

            // Act
            var result = await _service.CreateLegalLandfillTruck(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task CreateLegalLandfillTruck_WhenRepositoryThrowsException_ShouldReturnExceptionFail()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO();
            var entity = new LegalLandfillTruck();
            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto)).Returns(entity);
            _mockTruckRepository.Setup(r => r.Create(entity, true, default))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _service.CreateLegalLandfillTruck(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database connection failed", result.ErrMsg);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task CreateLegalLandfillTruck_WithNullDTO_ShouldReturnExceptionFail()
        {
            // Arrange
            LegalLandfillTruckDTO? dto = null;

            // Act
            var result = await _service.CreateLegalLandfillTruck(dto!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Object reference not set to an instance", result.ErrMsg);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task EditLegalLandfillTruck_ShouldUpdateTruck()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO();
            var entity = new LegalLandfillTruck();
            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto)).Returns(entity);
            _mockTruckRepository.Setup(r => r.Update(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.EditLegalLandfillTruck(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditLegalLandfillTruck_WhenUpdateFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO();
            var entity = new LegalLandfillTruck();
            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto)).Returns(entity);
            _mockTruckRepository.Setup(r => r.Update(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _service.EditLegalLandfillTruck(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillTruck_WhenMappingFails_ShouldReturnExceptionFail()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO
            {
                Id = Guid.NewGuid()
            };
            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto))
                .Throws(new AutoMapperMappingException("Mapping failed"));

            // Act
            var result = await _service.EditLegalLandfillTruck(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.ErrMsg);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task EditLegalLandfillTruck_WhenRepositoryThrowsException_ShouldReturnExceptionFail()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO
            {
                Id = Guid.NewGuid()
            };
            var entity = new LegalLandfillTruck();
            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto)).Returns(entity);
            _mockTruckRepository.Setup(r => r.Update(entity, true, default))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _service.EditLegalLandfillTruck(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database connection failed", result.ErrMsg);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task EditLegalLandfillTruck_WithNullDTO_ShouldReturnExceptionFail()
        {
            // Arrange
            LegalLandfillTruckDTO? dto = null;

            // Act
            var result = await _service.EditLegalLandfillTruck(dto!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Object reference not set to an instance", result.ErrMsg);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }



        [Fact]
        public async Task DeleteLegalLandfillTruck_ShouldDeleteTruck()
        {
            // Arrange
            var truckId = Guid.NewGuid();
            var dto = new LegalLandfillTruckDTO();
            var entity = new LegalLandfillTruck();

            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto)).Returns(entity);
            _mockTruckRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());
            _mockWasteImportRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<LegalLandfillWasteImport, bool>>>(),
                It.IsAny<Func<IQueryable<LegalLandfillWasteImport>, IOrderedQueryable<LegalLandfillWasteImport>>>(),
                false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillWasteImport>>.Ok(Enumerable.Empty<LegalLandfillWasteImport>()));


            // Act
            var result = await _service.DeleteLegalLandfillTruck(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteLegalLandfillTruck_WhenDeleteFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO();
            var entity = new LegalLandfillTruck();

            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto)).Returns(entity);

            _mockTruckRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Delete failed"));

            _mockWasteImportRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<LegalLandfillWasteImport, bool>>>(),
                It.IsAny<Func<IQueryable<LegalLandfillWasteImport>, IOrderedQueryable<LegalLandfillWasteImport>>>(),
                false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillWasteImport>>.Ok(Enumerable.Empty<LegalLandfillWasteImport>()));

            // Act
            var result = await _service.DeleteLegalLandfillTruck(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillTruck_WhenTruckIsUsedInWasteImports_ShouldReturnFailResult()
        {
            // Arrange
            var truckId = Guid.NewGuid();
            var dto = new LegalLandfillTruckDTO { Id = truckId };
            var entity = new LegalLandfillTruck { Id = truckId };

            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(dto)).Returns(entity);
            _mockWasteImportRepository.Setup(r => r.GetAll(
                It.IsAny<Expression<Func<LegalLandfillWasteImport, bool>>>(),
                It.IsAny<Func<IQueryable<LegalLandfillWasteImport>, IOrderedQueryable<LegalLandfillWasteImport>>>(),
                false,
                null,
                null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillWasteImport>>.Ok(new List<LegalLandfillWasteImport> { new LegalLandfillWasteImport() }));

            // Act
            var result = await _service.DeleteLegalLandfillTruck(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Cannot delete this truck. It is being used in waste imports", result.ErrMsg);

        }

        [Fact]
        public async Task GetAllLegalLandfillTrucks_ShouldReturnAllTrucks()
        {
            // Arrange
            var entities = new List<LegalLandfillTruck> { new LegalLandfillTruck() };
            var dtos = new List<LegalLandfillTruckDTO> { new LegalLandfillTruckDTO() };

            _mockTruckRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<LegalLandfillTruck, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillTruck>>.Fail("Repository error"));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillTruckDTO>>(entities))
                .Returns(dtos);

            // Act
            var result = await _service.GetAllLegalLandfillTrucks();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllLegalLandfillTrucks_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            _mockTruckRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<LegalLandfillTruck, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillTruck>>.Fail("Repository error"));

            // Act
            var result = await _service.GetAllLegalLandfillTrucks();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillTruckById_ShouldReturnTruck()
        {
            // Arrange
            var truckId = Guid.NewGuid();
            var entity = new LegalLandfillTruck();
            var dto = new LegalLandfillTruckDTO();
            _mockTruckRepository.Setup(r => r.GetById(truckId, false, null))
                .ReturnsAsync(ResultDTO<LegalLandfillTruck?>.Ok(entity));
            _mockMapper.Setup(m => m.Map<LegalLandfillTruckDTO>(entity))
                .Returns(dto);

            // Act
            var result = await _service.GetLegalLandfillTruckById(truckId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
        }

        [Fact]
        public async Task GetLegalLandfillTruckById_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var truckId = Guid.NewGuid();
            _mockTruckRepository.Setup(r => r.GetById(truckId, false, null))
                .ReturnsAsync(ResultDTO<LegalLandfillTruck?>.Fail("Repository error"));

            // Act
            var result = await _service.GetLegalLandfillTruckById(truckId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task DisableLegalLandfillTruck_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var legalLandfillTruckDTO = new LegalLandfillTruckDTO();
            var legalLandfillTruckEntity = new LegalLandfillTruck();
            var resultDTO = ResultDTO.Ok();

            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(legalLandfillTruckDTO)).Returns(legalLandfillTruckEntity);
            _mockTruckRepository.Setup(r => r.Update(legalLandfillTruckEntity, true, default)).ReturnsAsync(resultDTO);

            // Act
            var result = await _service.DisableLegalLandfillTruck(legalLandfillTruckDTO);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DisableLegalLandfillTruck_ShouldReturnFail_WhenUpdateFails()
        {
            // Arrange
            var legalLandfillTruckDTO = new LegalLandfillTruckDTO();
            var legalLandfillTruckEntity = new LegalLandfillTruck();
            var resultDTO = ResultDTO.Fail("Update failed");

            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(legalLandfillTruckDTO)).Returns(legalLandfillTruckEntity);
            _mockTruckRepository.Setup(r => r.Update(legalLandfillTruckEntity, true, default)).ReturnsAsync(resultDTO);

            // Act
            var result = await _service.DisableLegalLandfillTruck(legalLandfillTruckDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed", result.ErrMsg);
        }

        [Fact]
        public async Task DisableLegalLandfillTruck_ShouldReturnExceptionFail_WhenExceptionOccurs()
        {
            // Arrange
            var legalLandfillTruckDTO = new LegalLandfillTruckDTO();

            _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(legalLandfillTruckDTO)).Throws(new Exception("Mapping error"));

            // Act
            var result = await _service.DisableLegalLandfillTruck(legalLandfillTruckDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Mapping error", result.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillTruckById_ShouldReturnExceptionFail_WhenExceptionThrown()
        {
            // Arrange
            var truckId = Guid.NewGuid();
            _mockTruckRepository
                .Setup(r => r.GetById(truckId, false, null))
                .ThrowsAsync(new Exception("Get by ID failed"));

            // Act
            var result = await _service.GetLegalLandfillTruckById(truckId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Get by ID failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllLegalLandfillTrucks_ShouldReturnExceptionFail_WhenExceptionThrown()
        {
            // Arrange
            _mockTruckRepository
                .Setup(r => r.GetAll(It.IsAny<Expression<Func<LegalLandfillTruck, bool>>>(), null, false, null, null))
                .ThrowsAsync(new Exception("Get all trucks failed"));

            // Act
            var result = await _service.GetAllLegalLandfillTrucks();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Get all trucks failed", result.ErrMsg);
        }

        // Fix IsTruckInUse
        //[Fact]
        //public async Task DeleteLegalLandfillTruck_ShouldReturnExceptionFail_WhenExceptionThrown()
        //{
        //    // Arrange
        //    var truckDto = new LegalLandfillTruckDTO { Id = Guid.NewGuid() };
        //    var truckEntity = new LegalLandfillTruck { Id = truckDto.Id };
        //    var legalLandfillWasteImport = new LegalLandfillWasteImport { Id = Guid.NewGuid(), LegalLandfillTruckId = truckDto.Id };
        //    _mockMapper.Setup(m => m.Map<LegalLandfillTruck>(truckDto)).Returns(truckEntity);

        //    _mockTruckRepository
        //        .Setup(r => r.Delete(truckEntity, true, default))
        //        .ThrowsAsync(new Exception("Delete truck failed"));

        //    // Act
        //    var result = await _service.DeleteLegalLandfillTruck(truckDto);

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Equal("Delete truck failed", result.ErrMsg);
        //}


    }

}
