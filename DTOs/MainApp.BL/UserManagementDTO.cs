namespace DTOs.MainApp.BL
{
    public class UserManagementDTO : UserDTO
    {
        public List<string> RolesInsert { get; set; }
        public List<string> ClaimsInsert { get; set; }
        public IEnumerable<UserDTO> AllUsersExceptCurrent { get; set; }
        public List<RoleDTO> Roles { get; set; }
        public IEnumerable<UserDTO> AllUsers { get; set; }
        public List<RoleClaimDTO> RoleClaims { get; set; }
        public int PasswordMinLength { get; set; }
        public bool PasswordMustHaveNumbers { get; set; }
        public bool PasswordMustHaveLetters { get; set; }

        public UserManagementDTO()
        {
            Roles = new List<RoleDTO>();
            RolesInsert = new List<string>();
            ClaimsInsert = new List<string>();
            AllUsersExceptCurrent = new List<UserDTO>();
            RoleClaims = new List<RoleClaimDTO>();
            AllUsers = new List<UserDTO>();
        }
    }
}
