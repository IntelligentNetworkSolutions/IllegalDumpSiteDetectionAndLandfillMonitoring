using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Services.LegalLandfillManagementServices;
using Microsoft.Extensions.Logging;
using Moq;
using NetTopologySuite.Operation.Distance;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Services
{
    public class LegalLandfillPointCloudFileServiceTests
    {
        private readonly Mock<ILegalLandfillPointCloudFileRepository> _mockRepository;
        private readonly Mock<IAppSettingsAccessor> _mockAppSettingsAccessor;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<LegalLandfillPointCloudFileService>> _mockLogger;
        private readonly LegalLandfillPointCloudFileService _service;

        public LegalLandfillPointCloudFileServiceTests()
        {
            _mockRepository = new Mock<ILegalLandfillPointCloudFileRepository>();
            _mockAppSettingsAccessor = new Mock<IAppSettingsAccessor>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<LegalLandfillPointCloudFileService>>();
            _service = new LegalLandfillPointCloudFileService(
                _mockRepository.Object,
                _mockAppSettingsAccessor.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetAllLegalLandfillPointCloudFiles_ShouldReturnAllFiles()
        {
            // Arrange
            var entities = new List<LegalLandfillPointCloudFile> { new LegalLandfillPointCloudFile() };
            var dtos = new List<LegalLandfillPointCloudFileDTO> { new LegalLandfillPointCloudFileDTO() };
            _mockRepository.Setup(r => r.GetAll(null, null, false, "LegalLandfill", null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillPointCloudFile>>.Ok(entities));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillPointCloudFileDTO>>(It.IsAny<IEnumerable<LegalLandfillPointCloudFile>>()))
                .Returns(dtos);

            // Act
            var result = await _service.GetAllLegalLandfillPointCloudFiles();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dtos, result.Data);
        }

        [Fact]
        public async Task GetLegalLandfillPointCloudFilesByLandfillId_ShouldReturnFilesForLandfill()
        {
            // Arrange
            var landfillId = Guid.NewGuid();
            var entities = new List<LegalLandfillPointCloudFile> { new LegalLandfillPointCloudFile() };
            var dtos = new List<LegalLandfillPointCloudFileDTO> { new LegalLandfillPointCloudFileDTO() };
            _mockRepository.Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<LegalLandfillPointCloudFile, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillPointCloudFile>>.Ok(entities));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillPointCloudFileDTO>>(It.IsAny<IEnumerable<LegalLandfillPointCloudFile>>()))
                .Returns(dtos);

            // Act
            var result = await _service.GetLegalLandfillPointCloudFilesByLandfillId(landfillId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dtos, result.Data);
        }

        [Fact]
        public async Task GetLegalLandfillPointCloudFilesById_ShouldReturnFileById()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var entity = new LegalLandfillPointCloudFile();
            var dto = new LegalLandfillPointCloudFileDTO();
            _mockRepository.Setup(r => r.GetById(fileId, false, "LegalLandfill"))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFile?>.Ok(entity));
            _mockMapper.Setup(m => m.Map<LegalLandfillPointCloudFileDTO>(entity))
                .Returns(dto);

            // Act
            var result = await _service.GetLegalLandfillPointCloudFilesById(fileId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
        }

        [Fact]
        public async Task CreateLegalLandfillPointCloudFile_ShouldCreateAndReturnFile()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO();
            var entity = new LegalLandfillPointCloudFile();
            _mockMapper.Setup(m => m.Map<LegalLandfillPointCloudFile>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.CreateAndReturnEntity(entity, true, default))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFile>.Ok(entity));
            _mockMapper.Setup(m => m.Map<LegalLandfillPointCloudFileDTO>(entity)).Returns(dto);

            // Act
            var result = await _service.CreateLegalLandfillPointCloudFile(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFile_ShouldDeleteFile()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var entity = new LegalLandfillPointCloudFile();
            _mockRepository.Setup(r => r.GetFirstOrDefault(It.IsAny<System.Linq.Expressions.Expression<Func<LegalLandfillPointCloudFile, bool>>>(), false, null))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFile?>.Ok(entity));
            _mockRepository.Setup(r => r.Delete(entity, true, default))
                .ReturnsAsync(ResultDTO.Ok());

            // Act
            var result = await _service.DeleteLegalLandfillPointCloudFile(fileId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllLegalLandfillPointCloudFiles_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll(null, null, false, "LegalLandfill", null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillPointCloudFile>>.Fail("Repository error"));

            // Act
            var result = await _service.GetAllLegalLandfillPointCloudFiles();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Repository error", result.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillPointCloudFilesByLandfillId_WhenNoFilesFound_ShouldReturnEmptyList()
        {
            // Arrange
            var landfillId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<LegalLandfillPointCloudFile, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillPointCloudFile>>.Ok(new List<LegalLandfillPointCloudFile>()));
            _mockMapper.Setup(m => m.Map<List<LegalLandfillPointCloudFileDTO>>(It.IsAny<IEnumerable<LegalLandfillPointCloudFile>>()))
                .Returns(new List<LegalLandfillPointCloudFileDTO>());

            // Act
            var result = await _service.GetLegalLandfillPointCloudFilesByLandfillId(landfillId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
        }
               
        [Fact]
        public async Task CreateLegalLandfillPointCloudFile_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO();
            var entity = new LegalLandfillPointCloudFile();
            _mockMapper.Setup(m => m.Map<LegalLandfillPointCloudFile>(dto)).Returns(entity);
            _mockRepository.Setup(r => r.CreateAndReturnEntity(entity, true, default))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFile>.Fail("Creation failed"));

            // Act
            var result = await _service.CreateLegalLandfillPointCloudFile(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation failed", result.ErrMsg);
        }
             
        [Fact]
        public async Task EditLegalLandfillPointCloudFile_WhenFileNotFound_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), LegalLandfillId = Guid.NewGuid() };
            _mockRepository.Setup(r => r.GetById(dto.Id, true, null))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFile?>.Ok(null));

            // Act
            var result = await _service.EditLegalLandfillPointCloudFile(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("File not exists in database!", result.ErrMsg);
        }
               
        [Fact]
        public async Task EditLegalLandfillPointCloudFile_WhenUpdateFails_ShouldReturnFailResult()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), LegalLandfillId = Guid.NewGuid() };
            var entity = new LegalLandfillPointCloudFile { Id = dto.Id, LegalLandfillId = dto.LegalLandfillId };

            _mockRepository.Setup(r => r.GetById(dto.Id, true, null))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFile?>.Ok(entity));
            _mockMapper.Setup(m => m.Map(dto, entity)).Returns(entity);
            _mockRepository.Setup(r => r.UpdateAndReturnEntity(It.IsAny<LegalLandfillPointCloudFile>(), true, default))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFile>.Fail("Update failed"));

            // Act
            var result = await _service.EditLegalLandfillPointCloudFile(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed", result.ErrMsg);
        }


    }
}
