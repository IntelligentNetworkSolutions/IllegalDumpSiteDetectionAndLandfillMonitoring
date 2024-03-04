using Entities.DatasetEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IDataset_DatasetClassRepository
    {
        #region Read
        #region Get Datasetclass/es
        Task<List<Dataset_DatasetClass>> GetAll();
        Task<Dataset_DatasetClass> GetByDatasetAndDatasetClassId(Guid selectedClassId, Guid datasetId);
        Task<List<Dataset_DatasetClass>> GetListByDatasetClassId(Guid datasetClassId);
        Task<List<Dataset_DatasetClass>> GetListByDatasetId(Guid datasetId);
        #endregion
        #endregion

        #region Create
        Task<int> Create(Dataset_DatasetClass dataset_datasetClass);
        Task<int> AddRange(List<Dataset_DatasetClass> dataset_DatasetClasses);
        #endregion

        #region Update

        #endregion

        #region Delete
        Task<int> Delete(Dataset_DatasetClass dataset_datasetClass);
        Task<int> DeleteRange(List<Dataset_DatasetClass> list_dataset_datasetClass);
        #endregion
    }
}
