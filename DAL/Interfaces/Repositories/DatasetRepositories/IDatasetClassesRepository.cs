using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.DatasetEntities;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IDatasetClassesRepository 
    {
        #region Read
        #region Get Datasetclass/es
        Task<List<DatasetClass>> GetAllDatasetClasses();
        Task<DatasetClass> GetDatasetClassById(Guid classId);
        #endregion
        #endregion

        #region Create
        Task<int> AddClass(DatasetClass newClass);
        #endregion

        #region Update
        Task<int> EditClass(DatasetClass datasetClass);
        #endregion

        #region Delete

        Task<int> DeleteClass(DatasetClass datasetClass);
        #endregion
    }
}
