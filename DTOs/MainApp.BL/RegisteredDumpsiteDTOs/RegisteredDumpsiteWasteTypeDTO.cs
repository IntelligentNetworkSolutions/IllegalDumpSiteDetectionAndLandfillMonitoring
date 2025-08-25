namespace DTOs.MainApp.BL.RegisteredDumpsiteDTOs;

public class RegisteredDumpsiteWasteTypeDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
    public virtual UserDTO? CreatedBy { get; set; }
}
