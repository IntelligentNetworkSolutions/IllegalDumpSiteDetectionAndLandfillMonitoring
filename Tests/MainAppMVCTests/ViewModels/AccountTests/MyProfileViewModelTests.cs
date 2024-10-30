using MainApp.MVC.ViewModels.IntranetPortal.Account;

namespace Tests.MainAppMVCTests.ViewModels.AccountTests
{
    public class MyProfileViewModelTests
    {
        [Fact]
        public void MyProfileUserIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var userId = "user123";
            var viewModel = new MyProfileViewModel { UserId = userId };

            // Act & Assert
            Assert.Equal(userId, viewModel.UserId);
        }

        [Fact]
        public void CurrentPasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var currentPassword = "current_password";
            var viewModel = new MyProfileViewModel { CurrentPassword = currentPassword };

            // Act & Assert
            Assert.Equal(currentPassword, viewModel.CurrentPassword);
        }

        [Fact]
        public void MyProfileNewPasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var newPassword = "new_password";
            var viewModel = new MyProfileViewModel { NewPassword = newPassword };

            // Act & Assert
            Assert.Equal(newPassword, viewModel.NewPassword);
        }

        [Fact]
        public void MyProfileConfirmNewPasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var confirmNewPassword = "new_password";
            var viewModel = new MyProfileViewModel { ConfirmNewPassword = confirmNewPassword };

            // Act & Assert
            Assert.Equal(confirmNewPassword, viewModel.ConfirmNewPassword);
        }
    }
}
