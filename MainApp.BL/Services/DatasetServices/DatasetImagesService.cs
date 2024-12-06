using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using SD;

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
        public async Task<ResultDTO<DatasetImageDTO>> GetDatasetImageById(Guid datasetImageId)
        {
            ResultDTO<DatasetImage?> datasetImageResult = await _datasetImagesRepository.GetById(datasetImageId);

            if (datasetImageResult.IsSuccess == false && datasetImageResult.HandleError())
                return ResultDTO<DatasetImageDTO>.Fail(datasetImageResult.ErrMsg);

            var datasetImageDto = _mapper.Map<DatasetImageDTO>(datasetImageResult.Data);

            if (datasetImageDto is null)
                return ResultDTO<DatasetImageDTO>.Fail("Mapping failed for this dataset image");

            return ResultDTO<DatasetImageDTO>.Ok(datasetImageDto);
        }

        public async Task<ResultDTO<List<DatasetImageDTO>>> GetImagesForDataset(Guid datasetId)
        {
            ResultDTO<IEnumerable<DatasetImage>> datasetImages =
                await _datasetImagesRepository.GetAll(filter: x => x.DatasetId == datasetId);

            if (datasetImages.IsSuccess == false && datasetImages.HandleError())
                return ResultDTO<List<DatasetImageDTO>>.Fail(datasetImages.ErrMsg!);

            var mappedDto = _mapper.Map<List<DatasetImageDTO>>(datasetImages.Data);

            if (mappedDto is null)
                return ResultDTO<List<DatasetImageDTO>>.Fail("Mapping failed");


            return ResultDTO<List<DatasetImageDTO>>.Ok(mappedDto);
        }
        #endregion
        #endregion

        #region Create
        public async Task<ResultDTO<Guid>> AddDatasetImage(DatasetImageDTO datasetImageDto)
        {
            // TODO: Refactor to not throw exceptions 
            Guid datasetId = datasetImageDto.DatasetId ?? Guid.Empty; // To handle since it is Guid?  will be handled on first if clause

            if (datasetId == Guid.Empty)
                return new ResultDTO<Guid>(IsSuccess: false, Guid.Empty, "Dataset id is null", null);

            ResultDTO<Dataset?> datasetDb =
                await _datasetsRepository.GetById(datasetId, track: true, includeProperties: "CreatedBy,UpdatedBy,ParentDataset");
            if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                return new ResultDTO<Guid>(IsSuccess: false, Guid.Empty, datasetDb.ErrMsg!, null);

            if (datasetDb.Data is null)
                return new ResultDTO<Guid>(IsSuccess: false, Guid.Empty, "Could not retrive dataset.", null);

            Dataset datasetDbData = datasetDb.Data;

            DatasetImage datasetImage = _mapper.Map<DatasetImage>(datasetImageDto);
            ResultDTO<DatasetImage> imageAddedResult = await _datasetImagesRepository.CreateAndReturnEntity(datasetImage);

            if (imageAddedResult.IsSuccess == false && imageAddedResult.HandleError())
                return new ResultDTO<Guid>(IsSuccess: false, Guid.Empty, imageAddedResult.ErrMsg!, null);


            DatasetImage imageAddedData = imageAddedResult.Data;

            if (imageAddedData is null)
                return new ResultDTO<Guid>(IsSuccess: false, Guid.Empty, "Object not found", null);

            datasetDbData.UpdatedOn = DateTime.UtcNow;
            datasetDbData.UpdatedById = datasetImageDto.UpdatedById;
            await _datasetsRepository.Update(datasetDbData);
            if (imageAddedData.Id == Guid.Empty)
                return new ResultDTO<Guid>(IsSuccess: false, Guid.Empty, "Dataset image was not added", null);

            return new ResultDTO<Guid>(IsSuccess: true, imageAddedData.Id, null, null);
        }
        #endregion

        #region Update
        public async Task<ResultDTO<int>> EditDatasetImage(EditDatasetImageDTO editDatasetImageDTO)
        {
            if (editDatasetImageDTO is null)
                return ResultDTO<int>.Fail("Null EditDatasetImageDTO");

            ResultDTO<Dataset?> resultDatasetIncluded = await _datasetsRepository.GetByIdInclude(editDatasetImageDTO.DatasetId, true,
                includeProperties:
                [
                    d => d.CreatedBy,
                    d => d.UpdatedBy,
                    d => d.ParentDataset
                ]
            );
            if (resultDatasetIncluded.IsSuccess == false)
                return ResultDTO<int>.Fail(resultDatasetIncluded.ErrMsg!);
            if (resultDatasetIncluded.Data is null)
                return ResultDTO<int>.Fail("Error getting Dataset");

            ResultDTO<DatasetImage?> resultDatasetImage = await _datasetImagesRepository.GetByIdInclude(editDatasetImageDTO.Id, true,
                [
                    di => di.ImageAnnotations,
                    di => di.CreatedBy,
                    di => di.UpdatedBy,
                ]);
            if (resultDatasetImage.IsSuccess == false)
                return ResultDTO<int>.Fail(resultDatasetImage.ErrMsg!);
            if (resultDatasetImage.Data is null)
                return ResultDTO<int>.Fail("Error getting Dataset Image");

            Dataset datasetEntity = resultDatasetIncluded.Data;
            DatasetImage datasetImageEntity = resultDatasetImage.Data;

            if (editDatasetImageDTO.IsEnabled == true)
                if (datasetImageEntity.ImageAnnotations.Count < 1)
                    return new ResultDTO<int>(IsSuccess: false, 2, "You can not enable this image because there are not annotations!", null);

            DatasetImage datasetImage = _mapper.Map(editDatasetImageDTO, datasetImageEntity);
            ResultDTO resultImageUpdate = await _datasetImagesRepository.Update(datasetImage);
            if (resultImageUpdate.IsSuccess == false)
                return ResultDTO<int>.Fail(resultImageUpdate.ErrMsg!);

            datasetEntity.UpdatedOn = DateTime.UtcNow;
            datasetEntity.UpdatedById = editDatasetImageDTO.UpdatedById;
            ResultDTO resultDatasetUpdate = await _datasetsRepository.Update(datasetEntity);
            if (resultDatasetUpdate.IsSuccess == false)
                return ResultDTO<int>.Fail(resultDatasetUpdate.ErrMsg!);

            if (resultImageUpdate.IsSuccess == true)
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);

            return new ResultDTO<int>(IsSuccess: false, 3, resultImageUpdate.ErrMsg, null);
        }
        #endregion

        #region Delete
        public async Task<ResultDTO<int>> DeleteDatasetImage(Guid datasetImageId, bool deleteAnnotations)
        {
            var datasetImageDb = await _datasetImagesRepository.GetById(datasetImageId, includeProperties: "ImageAnnotations");
            if (datasetImageDb.IsSuccess == false && datasetImageDb.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 2, datasetImageDb.ErrMsg!, null);


            DatasetImage data = datasetImageDb.Data;
            if (data is null)
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset image is null.", null);

            if (deleteAnnotations)
            {
                var resultDeleteAnnotations = await _imageAnnotationsRepository.DeleteRange(datasetImageDb.Data.ImageAnnotations);

                if (resultDeleteAnnotations.IsSuccess == false && resultDeleteAnnotations.HandleError())
                    return new ResultDTO<int>(IsSuccess: false, 2, resultDeleteAnnotations.ErrMsg!, null);
            }

            var isImageDeleted = await _datasetImagesRepository.Delete(datasetImageDb.Data);

            if (isImageDeleted.IsSuccess == false && isImageDeleted.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset image was not deleted", null);


            return new ResultDTO<int>(IsSuccess: true, 1, null, null);

        }
        #endregion
    }
}