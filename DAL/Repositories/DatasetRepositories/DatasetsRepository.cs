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
    public class DatasetsRepository : IDatasetsRepository
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<DatasetsRepository> _logger;
        public DatasetsRepository(ApplicationDbContext db, ILogger<DatasetsRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        #region Read
        #region Get Dataset/s
        public async Task<List<Dataset>> GetAllDatasets()
        {
            try
            {
                return await _db.Datasets.Include(x => x.CreatedBy).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
       
        public async Task<Dataset> GetDatasetById(Guid datasetId)
        {
            try
            {
                return await _db.Datasets.Where(x => x.Id == datasetId)
                                .Include(x => x.CreatedBy)
                                .Include(x => x.UpdatedBy)
                                .Include(x => x.ParentDataset)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public int GetNumberOfChildrenDatasetsByDatasetId(Guid datasetId)
        {
            try
            {
                return _db.Datasets.Where(x => x.ParentDatasetId == datasetId).Count();
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
        public async Task<Dataset> CreateDataset(Dataset dataset)
        {
            try
            {
                _db.Datasets.Add(dataset);
                await _db.SaveChangesAsync();
                return dataset;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion

        #region Update
        public async Task<int> UpdateDataset(Dataset dataset)
        {
            try
            {
                _db.Datasets.Update(dataset);
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
        public async Task<int> DeleteDataset(Dataset dataset)
        {
            try
            {
                dataset.ParentDatasetId = null;
                _db.Datasets.Remove(dataset);
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
