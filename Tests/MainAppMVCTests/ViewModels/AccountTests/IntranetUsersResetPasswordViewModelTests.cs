using MainApp.MVC.ViewModels.IntranetPortal.Account;

namespace Tests.MainAppMVCTests.ViewModels.AccountTests
{
    public class IntranetUsersResetPasswordViewModelTests
    {
        [Fact]
        public void UserIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var userId = "user123";
            var viewModel = new IntranetUsersResetPasswordViewModel { UserId = userId };

            // Act & Assert
            Assert.Equal(userId, viewModel.UserId);
        }

        [Fact]
        public void TokenProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var token = "reset_token";
            var viewModel = new IntranetUsersResetPasswordViewModel { Token = token };

            // Act & Assert
            Assert.Equal(token, viewModel.Token);
        }

        [Fact]
        public void NewPasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var newPassword = "new_password";
            var viewModel = new IntranetUsersResetPasswordViewModel { NewPassword = newPassword };

            // Act & Assert
            Assert.Equal(newPassword, viewModel.NewPassword);
        }

        [Fact]
        public void ConfirmNewPasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var confirmNewPassword = "new_password";
            var viewModel = new IntranetUsersResetPasswordViewModel { ConfirmNewPassword = confirmNewPassword };

            // Act & Assert
            Assert.Equal(confirmNewPassword, viewModel.ConfirmNewPassword);
        }

        [Fact]
        public void PasswordMinLengthProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var passwordMinLength = 10;
            var viewModel = new IntranetUsersResetPasswordViewModel { PasswordMinLength = passwordMinLength };

            // Act & Assert
            Assert.Equal(passwordMinLength, viewModel.PasswordMinLength);
        }

        [Fact]
        public void PasswordMustHaveNumbersProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var mustHaveNumbers = true;
            var viewModel = new IntranetUsersResetPasswordViewModel { PasswordMustHaveNumbers = mustHaveNumbers };

            // Act & Assert
            Assert.True(viewModel.PasswordMustHaveNumbers);
        }

        [Fact]
        public void PasswordMustHaveLettersProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var mustHaveLetters = true;
            var viewModel = new IntranetUsersResetPasswordViewModel { PasswordMustHaveLetters = mustHaveLetters };

            // Act & Assert
            Assert.True(viewModel.PasswordMustHaveLetters);
        }

        [Fact]
        public void PasswordMustHaveSpecialCharProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var mustHaveSpecialChar = true;
            var viewModel = new IntranetUsersResetPasswordViewModel { PasswordMustHaveSpecialChar = mustHaveSpecialChar };

            // Act & Assert
            Assert.True(viewModel.PasswordMustHaveSpecialChar);
        }
    }
}
