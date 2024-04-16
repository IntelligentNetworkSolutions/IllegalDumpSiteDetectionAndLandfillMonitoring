using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.DatasetEntities;

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
        #endregion

        #region Delete

        #endregion
    }
}
