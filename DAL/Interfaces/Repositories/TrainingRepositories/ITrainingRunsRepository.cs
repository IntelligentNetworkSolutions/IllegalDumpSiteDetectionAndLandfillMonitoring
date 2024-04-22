using System;
using Entities.TrainingEntities;

namespace DAL.Interfaces.Repositories.TrainingRepositories
{
    public interface ITrainingRunsRepository : IBaseResultRepository<TrainingRun, Guid>
    {
    }
}
