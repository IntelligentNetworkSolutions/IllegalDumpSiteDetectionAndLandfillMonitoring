using DTOs.MainApp.BL;
using SD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class AppSettingDTOTests
    {
        [Fact]
        public void AppSettingDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var key = "TestKey";
            var value = "TestValue";
            var description = "Test Description";
            var module = "Test Module";
            var dataType = ApplicationSettingsDataType.String;

            // Act
            var dto = new AppSettingDTO
            {
                Key = key,
                Value = value,
                Description = description,
                Module = module,
                DataType = dataType
            };

            // Assert
            Assert.Equal(key, dto.Key);
            Assert.Equal(value, dto.Value);
            Assert.Equal(description, dto.Description);
            Assert.Equal(module, dto.Module);
            Assert.Equal(dataType, dto.DataType);
        }

        [Fact]
        public void AppSettingDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new AppSettingDTO();

            // Assert
            Assert.Null(dto.Key);
            Assert.Null(dto.Value);
            Assert.Null(dto.Description);
            Assert.Null(dto.Module);
            Assert.Equal(default(ApplicationSettingsDataType), dto.DataType);
        }

        [Fact]
        public void AppSettingDTO_Properties_ShouldBeInitializedWithProvidedValues()
        {
            // Arrange
            var dto = new AppSettingDTO
            {
                Key = "AnotherKey",
                Value = "AnotherValue",
                Description = "Another Description",
                Module = "Another Module",
                DataType = ApplicationSettingsDataType.Integer
            };

            // Assert
            Assert.Equal("AnotherKey", dto.Key);
            Assert.Equal("AnotherValue", dto.Value);
            Assert.Equal("Another Description", dto.Description);
            Assert.Equal("Another Module", dto.Module);
            Assert.Equal(ApplicationSettingsDataType.Integer, dto.DataType);
        }
    }
}
