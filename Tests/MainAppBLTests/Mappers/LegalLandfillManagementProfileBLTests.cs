using AutoMapper;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Mappers
{
    public class LegalLandfillManagementProfileBLTests
    {
        private readonly IMapper _mapper;

        public LegalLandfillManagementProfileBLTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LegalLandfillManagementProfileBL>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void LegalLandfillDTO_To_LegalLandfill_MapsCorrectly()
        {
            // Arrange
            var dto = new LegalLandfillDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Landfill",
                Description = "Test Description",
                LegalLandfillPointCloudFiles = new List<LegalLandfillPointCloudFileDTO>
            {
                new LegalLandfillPointCloudFileDTO
                {
                    Id = Guid.NewGuid(),
                    FileName = "test.xyz",
                    FilePath = "/path/to/file",
                    ScanDateTime = DateTime.UtcNow
                }
            }
            };

            // Act
            var entity = _mapper.Map<LegalLandfill>(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id, entity.Id);
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Description, entity.Description);
            Assert.NotNull(entity.LegalLandfillPointCloudFiles);
            Assert.Single(entity.LegalLandfillPointCloudFiles);
        }

        [Fact]
        public void LegalLandfillPointCloudFileDTO_To_LegalLandfillPointCloudFile_MapsCorrectly()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                FileName = "pointcloud.xyz",
                FilePath = "/data/pointcloud.xyz",
                ScanDateTime = DateTime.UtcNow,
                LegalLandfillId = Guid.NewGuid()
            };

            // Act
            var entity = _mapper.Map<LegalLandfillPointCloudFile>(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id, entity.Id);
            Assert.Equal(dto.FileName, entity.FileName);
            Assert.Equal(dto.FilePath, entity.FilePath);
            Assert.Equal(dto.ScanDateTime, entity.ScanDateTime);
            Assert.Equal(dto.LegalLandfillId, entity.LegalLandfillId);
        }

        [Fact]
        public void LegalLandfillTruckDTO_To_LegalLandfillTruck_MapsCorrectly()
        {
            // Arrange
            var dto = new LegalLandfillTruckDTO
            {
                Id = Guid.NewGuid(),
                Name = "Truck 1",
                Registration = "ABC123",
                UnladenWeight = 5000.5,
                PayloadWeight = 10000.0,
                Capacity = 15000.0,
                IsEnabled = true,
                Description = "Test Truck"
            };

            // Act
            var entity = _mapper.Map<LegalLandfillTruck>(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id, entity.Id);
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Registration, entity.Registration);
            Assert.Equal(dto.UnladenWeight, entity.UnladenWeight);
            Assert.Equal(dto.PayloadWeight, entity.PayloadWeight);
            Assert.Equal(dto.Capacity, entity.Capacity);
            Assert.Equal(dto.IsEnabled, entity.IsEnabled);
            Assert.Equal(dto.Description, entity.Description);
        }

        [Fact]
        public void LegalLandfillWasteTypeDTO_To_LegalLandfillWasteType_MapsCorrectly()
        {
            // Arrange
            var dto = new LegalLandfillWasteTypeDTO
            {
                Id = Guid.NewGuid(),
                Name = "Waste Type 1",
                Description = "Test Waste Type"
            };

            // Act
            var entity = _mapper.Map<LegalLandfillWasteType>(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id, entity.Id);
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Description, entity.Description);
        }

        [Fact]
        public void LegalLandfillWasteImportDTO_To_LegalLandfillWasteImport_MapsCorrectly()
        {
            // Arrange
            var dto = new LegalLandfillWasteImportDTO
            {
                Id = Guid.NewGuid(),
                ImportedOn = DateTime.UtcNow,
                ImportExportStatus = 1,
                Capacity = 1000.0,
                Weight = 950.5,
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow,
                LegalLandfillTruckId = Guid.NewGuid(),
                LegalLandfillId = Guid.NewGuid(),
                LegalLandfillWasteTypeId = Guid.NewGuid()
            };

            // Act
            var entity = _mapper.Map<LegalLandfillWasteImport>(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id, entity.Id);
            Assert.Equal(dto.ImportedOn, entity.ImportedOn);
            Assert.Equal(dto.ImportExportStatus, entity.ImportExportStatus);
            Assert.Equal(dto.Capacity, entity.Capacity);
            Assert.Equal(dto.Weight, entity.Weight);
            Assert.Equal(dto.CreatedById, entity.CreatedById);
            Assert.Equal(dto.CreatedOn, entity.CreatedOn);
            Assert.Equal(dto.LegalLandfillTruckId, entity.LegalLandfillTruckId);
            Assert.Equal(dto.LegalLandfillId, entity.LegalLandfillId);
            Assert.Equal(dto.LegalLandfillWasteTypeId, entity.LegalLandfillWasteTypeId);
        }

        [Fact]
        public void ReverseMapping_LegalLandfill_To_LegalLandfillDTO_MapsCorrectly()
        {
            // Arrange
            var entity = new LegalLandfill
            {
                Id = Guid.NewGuid(),
                Name = "Test Landfill",
                Description = "Test Description",
                LegalLandfillPointCloudFiles = new List<LegalLandfillPointCloudFile>
            {
                new LegalLandfillPointCloudFile
                {
                    Id = Guid.NewGuid(),
                    FileName = "test.xyz",
                    FilePath = "/path/to/file",
                    ScanDateTime = DateTime.UtcNow
                }
            }
            };

            // Act
            var dto = _mapper.Map<LegalLandfillDTO>(entity);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Description, dto.Description);
            Assert.NotNull(dto.LegalLandfillPointCloudFiles);
            Assert.Single(dto.LegalLandfillPointCloudFiles);
        }     
    }
}
