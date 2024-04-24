using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DocumentFormat.OpenXml.VariantTypes;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using SD;
using System.Data;
using System.Runtime.InteropServices;

namespace MainApp.BL.Services.DatasetServices
{
    public class DatasetClassesService : IDatasetClassesService
    {
        private readonly IDatasetsRepository _datasetsRepository;
        private readonly IDatasetClassesRepository _datasetClassesRepository;
        private readonly IDataset_DatasetClassRepository _datasetDatasetClassRepository;
        private readonly IMapper _mapper;

        public DatasetClassesService(IDatasetsRepository datasetsRepository, IDatasetClassesRepository datasetClassesRepository, IDataset_DatasetClassRepository datasetDatasetClassRepository, IMapper mapper)
        {
            _datasetsRepository = datasetsRepository;
            _datasetClassesRepository = datasetClassesRepository;
            _datasetDatasetClassRepository = datasetDatasetClassRepository;
            _mapper = mapper;
        }
        #region Read
        #region Get Datasetclass/es
        public async Task<List<DatasetClassDTO>> GetAllDatasetClasses()
        {
            var list = await _datasetClassesRepository.GetAll(includeProperties: "CreatedBy,ParentClass,Datasets") ?? throw new Exception("Model not found");
            var model = _mapper.Map<List<DatasetClassDTO>>(list.Data) ?? throw new Exception("Model not found");
            return model;

        }        
        
        public async Task<List<DatasetClassDTO>> GetAllDatasetClassesByDatasetId(Guid datasetId)
        {
            var list = await _datasetDatasetClassRepository.GetAll(filter: x=>x.DatasetId == datasetId, includeProperties: "DatasetClass") ?? throw new Exception("Model not found");
            var resList = list.Data.Select(x=>x.DatasetClass).ToList();
            var model = _mapper.Map<List<DatasetClassDTO>>(resList) ?? throw new Exception("Model not found");
            return model;

        }
        public async Task<DatasetClassDTO> GetDatasetClassById(Guid classId)
        {
            var datasetClass = await _datasetClassesRepository.GetById(classId,includeProperties: "CreatedBy,ParentClass") ?? throw new Exception("Model not found");
            var model = _mapper.Map<DatasetClassDTO>(datasetClass.Data) ?? throw new Exception("Model not found");
            return model;
        }
        #endregion
        #endregion

        #region Create
        public async Task<ResultDTO<int>> AddDatasetClass(CreateDatasetClassDTO dto)
        {
            if(dto.ParentClassId != null)
            {                
                var resultDTO = await _datasetClassesRepository.GetById(dto.ParentClassId.Value, includeProperties: "CreatedBy,ParentClass") ?? throw new Exception("Model not found");
                DatasetClass? parentClassDb = resultDTO.Data;               
                if (parentClassDb?.ParentClassId != null)
                {
                    return new ResultDTO<int>(IsSuccess: false, 2, "You can not add this class as a subclass because the selected parent class is already set as subclass!", null);
                }
            }
             
            var newClass = _mapper.Map<DatasetClass>(dto) ?? throw new Exception("Object not found");
            var isAdded = await _datasetClassesRepository.Create(newClass);
            if (isAdded.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 3, isAdded.ErrMsg, null);
            }
        }
        #endregion

