using DTOs.MainApp.BL.DatasetDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.DatasetServices
{
    public interface IDatasetImagesService
    {
        #region Read
        #region Get DatasetImage/es
        Task<DatasetImageDTO> GetDatasetImageById(Guid datasetImageId);
        Task<List<DatasetImageDTO>> GetImagesForDataset(Guid datasetId);
        #endregion
        #endregion

        #region Create
        Task<ResultDTO<Guid>> AddDatasetImage(DatasetImageDTO datasetImageDto);
        #endregion

        #region Update
        Task<ResultDTO<int>> EditDatasetImage(EditDatasetImageDTO editDatasetImageDTO);
        #endregion

        #region Delete
        Task<ResultDTO<int>> DeleteDatasetImage(Guid datasetImageId, bool deleteAnnotations);
        #endregion
    }
}
