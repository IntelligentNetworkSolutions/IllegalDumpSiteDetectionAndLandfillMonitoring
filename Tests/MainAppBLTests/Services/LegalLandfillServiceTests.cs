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
    public class LegalLandfillServiceTests
    {
        private readonly Mock<ILegalLandfillRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<LegalLandfillService>> _mockLogger;
        private readonly LegalLandfillService _service;

        public LegalLandfillServiceTests()
        {
            _mockRepository = new Mock<ILegalLandfillRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<LegalLandfillService>>();
            _service = new LegalLandfillService(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetAllLegalLandfills_ShouldReturnAllLandfills()
        {
            // Arrange
            var entities = new List<LegalLandfill> { new LegalLandfill() };
            var dtos = new List<LegalLandfillDTO> { new LegalLandfillDTO() };
            _mockRepository.Setup(r => r.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfill>>.Ok(entities));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillDTO>>(It.IsAny<IEnumerable<LegalLandfill>>()))
                .Returns(dtos);

            // Act
            var result = await _service.GetAllLegalLandfills();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dtos, result.Data);
        }

        [Fact]
        public async Task GetAllLegalLandfills_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfill>>.Fail("Repository error"));

            // Act
            var result = await _service.GetAllLegalLandfills();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillById_ShouldReturnLandfill()
        {
            // Arrange
            var landfillId = Guid.NewGuid();
            var entity = new LegalLandfill();
            var dto = new LegalLandfillDTO();
            _mockRepository.Setup(r => r.GetById(landfillId, false, null))
                .ReturnsAsync(ResultDTO<LegalLandfill?>.Ok(entity));
            _mockMapper.Setup(m => m.Map<LegalLandfillDTO>(entity))
                .Returns(dto);

            // Act
            var result = await _service.GetLegalLandfillById(landfillId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
        }
                
        [Fact]
        public async Task CreateLegalLandfill_ShouldCreateLandfill()
        {
            // Arrange
            var dto = new LegalLandfillDTO();
            var entity = new LegalLandfill();
            _mockMapper.Setup(m => m.Map<LegalLandfill>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Create(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.CreateLegalLandfill(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateLegalLandfill_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillDTO();
            var entity = new LegalLandfill();
            _mockMapper.Setup(m => m.Map<LegalLandfill>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Create(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Creation failed"));

            // Act
            var result = await _service.CreateLegalLandfill(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation failed", result.ErrMsg);
        }

        [Fact]
        public async Task EditLegalLandfill_ShouldUpdateLandfill()
        {
            // Arrange
            var dto = new LegalLandfillDTO();
            var entity = new LegalLandfill();
            _mockMapper.Setup(m => m.Map<LegalLandfill>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Update(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.EditLegalLandfill(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditLegalLandfill_WhenUpdateFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillDTO();
            var entity = new LegalLandfill();
            _mockMapper.Setup(m => m.Map<LegalLandfill>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Update(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Update failed"));

            // Act
            var result = await _service.EditLegalLandfill(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfill_ShouldDeleteLandfill()
        {
            // Arrange
            var dto = new LegalLandfillDTO();
            var entity = new LegalLandfill();
            _mockMapper.Setup(m => m.Map<LegalLandfill>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteLegalLandfill(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteLegalLandfill_WhenDeleteFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillDTO();
            var entity = new LegalLandfill();
            _mockMapper.Setup(m => m.Map<LegalLandfill>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Fail("Delete failed"));

            // Act
            var result = await _service.DeleteLegalLandfill(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.ErrMsg);
        }

    }
}
