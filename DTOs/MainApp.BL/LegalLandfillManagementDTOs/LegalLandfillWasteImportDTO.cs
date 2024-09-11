using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.LegalLandfillManagementDTOs
{
	public class LegalLandfillWasteImportDTO
	{
		public Guid Id { get; set; }
		public DateTime ImportedOn { get; set; }
		public int ImportExportStatus { get; set; }
		public double? Capacity { get; set; }
		public double? Weight { get; set; }

		public string? CreatedById { get; set; }
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
		public UserDTO? CreatedBy { get; set; }

		public Guid LegalLandfillTruckId { get; set; }
		public virtual LegalLandfillTruckDTO? LegalLandfillTruck { get; set; }

		public Guid LegalLandfillId { get; set; }
		public virtual LegalLandfillDTO? LegalLandfill { get; set; }

		public Guid LegalLandfillWasteTypeId { get; set; }
		public virtual LegalLandfillWasteTypeDTO? LegalLandfillWasteType { get; set; }
	}
}
