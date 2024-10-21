using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DetectionRepositories;
using Entities.DetectionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.DetectionRepositories
{
    public class DetectionRunsRepository : BaseResultRepository<DetectionRun, Guid>, IDetectionRunsRepository
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<DetectionRunsRepository> _logger;
        public DetectionRunsRepository(ApplicationDbContext db, ILogger<DetectionRunsRepository> logger) : base(db)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<DetectionRun>> GetDetectionRunsWithClasses()
        {
            try
            {
                var list = await _db.DetectionRuns.Include(x => x.CreatedBy).Include(x => x.DetectionInputImage).Include(x => x.DetectedDumpSites).ThenInclude(x => x.DatasetClass).ToListAsync();
                if (list == null)
                {
                    return new List<DetectionRun>();
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        public async Task<List<DetectionRun>> GetSelectedDetectionRunsWithClasses(List<Guid> selectedDetectionRunsIds)
        {
            try
            {
                var list = await _db.DetectionRuns.Where(x => selectedDetectionRunsIds.Contains(x.Id)).Include(x => x.CreatedBy).Include(x => x.DetectedDumpSites).ThenInclude(x => x.DatasetClass).ToListAsync();
                if (list == null)
                {
                    return new List<DetectionRun>();
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
    }
}
