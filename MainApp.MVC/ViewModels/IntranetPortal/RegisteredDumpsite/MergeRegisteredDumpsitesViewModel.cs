namespace MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite
{
    public class MergeRegisteredDumpsitesViewModel
    {
        public CreateRegisteredDumpsiteViewModel NewDumpsiteData { get; set; }
        public List<Guid> ExistingDumpsiteIds { get; set; }
    }
}
