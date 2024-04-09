using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.DatasetRepositories
{
    public class Dataset_DatasetClassRepository : BaseResultRepository<Dataset_DatasetClass, Guid>, IDataset_DatasetClassRepository
    {
        public Dataset_DatasetClassRepository(ApplicationDbContext db) : base(db)
        {
        }
        #region Read
        #region Get Dataset_DatasetClass  
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
