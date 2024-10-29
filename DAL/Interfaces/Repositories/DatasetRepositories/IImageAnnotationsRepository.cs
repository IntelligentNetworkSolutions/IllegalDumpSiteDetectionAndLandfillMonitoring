using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IImageAnnotationsRepository : IBaseResultRepository<ImageAnnotation, Guid>
    {
        #region Read
        #region Get Annotation/s
        #endregion
        #endregion

        #region Create
        #endregion

        #region Update
        public Task<bool> BulkUpdateImageAnnotations(List<ImageAnnotation> insertList, List<ImageAnnotation> updateList, List<ImageAnnotation> deleteList, IDbContextTransaction? dbContextTransaction = null);
        #endregion

        #region Delete

        #endregion
    }
}
