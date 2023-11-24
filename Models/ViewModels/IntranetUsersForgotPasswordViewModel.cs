using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Models.ViewModels
{
    public class IntranetUsersForgotPasswordViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Username Or Email")]
        public string UsernameOrEmail { get; set; }
    }
}
