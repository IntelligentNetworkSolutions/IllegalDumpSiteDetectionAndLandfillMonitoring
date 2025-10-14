namespace MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite;

public class CompleteInspectionViewModel
{
    public Guid InspectionId { get; set; }
    public string Findings { get; set; } = string.Empty;
    public string? Recommendations { get; set; }
    public string? Notes { get; set; }
}
