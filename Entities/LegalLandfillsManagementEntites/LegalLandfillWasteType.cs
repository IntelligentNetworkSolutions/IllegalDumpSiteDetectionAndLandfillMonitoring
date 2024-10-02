using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LegalLandfillsManagementEntites
{
	public class LegalLandfillWasteType : BaseEntity<Guid>
	{
		public string Name { get; set; }
		public string? Description { get; set; }

	}
}
