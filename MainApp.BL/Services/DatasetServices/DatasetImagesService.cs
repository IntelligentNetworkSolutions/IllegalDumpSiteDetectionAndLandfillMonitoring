using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DocumentFormat.OpenXml.EMMA;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using Microsoft.AspNetCore.Mvc;
using SD;
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
            var datasetImageDb = await _datasetImagesRepository.GetById(datasetImageId) ?? throw new Exception("Object not found");
            return _mapper.Map<DatasetImageDTO>(datasetImageDb.Data);
        }
        public async Task<List<DatasetImageDTO>> GetImagesForDataset(Guid datasetId)
        {
            var datasetImages = await _datasetImagesRepository.GetAll(filter: x => x.DatasetId == datasetId) ?? throw new Exception("Object not found");
            return _mapper.Map<List<DatasetImageDTO>>(datasetImages.Data);
        }
        #endregion
        #endregion

        #region Create
        public async Task<ResultDTO<Guid>> AddDatasetImage(DatasetImageDTO datasetImageDto)
        {
            var datasetId = datasetImageDto.DatasetId ?? throw new Exception("Dataset id is null");
            var datasetDb = await _datasetsRepository.GetById(datasetId,track: true, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var datasetDbData = datasetDb.Data ?? throw new Exception("Object not found");

            DatasetImage datasetImage = _mapper.Map<DatasetImage>(datasetImageDto);
            var imageAddedResult = await _datasetImagesRepository.CreateAndReturnEntity(datasetImage) ?? throw new Exception("Object not found");
            var imageAddedData = imageAddedResult.Data ?? throw new Exception("Object not found");

            datasetDbData.UpdatedOn = DateTime.UtcNow;
            datasetDbData.UpdatedById = datasetImageDto.UpdatedById; 
            await _datasetsRepository.Update(datasetDbData);
            if(imageAddedData.Id == Guid.Empty)
            {
                return new ResultDTO<Guid>(IsSuccess: false, Guid.Empty,"Dataset image was not added", null);
            }
            else
            {
                return new ResultDTO<Guid>(IsSuccess: true, imageAddedData.Id, null, null);
            }
        }
        #endregion

        #region Update
        public async Task<ResultDTO<int>> EditDatasetImage(EditDatasetImageDTO editDatasetImageDTO)
        {
            var datasetDb = await _datasetsRepository.GetById(editDatasetImageDTO.DatasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var datasetDbData = datasetDb.Data ?? throw new Exception("Object not found");
            var datasetImageDb = await _datasetImagesRepository.GetById(editDatasetImageDTO.Id) ?? throw new Exception("Object not found");
            var data = datasetImageDb.Data ?? throw new Exception("Object not found");

            if (editDatasetImageDTO.IsEnabled == true)
            {
                var imageAnnotations = await _imageAnnotationsRepository.GetAll() ?? throw new Exception("Object not found");
                var imageAnnotationsByImgId = imageAnnotations.Data?.Where(x => x.DatasetImageId == editDatasetImageDTO.Id).ToList();
                if (imageAnnotationsByImgId?.Count < 1)
                {
                    return new ResultDTO<int>(IsSuccess: false, 2, "You can not enable this image because there are not annotations!", null);
                }
            }
           
            DatasetImage datasetImage = _mapper.Map(editDatasetImageDTO, data);
            var isImageUpdated = await _datasetImagesRepository.Update(datasetImage);
            datasetDbData.UpdatedOn = DateTime.UtcNow;
            datasetDbData.UpdatedById = editDatasetImageDTO.UpdatedById;
            
            var isDatasetUpdated = await _datasetsRepository.Update(datasetDbData);

            if (isImageUpdated.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 3, isImageUpdated.ErrMsg, null);
            }
        }
        #endregion

        #region Delete
        public async Task<ResultDTO<int>> DeleteDatasetImage(Guid datasetImageId)
        {
            var datasetImageDb = await _datasetImagesRepository.GetById(datasetImageId) ?? throw new Exception("Object not found");
            var data = datasetImageDb.Data ?? throw new Exception("Object not found");            
            var isImageDeleted = await _datasetImagesRepository.Delete(data);
            if (isImageDeleted.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset image was not deleted", null);
            }
        }
        #endregion
    }
}
