using System;
using Entities;
using System.Threading.Tasks;
using Entities.DatasetEntities;
using System.Collections.Generic;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IDatasetsRepository
    {
        #region Read
            #region Get Dataset/s
                Task<List<Dataset>> GetAllDatasets();            
                Task<Dataset> GetDatasetById(Guid datasetId);
                int GetNumberOfChildrenDatasetsByDatasetId(Guid datasetId);
           #endregion
        #endregion

        #region Create
              Task<Dataset> CreateDataset(Dataset dataset);
        #endregion

        #region Update
              Task<int> UpdateDataset(Dataset dataset);
        #endregion

        #region Delete
             Task<int> DeleteDataset(Dataset dataset);

        #endregion
    }
}
