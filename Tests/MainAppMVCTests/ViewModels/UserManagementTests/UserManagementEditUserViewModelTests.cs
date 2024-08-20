using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.UserManagementTests
{
    public class UserManagementEditUserViewModelTests
    {
        [Fact]
        public void RolesProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementEditUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.Roles);
            Assert.Empty(viewModel.Roles);
        }

        [Fact]
        public void RolesInsertProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementEditUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.RolesInsert);
            Assert.Empty(viewModel.RolesInsert);
        }

        [Fact]
        public void ClaimsProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementEditUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.Claims);
            Assert.Empty(viewModel.Claims);
        }

        [Fact]
        public void RoleClaimsProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementEditUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.RoleClaims);
            Assert.Empty(viewModel.RoleClaims);
        }

        [Fact]
        public void ClaimsInsertProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementEditUserViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.ClaimsInsert);
            Assert.Empty(viewModel.ClaimsInsert);
        }

        [Fact]
        public void PasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var password = "Password123!";
            var viewModel = new UserManagementEditUserViewModel
            {
                Password = password
            };

            // Act & Assert
            Assert.Equal(password, viewModel.Password);
        }

        [Fact]
        public void ConfirmPasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var confirmPassword = "Password123!";
            var viewModel = new UserManagementEditUserViewModel
            {
                ConfirmPassword = confirmPassword
            };

            // Act & Assert
            Assert.Equal(confirmPassword, viewModel.ConfirmPassword);
        }

        [Fact]
        public void EmailProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var email = "test@example.com";
            var viewModel = new UserManagementEditUserViewModel
            {
                Email = email
            };

            // Act & Assert
            Assert.Equal(email, viewModel.Email);
        }

    }
}
