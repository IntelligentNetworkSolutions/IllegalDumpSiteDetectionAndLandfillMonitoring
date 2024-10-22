using AutoMapper;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Services.LegalLandfillManagementServices;
using Microsoft.Extensions.Logging;
using Moq;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Services
{
    public class LegalLandfillWasteTypeServiceTests
    {
        private readonly Mock<ILegalLandfillWasteTypeRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<LegalLandfillWasteTypeService>> _mockLogger;
        private readonly LegalLandfillWasteTypeService _service;

        public LegalLandfillWasteTypeServiceTests()
        {
            _mockRepository = new Mock<ILegalLandfillWasteTypeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<LegalLandfillWasteTypeService>>();
            _service = new LegalLandfillWasteTypeService(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateLegalLandfillWasteType_ShouldCreateWasteType()
        {
            // Arrange
            var dto = new LegalLandfillWasteTypeDTO();
            var entity = new LegalLandfillWasteType();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteType>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Create(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.CreateLegalLandfillWasteType(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateLegalLandfillWasteType_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillWasteTypeDTO();
            var entity = new LegalLandfillWasteType();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteType>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Create(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Creation failed"));

            // Act
            var result = await _service.CreateLegalLandfillWasteType(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation failed", result.ErrMsg);
        }

        

        [Fact]
        public async Task EditLegalLandfillWasteType_ShouldUpdateWasteType()
        {
            // Arrange
            var dto = new LegalLandfillWasteTypeDTO();
            var entity = new LegalLandfillWasteType();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteType>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Update(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.EditLegalLandfillWasteType(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditLegalLandfillWasteType_WhenUpdateFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillWasteTypeDTO();
            var entity = new LegalLandfillWasteType();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteType>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Update(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _service.EditLegalLandfillWasteType(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteType_ShouldDeleteWasteType()
        {
            // Arrange
            var dto = new LegalLandfillWasteTypeDTO();
            var entity = new LegalLandfillWasteType();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteType>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteLegalLandfillWasteType(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteLegalLandfillWasteType_WhenDeleteFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillWasteTypeDTO();
            var entity = new LegalLandfillWasteType();
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteType>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Delete failed"));

            // Act
            var result = await _service.DeleteLegalLandfillWasteType(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllLegalLandfillWasteTypes_ShouldReturnAllWasteTypes()
        {
            // Arrange
            var entities = new List<LegalLandfillWasteType> { new LegalLandfillWasteType() };
            var dtos = new List<LegalLandfillWasteTypeDTO> { new LegalLandfillWasteTypeDTO() };
            _mockRepository.Setup(r => r.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillWasteType>>.Ok(entities));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillWasteTypeDTO>>(It.IsAny<IEnumerable<LegalLandfillWasteType>>()))
                .Returns(dtos);

            // Act
            var result = await _service.GetAllLegalLandfillWasteTypes();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dtos, result.Data);
        }

        [Fact]
        public async Task GetAllLegalLandfillWasteTypes_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillWasteType>>.Fail("Repository error"));

            // Act
            var result = await _service.GetAllLegalLandfillWasteTypes();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillWasteTypeById_ShouldReturnWasteType()
        {
            // Arrange
            var wasteTypeId = Guid.NewGuid();
            var entity = new LegalLandfillWasteType();
            var dto = new LegalLandfillWasteTypeDTO();
            _mockRepository.Setup(r => r.GetById(wasteTypeId, false, null))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteType?>.Ok(entity));
            _mockMapper.Setup(m => m.Map<LegalLandfillWasteTypeDTO>(entity))
                .Returns(dto);

            // Act
            var result = await _service.GetLegalLandfillWasteTypeById(wasteTypeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
        }

        [Fact]
        public async Task GetLegalLandfillWasteTypeById_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var wasteTypeId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetById(wasteTypeId, false, null))
                .ReturnsAsync(ResultDTO<LegalLandfillWasteType?>.Fail("Repository error"));

            // Act
            var result = await _service.GetLegalLandfillWasteTypeById(wasteTypeId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }
    }

}
