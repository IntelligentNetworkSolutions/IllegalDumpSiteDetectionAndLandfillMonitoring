using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DetectionRepositories;
using Entities.DetectionEntities;
using Microsoft.Extensions.Logging;
using System;

namespace DAL.Repositories.DetectionRepositories
{
    public class DetectionInputImageRepository : BaseResultRepository<DetectionInputImage, Guid>, IDetectionInputImageRepository
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<DetectionInputImageRepository> _logger;

        public DetectionInputImageRepository(ApplicationDbContext db, ILogger<DetectionInputImageRepository> logger) : base(db)
        {
            _db = db;
            _logger = logger;
        }
    }
}
