using System;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;

namespace DAL.Repositories.DatasetRepositories
{
    public class DatasetClassesRepository : BaseResultRepository<DatasetClass, Guid>, IDatasetClassesRepository
    {
        public DatasetClassesRepository(ApplicationDbContext db) : base(db) {}
    }
}
