using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using System.Data;

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
            var list = await _datasetClassesRepository.GetAllDatasetClasses() ?? throw new Exception("Model not found");
            var model = _mapper.Map<List<DatasetClassDTO>>(list) ?? throw new Exception("Model not found");
            return model;

        }
        public async Task<DatasetClassDTO> GetDatasetClassById(Guid classId)
        {
            var datasetClass = await _datasetClassesRepository.GetDatasetClassById(classId) ?? throw new Exception("Model not found");
            var model = _mapper.Map<DatasetClassDTO>(datasetClass) ?? throw new Exception("Model not found");
            return model;
        }
        #endregion
        #endregion

        #region Create
        public async Task<int> AddDatasetClass(CreateDatasetClassDTO dto)
        {
            if(dto.ParentClassId != null)
            {
                DatasetClass? parentClassDb = await _datasetClassesRepository.GetDatasetClassById(dto.ParentClassId.Value) ?? throw new Exception("Object not found");
                if (parentClassDb.ParentClassId != null)
                {
                    return 2;
                }
            }
            
            var newClass = _mapper.Map<DatasetClass>(dto) ?? throw new Exception("Object not found");
            var isAdded = await _datasetClassesRepository.AddClass(newClass);
            return isAdded;
        }
        #endregion

        #region Update
        public async Task<int> EditDatasetClass(EditDatasetClassDTO dto)
        {
            var datasetClassDb = await _datasetClassesRepository.GetDatasetClassById(dto.Id) ?? throw new Exception("Object not found");
            var allClasses = await _datasetClassesRepository.GetAllDatasetClasses() ?? throw new Exception("Object not found");
            var childrenClassesList = allClasses.Where(x => x.ParentClassId == datasetClassDb.Id).ToList() ?? throw new Exception("Object not found");
            var all_dataset_datasetClasses = await _datasetDatasetClassRepository.GetAll() ?? throw new Exception("Object not found");
            var dataset_datasetClasses = all_dataset_datasetClasses.Where(x => x.DatasetClassId == dto.Id).ToList();
            
            if (childrenClassesList.Count > 0 && dto.ParentClassId != null)
            {
                return 2;
            }            
            if (dataset_datasetClasses.Count > 0)
            {
                return 3;
            }
            if (dto.ParentClassId != null)
            {
                var parentClassDb = await _datasetClassesRepository.GetDatasetClassById(dto.ParentClassId.Value) ?? throw new Exception("Object not found");
                if (parentClassDb.ParentClassId != null)
                {
                    return 4;
                }
            }

            var mappedClass = _mapper.Map(dto, datasetClassDb);
            var isUpdated = await _datasetClassesRepository.EditClass(mappedClass);
            return isUpdated;
        }
        #endregion

        #region Delete
        public async Task<int> DeleteDatasetClass(Guid classId)
        {
            var datasetClass = await _datasetClassesRepository.GetDatasetClassById(classId) ?? throw new Exception("Object not found");
            var listOfAllDatasetClasses = await _datasetClassesRepository.GetAllDatasetClasses() ?? throw new Exception("Object not found");
            var childrenDatasetClasses = listOfAllDatasetClasses.Where(x => x.ParentClassId == classId).ToList();
            var all_dataset_datasetClasses = await _datasetDatasetClassRepository.GetAll() ?? throw new Exception("Object not found");
            var dataset_datasetClasses = all_dataset_datasetClasses.Where(x => x.DatasetClassId == classId).ToList();
            if (childrenDatasetClasses.Count > 0)
            {
                return 2;
            }
            if (dataset_datasetClasses.Count > 0)
            {
                return 3;
            }
            List<Dataset_DatasetClass> list_dataset_DatasetClassDb = await _datasetDatasetClassRepository.GetListByDatasetClassId(classId);
            if(list_dataset_DatasetClassDb.Count > 0)
            {
                await _datasetDatasetClassRepository.DeleteRange(list_dataset_DatasetClassDb);
            } 
            var isDeleted = await _datasetClassesRepository.DeleteClass(datasetClass);
            return isDeleted;
        }
        #endregion
    }
}
