using System;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.TrainingRepositories;
using Entities.TrainingEntities;

namespace DAL.Repositories.TrainingRepositories
{
    public class TrainedModelStatisticsRepository : BaseResultRepository<TrainedModelStatistics, Guid>, ITrainedModelStatisticsRepository
    {
        public TrainedModelStatisticsRepository(ApplicationDbContext dbContext) : base(dbContext) {}
    }
}
