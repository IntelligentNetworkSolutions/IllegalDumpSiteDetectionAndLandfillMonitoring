using System.ComponentModel.DataAnnotations;

namespace MainApp.MVC.ViewModels.IntranetPortal.Account
{
    public class MyProfileViewModel
    {
        public string UserId { get; set; }
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
        public int PasswordMinLength { get; set; }
        public bool PasswordMustHaveLetters { get; set; }
        public bool PasswordMustHaveNumbers { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? PreferredLanguage { get; set; }
    }
}
