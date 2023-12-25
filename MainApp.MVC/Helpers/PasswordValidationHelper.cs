using System.Text.RegularExpressions;

namespace MainApp.MVC.Helpers
{
    public class PasswordValidationHelper
    {
        const int passwordMinLength = 10;
        const bool passwordMustHaveLetters = true;
        const bool passwordMustHaveNumbers = true;
        const bool passwordMustHaveSpecialChar = true;

        public PasswordValidationHelper()
        {

        }

        // validate password: validate strong password with at least 1 letter, 1 number and 1 special char and its length is at least 10
        public bool ValidateStrongPassword(string password)
        {
            Regex validateGuidRegex = new Regex("^(?=.*?[a-zA-Z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{" + passwordMinLength + ",}$");

            if (password.Length < passwordMinLength || (!validateGuidRegex.IsMatch(password)))
            {
                return false;
            }

            else
            {
                return true;
            }
        }

    }
}
