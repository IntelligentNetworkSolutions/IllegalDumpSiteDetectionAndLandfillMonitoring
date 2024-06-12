using Entities;
using SD.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests
{
    public class ApplicationSettingsTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var key = "TestKey";
            var value = "TestValue";
            var description = "Test Description";
            var module = "TestModule";
            var dataType = ApplicationSettingsDataType.String;

            // Act
            var settings = new ApplicationSettings
            {
                Key = key,
                Value = value,
                Description = description,
                Module = module,
                DataType = dataType
            };

            // Assert
            Assert.Equal(key, settings.Key);
            Assert.Equal(value, settings.Value);
            Assert.Equal(description, settings.Description);
            Assert.Equal(module, settings.Module);
            Assert.Equal(dataType, settings.DataType);
        }

        [Fact]
        public void Constructor_DefaultValuesShouldBeSet()
        {
            // Act
            var settings = new ApplicationSettings();

            // Assert
            Assert.Null(settings.Module);
        }

        [Fact]
        public void ShouldAllowNullForOptionalProperties()
        {
            // Arrange
            var settings = new ApplicationSettings
            {
                Key = "TestKey",
                Value = "TestValue",
                Description = "Test Description",
                DataType = ApplicationSettingsDataType.String
            };

            // Act
            settings.Module = null;

            // Assert
            Assert.Null(settings.Module);
        }
            
        [Fact]
        public void KeyProperty_ShouldHaveDatabaseGeneratedOptionNone()
        {
            // Arrange & Act
            var keyProperty = typeof(ApplicationSettings).GetProperty("Key");

            // Assert
            var databaseGeneratedAttribute = (DatabaseGeneratedAttribute)Attribute.GetCustomAttribute(keyProperty, typeof(DatabaseGeneratedAttribute));
            Assert.NotNull(databaseGeneratedAttribute);
            Assert.Equal(DatabaseGeneratedOption.None, databaseGeneratedAttribute.DatabaseGeneratedOption);
        }
    }
}
