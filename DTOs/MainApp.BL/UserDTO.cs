namespace DTOs.MainApp.BL
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsActive { get; set; } = false;
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }
        public UserDTO()
        {

        }

        public void TrimProperies()
        {
            UserName = UserName?.Trim();
            FirstName = FirstName?.Trim();
            LastName = LastName?.Trim();
            NormalizedUserName = NormalizedUserName?.Trim();
            Email = Email?.Trim();
            Password = Password?.Trim();
            ConfirmPassword = ConfirmPassword?.Trim();
        }
    }
}
