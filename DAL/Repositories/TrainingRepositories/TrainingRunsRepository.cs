using System;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.TrainingRepositories;
using Entities.TrainingEntities;

namespace DAL.Repositories.TrainingRepositories
{
    public class TrainingRunsRepository : BaseResultRepository<TrainingRun, Guid>, ITrainingRunsRepository
    {
        public TrainingRunsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
