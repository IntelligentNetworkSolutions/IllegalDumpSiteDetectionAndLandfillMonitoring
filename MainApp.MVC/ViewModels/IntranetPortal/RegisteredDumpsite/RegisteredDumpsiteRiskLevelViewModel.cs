using DTOs.MainApp.BL;

namespace MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite;

public class RegisteredDumpsiteRiskLevelViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
    public virtual UserDTO? CreatedBy { get; set; }
}
