using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DetectionRepositories;
using Entities.DetectionEntities;

namespace DAL.Repositories.DetectionRepositories
{
    public class DetectedDumpSitesRepository : BaseResultRepository<DetectedDumpSite, Guid>, IDetectedDumpSitesRepository
    {
        public DetectedDumpSitesRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
