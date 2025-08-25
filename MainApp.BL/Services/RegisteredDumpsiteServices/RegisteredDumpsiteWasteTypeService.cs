using AutoMapper;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using Entities.RegisteredDumpsiteEntities;
using MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;
using Microsoft.Extensions.Logging;
using SD;

namespace MainApp.BL.Services.RegisteredDumpsiteServices;

public class RegisteredDumpsiteWasteTypeService : IRegisteredDumpsiteWasteTypeService
{
    private readonly IRegisteredDumpsiteWasteTypeRepository _registeredDumpsiteWasteTypeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisteredDumpsiteWasteTypeService> _logger;
    private readonly IRegisteredDumpsiteRepository _registeredDumpsiteRepository;

    public RegisteredDumpsiteWasteTypeService(IRegisteredDumpsiteWasteTypeRepository registeredDumpsiteWasteTypeRepository, IMapper mapper, ILogger<RegisteredDumpsiteWasteTypeService> logger, IRegisteredDumpsiteRepository registeredDumpsiteRepository)
    {
        _registeredDumpsiteWasteTypeRepository = registeredDumpsiteWasteTypeRepository;
        _mapper = mapper;
        _logger = logger;
        _registeredDumpsiteRepository = registeredDumpsiteRepository;
    }

    #region Create
    public async Task<ResultDTO> CreateRegisteredDumpsiteWasteType(RegisteredDumpsiteWasteTypeDTO registeredDumpsiteWasteTypeDTO)
    {
        try
        {
            RegisteredDumpsiteWasteType? registeredDumpsiteWasteTypeEntity = _mapper.Map<RegisteredDumpsiteWasteType>(registeredDumpsiteWasteTypeDTO);
            if (registeredDumpsiteWasteTypeEntity == null)
                return ResultDTO.Fail("Mapping registered dumpsite waste type failed");

            ResultDTO resultCreate = await _registeredDumpsiteWasteTypeRepository.Create(registeredDumpsiteWasteTypeEntity);
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
    public async Task<ResultDTO> EditRegisteredDumpsiteWasteType(RegisteredDumpsiteWasteTypeDTO registeredDumpsiteWasteTypeDTO)
    {
        try
        {
            RegisteredDumpsiteWasteType registeredDumpsiteWasteTypeEntity = _mapper.Map<RegisteredDumpsiteWasteType>(registeredDumpsiteWasteTypeDTO);
            if (registeredDumpsiteWasteTypeEntity == null)
                return ResultDTO.Fail("Mapping registered dumpsite waste type failed");

            ResultDTO resultCreate = await _registeredDumpsiteWasteTypeRepository.Update(registeredDumpsiteWasteTypeEntity);
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
    public async Task<ResultDTO> DeleteRegisteredDumpsiteWasteType(RegisteredDumpsiteWasteTypeDTO registeredDumpsiteWasteTypeDTO)
    {
        try
        {
            RegisteredDumpsiteWasteType registeredDumpsiteWasteTypeEntity = _mapper.Map<RegisteredDumpsiteWasteType>(registeredDumpsiteWasteTypeDTO);
            if (registeredDumpsiteWasteTypeEntity == null)
                return ResultDTO.Fail("Mapping registered dumpsite waste type failed");

            ResultDTO<IEnumerable<RegisteredDumpsite>>? resultGetRegisteredDumpsites = await _registeredDumpsiteRepository.GetAll();
            if (resultGetRegisteredDumpsites.IsSuccess == false && resultGetRegisteredDumpsites.HandleError())
                return ResultDTO.Fail(resultGetRegisteredDumpsites.ErrMsg!);

            if (resultGetRegisteredDumpsites.Data == null)
                return ResultDTO.Fail("Registered dumpsites list not found");

            bool isWasteTypeUsed = resultGetRegisteredDumpsites.Data.Any(x => x.RegisteredDumpsiteWasteTypeId == registeredDumpsiteWasteTypeEntity.Id);

            if (isWasteTypeUsed)
                return ResultDTO.Fail("Waste type is used in registered dumpsites, please delete the registered dumpsite.");

            ResultDTO resultCreate = await _registeredDumpsiteWasteTypeRepository.Delete(registeredDumpsiteWasteTypeEntity);
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
    public async Task<ResultDTO<List<RegisteredDumpsiteWasteTypeDTO>>> GetAllRegisteredDumpsiteWasteTypes()
    {
        try
        {
            ResultDTO<IEnumerable<RegisteredDumpsiteWasteType>> resultGetAllEntites = await _registeredDumpsiteWasteTypeRepository.GetAll();

            if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                return ResultDTO<List<RegisteredDumpsiteWasteTypeDTO>>.Fail(resultGetAllEntites.ErrMsg!);
            if (resultGetAllEntites.Data == null)
                return ResultDTO<List<RegisteredDumpsiteWasteTypeDTO>>.Fail("Waste type not found");

            List<RegisteredDumpsiteWasteTypeDTO> dtos = _mapper.Map<List<RegisteredDumpsiteWasteTypeDTO>>(resultGetAllEntites.Data);
            if (dtos == null)
                return ResultDTO<List<RegisteredDumpsiteWasteTypeDTO>>.Fail("Mapping registered dumpsite waste type list failed");

            return ResultDTO<List<RegisteredDumpsiteWasteTypeDTO>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<List<RegisteredDumpsiteWasteTypeDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<RegisteredDumpsiteWasteTypeDTO>> GetRegisteredDumpsiteWasteTypeById(Guid registeredDumpsiteWasteTypeId)
    {
        try
        {
            ResultDTO<RegisteredDumpsiteWasteType?> resultGetEntity = await _registeredDumpsiteWasteTypeRepository.GetById(registeredDumpsiteWasteTypeId);
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                return ResultDTO<RegisteredDumpsiteWasteTypeDTO>.Fail(resultGetEntity.ErrMsg!);
            if (resultGetEntity.Data == null)
                return ResultDTO<RegisteredDumpsiteWasteTypeDTO>.Fail("Waste type not found");

            RegisteredDumpsiteWasteTypeDTO dto = _mapper.Map<RegisteredDumpsiteWasteTypeDTO>(resultGetEntity.Data);
            if (dto == null)
                return ResultDTO<RegisteredDumpsiteWasteTypeDTO>.Fail("Mapping waste type failed");

            return ResultDTO<RegisteredDumpsiteWasteTypeDTO>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<RegisteredDumpsiteWasteTypeDTO>.ExceptionFail(ex.Message, ex);
        }
    }
    #endregion
}
