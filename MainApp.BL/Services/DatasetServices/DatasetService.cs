using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using Microsoft.EntityFrameworkCore;

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
                var allClasses = await _datasetClassesRepository.GetAllDatasetClasses();
                dto.AllDatasetClasses = _mapper.Map<List<DatasetClassDTO>>(allClasses);
            }
            return dto;
        }

        public async Task<List<DatasetDTO>> GetAllDatasets()
        {
            var listDatasets = await _datasetsRepository.GetAllDatasets();
            var listDatasetsDTOs = _mapper.Map<List<DatasetDTO>>(listDatasets) ?? throw new Exception("Dataset list not found");
            return listDatasetsDTOs;
        }       
        public async Task<DatasetDTO> GetDatasetById(Guid datasetId)
        {
            var datasetDb = await _datasetsRepository.GetDatasetById(datasetId) ?? throw new Exception("Object not found");
            var datasetDto = _mapper.Map<DatasetDTO>(datasetDb) ?? throw new Exception("Object not found");
            return datasetDto;
        }

        public async Task<EditDatasetDTO> GetObjectForEditDataset(Guid datasetId, string? searchByImageName, bool? searchByIsAnnotatedImage, bool? searchByIsEnabledImage, string? orderByImages)
        {
            var allDataset_DatasetClasses = await _datasetDatasetClassRepository.GetAll() ?? throw new Exception("Object not found");
            var allClasses = await _datasetClassesRepository.GetAllDatasetClasses() ?? throw new Exception("Object not found");

            var insertedClasses = allDataset_DatasetClasses.Where(x => x.DatasetId == datasetId).Select(x => x.DatasetClass).ToList() ?? throw new Exception("Object not found");
            var insertedClassesIds = allDataset_DatasetClasses.Where(x => x.DatasetId == datasetId).Select(x => x.DatasetClassId).ToList() ?? throw new Exception("Object not found");
            var uninsertedRootClasses = allClasses.Where(x => !insertedClassesIds.Contains(x.Id) && x.ParentClassId == null).ToList() ?? throw new Exception("Object not found");
            var uninsertedSubclasses = allClasses.Where(x => !insertedClassesIds.Contains(x.Id) && x.ParentClassId != null).ToList() ?? throw new Exception("Object not found");
            var currentDataset = await _datasetsRepository.GetDatasetById(datasetId) ?? throw new Exception("Object not found");

            var allDatasetImagesUnfiltered = await _datasetImagesRepository.GetImagesForDataset(datasetId) ?? throw new Exception("Object not found");
            var allDatasetImages = await _datasetImagesRepository.GetImagesForDataset(datasetId) ?? throw new Exception("Object not found");
            if(allDatasetImages.Count > 0 && !string.IsNullOrEmpty(searchByImageName))
            {
                allDatasetImages = allDatasetImages.Where(x => x.Name == searchByImageName).ToList();
            }
            if (allDatasetImages.Count > 0 && !string.IsNullOrEmpty(orderByImages))
            {
                if(orderByImages == "ascName")
                {
                    allDatasetImages = allDatasetImages.OrderBy(x => x.Name).ToList();
                }
                if (orderByImages == "descName")
                {
                    allDatasetImages = allDatasetImages.OrderByDescending(x => x.Name).ToList();
                }
                if (orderByImages == "ascCreatedOn")
                {
                    allDatasetImages = allDatasetImages.OrderByDescending(x => x.CreatedOn).ToList();
                }
                if (orderByImages == "descCreatedOn")
                {                    
                    allDatasetImages = allDatasetImages.OrderBy(x => x.CreatedOn).ToList();
                }
            }
            if (allDatasetImages.Count > 0 && searchByIsEnabledImage is not null)
            {
                allDatasetImages = allDatasetImages.Where(x => x.IsEnabled == searchByIsEnabledImage).ToList();
            }

            var imageAnnotationsDbList = await _imageAnnotationsRepository.GetAllImageAnnotations() ?? throw new Exception("Object not found");
            var allImageAnnotationsList = imageAnnotationsDbList.Where(x => x.IsEnabled == true).ToList();
            var annotationsForDatasetImages = allImageAnnotationsList.Where(x => allDatasetImages.Select(m => m.Id).ToList().Contains(x.DatasetImageId.Value)).ToList();
            if (searchByIsAnnotatedImage is not null)
            {                
                if(searchByIsAnnotatedImage == true)
                {
                    allDatasetImages = allDatasetImages.Where(x => annotationsForDatasetImages.Select(m => m.DatasetImageId).Contains(x.Id)).ToList();
                }
                if(searchByIsAnnotatedImage == false)
                {
                    allDatasetImages = allDatasetImages.Where(x => !annotationsForDatasetImages.Select(m => m.DatasetImageId).Contains(x.Id)).ToList();
                }
            }


            var numberOfEnabledImages = allDatasetImages.Where(x => x.IsEnabled == true).Count();
            var numberOfAnnotatedImages = allImageAnnotationsList.Where(x => allDatasetImages.Select(m => m.Id).Contains(x.DatasetImageId.Value)).Count();
            var enabledImagesList = allDatasetImages.Where(x => x.IsEnabled == true).ToList() ?? throw new Exception("Object not found");
            var allEnabledImagesHaveAnnotations = enabledImagesList.Any() ?
                             enabledImagesList.All(x => allImageAnnotationsList.Where(x => x.DatasetImageId == x.Id).Select(x => x.DatasetImageId).Any(a => a == x.Id)) : false;
            var nubmerOfImagesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100);
            var nubmerOfClassesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1);
            var classesForParentDataset = allDataset_DatasetClasses.Where(x => x.DatasetId == currentDataset.ParentDatasetId).Select(x => x.DatasetClass).ToList();
            EditDatasetDTO dto = new()
            {
                UninsertedDatasetRootClasses = _mapper.Map<List<DatasetClassDTO>>(uninsertedRootClasses),
                UninsertedDatasetSubclasses = _mapper.Map<List<DatasetClassDTO>>(uninsertedSubclasses),
                ClassesByDatasetId = _mapper.Map<List<DatasetClassDTO>>(insertedClasses),
                NumberOfDatasetClasses = insertedClasses.Count,
                CurrentDataset = _mapper.Map<DatasetDTO>(currentDataset),
                NumberOfChildrenDatasets = _datasetsRepository.GetNumberOfChildrenDatasetsByDatasetId(datasetId),
                ParentDatasetClasses = _mapper.Map<List<DatasetClassDTO>>(classesForParentDataset),
                ListOfDatasetImages = _mapper.Map<List<DatasetImageDTO>>(allDatasetImages),
                NumberOfDatasetImages = allDatasetImages.Count,
                AllImageAnnotations = _mapper.Map<List<ImageAnnotationDTO>>(allImageAnnotationsList),
                NumberOfEnabledImages = numberOfEnabledImages,
                NumberOfAnnotatedImages = numberOfAnnotatedImages,
                AllEnabledImagesHaveAnnotations = allEnabledImagesHaveAnnotations,
                NumberOfClassesNeededToPublishDataset = nubmerOfClassesNeededToPublishDataset.Data,
                NumberOfImagesNeededToPublishDataset = nubmerOfImagesNeededToPublishDataset.Data,
                ListOfAllDatasetImagesUnFiltered = _mapper.Map<List<DatasetImageDTO>>(allDatasetImagesUnfiltered)
            };
            return dto;
        }
              
        #endregion

        #region Create
        public async Task<DatasetDTO> CreateDataset(DatasetDTO dto)
        {
            Dataset dataset = _mapper.Map<Dataset>(dto);
            var insertedDataset = await _datasetsRepository.CreateDataset(dataset) ?? throw new Exception("Dataset not found");
            DatasetDTO newDTO = _mapper.Map<DatasetDTO>(insertedDataset);
            return newDTO;
        }
        public async Task<int> AddDatasetClassForDataset(Guid selectedClassId, Guid datasetId)
        {
            Dataset_DatasetClass dataset_datasetClass = new()
            {
                DatasetId = datasetId,
                DatasetClassId = selectedClassId
            };

            var datasetDb = await _datasetsRepository.GetDatasetById(datasetId) ?? throw new Exception("Dataset not found");                      
            var createdDatasetClass = await _datasetDatasetClassRepository.Create(dataset_datasetClass);
            datasetDb.UpdatedOn = DateTime.UtcNow;
            //TODO: updatedBy  
            await _datasetsRepository.UpdateDataset(datasetDb);
            return createdDatasetClass;
        }

        public async Task<int> AddInheritedParentClasses(Guid insertedDatasetId,  Guid parentDatasetId)
        {
            var parentClassesList = await _datasetDatasetClassRepository.GetListByDatasetId(parentDatasetId) ?? throw new Exception("Object not found");
            var parentClassesIdsList = parentClassesList.Select(x => x.DatasetClassId).ToList();

            List<Dataset_DatasetClass> dataset_DatasetClasses = new();
            foreach (var item in parentClassesIdsList)
            {
                Dataset_DatasetClass dataset_class = new();
                dataset_class.DatasetId = insertedDatasetId;
                dataset_class.DatasetClassId = item;
                dataset_DatasetClasses.Add(dataset_class);
            }
           return await _datasetDatasetClassRepository.AddRange(dataset_DatasetClasses);
        }
        

        #endregion

        #region Update
        public async Task<int> PublishDataset(Guid datasetId)
        {
            var datasetDb = await _datasetsRepository.GetDatasetById(datasetId) ?? throw new Exception("Dataset not found");
            var allDataset_DatasetClasses = await _datasetDatasetClassRepository.GetAll() ?? throw new Exception("Object not found");

            var allDatasetImages = await _datasetImagesRepository.GetImagesForDataset(datasetDb.Id) ?? throw new Exception("Object not found");
            var enabledImagesList = allDatasetImages.Where(x => x.IsEnabled == true).ToList() ?? throw new Exception("Object not found");
            var allImageAnnotationsList = await _imageAnnotationsRepository.GetAllImageAnnotations() ?? throw new Exception("Object not found");
            var allEnabledImagesHaveAnnotations = enabledImagesList.Any() ?
                            enabledImagesList.All(x => allImageAnnotationsList.Where(x => x.DatasetImageId == x.Id).Select(x => x.DatasetImageId).Any(a => a == x.Id)) : false;
            var nubmerOfImagesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100);
            var nubmerOfClassesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1);
            var insertedClasses = allDataset_DatasetClasses.Where(x => x.DatasetId == datasetDb.Id).Select(x => x.DatasetClass).ToList();

            if(insertedClasses.Count < nubmerOfClassesNeededToPublishDataset.Data || allEnabledImagesHaveAnnotations == false || allDatasetImages.Count < nubmerOfImagesNeededToPublishDataset.Data)
            {
                return 2;
            }

            datasetDb.IsPublished = true;
            datasetDb.UpdatedOn = DateTime.UtcNow;
            //TODO: updatedBy 
            var updatedDataset = await _datasetsRepository.UpdateDataset(datasetDb);
            return updatedDataset;
            
        }

        public async Task<int> SetAnnotationsPerSubclass(Guid datasetId, bool annotationsPerSubclass)
        {
            var datasetDb = await _datasetsRepository.GetDatasetById(datasetId) ?? throw new Exception("Object not found");
            datasetDb.AnnotationsPerSubclass = annotationsPerSubclass;
            datasetDb.UpdatedOn = DateTime.UtcNow;
            //TODO: updatedBy
            var updatedDataset = await _datasetsRepository.UpdateDataset(datasetDb);
            return updatedDataset;
        }
        #endregion

        #region Delete
        public async Task<int> DeleteDatasetClassForDataset(Guid selectedClassId, Guid datasetId)
        {
            Dataset_DatasetClass dataset_DatasetClassDb = await _datasetDatasetClassRepository.GetByDatasetAndDatasetClassId(selectedClassId, datasetId);
            var datasetDb = await _datasetsRepository.GetDatasetById(datasetId) ?? throw new Exception("Dataset not found");    
            var deletedDataset_DatasetClass =  await _datasetDatasetClassRepository.Delete(dataset_DatasetClassDb);
            datasetDb.UpdatedOn = DateTime.UtcNow;
            //TODO: updatedBy 
            await _datasetsRepository.UpdateDataset(datasetDb);
            return deletedDataset_DatasetClass;
        }

        public async Task<int> DeleteDataset(Guid datasetId)
        {
            var datasetDb = await _datasetsRepository.GetDatasetById(datasetId) ?? throw new Exception("Dataset not found");

            var listOfAllDatasets = await _datasetsRepository.GetAllDatasets() ?? throw new Exception("Datasets not found");
            var childrenDatasets = listOfAllDatasets.Where(x => x.ParentDatasetId == datasetId).ToList();
            if(childrenDatasets.Count > 0)
            {
                return 2;
            }
            var list_dataset_datasetClassDb = await _datasetDatasetClassRepository.GetListByDatasetId(datasetId) ?? throw new Exception("List not found");
            if(list_dataset_datasetClassDb.Count > 0)
            {
                await _datasetDatasetClassRepository.DeleteRange(list_dataset_datasetClassDb);
            }
            var datasetImagesList = await _datasetImagesRepository.GetImagesForDataset(datasetId) ?? throw new Exception("List not found");
            if(datasetImagesList.Count > 0)
            {
                await _datasetImagesRepository.DeleteRange(datasetImagesList);
            }
            return await _datasetsRepository.DeleteDataset(datasetDb);
        }
        #endregion



    }
}
