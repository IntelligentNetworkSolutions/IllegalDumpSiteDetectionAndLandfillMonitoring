using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
        Task<Guid> AddDatasetImage(DatasetImageDTO datasetImageDto);
        #endregion

        #region Update
        Task<int> EditDatasetImage(EditDatasetImageDTO editDatasetImageDTO);
        #endregion

        #region Delete
        Task<int> DeleteDatasetImage(Guid datasetImageId);
        #endregion
    }
}
