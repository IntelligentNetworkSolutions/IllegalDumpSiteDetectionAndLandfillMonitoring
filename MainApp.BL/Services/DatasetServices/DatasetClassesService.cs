using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services.DatasetServices;
using SD;
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
        public async Task<ResultDTO<List<DatasetClassDTO>>> GetAllDatasetClasses()
        {
            var resultGetEntity = await _datasetClassesRepository.GetAll(includeProperties: "CreatedBy,ParentClass,Datasets");

            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                return ResultDTO<List<DatasetClassDTO>>.Fail(resultGetEntity.ErrMsg!);

            var model = _mapper.Map<List<DatasetClassDTO>>(resultGetEntity.Data);

            return ResultDTO<List<DatasetClassDTO>>.Ok(model);

        }

        public async Task<ResultDTO<List<DatasetClassDTO>>> GetAllDatasetClassesByDatasetId(Guid datasetId)
        {
            var resultGetEntity = await _datasetDatasetClassRepository.GetAll(filter: x => x.DatasetId == datasetId, includeProperties: "DatasetClass");

            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                return ResultDTO<List<DatasetClassDTO>>.Fail(resultGetEntity.ErrMsg!);

            var resList = resultGetEntity.Data.Select(x => x.DatasetClass).ToList();

            var model = _mapper.Map<List<DatasetClassDTO>>(resList);

            return ResultDTO<List<DatasetClassDTO>>.Ok(model);
        }

        public async Task<ResultDTO<DatasetClassDTO>> GetDatasetClassById(Guid classId)
        {
            var datasetClassResult = await _datasetClassesRepository.GetById(classId, includeProperties: "CreatedBy,ParentClass");
            if (datasetClassResult.IsSuccess == false && datasetClassResult.HandleError())
            {
                return ResultDTO<DatasetClassDTO>.Fail(datasetClassResult.ErrMsg!);
            }

            var model = _mapper.Map<DatasetClassDTO>(datasetClassResult.Data);
            if (model == null)
            {
                return ResultDTO<DatasetClassDTO>.Fail("Failed to map dataset class");
            }

            return ResultDTO<DatasetClassDTO>.Ok(model);
        }
        #endregion

        #region Create
        public async Task<ResultDTO<int>> AddDatasetClass(CreateDatasetClassDTO dto)
        {
            if (dto.ParentClassId != null)
            {
                var resultDTO = await _datasetClassesRepository.GetById(dto.ParentClassId.Value, includeProperties: "CreatedBy,ParentClass");

                if (resultDTO?.Data == null || resultDTO.IsSuccess == false && resultDTO.HandleError())
                {
                    return new ResultDTO<int>(IsSuccess: false, 0, "Parent class not found", null);
                }

                var parentClassDb = resultDTO.Data;
                if (parentClassDb.ParentClassId != null)
                {
                    return new ResultDTO<int>(IsSuccess: false, Data: 2, ErrMsg: "You cannot add this class as a subclass because the selected parent class is already set as a subclass!", null);
                }
            }

            var newClass = _mapper.Map<DatasetClass>(dto);
            if (newClass == null)
            {
                return new ResultDTO<int>(IsSuccess: false, 0, "Failed to map the new dataset class", null);
            }

            var isAdded = await _datasetClassesRepository.Create(newClass);

            return isAdded.IsSuccess
                ? new ResultDTO<int>(IsSuccess: true, Data: 1, ErrMsg: null, null)
                : new ResultDTO<int>(IsSuccess: false, Data: 3, ErrMsg: isAdded.ErrMsg, null);
        }
        #endregion

        #region Update
        public async Task<ResultDTO<int>> EditDatasetClass(EditDatasetClassDTO dto)
        {
            var datasetClassDb = await _datasetClassesRepository.GetById(dto.Id, track: true, includeProperties: "CreatedBy,ParentClass");
            if (datasetClassDb.IsSuccess == false && datasetClassDb.HandleError() || datasetClassDb.Data == null)
            {
                return new ResultDTO<int>(false, 0, "Dataset class not found", null);
            }

            var allClasses = await _datasetClassesRepository.GetAll(includeProperties: "CreatedBy,ParentClass,Datasets");
            if (allClasses.IsSuccess == false || allClasses.Data == null)
            {
                return new ResultDTO<int>(false, 0, "Failed to retrieve dataset classes", null);
            }

            var childrenClassesList = allClasses.Data.Where(x => x.ParentClassId == datasetClassDb.Data.Id).ToList();

            if (childrenClassesList.Count > 0 && dto.ParentClassId != null)
            {
                return new ResultDTO<int>(false, 2, "This dataset class has subclasses and cannot be set as a subclass too!", null);
            }

            var allDatasetDatasetClasses = await _datasetDatasetClassRepository.GetAll(includeProperties: "DatasetClass,Dataset");
            if (allDatasetDatasetClasses.IsSuccess == false && allDatasetDatasetClasses.HandleError() || allDatasetDatasetClasses.Data == null)
            {
                return new ResultDTO<int>(false, 0, "Failed to retrieve dataset-dataset class mappings", null);
            }

            var datasetDatasetClasses = allDatasetDatasetClasses.Data.Where(x => x.DatasetClassId == dto.Id).ToList();


            if (datasetDatasetClasses.Any())
            {
                return new ResultDTO<int>(false, 3, "The selected class is already in use in dataset(s). You can only change the class name!", null);
            }

            if (dto.ParentClassId != null)
            {
                var parentClassDb = await _datasetClassesRepository.GetById(dto.ParentClassId.Value, includeProperties: "CreatedBy,ParentClass");
                if (parentClassDb.IsSuccess == false && parentClassDb.HandleError() || parentClassDb.Data == null)
                {
                    return new ResultDTO<int>(false, 0, "Parent class not found", null);
                }

                if (parentClassDb.Data.ParentClassId != null)
                {
                    return new ResultDTO<int>(false, 4, "You cannot add this class as a subclass because the selected parent class is already set as a subclass!", null);
                }
            }

            var mappedClass = _mapper.Map(dto, datasetClassDb.Data);
            if (mappedClass == null)
            {
                return new ResultDTO<int>(false, 0, "Failed to map dataset class", null);
            }

            var isUpdated = await _datasetClassesRepository.Update(mappedClass);
            return isUpdated.IsSuccess
                ? new ResultDTO<int>(true, 1, null, null)
                : new ResultDTO<int>(false, 5, isUpdated.ErrMsg, null);
        }
        #endregion

        #region Delete
        public async Task<ResultDTO<int>> DeleteDatasetClass(Guid classId)
        {
            var datasetClass = await _datasetClassesRepository.GetById(classId, track: true, includeProperties: "CreatedBy,ParentClass");
            if (datasetClass.IsSuccess == false && datasetClass.HandleError() || datasetClass.Data == null)
            {
                return new ResultDTO<int>(IsSuccess: false, 0, "Dataset class not found.", null);
            }
            var datasetClassData = datasetClass.Data;

            var listOfAllDatasetClasses = await _datasetClassesRepository.GetAll(includeProperties: "CreatedBy,ParentClass,Datasets");
            if (listOfAllDatasetClasses.IsSuccess == false && listOfAllDatasetClasses.HandleError() || listOfAllDatasetClasses.Data == null)
            {
                return new ResultDTO<int>(IsSuccess: false, 0, "Failed to retrieve dataset classes.", null);
            }

            var childrenDatasetClasses = listOfAllDatasetClasses.Data
                .Where(x => x.ParentClassId == classId)
                .ToList();
            if (childrenDatasetClasses.Count > 0)
            {
                return new ResultDTO<int>(IsSuccess: false, 2, "This dataset class cannot be deleted because there are subclasses. Delete the subclasses first.", null);
            }

            var allDatasetDatasetClasses = await _datasetDatasetClassRepository.GetAll();
            if (allDatasetDatasetClasses.IsSuccess == false && allDatasetDatasetClasses.HandleError() || allDatasetDatasetClasses.Data == null)
            {
                return new ResultDTO<int>(IsSuccess: false, 0, "Failed to retrieve dataset-dataset class relationships.", null);
            }

            var datasetDatasetClasses = allDatasetDatasetClasses.Data
                .Where(x => x.DatasetClassId == classId)
                .ToList();

            if (datasetDatasetClasses.Count > 0)
            {
                return new ResultDTO<int>(IsSuccess: false, 3, "This dataset class cannot be deleted because it is in use in datasets.", null);
            }

            ResultDTO<IEnumerable<Dataset_DatasetClass>> associatedDatasetClasses = await _datasetDatasetClassRepository.GetAll(filter: x => x.DatasetClassId == classId);

            if (associatedDatasetClasses.IsSuccess == false && associatedDatasetClasses.HandleError())
            {
                return new ResultDTO<int>(IsSuccess: false, 0, "Failed to retrieve associated dataset-dataset class relationships.", null);
            }

            if (associatedDatasetClasses.Data != null)
            {
                var listDatasetDatasetClassDb = associatedDatasetClasses.Data.ToList();
                if (listDatasetDatasetClassDb.Count > 0)
                {
                    var deleteRelationsResult = await _datasetDatasetClassRepository.DeleteRange(listDatasetDatasetClassDb);
                    if (deleteRelationsResult.IsSuccess == false && deleteRelationsResult.HandleError())
                    {
                        return new ResultDTO<int>(IsSuccess: false, 0, "Failed to delete associated dataset-dataset class relationships.", null);
                    }
                }
            }

            var deleteResult = await _datasetClassesRepository.Delete(datasetClassData);
            if (deleteResult.IsSuccess == false && datasetClass.HandleError())
            {
                return new ResultDTO<int>(IsSuccess: false, 4, deleteResult.ErrMsg ?? "Failed to delete the dataset class.", null);
            }

            return new ResultDTO<int>(IsSuccess: true, 1, "Dataset class deleted successfully.", null);

        }
        #endregion
    }
}
