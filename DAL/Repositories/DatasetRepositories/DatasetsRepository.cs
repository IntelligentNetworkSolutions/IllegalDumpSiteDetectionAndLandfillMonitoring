using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.Repositories.DatasetRepositories
{
    public class DatasetsRepository : BaseResultRepository<Dataset, Guid>, IDatasetsRepository
    {
        public DatasetsRepository(ApplicationDbContext db) : base(db)
        {
        }
        #region Read
        #region Get Dataset/s    
        #endregion
        #endregion

        #region Create

        #endregion

        #region Update

        #endregion
    }
}
