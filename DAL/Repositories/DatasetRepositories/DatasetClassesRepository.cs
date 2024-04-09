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
    public class DatasetClassesRepository : BaseResultRepository<DatasetClass, Guid>, IDatasetClassesRepository
    {
        public DatasetClassesRepository(ApplicationDbContext db) : base(db)
        {
        }


        #region Read
        #region Get DatasetClass/es        
        #endregion
        #endregion

        #region Create
        #endregion

        #region Update
        #endregion

        #region Delete
        #endregion
    }
}
