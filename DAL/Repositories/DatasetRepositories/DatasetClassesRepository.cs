using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;

namespace DAL.Repositories.DatasetRepositories
{
    public class DatasetClassesRepository : BaseResultRepository<DatasetClass, Guid>, IDatasetClassesRepository
    {
        public DatasetClassesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
