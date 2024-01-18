using SD;

namespace DTOs.MainApp.BL
{
    public class RoleManagementDTO : RoleDTO
    {
        public ICollection<AuthClaim> Claims { get; set; }

        public List<string?> ClaimsInsert { get; set; }

        public RoleManagementDTO()
        {

            Claims = new List<AuthClaim>();

            ClaimsInsert = new List<string>();
        }
    }
}
