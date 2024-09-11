using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using Entities.LegalLandfillsManagementEntites;
using System;

namespace DAL.Repositories.LegalLandfillManagementRepositories
{
    public class LegalLandfillWasteImportRepository : BaseResultRepository<LegalLandfillWasteImport, Guid>, ILegalLandfillWasteImportRepository
    {
        public LegalLandfillWasteImportRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
