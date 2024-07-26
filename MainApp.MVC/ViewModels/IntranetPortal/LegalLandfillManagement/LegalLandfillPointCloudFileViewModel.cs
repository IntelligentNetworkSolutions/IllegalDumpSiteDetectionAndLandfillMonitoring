using Entities.LegalLandfillsManagementEntites;

namespace MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement
{
    public class LegalLandfillPointCloudFileViewModel
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public DateTime ScanDateTime { get; set; }
        public Guid LegalLandfillId { get; set; }
        public virtual LegalLandfillViewModel? LegalLandfill { get; set; }
    }
}
