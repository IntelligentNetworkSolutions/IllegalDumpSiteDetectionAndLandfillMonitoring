using MainApp.MVC.Helpers;

namespace Tests.MainAppMVCTests.Helpers
{
    public class PasswordValidationHelperTests
    {
        private readonly PasswordValidationHelper _passwordValidationHelper;

        public PasswordValidationHelperTests()
        {
            _passwordValidationHelper = new PasswordValidationHelper();
        }

        [Theory]
        [InlineData("StrongPassword1!", true)]
        [InlineData("weakpass", false)]
        [InlineData("1234567890", false)]
        [InlineData("Short1!", false)]
        [InlineData("Valid1Password@", true)]
        [InlineData("NoSpecialChar123", false)]
        public void ValidateStrongPassword_ShouldReturnExpectedResult(string password, bool expected)
        {
            var result = _passwordValidationHelper.ValidateStrongPassword(password);
            Assert.Equal(expected, result);
        }
    }
}
