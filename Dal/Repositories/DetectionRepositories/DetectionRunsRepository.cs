using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DetectionRepositories;
using DAL.Repositories.MapConfigurationRepositories;
using Entities.DetectionEntities;
using Entities.MapConfigurationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        #region Read
        #region Get DetectionRun/s
        public async Task<List<DetectionRun>> GetDetectionRunsWithClasses()
        {
            try
            {
                var list = await _db.DetectionRuns.Include(x => x.CreatedBy).Include(x => x.DetectedDumpSites).ThenInclude(x => x.DatasetClass).ToListAsync();
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
        #endregion
        #endregion

        #region Create
        #endregion

        #region Update
        #endregion

        #region Delete

        #endregion
    }
}
