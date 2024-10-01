using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.LegalLandfillManagementDTOs
{
	public class LegalLandfillWasteTypeDTO
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }

	}
}
