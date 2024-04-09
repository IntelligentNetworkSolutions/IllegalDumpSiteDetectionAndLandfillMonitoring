using System;
using Entities;
using System.Threading.Tasks;
using Entities.DatasetEntities;
using System.Collections.Generic;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IDatasetsRepository : IBaseResultRepository<Dataset, Guid>
    {
        #region Read
            #region Get Dataset/s
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
