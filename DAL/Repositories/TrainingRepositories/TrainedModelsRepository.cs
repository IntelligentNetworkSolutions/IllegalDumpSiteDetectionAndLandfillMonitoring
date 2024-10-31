using System;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.TrainingRepositories;
using Entities.TrainingEntities;

namespace DAL.Repositories.TrainingRepositories
{
    public class TrainedModelsRepository : BaseResultRepository<TrainedModel, Guid>, ITrainedModelsRepository
    {
        public TrainedModelsRepository(ApplicationDbContext dbContext) : base(dbContext) {}
    }
}
