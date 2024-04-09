using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services.DatasetServices
{
    public interface IDatasetService
    {
        #region Read
        #region Get Dataset/es
        Task<List<DatasetDTO>> GetAllDatasets();
        Task<DatasetDTO> GetDatasetById(Guid datasetId);
        Task<CreateDatasetDTO> FillDatasetDto(CreateDatasetDTO dto);
        Task<EditDatasetDTO> GetObjectForEditDataset(Guid datasetId, string? searchByImageName, bool? searchByIsAnnotatedImage, bool? searchByIsEnabledImage, string? orderByImages);
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
        #endregion

    }
}
