using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DAL.Repositories.DatasetRepositories
{
    public class ImageAnnotationsRepository : BaseResultRepository<ImageAnnotation,Guid>, IImageAnnotationsRepository
    {
        private readonly ApplicationDbContext _db;        
        public ImageAnnotationsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;            
        }
        #region Read
        #region Get Annotation/s
        #endregion
        #endregion

        #region Create

        #endregion

        #region Update

        public async Task<bool> BulkUpdateImageAnnotations(List<ImageAnnotation> insertList, List<ImageAnnotation> updateList, List<ImageAnnotation> deleteList)
        {
            IDbContextTransaction? transaction = null;
            try
            {
                transaction = await _db.Database.BeginTransactionAsync();

                //foreach (var claim in userClaims)
                if (deleteList != null)
                    _db.RemoveRange(deleteList);

                //foreach (var role in userRoles)
                if (updateList != null)
                    _db.UpdateRange(updateList);

                if (insertList != null)
                    _db.AddRange(insertList);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                if (transaction is not null)
                    await transaction.RollbackAsync();                
                throw;
            }
        }

        #endregion

        #region Delete

        #endregion
    }
}
