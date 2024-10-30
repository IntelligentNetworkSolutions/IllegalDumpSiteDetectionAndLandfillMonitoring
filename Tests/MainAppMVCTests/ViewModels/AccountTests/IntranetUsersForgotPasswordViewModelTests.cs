using MainApp.MVC.ViewModels.IntranetPortal.Account;

namespace Tests.MainAppMVCTests.ViewModels.AccountTests
{
    public class IntranetUsersForgotPasswordViewModelTests
    {
        [Fact]
        public void UsernameOrEmailProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var usernameOrEmail = "user@example.com";
            var viewModel = new IntranetUsersForgotPasswordViewModel { UsernameOrEmail = usernameOrEmail };

            // Act & Assert
            Assert.Equal(usernameOrEmail, viewModel.UsernameOrEmail);
        }
    }
}
