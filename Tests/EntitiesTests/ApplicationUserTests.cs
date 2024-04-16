using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Tests.EntitiesTests
{
    public class ApplicationUserTests
    {
        [Fact]
        public void NormalizeUserNameAndEmail_WithValidData()
        {
            // Arrange
            var applicationUser = new ApplicationUser
            {
                UserName = "johnDoe",
                Email = "john.doe@example.com"
            };

            // Act
            applicationUser.NormalizeUserNameAndEmail();

            // Assert
            Assert.Equal("JOHNDOE", applicationUser.NormalizedUserName);
            Assert.Equal("JOHN.DOE@EXAMPLE.COM", applicationUser.NormalizedEmail);
        }

        [Fact]
        public void NormalizeUserNameAndEmail_WithNullData()
        {
            // Arrange
            var applicationUser = new ApplicationUser();

            // Act
            applicationUser.NormalizeUserNameAndEmail();

            // Assert
            Assert.Null(applicationUser.NormalizedUserName);
            Assert.Null(applicationUser.NormalizedEmail);
        }

        [Fact]
        public void NormalizeUserNameAndEmail_WithEmptyData_NormalizedUserNameAndEmailAreNull()
        {
            // Arrange
            var applicationUser = new ApplicationUser
            {
                UserName = "",
                Email = ""
            };

            // Act
            applicationUser.NormalizeUserNameAndEmail();

            // Assert
            Assert.Null(applicationUser.NormalizedUserName);
            Assert.Null(applicationUser.NormalizedEmail);
        }
    }
}
