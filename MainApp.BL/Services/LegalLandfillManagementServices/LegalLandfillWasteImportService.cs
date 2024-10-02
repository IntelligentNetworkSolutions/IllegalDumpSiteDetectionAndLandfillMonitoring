using AutoMapper;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.LegalLandfillManagementServices
{
    public class LegalLandfillWasteImportService : ILegalLandfillWasteImportService
    {
        private readonly ILegalLandfillWasteImportRepository _legalLandfillWasteImportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LegalLandfillWasteImportService> _logger;

        public LegalLandfillWasteImportService(ILegalLandfillWasteImportRepository legalLandfillWasteImportRepository, IMapper mapper, ILogger<LegalLandfillWasteImportService> logger)
        {
            _legalLandfillWasteImportRepository = legalLandfillWasteImportRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDTO> CreateLegalLandfillWasteImport(LegalLandfillWasteImportDTO legalLandfillWasteImportDTO)
        {
            try
            {
                LegalLandfillWasteImport legalLandfillWasteImportEntity = _mapper.Map<LegalLandfillWasteImport>(legalLandfillWasteImportDTO);

                ResultDTO resultCreate = await _legalLandfillWasteImportRepository.Create(legalLandfillWasteImportEntity);

                if (resultCreate == null)
                {
                    return ResultDTO.Fail("Creation failed: Result is null.");
                }

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

        public async Task<ResultDTO<List<LegalLandfillWasteImportDTO>>> GetAllLegalLandfillWasteImports()
        {
            try
            {
                ResultDTO<IEnumerable<LegalLandfillWasteImport>> resultGetAllEntities = await _legalLandfillWasteImportRepository.GetAll(
                    includeProperties: "LegalLandfillWasteType,LegalLandfillTruck,LegalLandfill,CreatedBy"
                );

                if (!resultGetAllEntities.IsSuccess && resultGetAllEntities.HandleError())
                {
                    return ResultDTO<List<LegalLandfillWasteImportDTO>>.Fail(resultGetAllEntities.ErrMsg!);
                }

                List<LegalLandfillWasteImportDTO> dtos = _mapper.Map<List<LegalLandfillWasteImportDTO>>(resultGetAllEntities.Data);
                return ResultDTO<List<LegalLandfillWasteImportDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillWasteImportDTO>>.ExceptionFail(ex.Message, ex);
            }
        }


        public async Task<ResultDTO<LegalLandfillWasteImportDTO>> GetLegalLandfillWasteImportById(Guid legalLandfillWasteImportId)
        {
            try
            {
                ResultDTO<LegalLandfillWasteImport?> resultGetEntity = await _legalLandfillWasteImportRepository.GetByIdInclude(
                    legalLandfillWasteImportId,
                    includeProperties:
                    [
                    x => x.LegalLandfillWasteType,
                    x => x.LegalLandfillTruck,
                    x => x.LegalLandfill,
                    x => x.CreatedBy
                    ]
                );

                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                {
                    return ResultDTO<LegalLandfillWasteImportDTO>.Fail(resultGetEntity.ErrMsg!);
                }

                LegalLandfillWasteImportDTO dto = _mapper.Map<LegalLandfillWasteImportDTO>(resultGetEntity.Data);
                return ResultDTO<LegalLandfillWasteImportDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<LegalLandfillWasteImportDTO>.ExceptionFail(ex.Message, ex);
            }
        }


        public async Task<ResultDTO> EditLegalLandfillWasteImport(LegalLandfillWasteImportDTO legalLandfillWasteImportDTO)
        {
            try
            {
                LegalLandfillWasteImport legalLandfillWasteImportEntity = _mapper.Map<LegalLandfillWasteImport>(legalLandfillWasteImportDTO);

                ResultDTO resultUpdate = await _legalLandfillWasteImportRepository.Update(legalLandfillWasteImportEntity);
                if (resultUpdate.IsSuccess == false && resultUpdate.HandleError())
                {
                    return ResultDTO.Fail(resultUpdate.ErrMsg!);
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteLegalLandfillWasteImport(LegalLandfillWasteImportDTO legalLandfillWasteImportDTO)
        {
            try
            {
                LegalLandfillWasteImport legalLandfillWasteImportEntity = _mapper.Map<LegalLandfillWasteImport>(legalLandfillWasteImportDTO);

                ResultDTO resultDelete = await _legalLandfillWasteImportRepository.Delete(legalLandfillWasteImportEntity);
                if (resultDelete.IsSuccess == false && resultDelete.HandleError())
                {
                    return ResultDTO.Fail(resultDelete.ErrMsg!);
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

