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
    public class DatasetImagesRepository :  IDatasetImagesRepository
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<DatasetImagesRepository> _logger;
        public DatasetImagesRepository(ApplicationDbContext db, ILogger<DatasetImagesRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        #region Read
        #region Get DatasetImage/s
        public async Task<List<DatasetImage>> GetAllImages()
        {
            try
            {
                return await _db.DatasetImages.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
       
        public async Task<List<DatasetImage>> GetImagesForDataset(Guid datasetId)
        {
            try
            {
                return await _db.DatasetImages.Where(x => x.DatasetId == datasetId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<DatasetImage> GetDatasetImageById(Guid datasetImageId)
        {
            try
            {
                return await _db.DatasetImages.Where(x => x.Id == datasetImageId).FirstOrDefaultAsync();
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
        public async Task<Guid> CreateDatasetImage(DatasetImage datasetImage)
        {
            try
            {
                _db.DatasetImages.Add(datasetImage);
                await _db.SaveChangesAsync();
                return datasetImage.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion

        #region Update
        public async Task<int> EditDatasetImage(DatasetImage datasetImage)
        {
            try
            {
                _db.DatasetImages.Update(datasetImage);
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
        public async Task<int> DeleteImage(DatasetImage datasetImage)
        {
            try
            {                
                _db.DatasetImages.Remove(datasetImage);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<int> DeleteRange(List<DatasetImage> datasetImagesList)
        {
            try
            {
                _db.DatasetImages.RemoveRange(datasetImagesList);
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
