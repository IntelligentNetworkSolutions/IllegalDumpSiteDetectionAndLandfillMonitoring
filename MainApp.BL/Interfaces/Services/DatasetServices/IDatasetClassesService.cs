using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services.DatasetServices
{
    public interface IDatasetClassesService
    {
        #region Read
        #region Get Datasetclass/es
        Task<List<DatasetClassDTO>> GetAllDatasetClasses();
        Task<DatasetClassDTO> GetDatasetClassById(Guid classId);
        #endregion
        #endregion

        #region Create
        Task<int> AddDatasetClass(CreateDatasetClassDTO dto);
        #endregion

        #region Update
        Task<int> EditDatasetClass(EditDatasetClassDTO dto);
        #endregion

        #region Delete
        Task<int> DeleteDatasetClass(Guid classId);
        
        #endregion
    }
}
