using DTOs.MainApp.BL.LegalLandfillManagementDTOs;

namespace MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement
{
    public class LegalLandfillViewModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<LegalLandfillPointCloudFileViewModel>? LegalLandfillPointCloudFiles { get; set; }

        public LegalLandfillViewModel()
        {
            LegalLandfillPointCloudFiles = new List<LegalLandfillPointCloudFileViewModel>();
        }
    }
}
