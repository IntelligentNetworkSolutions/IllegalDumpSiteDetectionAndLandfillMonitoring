using DTOs.MainApp.BL.DatasetDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.DatasetServices
{
    public interface IDatasetClassesService
    {
        #region Read
        #region Get Datasetclass/es
        Task<ResultDTO<List<DatasetClassDTO>>> GetAllDatasetClasses();
        Task<ResultDTO<DatasetClassDTO>> GetDatasetClassById(Guid classId);
        Task<ResultDTO<List<DatasetClassDTO>>> GetAllDatasetClassesByDatasetId(Guid datasetId);
        #endregion
        #endregion

        #region Create
        Task<ResultDTO<int>> AddDatasetClass(CreateDatasetClassDTO dto);
        #endregion

        #region Update
        Task<ResultDTO<int>> EditDatasetClass(EditDatasetClassDTO dto);
        #endregion

        #region Delete
        Task<ResultDTO<int>> DeleteDatasetClass(Guid classId);

        #endregion
    }
}