        #region Update
        public async Task<ResultDTO<int>> EditDatasetClass(EditDatasetClassDTO dto)
        {
            var datasetClassDb = await _datasetClassesRepository.GetById(dto.Id, track: true, includeProperties: "CreatedBy,ParentClass") ?? throw new Exception("Model not found");
            var allClasses = await _datasetClassesRepository.GetAll(includeProperties: "CreatedBy,ParentClass,Datasets") ?? throw new Exception("Object not found");
            var childrenClassesList = allClasses.Data?.Where(x => x.ParentClassId == datasetClassDb.Data?.Id).ToList() ?? throw new Exception("Object not found");
            var all_dataset_datasetClasses = await _datasetDatasetClassRepository.GetAll(includeProperties: "DatasetClass,Dataset") ?? throw new Exception("Object not found");
            var all_dataset_datasetClasses_data = all_dataset_datasetClasses.Data ?? throw new Exception("Object not found");
            var dataset_datasetClasses = all_dataset_datasetClasses_data.Where(x => x.DatasetClassId == dto.Id).ToList();
            
            if (childrenClassesList.Count > 0 && dto.ParentClassId != null)
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "This dataset class has subclasses and can not be set as a subclass too!", null);
            }            
            if (dataset_datasetClasses.Count > 0)
            {
                return new ResultDTO<int>(IsSuccess: false, 3, "The selected class is already in use in dataset/s, you can only change the class name!", null);
            }
            if (dto.ParentClassId != null)
            {
                var parentClassDb = await _datasetClassesRepository.GetById(dto.ParentClassId.Value, includeProperties: "CreatedBy,ParentClass") ?? throw new Exception("Model not found");
                if (parentClassDb.Data?.ParentClassId != null)
                {
                   return new ResultDTO<int> (IsSuccess:false,4, "You can not add this class as a subclass because the selected parent class is already set as subclass!", null);
                }
            }

            var mappedClass = _mapper.Map(dto, datasetClassDb.Data) ?? throw new Exception("Object not found");
            var isUpdated = await _datasetClassesRepository.Update(mappedClass);
            if(isUpdated.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 5, isUpdated.ErrMsg, null);
            }
        }
        #endregion

        #region Delete
        public async Task<ResultDTO<int>> DeleteDatasetClass(Guid classId)
        {
            var datasetClass = await _datasetClassesRepository.GetById(classId, track:true, includeProperties: "CreatedBy,ParentClass") ?? throw new Exception("Model not found");
            var datasetClassData = datasetClass.Data ?? throw new Exception("Object not found");
            var listOfAllDatasetClasses = await _datasetClassesRepository.GetAll(includeProperties: "CreatedBy,ParentClass,Datasets") ?? throw new Exception("Object not found");
            var childrenDatasetClasses = listOfAllDatasetClasses.Data?.Where(x => x.ParentClassId == classId).ToList();
            var all_dataset_datasetClasses = await _datasetDatasetClassRepository.GetAll() ?? throw new Exception("Object not found");
            var all_dataset_datasetClasses_data = all_dataset_datasetClasses.Data ?? throw new Exception("Object not found");
            var dataset_datasetClasses = all_dataset_datasetClasses_data.Where(x => x.DatasetClassId == classId).ToList() ?? throw new Exception("Object not found");
            if (childrenDatasetClasses?.Count > 0)
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "This dataset class can not be deleted because there are subclasses. Delete first the subclasses!", null);
            }
            if (dataset_datasetClasses.Count > 0)
            {
                return new ResultDTO<int>(IsSuccess: false, 3, "This dataset class can not be deleted because this class is already in use in dataset/s!", null);
            }
            var list_result = await _datasetDatasetClassRepository.GetAll(filter: x => x.DatasetClassId == classId) ?? throw new Exception("Object not found");
            List<Dataset_DatasetClass> list_dataset_DatasetClassDb = list_result?.Data?.ToList() ?? throw new Exception("Object not found");
            if(list_dataset_DatasetClassDb.Count > 0)
            {
                await _datasetDatasetClassRepository.DeleteRange(list_dataset_DatasetClassDb);
            }
            var isDeleted = await _datasetClassesRepository.Delete(datasetClass.Data);
            if (isDeleted.IsSuccess == true)
            {
                return new ResultDTO<int>(IsSuccess: true, 1, null, null);
            }
            else
            {
                return new ResultDTO<int>(IsSuccess: false, 4, isDeleted.ErrMsg, null);
            }
        }
        #endregion
    }
}
