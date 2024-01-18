namespace DTOs.MainApp.BL
{
    public class RoleClaimDTO
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }
    }
}
