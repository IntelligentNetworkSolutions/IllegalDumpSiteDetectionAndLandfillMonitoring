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
    public class ImageAnnotationsRepository : IImageAnnotationsRepository
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<ImageAnnotationsRepository> _logger;
        public ImageAnnotationsRepository(ApplicationDbContext db, ILogger<ImageAnnotationsRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        #region Read
        #region Get Annotation/s
        public async Task<List<ImageAnnotation>> GetAllImageAnnotations()
        {
            try
            {
                return await _db.ImageAnnotations.ToListAsync();
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

        #endregion

        #region Update

        #endregion

        #region Delete

        #endregion
    }
}
