using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.DatasetEntities;

namespace DAL.Interfaces.Repositories.DatasetRepositories
{
    public interface IDatasetImagesRepository
    {
        #region Read
        #region Get Dataset/s
        Task<List<DatasetImage>> GetAllImages();
        Task<List<DatasetImage>> GetImagesForDataset(Guid datasetId);
        Task<DatasetImage> GetDatasetImageById(Guid datasetImageId);        
        #endregion
        #endregion

        #region Create
        Task<Guid> CreateDatasetImage(DatasetImage datasetImage);
        #endregion

        #region Update
        Task<int> EditDatasetImage(DatasetImage datasetImage);
        #endregion

        #region Delete
        Task<int> DeleteImage(DatasetImage datasetImage);
        Task<int> DeleteRange(List<DatasetImage> datasetImagesList);
        #endregion
    }
}
