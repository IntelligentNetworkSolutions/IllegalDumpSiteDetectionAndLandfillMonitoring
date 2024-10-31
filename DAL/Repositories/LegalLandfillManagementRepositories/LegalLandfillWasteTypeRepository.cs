using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using Entities.LegalLandfillsManagementEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.LegalLandfillManagementRepositories
{
	public class LegalLandfillWasteTypeRepository : BaseResultRepository<LegalLandfillWasteType, Guid>, ILegalLandfillWasteTypeRepository
	{
		public LegalLandfillWasteTypeRepository(ApplicationDbContext db) : base(db) {}
	}
}
