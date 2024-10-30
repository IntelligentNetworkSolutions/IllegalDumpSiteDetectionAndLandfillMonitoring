using System;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;

namespace DAL.Repositories.DatasetRepositories
{
    public class DatasetImagesRepository : BaseResultRepository<DatasetImage, Guid>, IDatasetImagesRepository
    {
        public DatasetImagesRepository(ApplicationDbContext db) : base(db) {}
    }
}
