using System.ComponentModel.DataAnnotations;

namespace MainApp.MVC.ViewModels.IntranetPortal.Account
{
    public class IntranetUsersResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }

        [DataType(DataType.Password)]
        [MinLength(10, ErrorMessage = "New password field must be at least 10 characters long")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Cofirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }

        public int PasswordMinLength { get; set; }

        public bool PasswordMustHaveNumbers { get; set; }

        public bool PasswordMustHaveLetters { get; set; }
        public bool PasswordMustHaveSpecialChar { get; set; }
    }
}
