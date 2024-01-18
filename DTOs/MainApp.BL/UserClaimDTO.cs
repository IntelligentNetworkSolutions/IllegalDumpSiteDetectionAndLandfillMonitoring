namespace DTOs.MainApp.BL
{
    public class UserClaimDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }
        
    }
}
