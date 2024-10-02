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
    public class LegalLandfillWasteImportServiceTests
    {
        private readonly Mock<ILegalLandfillWasteImportRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<LegalLandfillWasteImportService>> _mockLogger;
        private readonly LegalLandfillWasteImportService _service;

        public LegalLandfillWasteImportServiceTests()
        {
            _mockRepository = new Mock<ILegalLandfillWasteImportRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<LegalLandfillWasteImportService>>();
            _service = new LegalLandfillWasteImportService(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_ShouldCreateImport()
        {
            // Arrange
            var dto = new LegalLandfillWasteImportDTO();
            var entity = new LegalLandfillWasteImport();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImport>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Create(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.CreateLegalLandfillWasteImport(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteImport_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillWasteImportDTO();
            var entity = new LegalLandfillWasteImport();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImport>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Create(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Creation failed"));

            // Act
            var result = await _service.CreateLegalLandfillWasteImport(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllLegalLandfillWasteImports_ShouldReturnAllWasteImports()
        {
            // Arrange
            var entities = new List<LegalLandfillWasteImport> { new LegalLandfillWasteImport() };
            var dtos = new List<LegalLandfillWasteImportDTO> { new LegalLandfillWasteImportDTO() };
            _mockRepository.Setup(r => r.GetAll(null, null, false, "LegalLandfillWasteType,LegalLandfillTruck,LegalLandfill,CreatedBy", null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillWasteImport>>.Ok(entities));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillWasteImportDTO>>(entities))
                .Returns(dtos);

            // Act
            var result = await _service.GetAllLegalLandfillWasteImports();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dtos, result.Data);
        }

        [Fact]
        public async Task GetAllLegalLandfillWasteImports_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll(null, null, false, "LegalLandfillWasteType,LegalLandfillTruck,LegalLandfill,CreatedBy", null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillWasteImport>>.Fail("Repository error"));

            // Act
            var result = await _service.GetAllLegalLandfillWasteImports();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillWasteImportById_ShouldReturnWasteImport()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new LegalLandfillWasteImport();
            var dto = new LegalLandfillWasteImportDTO();

            Expression<Func<LegalLandfillWasteImport, object>>[] includeProperties =
                [
                    x => x.LegalLandfillWasteType,
                    x => x.LegalLandfillTruck,
                    x => x.LegalLandfill,
                    x => x.CreatedBy
                ];

            _mockRepository.Setup(r => r.GetByIdInclude(id, false, It.IsAny<Expression<Func<LegalLandfillWasteImport, object>>[]>()))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteImport?>.Ok(entity));
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImportDTO>(entity))
                .Returns(dto);

            // Act
            var result = await _service.GetLegalLandfillWasteImportById(id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
        }

        [Fact]
        public async Task GetLegalLandfillWasteImportById_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var resultDTO = ResultDTO<LegalLandfillWasteImport>.Fail("Repository failure");
            // Define include properties as expressions
            Expression<Func<LegalLandfillWasteImport, object>>[] includeProperties =
            [
                x => x.LegalLandfillWasteType,
                x => x.LegalLandfillTruck,
                x => x.LegalLandfill,
                x => x.CreatedBy
            ];

            _mockRepository.Setup(x => x.GetByIdInclude(id, false, It.IsAny<Expression<Func<LegalLandfillWasteImport, object>>[]>()))
                .ReturnsAsync(resultDTO);

            // Act
            var result = await _service.GetLegalLandfillWasteImportById(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository failure", result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_ShouldUpdateImport()
        {
            // Arrange
            var dto = new LegalLandfillWasteImportDTO();
            var entity = new LegalLandfillWasteImport();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImport>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Update(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.EditLegalLandfillWasteImport(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditLegalLandfillWasteImport_WhenUpdateFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillWasteImportDTO();
            var entity = new LegalLandfillWasteImport();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImport>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Update(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _service.EditLegalLandfillWasteImport(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteImport_ShouldDeleteImport()
        {
            // Arrange
            var dto = new LegalLandfillWasteImportDTO();
            var entity = new LegalLandfillWasteImport();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImport>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteLegalLandfillWasteImport(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteImport_WhenDeleteFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillWasteImportDTO();
            var entity = new LegalLandfillWasteImport();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteImport>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Delete failed"));

            // Act
            var result = await _service.DeleteLegalLandfillWasteImport(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.ErrMsg);
        }
    }

}
