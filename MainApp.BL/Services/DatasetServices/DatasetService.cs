using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using Microsoft.EntityFrameworkCore;
using SD;

namespace MainApp.BL.Services.DatasetServices
{
    public class DatasetService : IDatasetService
    {
        private readonly IDatasetsRepository _datasetsRepository;
        private readonly IDatasetClassesRepository _datasetClassesRepository;
        private readonly IDataset_DatasetClassRepository _datasetDatasetClassRepository;
        private readonly IDatasetImagesRepository _datasetImagesRepository;
        private readonly IImageAnnotationsRepository _imageAnnotationsRepository;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IMapper _mapper;

        public DatasetService(IDatasetsRepository datasetsRepository, 
                              IDatasetClassesRepository datasetClassesRepository, 
                              IDataset_DatasetClassRepository datasetDatasetClassRepository, 
                              IDatasetImagesRepository datasetImagesRepository,
                              IImageAnnotationsRepository imageAnnotationsRepository,
                              IAppSettingsAccessor appSettingsAccessor,
                              IMapper mapper)
        {
            _datasetsRepository = datasetsRepository;
            _datasetClassesRepository = datasetClassesRepository;
            _datasetDatasetClassRepository = datasetDatasetClassRepository;
            _datasetImagesRepository = datasetImagesRepository;
            _imageAnnotationsRepository = imageAnnotationsRepository;
            _appSettingsAccessor = appSettingsAccessor;
            _mapper = mapper;
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
            var datasetDb = await _datasetsRepository.GetById(datasetId,includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var data = datasetDb.Data ?? throw new Exception("Object not found");
            var datasetDto = _mapper.Map<DatasetDTO>(data) ?? throw new Exception("Object not found");
            return datasetDto;
        }

        public async Task<EditDatasetDTO> GetObjectForEditDataset(Guid datasetId, string? searchByImageName, bool? searchByIsAnnotatedImage, bool? searchByIsEnabledImage, string? orderByImages)
        {
            var allDataset_DatasetClasses = await _datasetDatasetClassRepository.GetAll(includeProperties: "DatasetClass,Dataset") ?? throw new Exception("Object not found");
            var allClasses = await _datasetClassesRepository.GetAll(includeProperties: "ParentClass,Datasets") ?? throw new Exception("Model not found");

            var insertedClasses = allDataset_DatasetClasses.Data?.Where(x => x.DatasetId == datasetId).Select(x => x.DatasetClass).ToList() ?? throw new Exception("Object not found");
            var insertedClassesIds = allDataset_DatasetClasses.Data?.Where(x => x.DatasetId == datasetId).Select(x => x.DatasetClassId).ToList() ?? throw new Exception("Object not found");
            var uninsertedRootClasses = allClasses.Data?.Where(x => !insertedClassesIds.Contains(x.Id) && x.ParentClassId == null).ToList() ?? throw new Exception("Object not found");
            var uninsertedSubclasses = allClasses.Data?.Where(x => !insertedClassesIds.Contains(x.Id) && x.ParentClassId != null).ToList() ?? throw new Exception("Object not found");
            var currentDataset = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var currentDatasetData = currentDataset.Data ?? throw new Exception("Object not found");

            var allDatasetImagesUnfiltered = await _datasetImagesRepository.GetAll(filter: x => x.DatasetId == datasetId) ?? throw new Exception("Object not found");
            var allDatasetImagesUnfilteredData = allDatasetImagesUnfiltered.Data ?? throw new Exception("Object not found");
            var allDatasetImages = await _datasetImagesRepository.GetAll(filter: x => x.DatasetId == datasetId) ?? throw new Exception("Object not found");
            var allDatasetImagesData = allDatasetImages.Data ?? throw new Exception("Object not found");
            if (allDatasetImagesData.Count() > 0 && !string.IsNullOrEmpty(searchByImageName))
            {
                allDatasetImagesData = allDatasetImagesData.Where(x => x.Name == searchByImageName).ToList();
            }
            if (allDatasetImagesData.Count() > 0 && !string.IsNullOrEmpty(orderByImages))
            {
                if(orderByImages == "ascName")
                {
                    allDatasetImagesData = allDatasetImagesData.OrderBy(x => x.Name).ToList();
                }
                if (orderByImages == "descName")
                {
                    allDatasetImagesData = allDatasetImagesData.OrderByDescending(x => x.Name).ToList();
                }
                if (orderByImages == "ascCreatedOn")
                {
                    allDatasetImagesData = allDatasetImagesData.OrderByDescending(x => x.CreatedOn).ToList();
                }
                if (orderByImages == "descCreatedOn")
                {                    
                    allDatasetImagesData = allDatasetImagesData.OrderBy(x => x.CreatedOn).ToList();
                }
            }
            if (allDatasetImagesData.Count() > 0 && searchByIsEnabledImage is not null)
            {
                allDatasetImagesData = allDatasetImagesData.Where(x => x.IsEnabled == searchByIsEnabledImage).ToList();
            }

            var imageAnnotationsDbList = await _imageAnnotationsRepository.GetAll() ?? throw new Exception("Object not found");
            var allImageAnnotationsList = imageAnnotationsDbList.Data?.Where(x => x.IsEnabled == true).ToList();
            var annotationsForDatasetImages = allImageAnnotationsList?.Where(x => allDatasetImagesData.Select(m => m.Id).ToList().Contains(x.DatasetImageId.Value)).ToList();
            if (searchByIsAnnotatedImage is not null)
            {                
                if(searchByIsAnnotatedImage == true)
                {
                    allDatasetImagesData = allDatasetImagesData.Where(x => annotationsForDatasetImages.Select(m => m.DatasetImageId).Contains(x.Id)).ToList();
                }
                if(searchByIsAnnotatedImage == false)
                {
                    allDatasetImagesData = allDatasetImagesData.Where(x => !annotationsForDatasetImages.Select(m => m.DatasetImageId).Contains(x.Id)).ToList();
                }
            }


            var numberOfEnabledImages = allDatasetImagesData.Where(x => x.IsEnabled == true).Count();
            var numberOfAnnotatedImages = allImageAnnotationsList.Where(x => allDatasetImagesData.Select(m => m.Id).Contains(x.DatasetImageId.Value)).Count();
            var enabledImagesList = allDatasetImagesData.Where(x => x.IsEnabled == true).ToList() ?? throw new Exception("Object not found");
            var allEnabledImagesHaveAnnotations = enabledImagesList.Any() ?
                             enabledImagesList.All(x => allImageAnnotationsList.Where(x => x.DatasetImageId == x.Id).Select(x => x.DatasetImageId).Any(a => a == x.Id)) : false;
            var nubmerOfImagesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100);
            var nubmerOfClassesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1);
            var classesForParentDataset = allDataset_DatasetClasses.Data?.Where(x => x.DatasetId == currentDatasetData.ParentDatasetId).Select(x => x.DatasetClass).ToList();
            var numOfChildrenDatasetsByDatasetIdResult = await _datasetsRepository.GetAll(filter: x => x.ParentDatasetId == datasetId, includeProperties: "ParentDataset") ?? throw new Exception("Object not found");
            var numOfChildrenDatasetsByDatasetIdData = numOfChildrenDatasetsByDatasetIdResult.Data ?? throw new Exception("Object not found");
           
