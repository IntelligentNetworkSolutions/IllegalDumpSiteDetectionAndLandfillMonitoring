using MainApp.MVC.ViewModels.IntranetPortal.Account;

namespace Tests.MainAppMVCTests.ViewModels.AccountTests
{
    public class LoginViewModelTests
    {
        [Fact]
        public void UsernameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var username = "user1";
            var viewModel = new LoginViewModel { Username = username };

            // Act & Assert
            Assert.Equal(username, viewModel.Username);
        }

        [Fact]
        public void PasswordProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var password = "password123";
            var viewModel = new LoginViewModel { Password = password };

            // Act & Assert
            Assert.Equal(password, viewModel.Password);
        }

        [Fact]
        public void RememberMeProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var rememberMe = true;
            var viewModel = new LoginViewModel { RememberMe = rememberMe };

            // Act & Assert
            Assert.True(viewModel.RememberMe);
        }

    }
}
