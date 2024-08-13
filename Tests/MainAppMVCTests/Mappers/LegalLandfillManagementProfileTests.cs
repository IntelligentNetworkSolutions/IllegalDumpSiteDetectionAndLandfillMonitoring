using AutoMapper;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using MainApp.MVC.Mappers;
using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Mappers
{
    public class LegalLandfillManagementProfileTests
    {
        private readonly IMapper _mapper;

        public LegalLandfillManagementProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<LegalLandfillManagementProfile>());
            _mapper = config.CreateMapper();
        }
               
        [Fact]
        public void Map_LegalLandfillDTO_To_LegalLandfillViewModel()
        {
            // Arrange
            var dto = new LegalLandfillDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Landfill",
                Description = "Test Description"
            };

            // Act
            var viewModel = _mapper.Map<LegalLandfillViewModel>(dto);

            // Assert
            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.Name, viewModel.Name);
            Assert.Equal(dto.Description, viewModel.Description);
        }

        [Fact]
        public void Map_LegalLandfillViewModel_To_LegalLandfillDTO()
        {
            // Arrange
            var viewModel = new LegalLandfillViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Landfill",
                Description = "Test Description"
            };

            // Act
            var dto = _mapper.Map<LegalLandfillDTO>(viewModel);

            // Assert
            Assert.Equal(viewModel.Id, dto.Id);
            Assert.Equal(viewModel.Name, dto.Name);
            Assert.Equal(viewModel.Description, dto.Description);
        }

        [Fact]
        public void Map_LegalLandfillPointCloudFileDTO_To_LegalLandfillPointCloudFileViewModel()
        {
            // Arrange
            var dto = new LegalLandfillPointCloudFileDTO
            {
                Id = Guid.NewGuid(),
                FileName = "testfile.las",
                FilePath = "/files/testfile.las",
                ScanDateTime = DateTime.UtcNow,
                LegalLandfillId = Guid.NewGuid()
            };

            // Act
            var viewModel = _mapper.Map<LegalLandfillPointCloudFileViewModel>(dto);

            // Assert
            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.FileName, viewModel.FileName);
            Assert.Equal(dto.FilePath, viewModel.FilePath);
            Assert.Equal(dto.ScanDateTime, viewModel.ScanDateTime);
            Assert.Equal(dto.LegalLandfillId, viewModel.LegalLandfillId);
        }

        [Fact]
        public void Map_LegalLandfillPointCloudFileViewModel_To_LegalLandfillPointCloudFileDTO()
        {
            // Arrange
            var viewModel = new LegalLandfillPointCloudFileViewModel
            {
                Id = Guid.NewGuid(),
                FileName = "testfile.las",
                FilePath = "/files/testfile.las",
                ScanDateTime = DateTime.UtcNow,
                LegalLandfillId = Guid.NewGuid()
            };

            // Act
            var dto = _mapper.Map<LegalLandfillPointCloudFileDTO>(viewModel);

            // Assert
            Assert.Equal(viewModel.Id, dto.Id);
            Assert.Equal(viewModel.FileName, dto.FileName);
            Assert.Equal(viewModel.FilePath, dto.FilePath);
            Assert.Equal(viewModel.ScanDateTime, dto.ScanDateTime);
            Assert.Equal(viewModel.LegalLandfillId, dto.LegalLandfillId);
        }

    }
}
