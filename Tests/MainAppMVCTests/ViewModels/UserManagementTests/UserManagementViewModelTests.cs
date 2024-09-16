using DTOs.MainApp.BL;
using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.UserManagementTests
{
    public class UserManagementViewModelTests
    {
        [Fact]
        public void RolesProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.Roles);
            Assert.Empty(viewModel.Roles);
        }

        [Fact]
        public void UsersProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.Users);
            Assert.Empty(viewModel.Users);
        }

        [Fact]
        public void RolesProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var role = new RoleDTO { Id = "1", Name = "Admin" };
            var viewModel = new UserManagementViewModel
            {
                Roles = new List<RoleDTO> { role }
            };

            // Act & Assert
            Assert.Single(viewModel.Roles);
            Assert.Equal(role, viewModel.Roles[0]);
        }

        [Fact]
        public void UsersProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var user = new UserManagementUserViewModel { Id = "user1", UserName = "testuser" };
            var viewModel = new UserManagementViewModel
            {
                Users = new List<UserManagementUserViewModel> { user }
            };

            // Act & Assert
            Assert.Single(viewModel.Users);
            Assert.Equal(user, viewModel.Users[0]);
        }
    }
}
