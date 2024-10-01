using DTOs.MainApp.BL;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;


namespace MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement
{
    public class LegalLandfillWasteImportViewModel
    {
        public Guid Id { get; set; }
        public DateTime ImportedOn { get; set; }
        public int ImportExportStatus { get; set; }
        public double? Capacity { get; set; }
        public double? Weight { get; set; }
        public bool IsEnabled { get; set; }
        public string? CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public UserDTO? CreatedBy { get; set; }

        public Guid? LegalLandfillTruckId { get; set; }
        public virtual LegalLandfillTruckDTO? LegalLandfillTruck { get; set; }
        public virtual List<LegalLandfillTruckDTO>? LegalLandfillTrucks { get; set; }

        public Guid LegalLandfillId { get; set; }
        public virtual LegalLandfillDTO? LegalLandfill { get; set; }
        public virtual List<LegalLandfillDTO>? LegalLandfills { get; set; }

        public Guid LegalLandfillWasteTypeId { get; set; }
        public virtual LegalLandfillWasteTypeDTO? LegalLandfillWasteType { get; set; }
        public virtual List<LegalLandfillWasteTypeDTO>? LegalLandfillWasteTypes { get; set; }

    }
}
