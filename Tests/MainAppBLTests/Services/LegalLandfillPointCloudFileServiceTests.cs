using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Services.LegalLandfillManagementServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SD;
using System.Linq.Expressions;

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
            _mockRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(), null, false, null, null))
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
        public async Task GetLegalLandfillPointCloudFilesById_ReturnsFailureResult_WhenRepositoryFails()
        {
            // Arrange
            var legalLandfillPointCloudFileId = Guid.NewGuid();

            var resultGetEntity = ResultDTO<LegalLandfillPointCloudFile?>.Fail("Error retrieving data");

            _mockRepository.Setup(repo => repo.GetById(legalLandfillPointCloudFileId, It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(resultGetEntity);

            // Act
            var result = await _service.GetLegalLandfillPointCloudFilesById(legalLandfillPointCloudFileId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error retrieving data", result.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillPointCloudFilesByLandfillId_WhenRepositoryFails_ShouldReturnFailResult()
        {
            // Arrange
            var landfillId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(), null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<LegalLandfillPointCloudFile>>.Fail("Error retrieving data"));

            // Act
            var result = await _service.GetLegalLandfillPointCloudFilesByLandfillId(landfillId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error retrieving data", result.ErrMsg);
        }

        [Fact]
        public async Task GetLegalLandfillPointCloudFilesByLandfillId_WhenExceptionThrown_ShouldReturnExceptionFailResult()
        {
            // Arrange
            var landfillId = Guid.NewGuid();
            var exceptionMessage = "Unexpected error";
            _mockRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(), null, false, null, null))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.GetLegalLandfillPointCloudFilesByLandfillId(landfillId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
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
        public async Task DeleteLegalLandfillPointCloudFile_ReturnsFailureResult_WhenEntityNotFound()
        {
            // Arrange
            var legalLandfillPointCloudFileId = Guid.NewGuid();

            var resultGetEntity = ResultDTO<LegalLandfillPointCloudFile?>.Fail("Entity not found");

            _mockRepository.Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(), false, null))
                .ReturnsAsync(resultGetEntity);

            // Act
            var result = await _service.DeleteLegalLandfillPointCloudFile(legalLandfillPointCloudFileId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Entity not found", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFile_ReturnsFailureResult_WhenDataIsNull()
        {
            // Arrange
            var legalLandfillPointCloudFileId = Guid.NewGuid();

            var resultGetEntity = ResultDTO<LegalLandfillPointCloudFile?>.Ok(null);

            _mockRepository.Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(), false, null))
                .ReturnsAsync(resultGetEntity);

            // Act
            var result = await _service.DeleteLegalLandfillPointCloudFile(legalLandfillPointCloudFileId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Data is null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFile_ReturnsFailureResult_WhenDeleteFails()
        {
            // Arrange
            var legalLandfillPointCloudFileId = Guid.NewGuid();
            var legalLandfillPointCloudFile = new LegalLandfillPointCloudFile { Id = legalLandfillPointCloudFileId };

            var resultGetEntity = ResultDTO<LegalLandfillPointCloudFile?>.Ok(legalLandfillPointCloudFile);

            _mockRepository.Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(), false, null))
                .ReturnsAsync(resultGetEntity);

            var resultDeleteEntity = ResultDTO.Fail("Error deleting entity");
            _mockRepository.Setup(repo => repo.Delete(legalLandfillPointCloudFile, true, default))
                .ReturnsAsync(resultDeleteEntity);

            // Act
            var result = await _service.DeleteLegalLandfillPointCloudFile(legalLandfillPointCloudFileId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error deleting entity", result.ErrMsg);
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
        public async Task GetAllLegalLandfillPointCloudFiles_WhenExceptionThrown_ShouldReturnExceptionFailResult()
        {
            // Arrange
            var exceptionMessage = "Unexpected error";
            _mockRepository.Setup(r => r.GetAll(null, null, false, "LegalLandfill", null))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.GetAllLegalLandfillPointCloudFiles();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
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
        public async Task CreateLegalLandfillPointCloudFile_WhenExceptionThrown_ShouldReturnExceptionFailResult()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO();
            var entity = new LegalLandfillPointCloudFile();
            _mockMapper.Setup(m => m.Map<LegalLandfillPointCloudFile>(dto)).Returns(entity);

            // Set up the repository to throw an exception
            _mockRepository.Setup(r => r.CreateAndReturnEntity(entity, true, default))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.CreateLegalLandfillPointCloudFile(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.ErrMsg);
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

        [Fact]
        public async Task EditLegalLandfillPointCloudFile_WhenLegalLandfillIdChanges_ShouldUpdateFilePath()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                LegalLandfillId = Guid.NewGuid()
            };

            var existingEntity = new LegalLandfillPointCloudFile
            {
                Id = dto.Id,
                LegalLandfillId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetById(dto.Id, true, null))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFile?>.Ok(existingEntity));

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileUploads", "Uploads\\LegalLandfillUploads\\PointCloudUploads"))
                .ReturnsAsync(ResultDTO<string>.Ok("C:\\Uploads"));

            _mockMapper.Setup(m => m.Map(dto, existingEntity)).Returns(existingEntity);

            var updatedEntity = new LegalLandfillPointCloudFile { Id = dto.Id, LegalLandfillId = dto.LegalLandfillId };
            _mockRepository.Setup(r => r.UpdateAndReturnEntity(existingEntity, true, default))
                .ReturnsAsync(ResultDTO<LegalLandfillPointCloudFile>.Ok(updatedEntity));

            // Act
            var result = await _service.EditLegalLandfillPointCloudFile(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("C:\\Uploads\\" + dto.LegalLandfillId.ToString() + "\\", dto.FilePath);
        }

        [Fact]
        public async Task ReadAndDeleteDiffWasteVolumeComparisonFile_FileDoesNotExist_ReturnsFail()
        {
            // Arrange
            string filePath = "non_existing_file.tif";

            // Act
            var result = await _service.ReadAndDeleteDiffWasteVolumeComparisonFile(filePath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ReadAndDeleteDiffWasteVolumeComparisonFile_FileExistsButCannotOpen_ReturnsFail()
        {
            // Arrange
            string filePath = "test_file.tif";
            File.Create(filePath).Dispose();

            // Act
            var result = await _service.ReadAndDeleteDiffWasteVolumeComparisonFile(filePath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);

            // Clean up
            File.Delete(filePath);
        }

        [Fact]
        public async Task UploadFile_ValidFile_ReturnsOk()
        {
            // Arrange
            string uploadFolder = "test_uploads";
            string fileName = "testfile.txt";
            Directory.CreateDirectory(uploadFolder);

            var fileMock = new Mock<IFormFile>();
            var content = "Hello World!";
            var fileNameOnDisk = fileName;
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(content);
            writer.Flush();
            memoryStream.Position = 0;

            fileMock.Setup(f => f.FileName).Returns(fileNameOnDisk);
            fileMock.Setup(f => f.Length).Returns(memoryStream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream stream, CancellationToken _) => memoryStream.CopyToAsync(stream));

            // Act
            var result = await _service.UploadFile(fileMock.Object, uploadFolder, fileName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Path.Combine(uploadFolder, fileName), result.Data);
            Assert.True(File.Exists(result.Data));

            // Clean up
            File.Delete(result.Data);
            Directory.Delete(uploadFolder);
        }

        [Fact]
        public async Task UploadFile_InvalidPath_ReturnsExceptionFail()
        {
            // Arrange
            string invalidUploadFolder = "<>:\"/\\|?*";
            string fileName = "testfile.txt";

            var fileMock = new Mock<IFormFile>();
            var content = "Hello World!";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(content);
            writer.Flush();
            memoryStream.Position = 0;

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(memoryStream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream stream, CancellationToken _) => memoryStream.CopyToAsync(stream));

            // Act
            var result = await _service.UploadFile(fileMock.Object, invalidUploadFolder, fileName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task EditFileInUploads_AppSettingsFail_ReturnsFail()
        {
            // Arrange
            string webRootPath = "mockWebRootPath";
            string filePath = "mockFilePath";
            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), FileName = "mockFile.txt", LegalLandfillId = Guid.NewGuid() };

            var appSettingResult = ResultDTO<string>.Fail("Failed to get setting");
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appSettingResult);

            // Act
            var result = await _service.EditFileInUploads(webRootPath, filePath, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to get setting", result.ErrMsg);
        }

        [Fact]
        public async Task EditFileInUploads_AppSettingsNull_ReturnsFail()
        {
            // Arrange
            string webRootPath = "mockWebRootPath";
            string filePath = "mockFilePath";
            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), FileName = "mockFile.txt", LegalLandfillId = Guid.NewGuid() };

            var appSettingResult = ResultDTO<string>.Ok(null);
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appSettingResult);

            // Act
            var result = await _service.EditFileInUploads(webRootPath, filePath, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Point cloud upload folder path is null", result.ErrMsg);
        }

        [Fact]
        public async Task EditFileInUploads_ReturnsFail_WhenUploadFolderPathIsNull()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { LegalLandfillId = Guid.NewGuid(), Id = Guid.NewGuid(), FileName = "file.txt" };
            string webRootPath = "path/to/webroot";
            string filePath = "uploads";

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileUploads", "Uploads\\LegalLandfillUploads\\PointCloudUploads"))
                .ReturnsAsync(ResultDTO<string>.Ok(null));

            // Act
            var result = await _service.EditFileInUploads(webRootPath, filePath, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Point cloud upload folder path is null", result.ErrMsg);
        }

        [Fact]
        public async Task EditFileInUploads_ReturnsFail_WhenUploadFolderRetrievalFails()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { LegalLandfillId = Guid.NewGuid(), Id = Guid.NewGuid(), FileName = "file.txt" };
            string webRootPath = "path/to/webroot";
            string filePath = "uploads";

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileUploads", "Uploads\\LegalLandfillUploads\\PointCloudUploads"))
                .ReturnsAsync(ResultDTO<string>.Fail("Failed to retrieve upload folder path"));

            // Act
            var result = await _service.EditFileInUploads(webRootPath, filePath, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to retrieve upload folder path", result.ErrMsg);
        }

        [Fact]
        public async Task EditFileConverts_AppSettingsFail_ReturnsFail()
        {
            // Arrange
            string webRootPath = "mockWebRootPath";
            var oldLegalLandfillId = Guid.NewGuid();
            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), LegalLandfillId = Guid.NewGuid() };

            var appSettingResult = ResultDTO<string>.Fail("Failed to get setting");
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appSettingResult);

            // Act
            var result = await _service.EditFileConverts(webRootPath, oldLegalLandfillId, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to get setting", result.ErrMsg);
        }

        [Fact]
        public async Task EditFileConverts_AppSettingsNull_ReturnsFail()
        {
            // Arrange
            string webRootPath = "mockWebRootPath";
            var oldLegalLandfillId = Guid.NewGuid();
            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), LegalLandfillId = Guid.NewGuid() };

            var appSettingResult = ResultDTO<string>.Ok(null);
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appSettingResult);

            // Act
            var result = await _service.EditFileConverts(webRootPath, oldLegalLandfillId, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Point cloud upload folder path is null", result.ErrMsg);
        }


        [Fact]
        public async Task EditFileConverts_SuccessfullyMovesFiles()
        {
            // Arrange
            string webRootPath = "mockWebRootPath";
            var oldLegalLandfillId = Guid.NewGuid();
            var newLegalLandfillId = Guid.NewGuid();

            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), LegalLandfillId = newLegalLandfillId };

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts"))
                .ReturnsAsync(ResultDTO<string>.Ok("uploads/legal_landfill_uploads/point_cloud_converts"));

            string oldSubfolderPath = Path.Combine(webRootPath, "uploads/legal_landfill_uploads/point_cloud_converts", oldLegalLandfillId.ToString(), dto.Id.ToString());
            Directory.CreateDirectory(oldSubfolderPath);

            string oldFilePath = Path.Combine(oldSubfolderPath, "file.txt");
            using (var fs = File.Create(oldFilePath)) { }

            // Act
            var result = await _service.EditFileConverts(webRootPath, oldLegalLandfillId, dto);

            // Assert
            Assert.True(result.IsSuccess);

            string newSubfolderPath = Path.Combine(webRootPath, "uploads/legal_landfill_uploads/point_cloud_converts", newLegalLandfillId.ToString(), dto.Id.ToString());

            Assert.True(File.Exists(Path.Combine(newSubfolderPath, "file.txt")), "Expected file to be moved to the new subfolder.");
        }

        [Fact]
        public async Task EditFileConverts_DeletesOldDirectory_WhenEmpty()
        {
            // Arrange
            string webRootPath = "mockWebRootPath";
            var oldLegalLandfillId = Guid.NewGuid();

            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), LegalLandfillId = Guid.NewGuid() };

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts"))
                .ReturnsAsync(ResultDTO<string>.Ok("uploads/legal_landfill_uploads/point_cloud_converts"));

            string oldSubfolderPath = Path.Combine(webRootPath, "uploads/legal_landfill_uploads/point_cloud_converts", oldLegalLandfillId.ToString(), dto.Id.ToString());

            Directory.CreateDirectory(oldSubfolderPath);
            string oldDirectoryPath = Path.Combine(webRootPath, "uploads/legal_landfill_uploads/point_cloud_converts", oldLegalLandfillId.ToString());
            Directory.CreateDirectory(oldDirectoryPath);

            string oldFilePath = Path.Combine(oldSubfolderPath, "file.txt");
            using (var fs = File.Create(oldFilePath)) { }

            string newSubfolderPath = Path.Combine(webRootPath, "uploads/legal_landfill_uploads/point_cloud_converts", dto.LegalLandfillId.ToString(), dto.Id.ToString());
            Directory.CreateDirectory(newSubfolderPath);

            File.Move(oldFilePath, Path.Combine(newSubfolderPath, "file.txt"));

            // Act
            var result = await _service.EditFileConverts(webRootPath, oldLegalLandfillId, dto);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.False(Directory.Exists(oldSubfolderPath), "Expected old subfolder to be deleted.");
            Assert.False(Directory.Exists(oldDirectoryPath), "Expected old directory to be deleted.");
        }

        [Fact]
        public async Task EditFileConverts_WhenDirectoryCreationThrowsException_ShouldReturnExceptionFail()
        {
            // Arrange
            string webRootPath = "mockWebRootPath";
            var oldLegalLandfillId = Guid.NewGuid();
            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), LegalLandfillId = Guid.NewGuid() };

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts"))
                .ReturnsAsync(ResultDTO<string>.Ok("uploads/legal_landfill_uploads/point_cloud_converts"));

            // Simulate exception during directory creation
            _mockLogger.Setup(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            )).Verifiable();

            // Act
            var result = await _service.EditFileConverts(webRootPath, oldLegalLandfillId, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
        }


        [Fact]
        public async Task CheckSupportingFiles_AppSettingsFail_ReturnsFail()
        {
            // Arrange
            string fileUploadExtension = ".las";
            var appSettingResult = ResultDTO<string>.Fail("Failed to get supported extensions");
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("SupportedPointCloudFileExtensions", It.IsAny<string>()))
                .ReturnsAsync(appSettingResult);

            // Act
            var result = await _service.CheckSupportingFiles(fileUploadExtension);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to get supported extensions", result.ErrMsg);
        }

        [Fact]
        public async Task CheckSupportingFiles_AppSettingsNull_ReturnsFail()
        {
            // Arrange
            string fileUploadExtension = ".las";
            var appSettingResult = ResultDTO<string>.Ok(null);
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("SupportedPointCloudFileExtensions", It.IsAny<string>()))
                .ReturnsAsync(appSettingResult);

            // Act
            var result = await _service.CheckSupportingFiles(fileUploadExtension);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No data for supported file extensions", result.ErrMsg);
        }

        [Fact]
        public async Task CheckSupportingFiles_UnsupportedExtension_ReturnsFail()
        {
            // Arrange
            string fileUploadExtension = ".xyz";
            var supportedExtensions = ".las, .laz";
            var appSettingResult = ResultDTO<string>.Ok(supportedExtensions);
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("SupportedPointCloudFileExtensions", It.IsAny<string>()))
                .ReturnsAsync(appSettingResult);

            // Act
            var result = await _service.CheckSupportingFiles(fileUploadExtension);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Not supported file extension. Supported extensions are {supportedExtensions}", result.ErrMsg);
        }

        [Fact]
        public async Task CheckSupportingFiles_WhenExceptionThrown_ReturnsExceptionFail()
        {
            // Arrange
            string fileUploadExtension = ".las";
            var exceptionMessage = "Unexpected error while fetching app settings.";

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("SupportedPointCloudFileExtensions", It.IsAny<string>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.CheckSupportingFiles(fileUploadExtension);

            // Assert
            Assert.False(result.IsSuccess, "Expected the result to indicate failure.");
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }


        [Fact]
        public async Task CreateDiffWasteVolumeComparisonFile_NullFilePaths_ReturnsFail()
        {
            // Arrange
            var orderedList = new List<LegalLandfillPointCloudFileDTO>
            {
                new LegalLandfillPointCloudFileDTO { FilePath = null, Id = Guid.NewGuid() },
                new LegalLandfillPointCloudFileDTO { FilePath = "pathB", Id = Guid.NewGuid() }
            };
            string webRootPath = "webRootPath";

            // Act
            var result = await _service.CreateDiffWasteVolumeComparisonFile(orderedList, webRootPath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("File path/s is/are null", result.ErrMsg);
        }

        [Fact]
        public async Task CreateDiffWasteVolumeComparisonFile_MissingAppSettings_ReturnsFail()
        {
            // Arrange
            var orderedList = new List<LegalLandfillPointCloudFileDTO>
            {
                new LegalLandfillPointCloudFileDTO { FilePath = "pathA", Id = Guid.NewGuid() },
                new LegalLandfillPointCloudFileDTO { FilePath = "pathB", Id = Guid.NewGuid() }
            };
            string webRootPath = "webRootPath";

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("PythonExeAbsPath", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Fail("Failed to get PythonExeAbsPath"));
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("GdalCalcAbsPath", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Fail("Failed to get GdalCalcAbsPath"));
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("OutputDiffFolderPath", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Fail("Failed to get OutputDiffFolderPath"));

            // Act
            var result = await _service.CreateDiffWasteVolumeComparisonFile(orderedList, webRootPath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Can not get some of the application settings", result.ErrMsg);
        }

        [Fact]
        public async Task CreateDiffWasteVolumeComparisonFile_NullAppSettingsData_ReturnsFail()
        {
            // Arrange
            var orderedList = new List<LegalLandfillPointCloudFileDTO>
            {
                new LegalLandfillPointCloudFileDTO { FilePath = "pathA", Id = Guid.NewGuid() },
                new LegalLandfillPointCloudFileDTO { FilePath = "pathB", Id = Guid.NewGuid() }
            };
            string webRootPath = "webRootPath";

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("PythonExeAbsPath", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok(null));
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("GdalCalcAbsPath", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok(null));
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("OutputDiffFolderPath", It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok(null));

            // Act
            var result = await _service.CreateDiffWasteVolumeComparisonFile(orderedList, webRootPath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Some of the paths are null", result.ErrMsg);
        }

        [Fact]
        public async Task CreateDiffWasteVolumeComparisonFile_ExceptionThrown_ReturnsExceptionFail()
        {
            // Arrange
            var orderedList = new List<LegalLandfillPointCloudFileDTO>
            {
                new LegalLandfillPointCloudFileDTO { FilePath = "pathA", Id = Guid.NewGuid() },
                new LegalLandfillPointCloudFileDTO { FilePath = "pathB", Id = Guid.NewGuid() }
            };
            string webRootPath = "webRootPath";

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("PythonExeAbsPath", It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _service.CreateDiffWasteVolumeComparisonFile(orderedList, webRootPath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Test Exception", result.ErrMsg);
        }

        [Fact]
        public async Task GetFilteredLegalLandfillPointCloudFiles_ReturnsSuccessResult_WhenDataIsRetrieved()
        {
            // Arrange
            var selectedIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            var legalLandfillFiles = new List<LegalLandfillPointCloudFile>
            {
                new LegalLandfillPointCloudFile { Id = selectedIds[0] },
                new LegalLandfillPointCloudFile { Id = selectedIds[1] }
            };

            var resultGetAllEntities = ResultDTO<IEnumerable<LegalLandfillPointCloudFile>>.Ok(legalLandfillFiles);

            _mockRepository.Setup(repo => repo.GetAll(
                    It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(),
                    It.IsAny<Func<IQueryable<LegalLandfillPointCloudFile>, IOrderedQueryable<LegalLandfillPointCloudFile>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(resultGetAllEntities);

            var expectedDtos = new List<LegalLandfillPointCloudFileDTO>
            {
                new LegalLandfillPointCloudFileDTO { /* Set properties matching legalLandfillFiles[0] */ },
                new LegalLandfillPointCloudFileDTO { /* Set properties matching legalLandfillFiles[1] */ }
            };

            _mockMapper.Setup(m => m.Map<List<LegalLandfillPointCloudFileDTO>>(It.IsAny<IEnumerable<LegalLandfillPointCloudFile>>()))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetFilteredLegalLandfillPointCloudFiles(selectedIds);

            // Assert
            Assert.True(result.IsSuccess, "Expected success but got failure.");
            Assert.Equal(expectedDtos, result.Data);
        }

        [Fact]
        public async Task GetFilteredLegalLandfillPointCloudFiles_ReturnsFailureResult_WhenRepositoryFails()
        {
            // Arrange
            var selectedIds = new List<Guid> { Guid.NewGuid() };

            var resultGetAllEntities = ResultDTO<IEnumerable<LegalLandfillPointCloudFile>>.Fail("Error retrieving filtered data");

            _mockRepository.Setup(repo => repo.GetAll(
                    It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(),
                    It.IsAny<Func<IQueryable<LegalLandfillPointCloudFile>, IOrderedQueryable<LegalLandfillPointCloudFile>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(resultGetAllEntities);

            // Act
            var result = await _service.GetFilteredLegalLandfillPointCloudFiles(selectedIds);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error retrieving filtered data", result.ErrMsg);
        }

        [Fact]
        public async Task GetFilteredLegalLandfillPointCloudFiles_WhenExceptionThrown_ShouldReturnExceptionFailResult()
        {
            // Arrange
            var selectedIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var exceptionMessage = "Unexpected error";
            _mockRepository.Setup(repo => repo.GetAll(
                    It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(),
                    It.IsAny<Func<IQueryable<LegalLandfillPointCloudFile>, IOrderedQueryable<LegalLandfillPointCloudFile>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.GetFilteredLegalLandfillPointCloudFiles(selectedIds);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }


        [Fact]
        public async Task ConvertToPointCloud_CreatesDirectory_WhenItDoesNotExist()
        {
            // Arrange
            var potreeConverterFilePath = "path/to/potreeConverter";
            var uploadResultData = "upload/result/data";
            var convertsFolder = "path/to/converts"; // This folder should not exist initially
            var filePath = "path/to/file";

            // Ensure the directory does not exist before the test
            if (Directory.Exists(convertsFolder))
            {
                Directory.Delete(convertsFolder, true);
            }

            // Act
            var result = await _service.ConvertToPointCloud(potreeConverterFilePath, uploadResultData, convertsFolder, filePath);

            // Assert
            Assert.True(Directory.Exists(convertsFolder), "Expected directory to be created.");
        }

        [Fact]
        public async Task DeleteFilesFromUploads_ShouldFail_WhenFilePathIsNull()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { FilePath = null };

            // Act
            var result = await _service.DeleteFilesFromUploads(dto, "webRootPath");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("File path is null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteFilesFromUploads_DeletesOnlyExistingFiles_AndReturnsSuccess()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                FilePath = "uploads",
                FileName = "file.txt"
            };

            string webRootPath = "path/to/webroot";
            string dirPath = Path.Combine(webRootPath, dto.FilePath);
            string mainFilePath = Path.Combine(dirPath, dto.Id + Path.GetExtension(dto.FileName));
            string tiffFilePath = Path.Combine(dirPath, dto.Id + "_dsm.tif");

            Directory.CreateDirectory(dirPath);
            using (var fs = File.Create(mainFilePath)) { }

            // Act
            var result = await _service.DeleteFilesFromUploads(dto, webRootPath);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(File.Exists(mainFilePath));
            Assert.False(Directory.Exists(dirPath));

            // Cleanup - just in case assertions fail
            if (File.Exists(mainFilePath)) File.Delete(mainFilePath);
            if (Directory.Exists(dirPath)) Directory.Delete(dirPath, true);
        }

        [Fact]
        public async Task DeleteFilesFromUploads_ReturnsFail_WhenDirectoryDoesNotExist()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                FilePath = "uploads",
                FileName = "file.txt"
            };

            string webRootPath = "path/to/webroot";
            string dirPath = Path.Combine(webRootPath, dto.FilePath);

            // Act
            var result = await _service.DeleteFilesFromUploads(dto, webRootPath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"{Path.Combine(dirPath, dto.Id + Path.GetExtension(dto.FileName))} directory does not exist", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteFilesFromUploads_SkipsNonExistentFiles_AndReturnsSuccess()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                FilePath = "uploads",
                FileName = "file.txt"
            };

            string webRootPath = "path/to/webroot";
            string dirPath = Path.Combine(webRootPath, dto.FilePath);

            Directory.CreateDirectory(dirPath);

            // Act
            var result = await _service.DeleteFilesFromUploads(dto, webRootPath);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(Directory.Exists(dirPath));

            // Cleanup - just in case assertion fails
            if (Directory.Exists(dirPath)) Directory.Delete(dirPath, true);
        }

        [Fact]
        public async Task DeleteFilesFromUploads_ReturnsSuccess_WhenFilesAreDeleted()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                FilePath = "uploads",
                FileName = "file.txt"
            };

            string webRootPath = "path/to/webroot";

            Directory.CreateDirectory(Path.Combine(webRootPath, dto.FilePath));

            string mainFilePath = Path.Combine(webRootPath, dto.FilePath, dto.Id + Path.GetExtension(dto.FileName));
            string tiffFilePath = Path.Combine(webRootPath, dto.FilePath, dto.Id + "_dsm.tif");

            using (var fs1 = File.Create(mainFilePath)) { }
            using (var fs2 = File.Create(tiffFilePath)) { }

            // Act
            var result = await _service.DeleteFilesFromUploads(dto, webRootPath);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.False(File.Exists(mainFilePath), "Main file should be deleted.");
            Assert.False(File.Exists(tiffFilePath), "TIF file should be deleted.");
            Assert.False(Directory.Exists(Path.Combine(webRootPath, dto.FilePath)), "Directory should be deleted.");
        }
       
        //TODO FOR LINUX

        //[Fact]
        //public async Task DeleteFilesFromUploads_WhenDeleteThrowsException_ShouldReturnExceptionFail()
        //{
        //    // Arrange
        //    var dto = new LegalLandfillPointCloudFileDTO
        //    {
        //        Id = Guid.NewGuid(),
        //        FilePath = "uploads",
        //        FileName = "file.txt"
        //    };

        //    string webRootPath = Path.GetTempPath();
        //    string filePath = Path.Combine(webRootPath, dto.FilePath);
        //    string mainFilePath = Path.Combine(filePath, dto.Id + Path.GetExtension(dto.FileName));
        //    string tiffFilePath = Path.Combine(filePath, dto.Id + "_dsm.tif");

        //    // Create directory and files
        //    Directory.CreateDirectory(filePath);
        //    File.WriteAllText(mainFilePath, "Dummy content for main file.");
        //    File.WriteAllText(tiffFilePath, "Dummy content for TIF file.");

        //    // Lock the main file to simulate deletion failure
        //    using (var fs = new FileStream(mainFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
        //    {
        //        // Act
        //        var result = await _service.DeleteFilesFromUploads(dto, webRootPath);

        //        // Assert
        //        Assert.False(result.IsSuccess);
        //        Assert.NotNull(result.ErrMsg); // Adjust to match your exception message format
        //    } // File lock is released here
        //}

        [Fact]
        public async Task DeleteFilesFromConverts_ReturnsFail_WhenConvertFolderPathIsNull()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { LegalLandfillId = Guid.NewGuid(), Id = Guid.NewGuid() };
            string webRootPath = "path/to/webroot";

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts"))
                .ReturnsAsync(ResultDTO<string>.Ok(null));

            // Act
            var result = await _service.DeleteFilesFromConverts(dto, webRootPath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Point cloud convert folder path is null", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteFilesFromConverts_ReturnsFail_WhenConvertFolderRetrievalFails()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { LegalLandfillId = Guid.NewGuid(), Id = Guid.NewGuid() };
            string webRootPath = "path/to/webroot";

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts"))
                .ReturnsAsync(ResultDTO<string>.Fail("Failed to retrieve convert folder path"));

            // Act
            var result = await _service.DeleteFilesFromConverts(dto, webRootPath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to retrieve convert folder path", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteFilesFromConverts_ReturnsFail_WhenConvertedFileDirectoryDoesNotExist()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { LegalLandfillId = Guid.NewGuid(), Id = Guid.NewGuid() };
            string webRootPath = "path/to/webroot";
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts"))
                .ReturnsAsync(ResultDTO<string>.Ok("uploads/legal_landfill_uploads/point_cloud_converts"));

            // Act
            var result = await _service.DeleteFilesFromConverts(dto, webRootPath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("directory does not exist and files were not deleted", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteFilesFromConverts_ReturnsSuccess_WhenDirectoriesExistAndDeleted()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { LegalLandfillId = Guid.NewGuid(), Id = Guid.NewGuid() };
            string webRootPath = "path/to/webroot";

            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts"))
                .ReturnsAsync(ResultDTO<string>.Ok("uploads/legal_landfill_uploads/point_cloud_converts"));

            string currentFolderOfConvertedFilePath = Path.Combine(webRootPath, "uploads/legal_landfill_uploads/point_cloud_converts", dto.LegalLandfillId.ToString(), dto.Id.ToString());

            Directory.CreateDirectory(currentFolderOfConvertedFilePath);
            Directory.CreateDirectory(Path.Combine(webRootPath, "uploads/legal_landfill_uploads/point_cloud_converts", dto.LegalLandfillId.ToString()));

            // Act
            var result = await _service.DeleteFilesFromConverts(dto, webRootPath);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.False(Directory.Exists(currentFolderOfConvertedFilePath), "Expected converted file directory to be deleted.");
        }


        [Fact]
        public async Task GetLegalLandfillPointCloudFilesById_WhenExceptionThrown_ShouldReturnExceptionFailResult()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var exceptionMessage = "Unexpected error";
            _mockRepository.Setup(repo => repo.GetById(fileId, false, It.IsAny<string>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.GetLegalLandfillPointCloudFilesById(fileId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }

        [Fact]
        public async Task DeleteLegalLandfillPointCloudFile_WhenExceptionThrown_ShouldReturnExceptionFailResult()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var exceptionMessage = "Unexpected error";
            _mockRepository.Setup(repo => repo.GetFirstOrDefault(It.IsAny<Expression<Func<LegalLandfillPointCloudFile, bool>>>(), false, null))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.DeleteLegalLandfillPointCloudFile(fileId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }

        [Fact]
        public async Task CheckSupportingFiles_WhenExceptionThrown_ShouldReturnExceptionFailResult()
        {
            // Arrange
            var fileExtension = ".txt";
            var exceptionMessage = "Unexpected error";
            _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _service.CheckSupportingFiles(fileExtension);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }


        //TODO FOR LINUX
        //[Fact]
        //public async Task DeleteFilesFromConverts_WhenExceptionThrown_ShouldReturnExceptionFailResult()
        //{
        //    // Arrange
        //    string webRootPath = "mockWebRootPath";
        //    var legalLandfillId = Guid.NewGuid();
        //    var fileId = Guid.NewGuid();

        //    var dto = new LegalLandfillPointCloudFileDTO { Id = fileId, LegalLandfillId = legalLandfillId };

        //    _mockAppSettingsAccessor
        //        .Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts"))
        //        .ReturnsAsync(ResultDTO<string>.Ok("uploads/legal_landfill_uploads/point_cloud_converts"));

        //    // Set up the directory paths
        //    string convertedFolderPath = Path.Combine(webRootPath, "uploads/legal_landfill_uploads/point_cloud_converts", legalLandfillId.ToString());
        //    string specificFolderPath = Path.Combine(convertedFolderPath, fileId.ToString());

        //    // Create directory and lock a file to trigger an exception on deletion
        //    Directory.CreateDirectory(specificFolderPath);
        //    string lockedFilePath = Path.Combine(specificFolderPath, "lockedFile.txt");

        //    using (var fs = new FileStream(lockedFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        //    {
        //        // Act
        //        var result = await _service.DeleteFilesFromConverts(dto, webRootPath);

        //        // Assert
        //        Assert.False(result.IsSuccess);
        //        Assert.NotNull(result.ErrMsg);
        //    }
        //}

        // TODO FOR LINUX
        //[Fact]
        //public async Task EditFileConverts_WhenExceptionThrown_ShouldReturnExceptionFailResult()
        //{
        //    // Arrange
        //    string webRootPath = "mockWebRootPath";
        //    var oldLegalLandfillId = Guid.NewGuid();
        //    var newLegalLandfillId = Guid.NewGuid();

        //    var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), LegalLandfillId = newLegalLandfillId, FilePath = "uploads/legal_landfill_uploads/point_cloud_converts" };

        //    _mockAppSettingsAccessor.Setup(a => a.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts"))
        //        .ReturnsAsync(ResultDTO<string>.Ok("uploads/legal_landfill_uploads/point_cloud_converts"));

        //    string oldSubfolderPath = Path.Combine(webRootPath, "uploads/legal_landfill_uploads/point_cloud_converts", oldLegalLandfillId.ToString(), dto.Id.ToString());
        //    Directory.CreateDirectory(oldSubfolderPath);

        //    string oldFilePath = Path.Combine(oldSubfolderPath, "file.txt");

        //    // Create and open a file stream to simulate a lock, preventing deletion
        //    using (var fs = new FileStream(oldFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        //    {
        //        // Act
        //        var result = await _service.EditFileConverts(webRootPath, oldLegalLandfillId, dto);

        //        // Assert
        //        Assert.False(result.IsSuccess);
        //        Assert.NotNull(result.ErrMsg);
        //    }
        //}

        [Fact]
        public async Task CreateTifFile_WhenExceptionThrown_ReturnsExceptionFail()
        {
            // Arrange
            string pipelineJsonTemplate = "{INPUT_FILE} -> {OUTPUT_FILE}";
            string pdalAbsPath = "/usr/bin/pdal";
            string filePath = "/path/to/input/file.las";
            string tifFilePath = "/path/to/output/file.tif";
            string exceptionMessage = "Failed to start a process with file path '/usr/bin/pdal'. Target file or working directory doesn't exist, or the provided credentials are invalid.";

            // Act
            var result = await _service.CreateTifFile(pipelineJsonTemplate, pdalAbsPath, filePath, tifFilePath);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrMsg);
        }


        [Fact]
        public async Task EditFileInUploads_ShouldMoveFiles_WhenPathsAreValid()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), FileName = "testfile.txt", LegalLandfillId = Guid.NewGuid() };
            string webRootPath = Path.GetTempPath();
            string filePath = Path.Combine(webRootPath, "Uploads", "TestFolder");

            // Create test directories and files
            Directory.CreateDirectory(filePath);
            string oldFileUploadPath = Path.Combine(filePath, dto.Id + Path.GetExtension(dto.FileName));
            string oldTifFilePath = Path.Combine(filePath, dto.Id + "_dsm.tif");

            // Create dummy files
            File.WriteAllText(oldFileUploadPath, "Dummy content for test file.");
            File.WriteAllText(oldTifFilePath, "Dummy content for test TIFF file.");

            // Set up mock for the app settings accessor
            _mockAppSettingsAccessor.Setup(m => m.GetApplicationSettingValueByKey<string>(
                "LegalLandfillPointCloudFileUploads",
                It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok("Uploads/LegalLandfillUploads/PointCloudUploads"));

            // Act
            var result = await _service.EditFileInUploads(webRootPath, filePath, dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(File.Exists(Path.Combine(webRootPath, "Uploads", "LegalLandfillUploads", "PointCloudUploads", dto.LegalLandfillId.ToString(), dto.Id + Path.GetExtension(dto.FileName))));
            Assert.True(File.Exists(Path.Combine(webRootPath, "Uploads", "LegalLandfillUploads", "PointCloudUploads", dto.LegalLandfillId.ToString(), dto.Id + "_dsm.tif")));
        }

        [Fact]
        public async Task EditFileInUploads_WhenFileMoveThrowsException_ShouldReturnExceptionFail()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO { Id = Guid.NewGuid(), FileName = "testfile.txt", LegalLandfillId = Guid.NewGuid() };
            string webRootPath = Path.GetTempPath();
            string filePath = Path.Combine(webRootPath, "Uploads", "TestFolder");

            // Set up mock for the app settings accessor to return a valid path
            _mockAppSettingsAccessor.Setup(m => m.GetApplicationSettingValueByKey<string>(
                "LegalLandfillPointCloudFileUploads",
                It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<string>.Ok("Uploads/LegalLandfillUploads/PointCloudUploads"));

            // Create the directory but make the file path invalid or restricted to force a file move failure
            Directory.CreateDirectory(filePath);
            string oldFileUploadPath = Path.Combine(filePath, dto.Id + Path.GetExtension(dto.FileName));
            File.WriteAllText(oldFileUploadPath, "Dummy content for test file.");

            string newFolderPath = Path.Combine(webRootPath, "Uploads", "LegalLandfillUploads", "PointCloudUploads", dto.LegalLandfillId.ToString());
            Directory.CreateDirectory(newFolderPath);

            // Act
            var result = await _service.EditFileInUploads(webRootPath, filePath, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrMsg);
        }


    }
}