            EditDatasetDTO dto = new()
            {
                UninsertedDatasetRootClasses = _mapper.Map<List<DatasetClassDTO>>(uninsertedRootClasses),
                UninsertedDatasetSubclasses = _mapper.Map<List<DatasetClassDTO>>(uninsertedSubclasses),
                ClassesByDatasetId = _mapper.Map<List<DatasetClassDTO>>(insertedClasses),
                NumberOfDatasetClasses = insertedClasses.Count,
                CurrentDataset = _mapper.Map<DatasetDTO>(currentDatasetData),
                NumberOfChildrenDatasets = numOfChildrenDatasetsByDatasetIdData.Count(),
                ParentDatasetClasses = _mapper.Map<List<DatasetClassDTO>>(classesForParentDataset),
                ListOfDatasetImages = _mapper.Map<List<DatasetImageDTO>>(allDatasetImagesData),
                NumberOfDatasetImages = allDatasetImagesData.Count(),
                AllImageAnnotations = _mapper.Map<List<ImageAnnotationDTO>>(allImageAnnotationsList),
                NumberOfEnabledImages = numberOfEnabledImages,
                NumberOfAnnotatedImages = numberOfAnnotatedImages,
                AllEnabledImagesHaveAnnotations = allEnabledImagesHaveAnnotations,
                NumberOfClassesNeededToPublishDataset = nubmerOfClassesNeededToPublishDataset.Data,
                NumberOfImagesNeededToPublishDataset = nubmerOfImagesNeededToPublishDataset.Data,
                ListOfAllDatasetImagesUnFiltered = _mapper.Map<List<DatasetImageDTO>>(allDatasetImagesUnfilteredData)
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
            if(createdDatasetClass.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset class was not added", null);
            }
        }

