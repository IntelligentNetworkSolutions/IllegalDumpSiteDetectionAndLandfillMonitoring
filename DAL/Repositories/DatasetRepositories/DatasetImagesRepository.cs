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
    public class DatasetImagesRepository : BaseResultRepository<DatasetImage, Guid>, IDatasetImagesRepository
    {
        public DatasetImagesRepository(ApplicationDbContext db) : base(db)
        {
        }
        #region Read
        #region Get DatasetImage/s

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
