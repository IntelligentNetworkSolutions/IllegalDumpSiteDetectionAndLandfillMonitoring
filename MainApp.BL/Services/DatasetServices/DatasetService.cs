using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.Helpers;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.ObjectDetection.API.CocoFormatDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SD;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Reflection;

namespace MainApp.BL.Services.DatasetServices
{
    public class DatasetService : IDatasetService
    {
        private readonly IDatasetsRepository _datasetsRepository;
        private readonly IDatasetClassesRepository _datasetClassesRepository;
        private readonly IDataset_DatasetClassRepository _datasetDatasetClassRepository;
        private readonly IDatasetImagesRepository _datasetImagesRepository;
        private readonly IImageAnnotationsRepository _imageAnnotationsRepository;
        private readonly ICocoUtilsService _cocoUtilsService;


        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<DatasetService> _logger;

        public DatasetService(IDatasetsRepository datasetsRepository,
                              IDatasetClassesRepository datasetClassesRepository,
                              IDataset_DatasetClassRepository datasetDatasetClassRepository,
                              IDatasetImagesRepository datasetImagesRepository,
                              IImageAnnotationsRepository imageAnnotationsRepository,
                              IAppSettingsAccessor appSettingsAccessor,
                              IMapper mapper,
                              ICocoUtilsService cocoUtilsService,
                              ILogger<DatasetService> logger)
        {
            _datasetsRepository = datasetsRepository;
            _datasetClassesRepository = datasetClassesRepository;
            _datasetDatasetClassRepository = datasetDatasetClassRepository;
            _datasetImagesRepository = datasetImagesRepository;
            _imageAnnotationsRepository = imageAnnotationsRepository;
            _appSettingsAccessor = appSettingsAccessor;
            _mapper = mapper;
            _cocoUtilsService = cocoUtilsService;
            _logger = logger;
        }

        #region Read

        public async Task<ResultDTO<List<DatasetDTO>>> GetAllDatasets()
        {
            var resultListDatasets = await _datasetsRepository.GetAll(includeProperties: "CreatedBy,UpdatedBy,ParentDataset");
            if (resultListDatasets.IsSuccess == false && resultListDatasets.HandleError())
            {
                return ResultDTO<List<DatasetDTO>>.Fail(resultListDatasets.ErrMsg!);
            }
            var data = resultListDatasets.Data;

            var listDatasetsDTOs = _mapper.Map<List<DatasetDTO>>(data);

            return ResultDTO<List<DatasetDTO>>.Ok(listDatasetsDTOs);
        }

