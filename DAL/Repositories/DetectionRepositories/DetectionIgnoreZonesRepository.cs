using System;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DetectionRepositories;
using Entities.DetectionEntities;

namespace DAL.Repositories.DetectionRepositories
{
    public class DetectionIgnoreZonesRepository : BaseResultRepository<DetectionIgnoreZone, Guid>, IDetectionIgnoreZonesRepository
    {
        public DetectionIgnoreZonesRepository(ApplicationDbContext dbContext) : base(dbContext) {}
    }
}
