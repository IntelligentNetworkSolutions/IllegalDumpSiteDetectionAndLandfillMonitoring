using System;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;

namespace DAL.Repositories.DatasetRepositories
{
    public class DatasetsRepository : BaseResultRepository<Dataset, Guid>, IDatasetsRepository
    {
        public DatasetsRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
