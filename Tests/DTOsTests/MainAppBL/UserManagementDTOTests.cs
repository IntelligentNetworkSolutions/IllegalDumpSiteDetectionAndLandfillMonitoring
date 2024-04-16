using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL;

namespace Tests.DTOsTests.MainAppBL
{
    public class UserManagementDTOTests
    {
        [Fact]
        public void Constructor_Initialization()
        {
            // Arrange & Act
            var userManagementDto = new UserManagementDTO();

            // Assert
            Assert.NotNull(userManagementDto.Roles);
            Assert.NotNull(userManagementDto.RolesInsert);
            Assert.NotNull(userManagementDto.ClaimsInsert);
            Assert.NotNull(userManagementDto.AllUsersExceptCurrent);
            Assert.NotNull(userManagementDto.RoleClaims);
            Assert.NotNull(userManagementDto.AllUsers);

            Assert.IsType<List<RoleDTO>>(userManagementDto.Roles);
            Assert.IsType<List<string>>(userManagementDto.RolesInsert);
            Assert.IsType<List<string>>(userManagementDto.ClaimsInsert);
            Assert.IsType<List<UserDTO>>(userManagementDto.AllUsersExceptCurrent);
            Assert.IsType<List<RoleClaimDTO>>(userManagementDto.RoleClaims);
            Assert.IsType<List<UserDTO>>(userManagementDto.AllUsers);

            Assert.Empty(userManagementDto.Roles);
            Assert.Empty(userManagementDto.RolesInsert);
            Assert.Empty(userManagementDto.ClaimsInsert);
            Assert.Empty(userManagementDto.AllUsersExceptCurrent);
            Assert.Empty(userManagementDto.RoleClaims);
            Assert.Empty(userManagementDto.AllUsers);
        }

        [Fact]
        public void Password_Properties_Initialization()
        {
            // Arrange & Act
            var userManagementDto = new UserManagementDTO();

            // Assert
            Assert.Equal(0, userManagementDto.PasswordMinLength);
            Assert.False(userManagementDto.PasswordMustHaveNumbers);
            Assert.False(userManagementDto.PasswordMustHaveLetters);
        }
    }
}
