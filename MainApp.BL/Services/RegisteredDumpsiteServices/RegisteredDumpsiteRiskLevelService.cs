using AutoMapper;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using Entities.RegisteredDumpsiteEntities;
using MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.RegisteredDumpsiteServices;

public class RegisteredDumpsiteRiskLevelService : IRegisteredDumpsiteRiskLevelService
{
    private readonly IRegisteredDumpsiteRiskLevelRepository _registeredDumpsiteRiskLevelRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisteredDumpsiteRiskLevelService> _logger;
    private readonly IRegisteredDumpsiteRepository _registeredDumpsiteRepository;

    public RegisteredDumpsiteRiskLevelService(IRegisteredDumpsiteRiskLevelRepository registeredDumpsiteRiskLevelRepository, IMapper mapper, ILogger<RegisteredDumpsiteRiskLevelService> logger, IRegisteredDumpsiteRepository registeredDumpsiteRepository)
    {
        _registeredDumpsiteRiskLevelRepository = registeredDumpsiteRiskLevelRepository;
        _mapper = mapper;
        _logger = logger;
        _registeredDumpsiteRepository = registeredDumpsiteRepository;
    }

    #region Create
    public async Task<ResultDTO> CreateRegisteredDumpsiteRiskLevel(RegisteredDumpsiteRiskLevelDTO registeredDumpsiteRiskLevelDTO)
    {
        try
        {
            RegisteredDumpsiteRiskLevel? registeredDumpsiteRiskLevelEntity = _mapper.Map<RegisteredDumpsiteRiskLevel>(registeredDumpsiteRiskLevelDTO);
            if (registeredDumpsiteRiskLevelEntity == null)
                return ResultDTO.Fail("Mapping registered dumpsite risk level failed");

            ResultDTO resultCreate = await _registeredDumpsiteRiskLevelRepository.Create(registeredDumpsiteRiskLevelEntity);
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
    public async Task<ResultDTO> EditRegisteredDumpsiteRiskLevel(RegisteredDumpsiteRiskLevelDTO registeredDumpsiteRiskLevelDTO)
    {
        try
        {
            RegisteredDumpsiteRiskLevel registeredDumpsiteRiskLevelEntity = _mapper.Map<RegisteredDumpsiteRiskLevel>(registeredDumpsiteRiskLevelDTO);
            if (registeredDumpsiteRiskLevelEntity == null)
                return ResultDTO.Fail("Mapping registered dumpsite risk level failed");

            ResultDTO resultCreate = await _registeredDumpsiteRiskLevelRepository.Update(registeredDumpsiteRiskLevelEntity);
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
    public async Task<ResultDTO> DeleteRegisteredDumpsiteRiskLevel(RegisteredDumpsiteRiskLevelDTO registeredDumpsiteRiskLevelDTO)
    {
        try
        {
            RegisteredDumpsiteRiskLevel registeredDumpsiteRiskLevelEntity = _mapper.Map<RegisteredDumpsiteRiskLevel>(registeredDumpsiteRiskLevelDTO);
            if (registeredDumpsiteRiskLevelEntity == null)
                return ResultDTO.Fail("Mapping registered dumpsite risk level failed");

            ResultDTO<IEnumerable<RegisteredDumpsite>>? resultGetRegisteredDumpsites = await _registeredDumpsiteRepository.GetAll();
            if (resultGetRegisteredDumpsites.IsSuccess == false && resultGetRegisteredDumpsites.HandleError())
                return ResultDTO.Fail(resultGetRegisteredDumpsites.ErrMsg!);

            if (resultGetRegisteredDumpsites.Data == null)
                return ResultDTO.Fail("Registered dumpsites list not found");

            bool isRiskLevelUsed = resultGetRegisteredDumpsites.Data.Any(x => x.RegisteredDumpsiteRiskLevelId == registeredDumpsiteRiskLevelEntity.Id);

            if (isRiskLevelUsed)
                return ResultDTO.Fail("Risk level is used in registered dumpsites, please delete the registered dumpsite.");

            ResultDTO resultCreate = await _registeredDumpsiteRiskLevelRepository.Delete(registeredDumpsiteRiskLevelEntity);
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
    public async Task<ResultDTO<List<RegisteredDumpsiteRiskLevelDTO>>> GetAllRegisteredDumpsiteRiskLevels()
    {
        try
        {
            ResultDTO<IEnumerable<RegisteredDumpsiteRiskLevel>> resultGetAllEntites = await _registeredDumpsiteRiskLevelRepository.GetAll();

            if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                return ResultDTO<List<RegisteredDumpsiteRiskLevelDTO>>.Fail(resultGetAllEntites.ErrMsg!);
            if (resultGetAllEntites.Data == null)
                return ResultDTO<List<RegisteredDumpsiteRiskLevelDTO>>.Fail("Risk level not found");

            List<RegisteredDumpsiteRiskLevelDTO> dtos = _mapper.Map<List<RegisteredDumpsiteRiskLevelDTO>>(resultGetAllEntites.Data);
            if (dtos == null)
                return ResultDTO<List<RegisteredDumpsiteRiskLevelDTO>>.Fail("Mapping registered dumpsite risk level list failed");

            return ResultDTO<List<RegisteredDumpsiteRiskLevelDTO>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<List<RegisteredDumpsiteRiskLevelDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<RegisteredDumpsiteRiskLevelDTO>> GetRegisteredDumpsiteRiskLevelById(Guid registeredDumpsiteRiskLevelId)
    {
        try
        {
            ResultDTO<RegisteredDumpsiteRiskLevel?> resultGetEntity = await _registeredDumpsiteRiskLevelRepository.GetById(registeredDumpsiteRiskLevelId);
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                return ResultDTO<RegisteredDumpsiteRiskLevelDTO>.Fail(resultGetEntity.ErrMsg!);
            if (resultGetEntity.Data == null)
                return ResultDTO<RegisteredDumpsiteRiskLevelDTO>.Fail("Risk level not found");

            RegisteredDumpsiteRiskLevelDTO dto = _mapper.Map<RegisteredDumpsiteRiskLevelDTO>(resultGetEntity.Data);
            if (dto == null)
                return ResultDTO<RegisteredDumpsiteRiskLevelDTO>.Fail("Mapping risk level failed");

            return ResultDTO<RegisteredDumpsiteRiskLevelDTO>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<RegisteredDumpsiteRiskLevelDTO>.ExceptionFail(ex.Message, ex);
        }
    }
    #endregion
}
