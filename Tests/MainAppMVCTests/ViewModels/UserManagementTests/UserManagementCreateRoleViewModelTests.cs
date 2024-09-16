using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.ViewModels.UserManagementTests
{
    public class UserManagementCreateRoleViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = "role-id";
            var viewModel = new UserManagementCreateRoleViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void NameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var name = "AdminRole";
            var viewModel = new UserManagementCreateRoleViewModel
            {
                Name = name
            };

            // Act & Assert
            Assert.Equal(name, viewModel.Name);
        }

        [Fact]
        public void NormalizedNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var normalizedName = "ADMINROLE";
            var viewModel = new UserManagementCreateRoleViewModel
            {
                NormalizedName = normalizedName
            };

            // Act & Assert
            Assert.Equal(normalizedName, viewModel.NormalizedName);
        }

        [Fact]
        public void ClaimsProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementCreateRoleViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.Claims);
            Assert.Empty(viewModel.Claims);
        }

        [Fact]
        public void ClaimsInsertProperty_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var viewModel = new UserManagementCreateRoleViewModel();

            // Act & Assert
            Assert.NotNull(viewModel.ClaimsInsert);
            Assert.Empty(viewModel.ClaimsInsert);
        }

        [Fact]
        public void ClaimsInsertProperty_ShouldAllowAddingItems()
        {
            // Arrange
            var viewModel = new UserManagementCreateRoleViewModel();
            var claim = "claim-value";

            // Act
            viewModel.ClaimsInsert.Add(claim);

            // Assert
            Assert.Single(viewModel.ClaimsInsert);
            Assert.Contains(claim, viewModel.ClaimsInsert);
        }                     
       
    }
}
