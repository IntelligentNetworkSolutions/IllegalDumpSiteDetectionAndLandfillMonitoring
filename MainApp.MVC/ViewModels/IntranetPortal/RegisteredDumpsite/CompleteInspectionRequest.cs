namespace MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite;

public class CompleteInspectionRequest
{
    public Guid InspectionId { get; set; }
    public string Findings { get; set; }
    public string? Recommendations { get; set; }
}
