using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
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
        Task<int> AddDatasetClassForDataset(Guid selectedClassId, Guid datasetId);
        Task<int> AddInheritedParentClasses(Guid insertedDatasetId, Guid parentDatasetId);
        #endregion

        #region Update
        Task<int> PublishDataset(Guid datasetId);
        Task<int> SetAnnotationsPerSubclass(Guid datasetId, bool annotationsPerSubclass);
        #endregion

        #region Delete
        Task<int> DeleteDatasetClassForDataset(Guid selectedClassId, Guid datasetId);
        Task<int> DeleteDataset(Guid datasetId);
        #endregion

    }
}
