using System;
using Entities.DatasetEntities;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IDatasetsRepository : IBaseResultRepository<Dataset, Guid>
    {
    }
}