        public async Task<ResultDTO<int>> AddInheritedParentClasses(Guid insertedDatasetId,  Guid parentDatasetId)
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
            var isAdded =  await _datasetDatasetClassRepository.CreateRange(dataset_DatasetClasses);
            if(isAdded.IsSuccess == true)
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

            var allDatasetImages = await _datasetImagesRepository.GetAll(filter: x => x.DatasetId == datasetDbData.Id) ?? throw new Exception("Object not found");
            var allDatasetImagesData = allDatasetImages.Data ?? throw new Exception("Object not found");
            var enabledImagesList = allDatasetImagesData.Where(x => x.IsEnabled == true).ToList() ?? throw new Exception("Object not found");
            var allImageAnnotationsList = await _imageAnnotationsRepository.GetAll() ?? throw new Exception("Object not found");
            var allEnabledImagesHaveAnnotations = enabledImagesList.Any() ?
                            enabledImagesList.All(x => allImageAnnotationsList.Data.Where(x => x.DatasetImageId == x.Id).Select(x => x.DatasetImageId).Any(a => a == x.Id)) : false;
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
            if(updatedDataset.IsSuccess == true)
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
            if(updatedDataset.IsSuccess == true)
            {
               return new ResultDTO<int> (IsSuccess: true, 1, null, null);
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
            var deletedDataset_DatasetClass =  await _datasetDatasetClassRepository.Delete(dataset_DatasetClassDb);
            datasetDbData.UpdatedOn = DateTime.UtcNow;
            datasetDbData.UpdatedById = userId;

            await _datasetsRepository.Update(datasetDbData);
            if(deletedDataset_DatasetClass.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "Dataset class was not deleted", null);
            }
        }

        public async Task<ResultDTO<int>> DeleteDataset(Guid datasetId)
        {
            var datasetDb = await _datasetsRepository.GetById(datasetId, includeProperties: "CreatedBy,UpdatedBy,ParentDataset") ?? throw new Exception("Object not found");
            var datasetDbData = datasetDb.Data ?? throw new Exception("Object not found");

            var listOfAllDatasets = await _datasetsRepository.GetAll(includeProperties: "CreatedBy") ?? throw new Exception("Datasets not found");
            var listOfAllDatasetsData = listOfAllDatasets.Data ?? throw new Exception("Object not found");
            var childrenDatasets = listOfAllDatasetsData.Where(x => x.ParentDatasetId == datasetId).ToList();
            if(childrenDatasets.Count > 0)
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



    }
}
