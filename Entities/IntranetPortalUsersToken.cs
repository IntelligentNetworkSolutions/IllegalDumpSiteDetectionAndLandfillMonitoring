using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class IntranetPortalUsersToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("ApplicationUsers")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUsers { get; set; }
        public string Token { get; set; }
        public bool isTokenUsed { get; set; }
    }
}
