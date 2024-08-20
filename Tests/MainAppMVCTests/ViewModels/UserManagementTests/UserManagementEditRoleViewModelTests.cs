using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.UserManagementTests
{
    public class UserManagementEditRoleViewModelTests
    {
        [Fact]
        public void ClaimsProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementEditRoleViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.Claims);
            Assert.Empty(viewModel.Claims);
        }

        [Fact]
        public void ClaimsInsertProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementEditRoleViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.ClaimsInsert);
            Assert.Empty(viewModel.ClaimsInsert);
        }

        [Fact]
        public void NameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var roleName = "Admin";
            var viewModel = new UserManagementEditRoleViewModel
            {
                Name = roleName
            };

            // Act & Assert
            Assert.Equal(roleName, viewModel.Name);
        }

        [Fact]
        public void NormalizedNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var normalizedRoleName = "ADMIN";
            var viewModel = new UserManagementEditRoleViewModel
            {
                NormalizedName = normalizedRoleName
            };

            // Act & Assert
            Assert.Equal(normalizedRoleName, viewModel.NormalizedName);
        }
              
    }
}
