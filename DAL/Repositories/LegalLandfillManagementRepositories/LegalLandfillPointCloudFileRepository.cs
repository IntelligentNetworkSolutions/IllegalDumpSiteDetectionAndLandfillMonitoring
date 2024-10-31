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
    public class LegalLandfillPointCloudFileRepository : BaseResultRepository<LegalLandfillPointCloudFile, Guid>, ILegalLandfillPointCloudFileRepository
    {
        public LegalLandfillPointCloudFileRepository(ApplicationDbContext db) : base(db) {}
    }
}
