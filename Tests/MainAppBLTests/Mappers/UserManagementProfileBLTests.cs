using AutoMapper;
using DTOs.MainApp.BL;
using Entities;
using MainApp.BL.Mappers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Mappers
{
    public class UserManagementProfileBLTests
    {
        private readonly IMapper _mapper;

        public UserManagementProfileBLTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserManagementProfileBL>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void UserDTO_To_ApplicationUser_Mapping_ShouldBeValid()
        {
            // Arrange
            var userDto = new UserDTO
            {
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "1234567890",
                IsActive = true
            };

            // Act
            var applicationUser = _mapper.Map<ApplicationUser>(userDto);

            // Assert
            Assert.NotNull(applicationUser);
            Assert.Equal(userDto.UserName, applicationUser.UserName);
            Assert.Equal(userDto.Email, applicationUser.Email);
            Assert.Equal(userDto.FirstName, applicationUser.FirstName);
            Assert.Equal(userDto.LastName, applicationUser.LastName);
            Assert.Equal(userDto.PhoneNumber, applicationUser.PhoneNumber);
            Assert.True(applicationUser.IsActive);
        }

        [Fact]
        public void ApplicationUser_To_UserDTO_Mapping_ShouldBeValid()
        {
            // Arrange
            var applicationUser = new ApplicationUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "1234567890",
                IsActive = true
            };

            // Act
            var userDto = _mapper.Map<UserDTO>(applicationUser);

            // Assert
            Assert.NotNull(userDto);
            Assert.Equal(applicationUser.UserName, userDto.UserName);
            Assert.Equal(applicationUser.Email, userDto.Email);
            Assert.Equal(applicationUser.FirstName, userDto.FirstName);
            Assert.Equal(applicationUser.LastName, userDto.LastName);
            Assert.Equal(applicationUser.PhoneNumber, userDto.PhoneNumber);
            Assert.True(userDto.IsActive);
        }

        [Fact]
        public void UserManagementDTO_To_ApplicationUser_Mapping_ShouldBeValid()
        {
            // Arrange
            var userManagementDto = new UserManagementDTO
            {
                UserName = "admin",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "0987654321",
                IsActive = true
            };

            // Act
            var applicationUser = _mapper.Map<ApplicationUser>(userManagementDto);

            // Assert
            Assert.NotNull(applicationUser);
            Assert.Equal(userManagementDto.UserName, applicationUser.UserName);
            Assert.Equal(userManagementDto.Email, applicationUser.Email);
            Assert.Equal(userManagementDto.FirstName, applicationUser.FirstName);
            Assert.Equal(userManagementDto.LastName, applicationUser.LastName);
            Assert.Equal(userManagementDto.PhoneNumber, applicationUser.PhoneNumber);
            Assert.True(applicationUser.IsActive);
        }


        [Fact]
        public void Map_UserDTO_To_ApplicationUser_WithNullIsActive_ShouldDefaultToFalse()
        {
            // Arrange
            var userDto = new UserDTO
            {
                UserName = "testuser",
                IsActive = null
            };

            // Act
            var applicationUser = _mapper.Map<ApplicationUser>(userDto);

            // Assert
            Assert.False(applicationUser.IsActive);
        }

        [Fact]
        public void Map_String_To_IdentityUserRole_ShouldMapCorrectly()
        {
            // Arrange
            var roleId = "admin-role";

            // Act
            var userRole = _mapper.Map<IdentityUserRole<string>>(roleId);

            // Assert
            Assert.Equal(roleId, userRole.RoleId);
            Assert.Null(userRole.UserId);
        }

        [Fact]
        public void Map_String_To_IdentityUserClaim_ShouldMapCorrectly()
        {
            // Arrange
            var claimValue = "permission:read";

            // Act
            var userClaim = _mapper.Map<IdentityUserClaim<string>>(claimValue);

            // Assert
            Assert.Equal(claimValue, userClaim.ClaimValue);
            Assert.Null(userClaim.UserId);
        }

        [Fact]
        public void Map_IdentityRole_To_RoleManagementDTO_ShouldMapCorrectly()
        {
            // Arrange
            var role = new IdentityRole
            {
                Id = "role-id",
                Name = "Admin",
                NormalizedName = "ADMIN"
            };

            // Act
            var roleDto = _mapper.Map<RoleManagementDTO>(role);

            // Assert
            Assert.Equal(role.Id, roleDto.Id);
            Assert.Equal(role.Name, roleDto.Name);
            Assert.Equal(role.NormalizedName, roleDto.NormalizedName);
        }

        [Fact]
        public void Map_String_To_IdentityRoleClaim_ShouldMapCorrectly()
        {
            // Arrange
            var claimValue = "manage:users";

            // Act
            var roleClaim = _mapper.Map<IdentityRoleClaim<string>>(claimValue);

            // Assert
            Assert.Equal("AuthorizationClaim", roleClaim.ClaimType);
            Assert.Equal(claimValue, roleClaim.ClaimValue);
            Assert.Null(roleClaim.RoleId);
        }

        [Fact]
        public void Map_ApplicationUser_To_UserDTO_ShouldMapCorrectly()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "user-id",
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "1234567890",
                IsActive = true,
                NormalizedUserName = "TESTUSER",
                PasswordHash = "hashedpassword"
            };

            // Act
            var userDto = _mapper.Map<UserDTO>(user);

            // Assert
            Assert.Equal(user.Id, userDto.Id);
            Assert.Equal(user.UserName, userDto.UserName);
            Assert.Equal(user.Email, userDto.Email);
            Assert.Equal(user.FirstName, userDto.FirstName);
            Assert.Equal(user.LastName, userDto.LastName);
            Assert.Equal(user.PhoneNumber, userDto.PhoneNumber);
            Assert.Equal(user.IsActive, userDto.IsActive);
            Assert.Equal(user.NormalizedUserName, userDto.NormalizedUserName);
            Assert.Equal(user.PasswordHash, userDto.PasswordHash);
        }
            
    }
}