        public async Task<ResultDTO<List<DatasetDTO>>> GetAllPublishedDatasets()
        {
            try
            {
                ResultDTO<IEnumerable<Dataset>>? resultGetEntities = await _datasetsRepository.GetAll(x => x.IsPublished == true, includeProperties: "CreatedBy,ParentDataset");
                if (resultGetEntities.IsSuccess == false && resultGetEntities.HandleError())
                    return ResultDTO<List<DatasetDTO>>.Fail(resultGetEntities.ErrMsg!);

                if (resultGetEntities.Data == null)
                    return ResultDTO<List<DatasetDTO>>.Fail("Training runs not found");

                List<DatasetDTO> dto = _mapper.Map<List<DatasetDTO>>(resultGetEntities.Data);

                return ResultDTO<List<DatasetDTO>>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<DatasetDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<DatasetDTO>> GetDatasetById(Guid datasetId)
        {
            ResultDTO<Dataset?> datasetDb = await _datasetsRepository.GetById(datasetId);

            if (datasetDb.IsSuccess == false && datasetDb.HandleError())
            {
                return ResultDTO<DatasetDTO>.Fail(datasetDb.ErrMsg!);
            }

            if (datasetDb.Data is null)
                return ResultDTO<DatasetDTO>.Fail($"Dataset not found, for id: {datasetId}");

            var data = datasetDb.Data;

            var datasetDTO = _mapper.Map<DatasetDTO>(data);

            if (datasetDTO is null)
                return ResultDTO<DatasetDTO>.Fail($"Dataset Mapping failed, for id: {datasetId}");

            return ResultDTO<DatasetDTO>.Ok(datasetDTO);

        }

        public async Task<ResultDTO<DatasetDTO>> GetDatasetDTOFullyIncluded(Guid datasetId, bool track = false)
        {
            ResultDTO<Dataset?> getDatasetFullyIncludedResult =
                await _datasetsRepository.GetByIdIncludeThenAll(datasetId, track,
                    includeProperties:
                    new (Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[]
                    {
                        (d => d.CreatedBy!, null),
                        (d => d.UpdatedBy, null),
                        (d => d.ParentDataset, null),
                        (d => d.DatasetClasses, [ ddc => ((Dataset_DatasetClass)ddc).DatasetClass, dc => ((DatasetClass)dc).ParentClass ]),
                        (d => d.DatasetImages, [ di => ((DatasetImage)di).ImageAnnotations, ia => ((ImageAnnotation)ia).CreatedBy! ])
                    });
            if (getDatasetFullyIncludedResult.IsSuccess == false && getDatasetFullyIncludedResult.HandleError())
                return ResultDTO<DatasetDTO>.Fail(getDatasetFullyIncludedResult.ErrMsg!);

            if (getDatasetFullyIncludedResult.Data is null)
                return ResultDTO<DatasetDTO>.Fail($"Dataset not found, for id: {datasetId}");

            DatasetDTO datasetDto = _mapper.Map<DatasetDTO>(getDatasetFullyIncludedResult.Data);
            if (datasetDto is null)
                return ResultDTO<DatasetDTO>.Fail($"Dataset Mapping failed, for id: {datasetId}");

            return ResultDTO<DatasetDTO>.Ok(datasetDto);
        }

        public async Task<ResultDTO<EditDatasetDTO>> GetObjectForEditDataset(Guid datasetId, string? searchByImageName, bool? searchByIsAnnotatedImage, bool? searchByIsEnabledImage, string? orderByImages, int pageNumber, int pageSize)
        {
            ResultDTO<IEnumerable<Dataset_DatasetClass>>? taskDatasetDatasetClasses = await _datasetDatasetClassRepository.GetAll(null, null, false, includeProperties: "DatasetClass,Dataset");

            if (taskDatasetDatasetClasses.IsSuccess == false && taskDatasetDatasetClasses.HandleError())
                return ResultDTO<EditDatasetDTO>.Fail(taskDatasetDatasetClasses.ErrMsg!);

            ResultDTO<IEnumerable<DatasetClass>> taskDatasetClasses = await _datasetClassesRepository.GetAll(null, null, false, includeProperties: "ParentClass,Datasets");

            if (taskDatasetClasses.IsSuccess == false && taskDatasetClasses.HandleError())
                return ResultDTO<EditDatasetDTO>.Fail(taskDatasetClasses.ErrMsg!);

            ResultDTO<Dataset?> taskCurrentDataset = await _datasetsRepository.GetByIdIncludeThenAll(datasetId, false,
                includeProperties: new (Expression<Func<Dataset, object>>, Expression<Func<object, object>>[]?)[] {
            (d => d.CreatedBy, null),
            (d => d.UpdatedBy, null),
            (d => d.ParentDataset, null),
            (d => d.DatasetClasses, new Expression<Func<object, object>>[] {
                ddc => ((Dataset_DatasetClass)ddc).DatasetClass,
                dc => ((DatasetClass)dc).ParentClass
            }),
            (d => d.DatasetImages, new Expression<Func<object, object>>[] {
                di => ((DatasetImage)di).ImageAnnotations,
                ia => ((ImageAnnotation)ia).CreatedBy
            })
                });

            if (taskCurrentDataset.IsSuccess == false && taskCurrentDataset.HandleError())
                return ResultDTO<EditDatasetDTO>.Fail(taskCurrentDataset.ErrMsg!);

            ResultDTO<int> taskNumberOfImagesToPublish = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100);

            if (taskNumberOfImagesToPublish.IsSuccess == false && taskNumberOfImagesToPublish.HandleError())
                return ResultDTO<EditDatasetDTO>.Fail(taskNumberOfImagesToPublish.ErrMsg!);

            var taskNumberOfClassesToPublish = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1);

            if (taskNumberOfClassesToPublish.IsSuccess == false && taskNumberOfClassesToPublish.HandleError())
                return ResultDTO<EditDatasetDTO>.Fail(taskNumberOfClassesToPublish.ErrMsg!);

            var imageResult = await _datasetImagesRepository.GetAll(x => x.DatasetId == datasetId, null, false, includeProperties: "ImageAnnotations");

            if (imageResult.IsSuccess == false && imageResult.HandleError())
                return ResultDTO<EditDatasetDTO>.Fail(imageResult.ErrMsg!);

            var imageList = imageResult.Data;

            if (!string.IsNullOrEmpty(searchByImageName))
            {
                imageList = imageList.Where(x => x.Name == searchByImageName).ToList();
            }
            if (searchByIsEnabledImage.HasValue)
            {
                imageList = imageList.Where(x => x.IsEnabled == searchByIsEnabledImage).ToList();
            }
            if (searchByIsAnnotatedImage.HasValue)
            {
                imageList = imageList.Where(x => searchByIsAnnotatedImage.Value
                            ? x.ImageAnnotations.Any()
                            : !x.ImageAnnotations.Any()).ToList();
            }

            // Apply ordering
            imageList = orderByImages switch
            {
                "ascName" => imageList.OrderBy(x => x.Name).ToList(),
                "descName" => imageList.OrderByDescending(x => x.Name).ToList(),
                "ascCreatedOn" => imageList.OrderBy(x => x.CreatedOn).ToList(),
                "descCreatedOn" => imageList.OrderByDescending(x => x.CreatedOn).ToList(),
                _ => imageList.OrderBy(x => x.IsEnabled).ToList(),
            };

            var pagedImagesData = imageList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var allImageAnnotationsList = await _imageAnnotationsRepository.GetAll(null, null, false);
            var annotationsForPagedImages = allImageAnnotationsList?.Data?.Where(x => pagedImagesData.Select(m => m.Id).Contains(x.DatasetImageId.Value)).ToList();
            var numberOfAnnotatedImages = imageList.Select(x => x.ImageAnnotations).Count();


            var annotatedImageIds = annotationsForPagedImages?.Select(x => x.DatasetImageId.Value).ToHashSet() ?? new HashSet<Guid>();

            if (searchByIsAnnotatedImage.HasValue)
            {
                imageList = imageList.Where(x => annotatedImageIds.Contains(x.Id)).ToList();
            }

            var listOfDatasetImages = pagedImagesData.Select(image => new DatasetImageDTO
            {
                Id = image.Id,
                FileName = image.FileName,
                Name = image.Name,
                ImagePath = image.ImagePath,
                ThumbnailPath = image.ThumbnailPath,
                IsEnabled = image.IsEnabled,
                IsAnnotated = annotatedImageIds.Contains(image.Id)
            }).ToList();


            var numberOfEnabledImages = imageList.Count(x => x.IsEnabled == true);
            var allEnabledImagesHaveAnnotations = listOfDatasetImages.All(x => annotatedImageIds.Contains(x.Id));

            if (taskDatasetDatasetClasses?.Data == null)
            {
                return ResultDTO<EditDatasetDTO>.Fail("Failed to retrieve dataset classes associated with the dataset.");
            }

            if (taskDatasetClasses?.Data == null)
            {
                return ResultDTO<EditDatasetDTO>.Fail("Failed to retrieve available dataset classes.");
            }

            var insertedClasses = taskDatasetDatasetClasses.Data.Where(x => x.DatasetId == datasetId).Select(x => x.DatasetClass).ToList();
            var insertedClassesIds = taskDatasetDatasetClasses.Data.Where(x => x.DatasetId == datasetId).Select(x => x.DatasetClassId).ToList();
            var uninsertedRootClasses = taskDatasetClasses.Data.Where(x => !insertedClassesIds.Contains(x.Id) && x.ParentClassId == null).ToList();
            var uninsertedSubclasses = taskDatasetClasses.Data.Where(x => !insertedClassesIds.Contains(x.Id) && x.ParentClassId != null).ToList();

            var dto = new EditDatasetDTO
            {
                UninsertedDatasetRootClasses = _mapper.Map<List<DatasetClassDTO>>(uninsertedRootClasses),
                UninsertedDatasetSubclasses = _mapper.Map<List<DatasetClassDTO>>(uninsertedSubclasses),
                ClassesByDatasetId = _mapper.Map<List<DatasetClassDTO>>(insertedClasses),
                NumberOfDatasetClasses = insertedClasses?.Count ?? 0,
                CurrentDataset = _mapper.Map<DatasetDTO>(taskCurrentDataset.Data),
                NumberOfChildrenDatasets = taskDatasetDatasetClasses?.Data?.Where(x => x.Dataset.ParentDatasetId == datasetId).Count() ?? 0,
                ParentDatasetClasses = _mapper.Map<List<DatasetClassDTO>>(taskDatasetDatasetClasses?.Data?.Where(x => x.DatasetId == taskCurrentDataset.Data?.ParentDatasetId).Select(x => x.DatasetClass).ToList()),
                ListOfDatasetImages = listOfDatasetImages,
                NumberOfDatasetImages = imageList.Count(),
                AllImageAnnotations = _mapper.Map<List<ImageAnnotationDTO>>(annotationsForPagedImages),
                NumberOfEnabledImages = numberOfEnabledImages,
                NumberOfAnnotatedImages = numberOfAnnotatedImages,
                AllEnabledImagesHaveAnnotations = allEnabledImagesHaveAnnotations,
                NumberOfClassesNeededToPublishDataset = taskNumberOfClassesToPublish.Data,
                NumberOfImagesNeededToPublishDataset = taskNumberOfImagesToPublish.Data,
                ListOfAllDatasetImagesUnFiltered = _mapper.Map<List<DatasetImageDTO>>(imageList)
            };

            return ResultDTO<EditDatasetDTO>.Ok(dto);
        }
        #endregion

        #region Create

        public async Task<ResultDTO<DatasetDTO>> CreateDataset(DatasetDTO dto)
        {
            Dataset dataset = _mapper.Map<Dataset>(dto);

            var insertedDatasetResult = await _datasetsRepository.CreateAndReturnEntity(dataset);
            if (insertedDatasetResult.IsSuccess == false && insertedDatasetResult.HandleError())
                return ResultDTO<DatasetDTO>.Fail(insertedDatasetResult.ErrMsg!);

            if (insertedDatasetResult.Data is null)
                return ResultDTO<DatasetDTO>.Fail("Failed to create dataset. Dataset is null.");

            var insertedDataset = insertedDatasetResult.Data;

            DatasetDTO newDTO = _mapper.Map<DatasetDTO>(insertedDataset);

            if (newDTO is null)
                return ResultDTO<DatasetDTO>.Fail("Mapping failed");

            return ResultDTO<DatasetDTO>.Ok(newDTO);
        }
        public async Task<ResultDTO<int>> AddDatasetClassForDataset(Guid selectedClassId, Guid datasetId, string userId)
        {
            Dataset_DatasetClass dataset_datasetClass = new()
            {
                DatasetId = datasetId,
                DatasetClassId = selectedClassId
            };

            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset");

            if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 2, datasetDb.ErrMsg!, null);

            var datasetDbData = datasetDb.Data;
            var createdDatasetClass = await _datasetDatasetClassRepository.Create(dataset_datasetClass);
            if (createdDatasetClass.IsSuccess == false && createdDatasetClass.HandleError())
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset class was not added", null);
            }
            datasetDbData.UpdatedOn = DateTime.UtcNow;
            datasetDbData.UpdatedById = userId;

