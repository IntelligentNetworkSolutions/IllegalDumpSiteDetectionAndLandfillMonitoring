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
    public class DatasetImagesRepository : BaseResultRepository<DatasetImage, Guid>, IDatasetImagesRepository
    {
        public DatasetImagesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
