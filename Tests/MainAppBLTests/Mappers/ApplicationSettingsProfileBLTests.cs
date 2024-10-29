using AutoMapper;
using DTOs.MainApp.BL;
using Entities;
using MainApp.BL.Mappers;
using SD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Mappers
{
    public class ApplicationSettingsProfileBLTests
    {
        private readonly IMapper _mapper;

        public ApplicationSettingsProfileBLTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ApplicationSettingsProfileBL>();
            });
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void ApplicationSettings_To_AppSettingDTO_Should_Map_Properties()
        {
            // Arrange
            var entity = new ApplicationSettings
            {
                Key = "SettingKey",
                Value = "SettingValue",
                Description = "A test setting",
                Module = "TestModule",
                DataType = ApplicationSettingsDataType.String
            };

            // Act
            var dto = _mapper.Map<AppSettingDTO>(entity);

            // Assert
            Assert.Equal(entity.Key, dto.Key);
            Assert.Equal(entity.Value, dto.Value);
            Assert.Equal(entity.Description, dto.Description);
            Assert.Equal(entity.Module, dto.Module);
            Assert.Equal(entity.DataType, dto.DataType);
        }

        [Fact]
        public void AppSettingDTO_To_ApplicationSettings_Should_Map_Properties()
        {
            // Arrange
            var dto = new AppSettingDTO
            {
                Key = "SettingKey",
                Value = "SettingValue",
                Description = "A test setting",
                Module = "TestModule",
                DataType = ApplicationSettingsDataType.String
            };

            // Act
            var entity = _mapper.Map<ApplicationSettings>(dto);

            // Assert
            Assert.Equal(dto.Key, entity.Key);
            Assert.Equal(dto.Value, entity.Value);
            Assert.Equal(dto.Description, entity.Description);
            Assert.Equal(dto.Module, entity.Module);
            Assert.Equal(dto.DataType, entity.DataType);
        }
    }
}
