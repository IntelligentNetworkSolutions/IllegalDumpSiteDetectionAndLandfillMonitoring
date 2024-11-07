using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.TrainingRepositories;
using Entities.TrainingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.TrainingRepositories
{
    public class TrainingRunTrainParamsRepository : BaseResultRepository<TrainingRunTrainParams, Guid>, ITrainingRunTrainParamsRepository
    {
        public TrainingRunTrainParamsRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
