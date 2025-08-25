namespace MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite
{
    public class CreateRegisteredDumpsiteViewModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string EnteredZonePolygon { get; set; }
        public bool IsEnabled { get; set; }
    }
}
