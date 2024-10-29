using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Repositories.DatasetRepositories
{
    public class ImageAnnotationsRepository : BaseResultRepository<ImageAnnotation,Guid>, IImageAnnotationsRepository
    {
        private readonly ApplicationDbContext _db;        
        public ImageAnnotationsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;            
        }

        #region Update
        public async Task<bool> BulkUpdateImageAnnotations(List<ImageAnnotation> insertList, List<ImageAnnotation> updateList, List<ImageAnnotation> deleteList, IDbContextTransaction? dbContextTransaction = null)
        {
            IDbContextTransaction? transaction = dbContextTransaction;
            try
            {
                if(dbContextTransaction is null)
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

                if(dbContextTransaction is null)
                    await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                if (transaction is not null && dbContextTransaction is null)
                    await transaction.RollbackAsync();                
                throw;
            }
        }
        #endregion
    }
}
