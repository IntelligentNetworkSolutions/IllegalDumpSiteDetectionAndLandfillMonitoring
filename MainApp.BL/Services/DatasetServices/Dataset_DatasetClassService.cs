using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services.DatasetServices;
using SD;

namespace MainApp.BL.Services.DatasetServices
{
    public class Dataset_DatasetClassService : IDataset_DatasetClassService
    {
        private readonly IDataset_DatasetClassRepository _datasetDatasetClassRepository;
        private readonly IMapper _mapper;

        public Dataset_DatasetClassService(IDataset_DatasetClassRepository datasetDatasetClassRepository, IMapper mapper)
        {
            _datasetDatasetClassRepository = datasetDatasetClassRepository;
            _mapper = mapper;
        }

        public async Task<ResultDTO<List<Dataset_DatasetClassDTO>>> GetDataset_DatasetClassByClassId(Guid classId)
        {
            var dataset_datasetClassResult = await _datasetDatasetClassRepository.GetAll(filter: x => x.DatasetClassId == classId);
            if (dataset_datasetClassResult.IsSuccess == false && dataset_datasetClassResult.HandleError())
            {
                return ResultDTO<List<Dataset_DatasetClassDTO>>.Fail(dataset_datasetClassResult.ErrMsg!);
            }
            var data = dataset_datasetClassResult.Data;

            if (data is null)
                return ResultDTO<List<Dataset_DatasetClassDTO>>.Fail("DatasetClass is null"!);

            var dtos = _mapper.Map<List<Dataset_DatasetClassDTO>>(data);

            return ResultDTO<List<Dataset_DatasetClassDTO>>.Ok(dtos);
        }
    }
}
