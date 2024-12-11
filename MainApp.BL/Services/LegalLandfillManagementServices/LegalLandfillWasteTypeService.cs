using AutoMapper;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.LegalLandfillManagementServices
{
    public class LegalLandfillWasteTypeService : ILegalLandfillWasteTypeService
    {
        private readonly ILegalLandfillWasteTypeRepository _legalLandfillWasteTypeRepository;
        private readonly ILegalLandfillWasteImportRepository _legalLandfillWasteImportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LegalLandfillWasteTypeService> _logger;
        public LegalLandfillWasteTypeService(ILegalLandfillWasteTypeRepository legalLandfillWasteTypeRepository, IMapper mapper, ILogger<LegalLandfillWasteTypeService> logger, ILegalLandfillWasteImportRepository legalLandfillWasteImportRepository)
        {
            _legalLandfillWasteTypeRepository = legalLandfillWasteTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _legalLandfillWasteImportRepository = legalLandfillWasteImportRepository;
        }

        #region Create
        public async Task<ResultDTO> CreateLegalLandfillWasteType(LegalLandfillWasteTypeDTO legalLandfillWasteTypeDTO)
        {
            try
            {
                LegalLandfillWasteType? legalLandfillWasteTypeEntity = _mapper.Map<LegalLandfillWasteType>(legalLandfillWasteTypeDTO);
                if (legalLandfillWasteTypeEntity == null)
                    return ResultDTO.Fail("Mapping legal landfill waste type failed");

                ResultDTO resultCreate = await _legalLandfillWasteTypeRepository.Create(legalLandfillWasteTypeEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())                
                    return ResultDTO.Fail(resultCreate.ErrMsg!);                               

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Update
        public async Task<ResultDTO> EditLegalLandfillWasteType(LegalLandfillWasteTypeDTO legalLandfillWasteTypeDTO)
        {
            try
            {
                LegalLandfillWasteType legalLandfillWasteTypeEntity = _mapper.Map<LegalLandfillWasteType>(legalLandfillWasteTypeDTO);
                if (legalLandfillWasteTypeEntity == null)
                    return ResultDTO.Fail("Mapping legal landfill waste type failed");

                ResultDTO resultCreate = await _legalLandfillWasteTypeRepository.Update(legalLandfillWasteTypeEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())                
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Delete
        public async Task<ResultDTO> DeleteLegalLandfillWasteType(LegalLandfillWasteTypeDTO legalLandfillWasteTypeDTO)
        {
            try
            {
                LegalLandfillWasteType legalLandfillWasteTypeEntity = _mapper.Map<LegalLandfillWasteType>(legalLandfillWasteTypeDTO);
                if (legalLandfillWasteTypeEntity == null)
                    return ResultDTO.Fail("Mapping legal landfill waste type failed");

                ResultDTO<IEnumerable<LegalLandfillWasteImport>>? resultGetWasteImports = await _legalLandfillWasteImportRepository.GetAll();
                if (resultGetWasteImports.IsSuccess == false && resultGetWasteImports.HandleError())
                    return ResultDTO.Fail(resultGetWasteImports.ErrMsg!);
                
                if(resultGetWasteImports.Data == null)
                    return ResultDTO.Fail("Waste imports list not found");
                
                bool isWasteTypeUsed = resultGetWasteImports.Data.Any(x => x.LegalLandfillWasteTypeId == legalLandfillWasteTypeEntity.Id);

                if (isWasteTypeUsed)                
                    return ResultDTO.Fail("Waste type is used in waste imports, please delete the waste import.");                

                ResultDTO resultCreate = await _legalLandfillWasteTypeRepository.Delete(legalLandfillWasteTypeEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                    return ResultDTO.Fail(resultCreate.ErrMsg!);                

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Read
        public async Task<ResultDTO<List<LegalLandfillWasteTypeDTO>>> GetAllLegalLandfillWasteTypes()
        {
            try
            {
                ResultDTO<IEnumerable<LegalLandfillWasteType>> resultGetAllEntites = await _legalLandfillWasteTypeRepository.GetAll();

                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())                
                    return ResultDTO<List<LegalLandfillWasteTypeDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                if (resultGetAllEntites.Data == null)
                    return ResultDTO<List<LegalLandfillWasteTypeDTO>>.Fail("Waste type not found");

                List<LegalLandfillWasteTypeDTO> dtos = _mapper.Map<List<LegalLandfillWasteTypeDTO>>(resultGetAllEntites.Data);
                if (dtos == null)
                    return ResultDTO<List<LegalLandfillWasteTypeDTO>>.Fail("Mapping legal landfill waste type list failed");

                return ResultDTO<List<LegalLandfillWasteTypeDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillWasteTypeDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<LegalLandfillWasteTypeDTO>> GetLegalLandfillWasteTypeById(Guid legalLandfillWasteTypeId)
        {
            try
            {
                ResultDTO<LegalLandfillWasteType?> resultGetEntity = await _legalLandfillWasteTypeRepository.GetById(legalLandfillWasteTypeId);
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())                
                    return ResultDTO<LegalLandfillWasteTypeDTO>.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data == null)
                    return ResultDTO<LegalLandfillWasteTypeDTO>.Fail("Waste type not found");

                LegalLandfillWasteTypeDTO dto = _mapper.Map<LegalLandfillWasteTypeDTO>(resultGetEntity.Data);
                if (dto == null)
                    return ResultDTO<LegalLandfillWasteTypeDTO>.Fail("Mapping waste type failed");

                return ResultDTO<LegalLandfillWasteTypeDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<LegalLandfillWasteTypeDTO>.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion
    }
}
