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
    public class Dataset_DatasetClassRepository : IDataset_DatasetClassRepository
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<Dataset_DatasetClassRepository> _logger;
        public Dataset_DatasetClassRepository(ApplicationDbContext db, ILogger<Dataset_DatasetClassRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        #region Read
        #region Get Dataset_DatasetClass
        public async Task<List<Dataset_DatasetClass>> GetAll()
        {
            try
            {
                return await _db.Datasets_DatasetClasses.Include(x => x.DatasetClass).Include(x => x.Dataset).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<Dataset_DatasetClass> GetByDatasetAndDatasetClassId(Guid selectedClassId, Guid datasetId)
        {
            try
            {
                return await _db.Datasets_DatasetClasses.Where(x => x.DatasetId == datasetId && x.DatasetClassId == selectedClassId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<Dataset_DatasetClass>> GetListByDatasetClassId(Guid datasetClassId)
        {
            try
            {
                return await _db.Datasets_DatasetClasses.Where(x => x.DatasetClassId == datasetClassId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<Dataset_DatasetClass>> GetListByDatasetId(Guid datasetId)
        {
            try
            {
                return await _db.Datasets_DatasetClasses.Where(x => x.DatasetId == datasetId).ToListAsync();
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
        public async Task<int> Create(Dataset_DatasetClass dataset_datasetClass)
        {
            try
            {
               _db.Datasets_DatasetClasses.Add(dataset_datasetClass);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<int> AddRange(List<Dataset_DatasetClass> dataset_DatasetClasses)
        {
            try
            {
                _db.Datasets_DatasetClasses.AddRange(dataset_DatasetClasses);
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

        #endregion

        #region Delete
        public async Task<int> Delete(Dataset_DatasetClass dataset_datasetClass)
        {
            try
            {
                _db.Datasets_DatasetClasses.Remove(dataset_datasetClass);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<int> DeleteRange(List<Dataset_DatasetClass> list_dataset_datasetClass)
        {
            try
            {
                _db.Datasets_DatasetClasses.RemoveRange(list_dataset_datasetClass);
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
