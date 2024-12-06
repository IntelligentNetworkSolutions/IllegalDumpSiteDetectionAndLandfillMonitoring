using DTOs.MainApp.BL.DatasetDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.DatasetServices
{
    public interface IDataset_DatasetClassService
    {
        #region Read
        #region Get obejct/s
        Task<ResultDTO<List<Dataset_DatasetClassDTO>>> GetDataset_DatasetClassByClassId(Guid classId);
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
