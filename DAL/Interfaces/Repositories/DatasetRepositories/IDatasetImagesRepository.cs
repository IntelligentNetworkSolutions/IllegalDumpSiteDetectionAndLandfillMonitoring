using System;
using Entities.DatasetEntities;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IDatasetImagesRepository : IBaseResultRepository<DatasetImage, Guid>
    {
    }
}
