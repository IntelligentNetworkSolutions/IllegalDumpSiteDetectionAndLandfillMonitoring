using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL;

namespace Tests.DTOsTests.MainAppBL
{
    public class UserDTOTests
    {
        [Fact]
        public void TrimProperties_WithNullValues()
        {
            // Arrange
            var userDto = new UserDTO
            {
                UserName = null,
                FirstName = null,
                LastName = null,
                NormalizedUserName = null,
                Email = null,
                Password = null,
                ConfirmPassword = null
            };

            // Act
            userDto.TrimProperies();

            // Assert
            Assert.Null(userDto.UserName);
            Assert.Null(userDto.FirstName);
            Assert.Null(userDto.LastName);
            Assert.Null(userDto.NormalizedUserName);
            Assert.Null(userDto.Email);
            Assert.Null(userDto.Password);
            Assert.Null(userDto.ConfirmPassword);
        }

        [Fact]
        public void TrimProperties_WithValues()
        {
            // Arrange
            var userDto = new UserDTO
            {
                UserName = "  username  ",
                FirstName = "  first name  ",
                LastName = "  last name  ",
                NormalizedUserName = "  normalized username  ",
                Email = "  email@example.com  ",
                Password = "  password  ",
                ConfirmPassword = "  confirm password  "
            };

            // Act
            userDto.TrimProperies();

            // Assert
            Assert.Equal("username", userDto.UserName);
            Assert.Equal("first name", userDto.FirstName);
            Assert.Equal("last name", userDto.LastName);
            Assert.Equal("normalized username", userDto.NormalizedUserName);
            Assert.Equal("email@example.com", userDto.Email);
            Assert.Equal("password", userDto.Password);
            Assert.Equal("confirm password", userDto.ConfirmPassword);
        }
    }
}
