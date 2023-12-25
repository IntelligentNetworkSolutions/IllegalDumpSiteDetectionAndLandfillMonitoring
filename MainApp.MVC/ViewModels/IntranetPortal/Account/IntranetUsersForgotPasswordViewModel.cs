using System.ComponentModel.DataAnnotations;

namespace MainApp.MVC.ViewModels.IntranetPortal.Account
{
    public class IntranetUsersForgotPasswordViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Username Or Email")]
        public string UsernameOrEmail { get; set; }
    }
}