            await _datasetsRepository.Update(datasetDbData);
            if (createdDatasetClass.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset class was not added", null);
            }
        }

        public async Task<ResultDTO<int>> AddInheritedParentClasses(Guid insertedDatasetId, Guid parentDatasetId)
        {
            var parentClassesList = await _datasetDatasetClassRepository.GetAll(filter: x => x.DatasetId == parentDatasetId);
            if (parentClassesList.IsSuccess == false && parentClassesList.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 2, parentClassesList.ErrMsg, null);

            if (parentClassesList.Data.Any() == false)
                return new ResultDTO<int>(IsSuccess: false, 2, "An error occurred while attempting to add the parent class.", null);


            var parentClassesIdsList = parentClassesList.Data?.Select(x => x.DatasetClassId).ToList();

            List<Dataset_DatasetClass> dataset_DatasetClasses = new();
            foreach (var item in parentClassesIdsList)
            {
                Dataset_DatasetClass dataset_class = new();
                dataset_class.DatasetId = insertedDatasetId;
                dataset_class.DatasetClassId = item;
                dataset_DatasetClasses.Add(dataset_class);
            }
            var isAdded = await _datasetDatasetClassRepository.CreateRange(dataset_DatasetClasses);

            if (isAdded.IsSuccess == false && isAdded.HandleError())
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset classes were not added", null);
            }

