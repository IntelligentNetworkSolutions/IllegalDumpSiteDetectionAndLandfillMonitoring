using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;
using System;

namespace DAL.Repositories.DatasetRepositories
{
    public class Dataset_DatasetClassRepository : BaseResultRepository<Dataset_DatasetClass, Guid>, IDataset_DatasetClassRepository
    {
        public Dataset_DatasetClassRepository(ApplicationDbContext db) : base(db) {}
    }
}
