using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services.DatasetServices;

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

        public async Task<List<Dataset_DatasetClassDTO>> GetDataset_DatasetClassByClassId(Guid classId)
        {
            var dataset_datasetClass = await _datasetDatasetClassRepository.GetListByDatasetClassId(classId) ?? throw new Exception("Object not found");
            return _mapper.Map<List<Dataset_DatasetClassDTO>>(dataset_datasetClass);
        }
    }
}