            return new ResultDTO<int>(IsSuccess: true, 1, null, null);

        }


        #endregion

        #region Update
        public async Task<ResultDTO<int>> PublishDataset(Guid datasetId, string userId)
        {
            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset");

            if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 3, datasetDb.ErrMsg, null);

            if (datasetDb.Data is null)
                return new ResultDTO<int>(IsSuccess: false, 3, "Dataset data could not be retrived", null);

            var datasetDbData = datasetDb.Data;

            var allDataset_DatasetClasses = await _datasetDatasetClassRepository.GetAll(includeProperties: "DatasetClass,Dataset");
            if (allDataset_DatasetClasses.IsSuccess == false && allDataset_DatasetClasses.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 3, allDataset_DatasetClasses.ErrMsg, null);

            var allDatasetImages = await _datasetImagesRepository.GetAll(filter: x => x.DatasetId == datasetDbData.Id, includeProperties: "ImageAnnotations");

            if (allDatasetImages.IsSuccess == false && allDatasetImages.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 3, allDatasetImages.ErrMsg, null);

            if (allDatasetImages.Data.Any() == false)
                return new ResultDTO<int>(IsSuccess: false, 3, "Dataset images could not be retrived", null);

            var allDatasetImagesData = allDatasetImages.Data;

            var enabledImagesList = allDatasetImagesData.Where(x => x.IsEnabled == true).ToList();

            if (enabledImagesList.Any() == false)
                return new ResultDTO<int>(IsSuccess: false, 2, null, null);

            var allImageAnnotationsList = await _imageAnnotationsRepository.GetAll();

            if (allImageAnnotationsList.IsSuccess == false && allImageAnnotationsList.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 3, allImageAnnotationsList.ErrMsg, null);

            if (allImageAnnotationsList.Data.Any() == false)
                return new ResultDTO<int>(IsSuccess: false, 3, "Image annotations could not be retrived", null);

            var allEnabledImagesHaveAnnotations = enabledImagesList.Any() ?
                            enabledImagesList.All(x => allImageAnnotationsList.Data.Where(m => m.DatasetImageId == x.Id).Select(x => x.DatasetImageId).Any(a => a == x.Id)) : false;
            var nubmerOfImagesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100);
            var nubmerOfClassesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1);

            var insertedClasses = allDataset_DatasetClasses.Data?.Where(x => x.DatasetId == datasetDbData.Id).Select(x => x.DatasetClass).ToList();

            if (insertedClasses.Any() == false)
                return new ResultDTO<int>(IsSuccess: false, 3, "No inserted classes were found", null);


            if (insertedClasses.Count < nubmerOfClassesNeededToPublishDataset.Data || allEnabledImagesHaveAnnotations == false || allDatasetImagesData.Count() < nubmerOfImagesNeededToPublishDataset.Data)
            {
                return new ResultDTO<int>(IsSuccess: false, 2, null, null);
            }

            datasetDbData.IsPublished = true;
            datasetDbData.UpdatedOn = DateTime.UtcNow;
            datasetDbData.UpdatedById = userId;

            var updatedDataset = await _datasetsRepository.Update(datasetDbData);
            if (updatedDataset.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 3, updatedDataset.ErrMsg, null);
            }

        }

        public async Task<ResultDTO<int>> SetAnnotationsPerSubclass(Guid datasetId, bool annotationsPerSubclass, string userId)
        {
            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset");

            if (datasetDb.IsSuccess == false && datasetDb.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 3, datasetDb.ErrMsg, null);

            if (datasetDb.Data is null)
                return new ResultDTO<int>(IsSuccess: false, 3, "Dataset data could not be retrived", null);

            var datasetDbData = datasetDb.Data;
            datasetDbData.AnnotationsPerSubclass = annotationsPerSubclass;
            datasetDbData.UpdatedOn = DateTime.UtcNow;
            datasetDbData.UpdatedById = userId;

            var updatedDataset = await _datasetsRepository.Update(datasetDbData);
            if (updatedDataset.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 2, updatedDataset.ErrMsg, null);
            }
        }

        public async Task<ResultDTO> EnableAllImagesInDataset(Guid datasetId)
        {
            var resultGetEntity = await _datasetsRepository.GetById(datasetId, includeProperties: "DatasetImages");

            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                return ResultDTO.Fail("Error getting Dataset");


            foreach (var image in resultGetEntity.Data.DatasetImages)
            {
                image.IsEnabled = true;
            }

            var updatedDataset = await _datasetsRepository.Update(resultGetEntity.Data);

            if (updatedDataset.IsSuccess == false && updatedDataset.HandleError())
                return ResultDTO.Fail("Error updating Dataset");

            return ResultDTO.Ok();

        }

        #endregion

        #region Delete
        public async Task<ResultDTO<int>> DeleteDatasetClassForDataset(Guid selectedClassId, Guid datasetId, string userId)
        {
            var result = await _datasetDatasetClassRepository.GetFirstOrDefault(filter: x => x.DatasetId == datasetId && x.DatasetClassId == selectedClassId);
            if (!result.IsSuccess && result.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 2, result.ErrMsg, null);

            if (result.Data is null)
                return new ResultDTO<int>(IsSuccess: false, 2, "Unable to retrieve data for the specified dataset class.", null);

            Dataset_DatasetClass dataset_DatasetClassDb = result.Data;
            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset");

            if (!datasetDb.IsSuccess && datasetDb.HandleError())
                return new ResultDTO<int>(IsSuccess: false, 2, datasetDb.ErrMsg, null);

            if (datasetDb.Data is null)
                return new ResultDTO<int>(IsSuccess: false, 2, "Unable to retrieve data for the specified dataset class.", null);

            var datasetDbData = datasetDb.Data;
            var deletedDataset_DatasetClass = await _datasetDatasetClassRepository.Delete(dataset_DatasetClassDb);
            datasetDbData.UpdatedOn = DateTime.UtcNow;
            datasetDbData.UpdatedById = userId;

            await _datasetsRepository.Update(datasetDbData);

            if (deletedDataset_DatasetClass.IsSuccess == false && deletedDataset_DatasetClass.HandleError())
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset class was not deleted", null);
            }

            return new ResultDTO<int>(IsSuccess: true, 1, null, null);
        }

        public async Task<ResultDTO> DeleteDatasetCompletelyIncluded(Guid datasetId)
        {
            if (datasetId.Equals(Guid.Empty))
                return ResultDTO.Fail("Invalid Dataset Id");

            ResultDTO<Dataset?> getDatasetIncludeThenAllResult =
                        await _datasetsRepository.GetByIdIncludeThenAll(datasetId, track: true,
                            includeProperties:
                            [
                                (d => d.ParentDataset, null),
                                (d => d.DatasetClasses, [ddc => ((Dataset_DatasetClass)ddc).DatasetClass, dc => ((DatasetClass)dc).ParentClass, ]),
                                (d => d.DatasetImages, [di => ((DatasetImage)di).ImageAnnotations,]),
                                (d => d.DatasetImages, [ di => ((DatasetImage)di).ImageAnnotations]),
                                (d => d.DatasetImages,
                                    [ di => ((DatasetImage)di).ImageAnnotations,
                                        ia => ((ImageAnnotation)ia).DatasetClass,
                                        dc => ((DatasetClass)dc).Datasets
                                    ]),
                            ]);

            if (getDatasetIncludeThenAllResult.IsSuccess == false && ResultDTO<Dataset?>.HandleError(getDatasetIncludeThenAllResult))
                return ResultDTO.Fail(getDatasetIncludeThenAllResult.ErrMsg!);

            if (getDatasetIncludeThenAllResult.Data is null)
                return ResultDTO.Fail("Error getting Dataset");

            Dataset dataset = getDatasetIncludeThenAllResult.Data;

            ICollection<Dataset_DatasetClass> datasetDatasetClasses = dataset.DatasetClasses;
            ICollection<DatasetImage> datasetImages = dataset.DatasetImages;
            IOrderedEnumerable<ImageAnnotation> annotations = datasetImages.SelectMany(image => image.ImageAnnotations)
                                                                            .OrderBy(annotation => annotation.CreatedOn);

            // Delete Annotations
            if (annotations.Count() > 0)
            {
                ResultDTO deleteDatasetImageAnnotationsResult = await _imageAnnotationsRepository.DeleteRange(annotations, false);
                if (deleteDatasetImageAnnotationsResult.IsSuccess == false && ResultDTO.HandleError(deleteDatasetImageAnnotationsResult))
                    return ResultDTO.Fail(deleteDatasetImageAnnotationsResult.ErrMsg!);
            }

            // Delete Images
            if (datasetImages.Count > 0)
            {
                ResultDTO deleteDatasetImagesResult = await _datasetImagesRepository.DeleteRange(datasetImages, false);
                if (deleteDatasetImagesResult.IsSuccess == false && ResultDTO.HandleError(deleteDatasetImagesResult))
                    return ResultDTO.Fail(deleteDatasetImagesResult.ErrMsg!);
            }

            // Delete Dataset_DatasetClasses
            if (datasetDatasetClasses.Count > 0)
            {
                ResultDTO deleteDatasetDatasetClassesResult = await _datasetDatasetClassRepository.DeleteRange(datasetDatasetClasses, false);
                if (deleteDatasetDatasetClassesResult.IsSuccess == false && ResultDTO.HandleError(deleteDatasetDatasetClassesResult))
                    return ResultDTO.Fail(deleteDatasetDatasetClassesResult.ErrMsg!);
            }

            // Delete Dataset
            ResultDTO deleteDatasetResult = await _datasetsRepository.Delete(getDatasetIncludeThenAllResult.Data, false);
            if (deleteDatasetResult.IsSuccess == false && ResultDTO.HandleError(deleteDatasetResult))
                return ResultDTO.Fail(deleteDatasetResult.ErrMsg!);

            // Save Changes after everything is marked for deletion
            ResultDTO<int> saveChangesResult = await _datasetsRepository.SaveChangesAsync();
            if (saveChangesResult.IsSuccess == false && ResultDTO<int>.HandleError(saveChangesResult))
                return ResultDTO.Fail(saveChangesResult.ErrMsg!);

            return ResultDTO.Ok();
        }
        #endregion

        #region Export
        // TODO: Create DatsetExportOption Enum/Static Class to replace magic string param
        public ResultDTO<DatasetFullIncludeDTO> GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity
            (Dataset datasetIncluded, string exportOption, bool includeDisabledImages = true, bool includeDisabledAnnotations = true)
        {
            try
            {
                if (datasetIncluded is null)
                    return ResultDTO<DatasetFullIncludeDTO>.Fail("Dataset is null");

                List<DatasetImage> orderedDatasetImages = exportOption switch
                {
                    "AllImages" => includeDisabledImages
                        ? datasetIncluded.DatasetImages.OrderBy(x => x.CreatedOn).ToList()
                        : datasetIncluded.DatasetImages.Where(x => x.IsEnabled).OrderBy(x => x.CreatedOn).ToList(),
                    "AnnotatedImages" => includeDisabledImages
                        ? datasetIncluded.DatasetImages.Where(x => x.ImageAnnotations.Any()).OrderBy(x => x.CreatedOn).ToList()
                        : datasetIncluded.DatasetImages.Where(x => x.IsEnabled && x.ImageAnnotations.Any()).OrderBy(x => x.CreatedOn).ToList(),
                    "EnabledImages" => datasetIncluded.DatasetImages.Where(x => x.IsEnabled).OrderBy(x => x.CreatedOn).ToList(),
                    "UnannotatedImages" => includeDisabledImages
                        ? datasetIncluded.DatasetImages.Where(x => !x.ImageAnnotations.Any()).OrderBy(x => x.CreatedOn).ToList()
                        : datasetIncluded.DatasetImages.Where(x => x.IsEnabled && !x.ImageAnnotations.Any()).OrderBy(x => x.CreatedOn).ToList(),
                    _ => includeDisabledImages
                        ? datasetIncluded.DatasetImages.OrderBy(x => x.CreatedOn).ToList()
                        : datasetIncluded.DatasetImages.Where(x => x.IsEnabled).OrderBy(x => x.CreatedOn).ToList()
                };

                List<DatasetImageDTO> datasetImageDTOs = _mapper.Map<List<DatasetImageDTO>>(orderedDatasetImages);

                List<ImageAnnotation> orderedAnnotations = (includeDisabledImages, includeDisabledAnnotations) switch
                {
                    (true, true) => orderedDatasetImages
                                        .SelectMany(image => image.ImageAnnotations)
                                        .OrderBy(annotation => annotation.CreatedOn)
                                        .ToList(),
                    (true, false) => orderedDatasetImages
                                        .SelectMany(image => image.ImageAnnotations
                                        .Where(ima => ima.IsEnabled))
                                        .OrderBy(annotation => annotation.CreatedOn)
                                        .ToList(),
                    (false, true) => orderedDatasetImages
                                        .Where(im => im.IsEnabled)
                                        .SelectMany(image => image.ImageAnnotations)
                                        .OrderBy(annotation => annotation.CreatedOn)
                                        .ToList(),
                    (false, false) => orderedDatasetImages
                                        .Where(im => im.IsEnabled)
                                        .SelectMany(image => image.ImageAnnotations.Where(ima => ima.IsEnabled))
                                        .OrderBy(annotation => annotation.CreatedOn)
                                        .ToList(),
                };

                List<ImageAnnotationDTO> imageAnnotationDTOs = _mapper.Map<List<ImageAnnotationDTO>>(orderedAnnotations);

                DatasetImageDTO[] datasetImagesDTOsArr = datasetImageDTOs.ToArray();
                ImageAnnotationDTO[] imageAnnotationDTOsArr = imageAnnotationDTOs.ToArray();

                for (int i = 0; i < datasetImagesDTOsArr.Length; i++)
                    datasetImagesDTOsArr[i].IdInt = i;

                for (int i = 0; i < imageAnnotationDTOsArr.Length; i++)
                {
                    imageAnnotationDTOsArr[i].IdInt = i;
                    imageAnnotationDTOsArr[i].DatasetImageIdInt =
                        datasetImagesDTOsArr.First(x => x.Id == imageAnnotationDTOsArr[i].DatasetImageId).IdInt;
                }

                var mappedDataset = _mapper.Map<DatasetDTO>(datasetIncluded);

                if (mappedDataset == null)
                {
                    return ResultDTO<DatasetFullIncludeDTO>.Fail("Failed to map the dataset. No data found for the provided dataset.");
                }

                DatasetFullIncludeDTO datasetFullIncludeDTO = new DatasetFullIncludeDTO(mappedDataset)
                {
                    DatasetImages = datasetImagesDTOsArr.ToList(),
                    ImageAnnotations = imageAnnotationDTOsArr.ToList(),
                    DatasetClassForDataset = datasetIncluded.DatasetClasses.Select(dc => new DatasetClassForDatasetDTO()
                    {
                        ClassName = dc.DatasetClass.ClassName,
                        ClassValue = dc.DatasetClassValue,
                        DatasetClassId = dc.DatasetClass.Id,
                        DatasetDatasetClassId = dc.DatasetClassId
                    }).ToList(),
                };


                return ResultDTO<DatasetFullIncludeDTO>.Ok(datasetFullIncludeDTO);
            }
            catch (Exception ex)
            {
                return ResultDTO<DatasetFullIncludeDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<string>> ExportDatasetAsCOCOFormat(Guid datasetId, string exportOption, string? downloadLocation, bool asSplit)
        {
            try
            {
                if (datasetId == Guid.Empty)
                    return ResultDTO<string>.Fail("Invalid Dataset Id");

                ResultDTO<Dataset?> resultDatasetIncludeThenAll =
                    await _datasetsRepository.GetByIdIncludeThenAll(datasetId, track: false,
                        includeProperties:
                        [ (d => d.CreatedBy, null),
                            (d => d.UpdatedBy, null),
                            (d => d.ParentDataset, null),
                            (d => d.DatasetClasses, [ddc => ((Dataset_DatasetClass)ddc).DatasetClass, dc => ((DatasetClass)dc).ParentClass ]),
                            (d => d.DatasetImages, [ di => ((DatasetImage)di).ImageAnnotations, ia => ((ImageAnnotation)ia).CreatedBy ]),
                            (d => d.DatasetImages, [ di => ((DatasetImage)di).ImageAnnotations, ia => ((ImageAnnotation)ia).UpdatedBy ]),
                            (d => d.DatasetImages, [ di => ((DatasetImage)di).ImageAnnotations, ia => ((ImageAnnotation)ia).DatasetClass, dc => ((DatasetClass)dc).Datasets ])
                        ]);

                if (resultDatasetIncludeThenAll.IsSuccess == false && resultDatasetIncludeThenAll.HandleError())
                    return ResultDTO<string>.Fail(resultDatasetIncludeThenAll.ErrMsg!);

                if (resultDatasetIncludeThenAll.Data is null)
                    return ResultDTO<string>.Fail("Error getting Dataset");

                if (asSplit)
                {
                    return await ConvertDatasetEntityToCocoDatasetWithAssignedIdIntsAsSplitDataset(resultDatasetIncludeThenAll.Data, exportOption, downloadLocation);
                }
                else
                {
                    return await ConvertDatasetEntityToCocoDatasetWithAssignedIdInts(resultDatasetIncludeThenAll.Data, exportOption, downloadLocation);
                }
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<string>> ConvertDatasetEntityToCocoDatasetWithAssignedIdIntsAsSplitDataset(Dataset dataset, string exportOption, string? downloadLocation)
        {

            // TODO: Implement for multiple classes
            if (dataset == null)
                return ResultDTO<string>.Fail("Dataset is null");

            try
            {
                ResultDTO<DatasetFullIncludeDTO> resultGetDatasetFullIncludeDTO =
                    GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity(datasetIncluded: dataset,
                                                                                exportOption: exportOption,
                                                                                includeDisabledImages: true,
                                                                                includeDisabledAnnotations: true);

                if (resultGetDatasetFullIncludeDTO.IsSuccess == false && resultGetDatasetFullIncludeDTO.HandleError())
                    return ResultDTO<string>.Fail(resultGetDatasetFullIncludeDTO.ErrMsg!);

                DatasetFullIncludeDTO datasetExtClassesImagesAnnotations = resultGetDatasetFullIncludeDTO.Data!;

                int totalImages = datasetExtClassesImagesAnnotations.DatasetImages.Count;
                int trainCount = (int)(totalImages * 0.7);
                int valCount = (int)(totalImages * 0.2);
                int testCount = totalImages - trainCount - valCount;

                Random random = new Random();
                List<DatasetImageDTO> shuffledImages = datasetExtClassesImagesAnnotations.DatasetImages
                    .OrderBy(x => random.Next())
                    .ToList();

                // Create directories for train, val, test
                string tempDirectory = downloadLocation ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                string trainDir = Path.Combine(tempDirectory, "train");
                string valDir = Path.Combine(tempDirectory, "valid");
                string testDir = Path.Combine(tempDirectory, "test");

                Directory.CreateDirectory(trainDir);
                Directory.CreateDirectory(valDir);
                Directory.CreateDirectory(testDir);

                // Create COCO datasets with required fields initialized
                CocoDatasetDTO trainCocoDataset = new CocoDatasetDTO
                {
                    Info = new CocoInfoDTO
                    {
                        Year = DateTime.Now.Year,
                        Version = "1.0",
                        Description = datasetExtClassesImagesAnnotations.Dataset.Description,
                        Contributor = "IllegalDumpSiteDetectionAndLandfillMonitoring",
                        DateCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                    Licenses = new List<CocoLicenseDTO> { new CocoLicenseDTO { Id = 1, Name = "Apache 2.0", Url = "https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring?tab=Apache-2.0-1-ov-file#readme" } },
                    Images = new List<CocoImageDTO>(),
                    Annotations = new List<CocoAnnotationDTO>(),
                    Categories = datasetExtClassesImagesAnnotations.DatasetClassForDataset
                        .Select(d => new CocoCategoryDTO { Id = d.ClassValue, Name = d.ClassName })
                        .ToList()
                };

                CocoDatasetDTO valCocoDataset = new CocoDatasetDTO
                {
                    Info = trainCocoDataset.Info,
                    Licenses = trainCocoDataset.Licenses,
                    Images = new List<CocoImageDTO>(),
                    Annotations = new List<CocoAnnotationDTO>(),
                    Categories = trainCocoDataset.Categories
                };

                CocoDatasetDTO testCocoDataset = new CocoDatasetDTO
                {
                    Info = trainCocoDataset.Info,
                    Licenses = trainCocoDataset.Licenses,
                    Images = new List<CocoImageDTO>(),
                    Annotations = new List<CocoAnnotationDTO>(),
                    Categories = trainCocoDataset.Categories
                };

                int trainImageId = 0, valImageId = 0, testImageId = 0;
                int trainAnnotationId = 0, valAnnotationId = 0, testAnnotationId = 0;

                for (int i = 0; i < totalImages; i++)
                {
                    DatasetImageDTO image = shuffledImages[i];
                    string fullImageName = image.Id.ToString() + Path.GetExtension(image.FileName);
                    string imagePath = Path.Combine("wwwroot", image.ImagePath.TrimStart('\\'), fullImageName);
                    string destPath = (i < trainCount) ? Path.Combine(trainDir, fullImageName) :
                        (i < trainCount + valCount) ? Path.Combine(valDir, fullImageName) :
                        Path.Combine(testDir, fullImageName);
                    File.Copy(imagePath, destPath);

                    CocoImageDTO cocoImage = new CocoImageDTO
                    {
                        Id = (i < trainCount) ? trainImageId++ : (i < trainCount + valCount) ? valImageId++ : testImageId++,
                        FileName = fullImageName,
                        Width = 1280,
                        Height = 1280,
                        DateCaptured = DateTime.Now.ToString("yyyy-MM-dd"),
                        License = 1
                    };

                    if (i < trainCount)
                    {
                        trainCocoDataset.Images.Add(cocoImage);
                    }
                    else if (i < trainCount + valCount)
                    {
                        valCocoDataset.Images.Add(cocoImage);
                    }
                    else
                    {
                        testCocoDataset.Images.Add(cocoImage);
                    }

                    // Add annotations to the respective dataset
                    IEnumerable<CocoAnnotationDTO> annotations = datasetExtClassesImagesAnnotations.ImageAnnotations
                        .Where(x => x.DatasetImageIdInt == image.IdInt)
                        .Select(x => new CocoAnnotationDTO
                        {
                            Id = (i < trainCount) ? trainAnnotationId++ : (i < trainCount + valCount) ? valAnnotationId++ : testAnnotationId++,
                            ImageId = cocoImage.Id,
                            CategoryId = datasetExtClassesImagesAnnotations.DatasetClassForDataset
                                .First(dcfd => dcfd.DatasetClassId == x.DatasetClass.Id).ClassValue,
                            IsCrowd = 0,
                            //Bbox = GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeightList(x.Geom),
                            Bbox = GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeightList(x.Geom),
                        });

                    if (i < trainCount)
                    {
                        trainCocoDataset.Annotations.AddRange(annotations);
                    }
                    else if (i < trainCount + valCount)
                    {
                        valCocoDataset.Annotations.AddRange(annotations);
                    }
                    else
                    {
                        testCocoDataset.Annotations.AddRange(annotations);
                    }
                }

                trainCocoDataset.Annotations.ForEach(a => a.Bbox.ForEach(coord => coord = (float)Math.Round(coord, 2)));
                valCocoDataset.Annotations.ForEach(a => a.Bbox.ForEach(coord => coord = (float)Math.Round(coord, 2)));
                testCocoDataset.Annotations.ForEach(a => a.Bbox.ForEach(coord => coord = (float)Math.Round(coord, 2)));

                string trainJson = NewtonsoftJsonHelper.Serialize(trainCocoDataset);
                string valJson = NewtonsoftJsonHelper.Serialize(valCocoDataset);
                string testJson = NewtonsoftJsonHelper.Serialize(testCocoDataset);

                await File.WriteAllTextAsync(Path.Combine(trainDir, "annotations_coco.json"), trainJson);
                await File.WriteAllTextAsync(Path.Combine(valDir, "annotations_coco.json"), valJson);
                await File.WriteAllTextAsync(Path.Combine(testDir, "annotations_coco.json"), testJson);

                // Zip the directories
                string zipFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");
                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);

                ZipFile.CreateFromDirectory(tempDirectory, zipFilePath);

                if (downloadLocation == null)
                    Directory.Delete(tempDirectory, true);

                return ResultDTO<string>.Ok(zipFilePath);
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<string>> ConvertDatasetEntityToCocoDatasetWithAssignedIdInts(Dataset dataset, string exportOption, string? downloadLocation)
        {
            if (dataset == null)
                return ResultDTO<string>.Fail("Dataset is null");

            try
            {
                ResultDTO<DatasetFullIncludeDTO> resultGetDatasetFullIncludeDTO =
                    GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity(datasetIncluded: dataset,
                                                                                exportOption: exportOption,
                                                                                includeDisabledImages: true,
                                                                                includeDisabledAnnotations: true);

                if (resultGetDatasetFullIncludeDTO.IsSuccess == false && resultGetDatasetFullIncludeDTO.HandleError())
                    return ResultDTO<string>.Fail(resultGetDatasetFullIncludeDTO.ErrMsg!);

                DatasetFullIncludeDTO datasetExtClassesImagesAnnotations = resultGetDatasetFullIncludeDTO.Data!;

                CocoDatasetDTO cocoDatasetDTO = new CocoDatasetDTO
                {
                    Images = datasetExtClassesImagesAnnotations.DatasetImages
                                .OrderBy(x => x.CreatedOn)
                                .Select(x => new CocoImageDTO
                                {
                                    Id = x.IdInt,
                                    FileName = x.Id.ToString() + ".jpg",
                                    Width = 1280,
                                    Height = 1280
                                }).ToList(),
                    Annotations = datasetExtClassesImagesAnnotations.ImageAnnotations
                                .Select(x => new CocoAnnotationDTO
                                {
                                    Id = x.IdInt,
                                    ImageId = x.DatasetImageIdInt,
                                    CategoryId = datasetExtClassesImagesAnnotations.DatasetClassForDataset
                                            .First(dcfd => dcfd.DatasetClassId == x.DatasetClass.Id).ClassValue,
                                    IsCrowd = 0,
                                    Bbox = GeoJsonHelpers.GeometryBBoxToTopLeftWidthHeightList(x.Geom)
                                }).ToList(),
                    Categories = datasetExtClassesImagesAnnotations.DatasetClassForDataset
                                .Select(d => new CocoCategoryDTO
                                {
                                    Id = d.ClassValue,
                                    Name = d.ClassName
                                }).ToList(),
                    Info = new CocoInfoDTO
                    {
                        Year = DateTime.Now.Year,
                        Version = "1.0",
                        Description = datasetExtClassesImagesAnnotations.Dataset.Description,
                        Contributor = "IllegalDumpSiteDetectionAndLandfillMonitoring",
                        DateCreated = datasetExtClassesImagesAnnotations.Dataset.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                    Licenses = new List<CocoLicenseDTO> { new CocoLicenseDTO() }
                };

                string cocoJson = JsonConvert.SerializeObject(cocoDatasetDTO, Formatting.Indented);

                string tempDirectory = downloadLocation ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDirectory);

                string jsonFilePath = Path.Combine(tempDirectory, "coco_dataset.json");
                await File.WriteAllTextAsync(jsonFilePath, cocoJson);

                foreach (var image in datasetExtClassesImagesAnnotations.DatasetImages)
                {
                    var fullImageName = image.Id.ToString() + Path.GetExtension(image.FileName);
                    string imagePath = Path.Combine("wwwroot", image.ImagePath.TrimStart('\\'), fullImageName);
                    string destPath = Path.Combine(tempDirectory, Path.GetFileName(imagePath));
                    File.Copy(imagePath, destPath);
                }

                if (downloadLocation == null)
                {
                    string zipFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");
                    if (File.Exists(zipFilePath))
                        File.Delete(zipFilePath);

                    ZipFile.CreateFromDirectory(tempDirectory, zipFilePath);

                    Directory.Delete(tempDirectory, true);

                    return ResultDTO<string>.Ok(zipFilePath);

                }

                return ResultDTO<string>.Ok(downloadLocation);
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Import
        public string GetWwwRootOrSaveDirectory(string? saveDir)
        {
            if (string.IsNullOrEmpty(saveDir) == false)
                return saveDir;

            string? applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (string.IsNullOrEmpty(applicationPath))
                return Path.Combine(Path.DirectorySeparatorChar.ToString(), "wwwroot");

            return Path.Combine(applicationPath, "wwwroot");
        }

        public async Task<ResultDTO<DatasetDTO>> ImportDatasetCocoFormatedAtDirectoryPath(string datasetName,
                                                                                          string cocoDirPath,
                                                                                          string userId,
                                                                                          string? saveDir = null,
                                                                                          bool allowUnannotatedImages = false)
        {
            string? datasetImgUploadAbsDir = null;

            try
            {
                string saveRoot = GetWwwRootOrSaveDirectory(saveDir);

                ResultDTO<CocoDatasetDTO> getCocoDatasetResult =
                    await _cocoUtilsService.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(cocoDirPath, allowUnannotatedImages);

                if (getCocoDatasetResult.IsSuccess == false && getCocoDatasetResult.HandleError() || getCocoDatasetResult.Data is null)
                    return ResultDTO<DatasetDTO>.Fail(getCocoDatasetResult.ErrMsg!);

                CocoDatasetDTO cocoDataset = getCocoDatasetResult.Data;
                Guid datasetEntityId = Guid.NewGuid();

                if (string.IsNullOrEmpty(userId))
                    return ResultDTO<DatasetDTO>.Fail("Invalid User Id");

                string datasetNameCalc = string.IsNullOrEmpty(datasetName) ? $"CocoDataset-{DateTime.UtcNow:yyyy-MM-dd}" : datasetName;
                string datasetDescription = cocoDataset.Info?.Description ?? $"Coco Dataset Imported at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";

                Dataset datasetEntity = new Dataset
                {
                    Id = datasetEntityId,
                    Name = datasetNameCalc,
                    Description = datasetDescription,
                    IsPublished = false,
                    CreatedById = userId,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedById = userId,
                    UpdatedOn = DateTime.UtcNow,
                    DatasetClasses = new List<Dataset_DatasetClass>(),
                    DatasetImages = new List<DatasetImage>()
                };

                Dictionary<int, Guid> cocoCategoriesToDatasetClassDict = new Dictionary<int, Guid>();

                foreach (CocoCategoryDTO category in cocoDataset.Categories)
                {
                    var existingDatasetClass = await _datasetClassesRepository.GetFirstOrDefault(x => x.ClassName == category.Name);
                    DatasetClass datasetClassEntity;

                    if (existingDatasetClass.IsSuccess && existingDatasetClass.Data != null)
                    {
                        datasetClassEntity = existingDatasetClass.Data;
                    }
                    else
                    {
                        datasetClassEntity = new DatasetClass
                        {
                            Id = Guid.NewGuid(),
                            ClassName = category.Name,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = null,
                            CreatedById = userId,

                            // TODO: Implement
                            ParentClass = null,
                            ParentClassId = null,
                        };
                        await _datasetClassesRepository.Create(datasetClassEntity);
                    }

                    cocoCategoriesToDatasetClassDict[category.Id] = datasetClassEntity.Id;

                    datasetEntity.DatasetClasses.Add(new Dataset_DatasetClass
                    {
                        Id = Guid.NewGuid(),
                        DatasetId = datasetEntityId,
                        DatasetClassId = datasetClassEntity.Id,
                        DatasetClassValue = category.Id
                    });
                }

                // Process Dataset Images
                List<DatasetImage> datasetImages = new List<DatasetImage>();
                Dictionary<int, Guid> cocoImagesToDatasetImagesDict = new Dictionary<int, Guid>();

                // Create Dataset Images Directory 
                ResultDTO<string> getImagesDirRelPathResult = await GetDatasetImagesDirectoryRelativePathByDatasetId(datasetEntityId);
                if (getImagesDirRelPathResult.IsSuccess == false && getImagesDirRelPathResult.HandleError())
                    return ResultDTO<DatasetDTO>.Fail(getImagesDirRelPathResult.ErrMsg!);

                string datasetImgUploadRelDir = getImagesDirRelPathResult.Data!;
                ResultDTO<string?> datasetThumbnailsFolder =
                    await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");

                ResultDTO<string> getImagesDirAbsPathResult = await GetDatasetImagesDirectoryAbsolutePathByDatasetId(saveRoot, datasetEntityId);
                if (getImagesDirAbsPathResult.IsSuccess == false && getImagesDirAbsPathResult.HandleError())
                    return ResultDTO<DatasetDTO>.Fail(getImagesDirAbsPathResult.ErrMsg!);

                datasetImgUploadAbsDir = getImagesDirAbsPathResult.Data!;
                if (!Directory.Exists(datasetImgUploadAbsDir))
                    Directory.CreateDirectory(datasetImgUploadAbsDir);

                foreach (CocoImageDTO img in cocoDataset.Images)
                {
                    Guid datasetImageId = Guid.NewGuid();
                    DatasetImage datasetImage = new DatasetImage
                    {
                        Id = datasetImageId,
                        DatasetId = datasetEntityId,
                        IsEnabled = false,
                        Name = Path.GetFileNameWithoutExtension(img.FileName),
                        FileName = datasetImageId.ToString() + Path.GetExtension(img.FileName),
                        ImagePath = Path.Combine(Path.DirectorySeparatorChar.ToString(), datasetImgUploadRelDir),
                        ThumbnailPath = Path.Combine(datasetThumbnailsFolder.Data, datasetEntityId.ToString()),
                        CreatedById = userId,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedById = userId,
                        UpdatedOn = DateTime.UtcNow,
                    };

                    cocoImagesToDatasetImagesDict.Add(img.Id, datasetImage.Id);
                    datasetImages.Add(datasetImage);

                    ResultDTO copyImageResult =
                        CopyImageFromSourcePathToDestinationPath(
                            Path.Combine(cocoDirPath, img.FileName),
                            Path.Combine(datasetImgUploadAbsDir, datasetImageId.ToString() + Path.GetExtension(img.FileName)));
                    if (copyImageResult.IsSuccess == false)
                        continue;
                }

                // Process Annotations
                List<ImageAnnotation> imageAnnotations = new List<ImageAnnotation>();
                foreach (CocoAnnotationDTO cocoAnnotation in cocoDataset.Annotations)
                {
                    ImageAnnotation imageAnnotationEntity = new ImageAnnotation
                    {
                        Id = Guid.NewGuid(),
                        IsEnabled = true,
                        Geom = GeoJsonHelpers.ConvertBoundingBoxToPolygon(cocoAnnotation.Bbox),
                        DatasetClassId = cocoCategoriesToDatasetClassDict.GetValueOrDefault(cocoAnnotation.CategoryId),
                        DatasetImageId = cocoImagesToDatasetImagesDict.GetValueOrDefault(cocoAnnotation.ImageId),
                        CreatedById = userId,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedById = userId,
                        UpdatedOn = DateTime.UtcNow,
                    };
                    imageAnnotations.Add(imageAnnotationEntity);
                }

                foreach (DatasetImage datasetImage in datasetImages)
                {
                    List<ImageAnnotation> datasetImageAnnotations =
                        imageAnnotations.Where(ann => ann.DatasetImageId == datasetImage.Id).ToList();
                    datasetImage.ImageAnnotations = datasetImageAnnotations;
                }

                datasetEntity.DatasetImages = datasetImages;

                // Save Dataset
                ResultDTO createDatasetResult = await _datasetsRepository.Create(datasetEntity);
                if (createDatasetResult.IsSuccess == false && createDatasetResult.HandleError())
                    return ResultDTO<DatasetDTO>.Fail(createDatasetResult.ErrMsg!);

                DatasetDTO? datasetDTO = _mapper.Map<DatasetDTO>(datasetEntity);
                if (datasetDTO is null)
                    return ResultDTO<DatasetDTO>.Fail("Failed mapping to Dataset DTO");

                return ResultDTO<DatasetDTO>.Ok(datasetDTO);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(datasetImgUploadAbsDir))
                    await DeleteAllFilesInDatasetDirectoryAtWwwRoot(datasetImgUploadAbsDir);

                return ResultDTO<DatasetDTO>.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Utils
        public async Task<ResultDTO> DeleteAllFilesInDatasetDirectoryAtWwwRoot(string deleteDirectoryPath)
        {
            if (string.IsNullOrEmpty(deleteDirectoryPath))
                return ResultDTO.Fail("Invalid Directory Path");

            try
            {
                IEnumerable<string> files = Directory.EnumerateFiles(deleteDirectoryPath);
                // Get all files in the directory
                foreach (string filePath in files)
                    File.Delete(filePath);

                return ResultDTO.Ok();
            }
            catch (IOException ioExp)
            {
                return ResultDTO.ExceptionFail(ioExp.Message, ioExp);
            }
        }

        public ResultDTO CopyImageFromSourcePathToDestinationPath(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                // Ensure that the target does not exist.
                if (File.Exists(destinationFilePath))
                    File.Delete(destinationFilePath);

                // Copy the file.
                File.Copy(sourceFilePath, destinationFilePath);

                return ResultDTO.Ok();
            }
            catch (IOException ioExp)
            {
                return ResultDTO.ExceptionFail(ioExp.Message, ioExp);
            }
        }

        public async Task<ResultDTO<string>> GetDatasetImagesDirectoryRelativePathByDatasetId(Guid datasetId)
        {
            ResultDTO<string?> getDatasetImagesDirResult =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages");
            if (getDatasetImagesDirResult.IsSuccess == false || string.IsNullOrEmpty(getDatasetImagesDirResult.Data))
                return ResultDTO<string>.Fail("Error getting base Dataset Images Directory");

            string thePath = Path.Combine(getDatasetImagesDirResult.Data, datasetId.ToString());
            return ResultDTO<string>.Ok(thePath);
        }

        public async Task<ResultDTO<string>> GetDatasetImagesDirectoryAbsolutePathByDatasetId(string wwwRoot, Guid datasetId)
        {
            if (string.IsNullOrEmpty(wwwRoot))
                return ResultDTO<string>.Fail("Invalid wwwRoot Path");

            ResultDTO<string> getDatasetRelativePathResult = await GetDatasetImagesDirectoryRelativePathByDatasetId(datasetId);
            if (getDatasetRelativePathResult.IsSuccess == false || string.IsNullOrEmpty(getDatasetRelativePathResult.Data))
                return ResultDTO<string>.Fail(getDatasetRelativePathResult.ErrMsg!);

            string thePath = Path.Combine(wwwRoot, getDatasetRelativePathResult.Data);
            return ResultDTO<string>.Ok(thePath);
        }

        public async Task<ResultDTO<string>> GetDatasetImageRelativePathByDatasetIdAndFileName(Guid datasetId, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return ResultDTO<string>.Fail("Invalid File Name");

            ResultDTO<string> getDatasetRelativePathResult = await GetDatasetImagesDirectoryRelativePathByDatasetId(datasetId);
            if (getDatasetRelativePathResult.IsSuccess == false || string.IsNullOrEmpty(getDatasetRelativePathResult.Data))
                return ResultDTO<string>.Fail(getDatasetRelativePathResult.ErrMsg!);

            string thePath = Path.Combine(getDatasetRelativePathResult.Data, fileName);

            return ResultDTO<string>.Ok(thePath);
        }

        public async Task<ResultDTO<string>> GetDatasetImageAbsolutePathByDatasetIdAndFileName(string wwwRoot, Guid datasetId, string fileName)
        {
            if (string.IsNullOrEmpty(wwwRoot))
                return ResultDTO<string>.Fail("Invalid wwwRoot Path");

            ResultDTO<string> getDatasetImageRelativePathResult = await GetDatasetImageRelativePathByDatasetIdAndFileName(datasetId, fileName);
            if (getDatasetImageRelativePathResult.IsSuccess == false || string.IsNullOrEmpty(getDatasetImageRelativePathResult.Data))
                return ResultDTO<string>.Fail(getDatasetImageRelativePathResult.ErrMsg!);

            string thePath = Path.Combine(wwwRoot, getDatasetImageRelativePathResult.Data);

            return ResultDTO<string>.Ok(thePath);
        }
        #endregion
    }
}