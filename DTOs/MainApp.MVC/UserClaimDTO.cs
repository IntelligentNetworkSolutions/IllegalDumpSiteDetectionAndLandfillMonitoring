using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.MVC
{
    public class UserClaimDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }
        
    }
}
