using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using Entities.DatasetEntities;
using Entities.LegalLandfillsManagementEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.LegalLandfillManagementRepositories
{
    public class LegalLandfillRepository : BaseResultRepository<LegalLandfill, Guid>, ILegalLandfillRepository
    {
        public LegalLandfillRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
