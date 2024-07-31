using System;
using Entities;
using System.Threading.Tasks;
using Entities.DatasetEntities;
using System.Collections.Generic;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IDatasetsRepository : IBaseResultRepository<Dataset, Guid>
    {
    }
}
