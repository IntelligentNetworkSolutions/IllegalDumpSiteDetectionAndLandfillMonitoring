using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.ObjectDetection.API.CocoFormatDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.DatasetServices
{
    public interface IDatasetService
    {
        #region Utils
        Task<ResultDTO<string>> GetDatasetImagesDirectoryRelativePathByDatasetId(Guid datasetId);

        Task<ResultDTO<string>> GetDatasetImagesDirectoryAbsolutePathByDatasetId(string wwwRoot, Guid datasetId);

        Task<ResultDTO<string>> GetDatasetImageRelativePathByDatasetIdAndFileName(Guid datasetId, string fileName);

        Task<ResultDTO<string>> GetDatasetImageAbsolutePathByDatasetIdAndFileName(string wwwRoot, Guid datasetId, string fileName);
        #endregion

        #region Read
        #region Get Dataset/es
        Task<List<DatasetDTO>> GetAllDatasets();
        Task<DatasetDTO> GetDatasetById(Guid datasetId);
        Task<CreateDatasetDTO> FillDatasetDto(CreateDatasetDTO dto);
        Task<EditDatasetDTO> GetObjectForEditDataset(Guid datasetId, string? searchByImageName, bool? searchByIsAnnotatedImage, bool? searchByIsEnabledImage, string? orderByImages, int pageNumber, int pageSize);
        #endregion
        #endregion

        #region Create
        Task<DatasetDTO> CreateDataset(DatasetDTO dto);
        Task<ResultDTO<int>> AddDatasetClassForDataset(Guid selectedClassId, Guid datasetId, string userId);
        Task<ResultDTO<int>> AddInheritedParentClasses(Guid insertedDatasetId, Guid parentDatasetId);
        #endregion

        #region Update
        Task<ResultDTO<int>> PublishDataset(Guid datasetId, string userId);
        Task<ResultDTO<int>> SetAnnotationsPerSubclass(Guid datasetId, bool annotationsPerSubclass, string userId);
        #endregion

        #region Delete
        Task<ResultDTO<int>> DeleteDatasetClassForDataset(Guid selectedClassId, Guid datasetId, string userId);
        Task<ResultDTO<int>> DeleteDataset(Guid datasetId);
        Task<ResultDTO> DeleteDatasetCompletelyIncluded(Guid datasetId);
        #endregion

        #region Export
        Task<ResultDTO<CocoDatasetDTO>> ExportDatasetAsCOCOFormat(Guid datasetId);
        #endregion

        #region Import
        Task<ResultDTO<DatasetDTO>> ImportDatasetCocoFormatedAtDirectoryPath(string datasetName, string dirPath, string userId, string? wwwRoot = null, bool allowUnannotatedImages = false);
        #endregion
    }
}
