using AutoMapper;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DAL.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using Microsoft.Extensions.Logging;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Services.LegalLandfillManagementServices
{
    public class LegalLandfillPointCloudFileService : ILegalLandfillPointCloudFileService
    {
        private readonly ILegalLandfillPointCloudFileRepository _legalLandfillPointCloudFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LegalLandfillPointCloudFileService> _logger;

        public LegalLandfillPointCloudFileService(ILegalLandfillPointCloudFileRepository legalLandfillPointCloudFileRepository, IMapper mapper, ILogger<LegalLandfillPointCloudFileService> logger)
        {
            _legalLandfillPointCloudFileRepository = legalLandfillPointCloudFileRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetAllLegalLandfillPointCloudFiles()
        {
            try
            {
                ResultDTO<IEnumerable<LegalLandfillPointCloudFile>> resultGetAllEntites =
                    await _legalLandfillPointCloudFileRepository.GetAll();

                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail(resultGetAllEntites.ErrMsg!);

                List<LegalLandfillPointCloudFileDTO> dtos = _mapper.Map<List<LegalLandfillPointCloudFileDTO>>(resultGetAllEntites.Data);

                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.ExceptionFail(ex.Message, ex);
            }
        }
    }
}
