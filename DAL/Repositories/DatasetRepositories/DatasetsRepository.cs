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
    public class DatasetsRepository : BaseResultRepository<Dataset, Guid>, IDatasetsRepository
    {
        public DatasetsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
