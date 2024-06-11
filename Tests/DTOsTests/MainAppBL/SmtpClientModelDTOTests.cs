using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class SmtpClientModelDTOTests
    {
        [Fact]
        public void SmtpClientModelDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var host = "smtp.example.com";
            var port = 587;
            var useSsl = true;
            var userName = "user@example.com";
            var password = "password123";

            // Act
            var dto = new SmtpClientModelDTO
            {
                Host = host,
                Port = port,
                UseSsl = useSsl,
                UserName = userName,
                Password = password
            };

            // Assert
            Assert.Equal(host, dto.Host);
            Assert.Equal(port, dto.Port);
            Assert.Equal(useSsl, dto.UseSsl);
            Assert.Equal(userName, dto.UserName);
            Assert.Equal(password, dto.Password);
        }

        [Fact]
        public void SmtpClientModelDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new SmtpClientModelDTO();

            // Assert
            Assert.Null(dto.Host);
            Assert.Equal(0, dto.Port);
            Assert.False(dto.UseSsl);
            Assert.Null(dto.UserName);
            Assert.Null(dto.Password);
        }

        [Fact]
        public void SmtpClientModelDTO_Properties_ShouldBeSettable()
        {
            // Arrange
            var dto = new SmtpClientModelDTO();

            // Act
            dto.Host = "smtp.example.com";
            dto.Port = 587;
            dto.UseSsl = true;
            dto.UserName = "user@example.com";
            dto.Password = "password123";

            // Assert
            Assert.Equal("smtp.example.com", dto.Host);
            Assert.Equal(587, dto.Port);
            Assert.True(dto.UseSsl);
            Assert.Equal("user@example.com", dto.UserName);
            Assert.Equal("password123", dto.Password);
        }

        [Fact]
        public void SmtpClientModelDTO_NullableProperties_ShouldAcceptNullValues()
        {
            // Arrange
            var dto = new SmtpClientModelDTO
            {
                Host = null,
                UserName = null,
                Password = null
            };

            // Assert
            Assert.Null(dto.Host);
            Assert.Null(dto.UserName);
            Assert.Null(dto.Password);
        }
    }
}
