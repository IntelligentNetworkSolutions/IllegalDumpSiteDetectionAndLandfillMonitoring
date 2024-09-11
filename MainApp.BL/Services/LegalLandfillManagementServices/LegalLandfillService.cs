using AutoMapper;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.LegalLandfillManagementServices
{
    public class LegalLandfillService : ILegalLandfillService
    {
        private readonly ILegalLandfillRepository _legalLandfillRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LegalLandfillService> _logger;

        public LegalLandfillService(ILegalLandfillRepository legalLandfillRepository, IMapper mapper, ILogger<LegalLandfillService> logger)
        {
            _legalLandfillRepository = legalLandfillRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ResultDTO<List<LegalLandfillDTO>>> GetAllLegalLandfills()
        {
            try
            {
                ResultDTO<IEnumerable<LegalLandfill>> resultGetAllEntites = await _legalLandfillRepository.GetAll();

                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                {
                    return ResultDTO<List<LegalLandfillDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                }

                List<LegalLandfillDTO> dtos = _mapper.Map<List<LegalLandfillDTO>>(resultGetAllEntites.Data);
                return ResultDTO<List<LegalLandfillDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillDTO>>.ExceptionFail(ex.Message, ex);
            }
        }
        public async Task<ResultDTO<LegalLandfillDTO>> GetLegalLandfillById(Guid legalLandfillId)
        {
            try
            {
                ResultDTO<LegalLandfill?> resultGetEntity = await _legalLandfillRepository.GetById(legalLandfillId);

                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                {
                    return ResultDTO<LegalLandfillDTO>.Fail(resultGetEntity.ErrMsg!);
                }

                LegalLandfillDTO dto = _mapper.Map<LegalLandfillDTO>(resultGetEntity.Data);
                return ResultDTO<LegalLandfillDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<LegalLandfillDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> CreateLegalLandfill(LegalLandfillDTO legalLandfillDTO)
        {
            try
            {
                LegalLandfill legalLandfillEntity = _mapper.Map<LegalLandfill>(legalLandfillDTO);

                ResultDTO resultCreate = await _legalLandfillRepository.Create(legalLandfillEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                {
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> EditLegalLandfill(LegalLandfillDTO legalLandfillDTO)
        {
            try
            {
                LegalLandfill legalLandfillEntity = _mapper.Map<LegalLandfill>(legalLandfillDTO);

                ResultDTO resultCreate = await _legalLandfillRepository.Update(legalLandfillEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                {
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteLegalLandfill(LegalLandfillDTO legalLandfillDTO)
        {
            try
            {
                LegalLandfill legalLandfillEntity = _mapper.Map<LegalLandfill>(legalLandfillDTO);

                ResultDTO resultCreate = await _legalLandfillRepository.Delete(legalLandfillEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                {
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
    }
}
