using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Services.DatasetServices
{
    public class DatasetImagesService : IDatasetImagesService
    {
        private readonly IDatasetsRepository _datasetsRepository;
        private readonly IDatasetClassesRepository _datasetClassesRepository;
        private readonly IDataset_DatasetClassRepository _datasetDatasetClassRepository;
        private readonly IDatasetImagesRepository _datasetImagesRepository;
        private readonly IImageAnnotationsRepository _imageAnnotationsRepository;
        private readonly IMapper _mapper;

        public DatasetImagesService(IDatasetsRepository datasetsRepository, 
                                    IDatasetClassesRepository datasetClassesRepository,
                                    IDataset_DatasetClassRepository datasetDatasetClassRepository, 
                                    IDatasetImagesRepository datasetImagesRepository,
                                    IImageAnnotationsRepository imageAnnotationsRepository,
                                    IMapper mapper)
        {
            _datasetsRepository = datasetsRepository;
            _datasetClassesRepository = datasetClassesRepository;
            _datasetDatasetClassRepository = datasetDatasetClassRepository;
            _datasetImagesRepository = datasetImagesRepository;
            _imageAnnotationsRepository = imageAnnotationsRepository;
            _mapper = mapper;
        }
        #region Read
        #region Get DatasetImage/es
        public async Task<DatasetImageDTO> GetDatasetImageById(Guid datasetImageId)
        {
            var datasetImageDb =  await _datasetImagesRepository.GetDatasetImageById(datasetImageId) ?? throw new Exception("Object is null");
            return _mapper.Map<DatasetImageDTO>(datasetImageDb);
        }
        public async Task<List<DatasetImageDTO>> GetImagesForDataset(Guid datasetId)
        {
            var datasetImages = await _datasetImagesRepository.GetImagesForDataset(datasetId) ?? throw new Exception("Dataset id is null"); 
            return _mapper.Map<List<DatasetImageDTO>>(datasetImages);
        }
        #endregion
        #endregion

        #region Create
        public async Task<Guid> AddDatasetImage(DatasetImageDTO datasetImageDto)
        {
            var datasetId = datasetImageDto.DatasetId ?? throw new Exception("Dataset id is null"); 
            var datasetDb = await _datasetsRepository.GetDatasetById(datasetId) ?? throw new Exception("Object not found");

            DatasetImage datasetImage = _mapper.Map<DatasetImage>(datasetImageDto);
            var isImageAdded = await _datasetImagesRepository.CreateDatasetImage(datasetImage);

            datasetDb.UpdatedOn = DateTime.UtcNow;
            //TODO: updatedBy 
            var isDatasetUpdated = await _datasetsRepository.UpdateDataset(datasetDb);
            return isImageAdded;
        }
        #endregion

        #region Update
        public async Task<int> EditDatasetImage(EditDatasetImageDTO editDatasetImageDTO)
        {
            var datasetDb = await _datasetsRepository.GetDatasetById(editDatasetImageDTO.DatasetId) ?? throw new Exception("Object not found");
            var datasetImageDb = await _datasetImagesRepository.GetDatasetImageById(editDatasetImageDTO.Id) ?? throw new Exception("Object not found");

            if (editDatasetImageDTO.IsEnabled == true)
            {
                var imageAnnotations = await _imageAnnotationsRepository.GetAllImageAnnotations() ?? throw new Exception("Object not found");
                var imageAnnotationsByImgId = imageAnnotations.Where(x => x.DatasetImageId == editDatasetImageDTO.Id).ToList();
                if (imageAnnotationsByImgId.Count < 1)
                {
                    return 2;
                }
            }
           
            DatasetImage datasetImage = _mapper.Map(editDatasetImageDTO, datasetImageDb);
            var isImageUpdated = await _datasetImagesRepository.EditDatasetImage(datasetImage);

            datasetDb.UpdatedOn = DateTime.UtcNow;
            //TODO: updatedBy 
            var isDatasetUpdated = await _datasetsRepository.UpdateDataset(datasetDb);
            return isImageUpdated;
        }
        #endregion

        #region Delete
        public async Task<int> DeleteDatasetImage(Guid datasetImageId)
        {
            var datasetImageDb = await _datasetImagesRepository.GetDatasetImageById(datasetImageId) ?? throw new Exception("Image not found");             
            var isImageDeleted = await _datasetImagesRepository.DeleteImage(datasetImageDb);
            return isImageDeleted;
        }
        #endregion
    }
}
