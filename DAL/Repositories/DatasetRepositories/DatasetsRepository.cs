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
        private readonly ApplicationDbContext _db;
        private static ILogger<DatasetsRepository> _logger;
        public DatasetsRepository(ApplicationDbContext db, ILogger<DatasetsRepository> logger) : base(db)
        {
            _db = db;
            _logger = logger;
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
