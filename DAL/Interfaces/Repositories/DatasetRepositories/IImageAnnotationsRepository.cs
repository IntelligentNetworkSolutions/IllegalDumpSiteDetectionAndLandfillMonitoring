using System;
using Entities.DatasetEntities;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IImageAnnotationsRepository : IBaseResultRepository<ImageAnnotation, Guid>
    {
    }
}
