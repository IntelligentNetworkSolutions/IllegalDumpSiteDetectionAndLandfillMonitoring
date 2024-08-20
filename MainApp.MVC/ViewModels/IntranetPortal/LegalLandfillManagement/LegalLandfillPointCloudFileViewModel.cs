using Entities.LegalLandfillsManagementEntites;

namespace MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement
{
    public class LegalLandfillPointCloudFileViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime ScanDateTime { get; set; }
        public IFormFile? FileUpload { get; set; }
        public Guid OldLegalLandfillId { get; set; }
        public Guid LegalLandfillId { get; set; }       
        public virtual LegalLandfillViewModel? LegalLandfill { get; set; }
    }
}
