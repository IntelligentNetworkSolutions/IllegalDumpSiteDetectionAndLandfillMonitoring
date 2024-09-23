using Entities.Intefaces;
using System.ComponentModel.DataAnnotations;

namespace Entities.LegalLandfillsManagementEntites
{
    public class LegalLandfillWasteImport : BaseEntity<Guid>, ICreatedByUser
    {
        public DateTime ImportedOn { get; set; }

        [Range(-1, 1)]
        public int ImportExportStatus { get; set; }
        public double? Capacity { get; set; }
        public double? Weight { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }

        public Guid? LegalLandfillTruckId { get; set; }
        public virtual LegalLandfillTruck? LegalLandfillTruck { get; set; }

        public Guid LegalLandfillId { get; set; }
        public virtual LegalLandfill LegalLandfill { get; set; }

        public Guid LegalLandfillWasteTypeId { get; set; }
        public virtual LegalLandfillWasteType LegalLandfillWasteType { get; set; }
    }
}
