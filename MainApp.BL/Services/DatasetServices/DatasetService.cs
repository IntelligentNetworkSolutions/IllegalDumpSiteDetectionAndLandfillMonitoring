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

        public DatasetService(IDatasetsRepository datasetsRepository,
                              IDatasetClassesRepository datasetClassesRepository,
                              IDataset_DatasetClassRepository datasetDatasetClassRepository,
                              IDatasetImagesRepository datasetImagesRepository,
                              IImageAnnotationsRepository imageAnnotationsRepository,
                              IAppSettingsAccessor appSettingsAccessor,
                              IMapper mapper,
                              ICocoUtilsService cocoUtilsService)
        {
            _datasetsRepository = datasetsRepository;
            _datasetClassesRepository = datasetClassesRepository;
            _datasetDatasetClassRepository = datasetDatasetClassRepository;
            _datasetImagesRepository = datasetImagesRepository;
            _imageAnnotationsRepository = imageAnnotationsRepository;
            _appSettingsAccessor = appSettingsAccessor;
            _mapper = mapper;
            _cocoUtilsService = cocoUtilsService;
        }

        #region Read
        public async Task<CreateDatasetDTO> FillDatasetDto(CreateDatasetDTO dto)
        {
            dto ??= new CreateDatasetDTO();
            if (dto.Id == Guid.Empty)
            {
                var allClasses = await _datasetClassesRepository.GetAll(includeProperties: "ParentClass,Datasets") ?? throw new Exception("Object not found");
                dto.AllDatasetClasses = _mapper.Map<List<DatasetClassDTO>>(allClasses.Data);
            }
            return dto;
        }

        public async Task<List<DatasetDTO>> GetAllDatasets()
        {
            var listDatasets = await _datasetsRepository.GetAll(includeProperties: "CreatedBy,UpdatedBy,ParentDataset");
            var data = listDatasets.Data ?? throw new Exception("Object not found");
            var listDatasetsDTOs = _mapper.Map<List<DatasetDTO>>(data) ?? throw new Exception("Dataset list not found");
            return listDatasetsDTOs;
        }
        public async Task<DatasetDTO> GetDatasetById(Guid datasetId)
        {
            //var datasetRawDto = GetDatasetJoinedClassesImagesAnnotationsById(datasetId);

            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var data = datasetDb.Data ?? throw new Exception("Object not found");
            var datasetDto = _mapper.Map<DatasetDTO>(data) ?? throw new Exception("Object not found");
            return datasetDto;
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

        public async Task<EditDatasetDTO> GetObjectForEditDataset(Guid datasetId, string? searchByImageName, bool? searchByIsAnnotatedImage, bool? searchByIsEnabledImage, string? orderByImages, int pageNumber, int pageSize)
        {
            var taskDatasetDatasetClasses = await _datasetDatasetClassRepository.GetAll(null, null, false, includeProperties: "DatasetClass,Dataset") ?? throw new Exception("Object not found"); ;
            var taskDatasetClasses = await _datasetClassesRepository.GetAll(null, null, false, includeProperties: "ParentClass,Datasets") ?? throw new Exception("Object not found"); ;
            var taskCurrentDataset = await _datasetsRepository.GetByIdIncludeThenAll(datasetId, false,
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
                }) ?? throw new Exception("Object not found"); ;
            var taskNumberOfImagesToPublish = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100) ?? throw new Exception("Object not found"); ;
            var taskNumberOfClassesToPublish = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1) ?? throw new Exception("Object not found"); ;

            var imageResult = await _datasetImagesRepository.GetAll(x => x.DatasetId == datasetId, null, false, includeProperties: "ImageAnnotations") ?? throw new Exception("Object not found");

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

            var insertedClasses = taskDatasetDatasetClasses?.Data?.Where(x => x.DatasetId == datasetId).Select(x => x.DatasetClass).ToList() ?? throw new Exception("Object not found"); ;
            var insertedClassesIds = taskDatasetDatasetClasses?.Data?.Where(x => x.DatasetId == datasetId).Select(x => x.DatasetClassId).ToList() ?? throw new Exception("Object not found"); ;
            var uninsertedRootClasses = taskDatasetClasses?.Data?.Where(x => !insertedClassesIds.Contains(x.Id) && x.ParentClassId == null).ToList() ?? throw new Exception("Object not found"); ;
            var uninsertedSubclasses = taskDatasetClasses?.Data?.Where(x => !insertedClassesIds.Contains(x.Id) && x.ParentClassId != null).ToList() ?? throw new Exception("Object not found"); ;

            var dto = new EditDatasetDTO
            {
                UninsertedDatasetRootClasses = _mapper.Map<List<DatasetClassDTO>>(uninsertedRootClasses),
                UninsertedDatasetSubclasses = _mapper.Map<List<DatasetClassDTO>>(uninsertedSubclasses),
                ClassesByDatasetId = _mapper.Map<List<DatasetClassDTO>>(insertedClasses),
                NumberOfDatasetClasses = insertedClasses?.Count ?? 0,
                CurrentDataset = _mapper.Map<DatasetDTO>(taskCurrentDataset.Data),
                NumberOfChildrenDatasets = taskDatasetDatasetClasses?.Data?.Where(x => x.Dataset.ParentDatasetId == datasetId).Count() ?? 0,
                ParentDatasetClasses = _mapper.Map<List<DatasetClassDTO>>(taskDatasetDatasetClasses?.Data?.Where(x => x.DatasetId == taskCurrentDataset.Data?.ParentDatasetId).Select(x => x.DatasetClass).ToList()) ?? throw new Exception("Object not found"),
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

            return dto;
        }
        #endregion

        #region Create
        public async Task<DatasetDTO> CreateDataset(DatasetDTO dto)
        {
            Dataset dataset = _mapper.Map<Dataset>(dto);
            var insertedDatasetResult = await _datasetsRepository.CreateAndReturnEntity(dataset) ?? throw new Exception("Object not found");
            var insertedDataset = insertedDatasetResult.Data ?? throw new Exception("Object not found");
            DatasetDTO newDTO = _mapper.Map<DatasetDTO>(insertedDataset);
            return newDTO;
        }
        public async Task<ResultDTO<int>> AddDatasetClassForDataset(Guid selectedClassId, Guid datasetId, string userId)
        {
            Dataset_DatasetClass dataset_datasetClass = new()
            {
                DatasetId = datasetId,
                DatasetClassId = selectedClassId
            };

            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var datasetDbData = datasetDb.Data ?? throw new Exception("Object not found");
            var createdDatasetClass = await _datasetDatasetClassRepository.Create(dataset_datasetClass);
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
            var parentClassesList = await _datasetDatasetClassRepository.GetAll(filter: x => x.DatasetId == parentDatasetId) ?? throw new Exception("Object not found");
            var parentClassesIdsList = parentClassesList.Data?.Select(x => x.DatasetClassId).ToList() ?? throw new Exception("Object not found");

            List<Dataset_DatasetClass> dataset_DatasetClasses = new();
            foreach (var item in parentClassesIdsList)
            {
                Dataset_DatasetClass dataset_class = new();
                dataset_class.DatasetId = insertedDatasetId;
                dataset_class.DatasetClassId = item;
                dataset_DatasetClasses.Add(dataset_class);
            }
            var isAdded = await _datasetDatasetClassRepository.CreateRange(dataset_DatasetClasses);
            if (isAdded.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset classes were not added", null);
            }
        }


        #endregion

        #region Update
        public async Task<ResultDTO<int>> PublishDataset(Guid datasetId, string userId)
        {
            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var datasetDbData = datasetDb.Data ?? throw new Exception("Object not found");
            var allDataset_DatasetClasses = await _datasetDatasetClassRepository.GetAll(includeProperties: "DatasetClass,Dataset") ?? throw new Exception("Object not found");

            var allDatasetImages = await _datasetImagesRepository.GetAll(filter: x => x.DatasetId == datasetDbData.Id, includeProperties: "ImageAnnotations") ?? throw new Exception("Object not found");
            var allDatasetImagesData = allDatasetImages.Data ?? throw new Exception("Object not found");
            var enabledImagesList = allDatasetImagesData.Where(x => x.IsEnabled == true).ToList() ?? throw new Exception("Object not found");
            var allImageAnnotationsList = await _imageAnnotationsRepository.GetAll() ?? throw new Exception("Object not found");
            var allEnabledImagesHaveAnnotations = enabledImagesList.Any() ?
                            enabledImagesList.All(x => allImageAnnotationsList.Data.Where(m => m.DatasetImageId == x.Id).Select(x => x.DatasetImageId).Any(a => a == x.Id)) : false;
            var nubmerOfImagesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100);
            var nubmerOfClassesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1);
            var insertedClasses = allDataset_DatasetClasses.Data?.Where(x => x.DatasetId == datasetDbData.Id).Select(x => x.DatasetClass).ToList() ?? throw new Exception("Object not found");

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
            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var datasetDbData = datasetDb.Data ?? throw new Exception("Object not found");
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
        #endregion

        #region Delete
        public async Task<ResultDTO<int>> DeleteDatasetClassForDataset(Guid selectedClassId, Guid datasetId, string userId)
        {
            var result = await _datasetDatasetClassRepository.GetFirstOrDefault(filter: x => x.DatasetId == datasetId && x.DatasetClassId == selectedClassId) ?? throw new Exception("Dataset not found");
            Dataset_DatasetClass dataset_DatasetClassDb = result.Data ?? throw new Exception("Object not found");
            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var datasetDbData = datasetDb.Data ?? throw new Exception("Object not found");
            var deletedDataset_DatasetClass = await _datasetDatasetClassRepository.Delete(dataset_DatasetClassDb);
            datasetDbData.UpdatedOn = DateTime.UtcNow;
            datasetDbData.UpdatedById = userId;

            await _datasetsRepository.Update(datasetDbData);
            if (deletedDataset_DatasetClass.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset class was not deleted", null);
            }
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
            ResultDTO deleteDatasetImageAnnotationsResult = await _imageAnnotationsRepository.DeleteRange(annotations, false);
            if (deleteDatasetImageAnnotationsResult.IsSuccess == false && ResultDTO.HandleError(deleteDatasetImageAnnotationsResult))
                return ResultDTO.Fail(deleteDatasetImageAnnotationsResult.ErrMsg!);

            // Delete Images
            ResultDTO deleteDatasetImagesResult = await _datasetImagesRepository.DeleteRange(datasetImages, false);
            if (deleteDatasetImagesResult.IsSuccess == false && ResultDTO.HandleError(deleteDatasetImagesResult))
                return ResultDTO.Fail(deleteDatasetImagesResult.ErrMsg!);

            // Delete Dataset_DatasetClasses
            ResultDTO deleteDatasetDatasetClassesResult = await _datasetDatasetClassRepository.DeleteRange(datasetDatasetClasses, false);
            if (deleteDatasetDatasetClassesResult.IsSuccess == false && ResultDTO.HandleError(deleteDatasetDatasetClassesResult))
                return ResultDTO.Fail(deleteDatasetDatasetClassesResult.ErrMsg!);

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

        public async Task<ResultDTO<int>> DeleteDataset(Guid datasetId)
        {
            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var datasetDbData = datasetDb.Data ?? throw new Exception("Object not found");

            var listOfAllDatasets = await _datasetsRepository.GetAll(includeProperties: "CreatedBy") ?? throw new Exception("Datasets not found");
            var listOfAllDatasetsData = listOfAllDatasets.Data ?? throw new Exception("Object not found");
            var childrenDatasets = listOfAllDatasetsData.Where(x => x.ParentDatasetId == datasetId).ToList();
            if (childrenDatasets.Count > 0)
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "This dataset can not be deleted because there are subdatasets. Delete first the subdatasets!", null);
            }
            var list_dataset_datasetClassDb = await _datasetDatasetClassRepository.GetAll(filter: x => x.DatasetId == datasetId) ?? throw new Exception("List not found");
            var list_dataset_datasetClassDb_data = list_dataset_datasetClassDb.Data ?? throw new Exception("Object not found");
            if (list_dataset_datasetClassDb_data.Count() > 0)
            {
                await _datasetDatasetClassRepository.DeleteRange(list_dataset_datasetClassDb_data);
            }
            var datasetImagesList = await _datasetImagesRepository.GetAll(filter: x => x.DatasetId == datasetId) ?? throw new Exception("Object not found");
            var datasetImagesListData = datasetImagesList.Data ?? throw new Exception("Object not found");
            if (datasetImagesListData.Count() > 0)
            {
                await _datasetImagesRepository.DeleteRange(datasetImagesListData);
            }
            var isDeleted = await _datasetsRepository.Delete(datasetDbData);
            if (isDeleted.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 3, isDeleted.ErrMsg, null);
            }
        }
        #endregion

        #region Export
        // TODO: Create DatsetExportOption Enum/Static Class to replace magic string param
        private ResultDTO<DatasetFullIncludeDTO> GetDatasetFullIncludeDTOWithIdIntsFromDatasetIncludedEntity
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

                DatasetFullIncludeDTO datasetFullIncludeDTO = new DatasetFullIncludeDTO(_mapper.Map<DatasetDTO>(datasetIncluded))
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

                if (resultDatasetIncludeThenAll.IsSuccess == false)
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

        private async Task<ResultDTO<string>> ConvertDatasetEntityToCocoDatasetWithAssignedIdIntsAsSplitDataset(Dataset dataset, string exportOption, string? downloadLocation)
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

                if (!resultGetDatasetFullIncludeDTO.IsSuccess)
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

        private async Task<ResultDTO<string>> ConvertDatasetEntityToCocoDatasetWithAssignedIdInts(Dataset dataset, string exportOption, string? downloadLocation)
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

                if (!resultGetDatasetFullIncludeDTO.IsSuccess)
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
                return Path.Combine("\\", "wwwroot");

            return Path.Combine(applicationPath, "wwwroot");
        }

        public async Task<ResultDTO<DatasetDTO>> ImportDatasetCocoFormatedAtDirectoryPath(string datasetName, string cocoDirPath, string userId, string? saveDir = null, bool allowUnannotatedImages = false)
        {
            string? datasetImgUploadAbsDir = null;
            try
            {
                string saveRoot = GetWwwRootOrSaveDirectory(saveDir);

                ResultDTO<CocoDatasetDTO> getCocoDatasetResult =
                    await _cocoUtilsService.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(cocoDirPath, allowUnannotatedImages);
                if ((getCocoDatasetResult.IsSuccess == false && ResultDTO<CocoDatasetDTO>.HandleError(getCocoDatasetResult))
                    || getCocoDatasetResult.Data is null)
                    return ResultDTO<DatasetDTO>.Fail(getCocoDatasetResult.ErrMsg!);

                CocoDatasetDTO cocoDataset = getCocoDatasetResult.Data;

                Guid datasetEntityId = Guid.NewGuid();

                if (string.IsNullOrEmpty(userId))
                    return ResultDTO<DatasetDTO>.Fail("Invalid User Id");

                string datasetNameCalc = string.IsNullOrEmpty(datasetName) == false
                                        ? datasetName
                                        : "CocoDataset-" + DateTime.UtcNow.ToString("yyyy-MM-dd");

                string datasetDescription = cocoDataset.Info is not null && string.IsNullOrEmpty(cocoDataset.Info.Description) == false
                                            ? cocoDataset.Info.Description
                                            : "Coco Dataset Imported at: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                Dataset datasetEntity = new Dataset()
                {
                    Id = datasetEntityId,
                    Name = datasetName,
                    Description = datasetDescription,
                    IsPublished = false,

                    CreatedBy = null,
                    CreatedById = userId,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedBy = null,
                    UpdatedById = userId,
                    UpdatedOn = DateTime.UtcNow,

                    // TODO: Implement
                    ParentDataset = null,
                    ParentDatasetId = null,
                    AnnotationsPerSubclass = false,
                };

                List<DatasetClass> datasetClassEntities = new List<DatasetClass>();
                List<Dataset_DatasetClass> datasetDatasetClassesEntities = new List<Dataset_DatasetClass>();
                Dictionary<int, Guid> cocoCategoriesToDatasetClassDict = new Dictionary<int, Guid>();
                Dictionary<int, Guid> cocoCategoriesToDatasetDatasetClassDict = new Dictionary<int, Guid>();

                foreach (CocoCategoryDTO category in cocoDataset.Categories)
                {
                    DatasetClass datasetClassEntity = new DatasetClass()
                    {
                        Id = Guid.NewGuid(),
                        ClassName = category.Name,

                        CreatedOn = DateTime.Now,
                        CreatedBy = null,
                        CreatedById = userId,

                        // TODO: Implement
                        ParentClass = null,
                        ParentClassId = null,
                    };
                    datasetClassEntities.Add(datasetClassEntity);
                    cocoCategoriesToDatasetClassDict.Add(category.Id, datasetClassEntity.Id);

                    Dataset_DatasetClass datasetDatasetClassEntity = new Dataset_DatasetClass()
                    {
                        Id = Guid.NewGuid(),
                        Dataset = datasetEntity,
                        DatasetId = datasetEntityId,
                        DatasetClassValue = category.Id,
                        DatasetClass = datasetClassEntity,
                        DatasetClassId = datasetClassEntity.Id,
                    };
                    datasetDatasetClassesEntities.Add(datasetDatasetClassEntity);
                    cocoCategoriesToDatasetDatasetClassDict.Add(category.Id, datasetDatasetClassEntity.Id);
                }

                // Assign Dataset Classes
                datasetEntity.DatasetClasses = datasetDatasetClassesEntities;

                // Dataset Images
                List<DatasetImage> datasetImages = new List<DatasetImage>();
                Dictionary<int, Guid> cocoImagesToDatasetImagesDict = new Dictionary<int, Guid>();

                // Create Dataset Images Directory 
                ResultDTO<string> getImagesDirRelPathResult = await GetDatasetImagesDirectoryRelativePathByDatasetId(datasetEntityId);
                if (getImagesDirRelPathResult.IsSuccess == false)
                    return ResultDTO<DatasetDTO>.Fail(getImagesDirRelPathResult.ErrMsg!);

                string datasetImgUploadRelDir = getImagesDirRelPathResult.Data!;
                ResultDTO<string?> datasetThumbnailsFolder =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetThumbnailsFolder", "DatasetThumbnails");

                ResultDTO<string> getImagesDirAbsPathResult = await GetDatasetImagesDirectoryAbsolutePathByDatasetId(saveRoot, datasetEntityId);
                if (getImagesDirAbsPathResult.IsSuccess == false)
                    return ResultDTO<DatasetDTO>.Fail(getImagesDirAbsPathResult.ErrMsg!);

                datasetImgUploadAbsDir = getImagesDirAbsPathResult.Data!;
                if (Directory.Exists(datasetImgUploadAbsDir) == false)
                    Directory.CreateDirectory(datasetImgUploadAbsDir);

                foreach (CocoImageDTO img in cocoDataset.Images)
                {
                    Guid datasetImageId = Guid.NewGuid();
                    DatasetImage datasetImage = new DatasetImage()
                    {
                        Id = datasetImageId,
                        DatasetId = datasetEntityId,
                        IsEnabled = false,
                        Name = Path.GetFileNameWithoutExtension(img.FileName),
                        FileName = datasetImageId.ToString() + Path.GetExtension(img.FileName),
                        ImagePath = "\\" + datasetImgUploadRelDir + "\\",
                        ThumbnailPath = Path.Combine(datasetThumbnailsFolder.Data, datasetEntityId.ToString()),

                        CreatedBy = null,
                        CreatedById = userId,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedBy = null,
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
                    // TODO: REVIEW !!!
                }

                // Annotations
                List<ImageAnnotation> imageAnnotations = new List<ImageAnnotation>();
                foreach (CocoAnnotationDTO cocoAnnotation in cocoDataset.Annotations)
                {
                    ImageAnnotation imageAnnotationEntity = new ImageAnnotation()
                    {
                        Id = Guid.NewGuid(),
                        IsEnabled = true,

                        Geom = GeoJsonHelpers.ConvertBoundingBoxToPolygon(cocoAnnotation.Bbox),

                        DatasetClassId = cocoCategoriesToDatasetClassDict.GetValueOrDefault(cocoAnnotation.CategoryId),
                        DatasetClass = null,

                        DatasetImageId = cocoImagesToDatasetImagesDict.GetValueOrDefault(cocoAnnotation.ImageId),
                        DatasetImage = null,

                        CreatedBy = null,
                        CreatedById = userId,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedBy = null,
                        UpdatedById = userId,
                        UpdatedOn = DateTime.UtcNow,
                    };
                    imageAnnotations.Add(imageAnnotationEntity);
                }

                // Assign Annotations to Images
                foreach (DatasetImage datasetImage in datasetImages)
                {
                    List<ImageAnnotation> datasetImageAnnotations =
                        imageAnnotations.Where(ann => ann.DatasetImageId == datasetImage.Id).ToList();
                    datasetImage.ImageAnnotations = datasetImageAnnotations;
                }

                datasetEntity.DatasetImages = datasetImages;

                ResultDTO createDatasetResult = await _datasetsRepository.Create(datasetEntity);
                if (createDatasetResult.IsSuccess == false)
                    return ResultDTO<DatasetDTO>.Fail(createDatasetResult.ErrMsg!);

                DatasetDTO? datasetDTO = _mapper.Map<DatasetDTO>(datasetEntity);
                if (datasetDTO is null)
                    return ResultDTO<DatasetDTO>.Fail("Failed mapping to Dataset DTO");

                return ResultDTO<DatasetDTO>.Ok(datasetDTO);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(datasetImgUploadAbsDir) == false)
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