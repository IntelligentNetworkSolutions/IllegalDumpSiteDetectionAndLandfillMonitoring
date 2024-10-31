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
	public class LegalLandfillTruckRepository : BaseResultRepository<LegalLandfillTruck, Guid>, ILegalLandfillTruckRepository
	{
		public LegalLandfillTruckRepository(ApplicationDbContext db) : base(db) {}
	}
}
