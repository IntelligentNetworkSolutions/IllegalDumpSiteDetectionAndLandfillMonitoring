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
    public class DetectionRunsRepository : BaseResultRepository<DetectionRun, Guid>, IDetectionRunsRepository
    {
        public DetectionRunsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
