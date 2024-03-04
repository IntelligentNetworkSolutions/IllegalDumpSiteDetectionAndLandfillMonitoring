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
    public class DatasetClassesRepository : IDatasetClassesRepository
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<DatasetClassesRepository> _logger;
        public DatasetClassesRepository(ApplicationDbContext db, ILogger<DatasetClassesRepository> logger)
        {
            _db = db;
            _logger = logger;
        }


        #region Read
        #region Get DatasetClass/es        
        public async Task<List<DatasetClass>> GetAllDatasetClasses()
        {
            try
            {
                return await _db.DatasetClasses.Include(x => x.CreatedBy).Include(x => x.ParentClass).Include(x => x.Datasets).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<DatasetClass> GetDatasetClassById(Guid classId)
        {
            try
            {
                return await _db.DatasetClasses.Where(x => x.Id == classId).Include(x => x.CreatedBy).Include(x => x.ParentClass).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion
        #endregion

        #region Create
        public async Task<int> AddClass(DatasetClass newClass)
        {
            try
            {
                _db.DatasetClasses.Add(newClass);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion

        #region Update
        public async Task<int> EditClass(DatasetClass datasetClass)
        {
            try
            {
                _db.DatasetClasses.Update(datasetClass);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion

        #region Delete
        public async Task<int> DeleteClass(DatasetClass datasetClass)
        {
            try
            {
                _db.DatasetClasses.Remove(datasetClass);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion
    }
}
