using AutoMapper;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using Entities.RegisteredDumpsiteEntities;
using MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using SD;
using SD.Enums;

namespace MainApp.BL.Services.RegisteredDumpsiteServices;

public class RegisteredDumpsiteService : IRegisteredDumpsiteService
{
    private readonly IRegisteredDumpsiteRepository _registeredDumpsiteRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisteredDumpsiteService> _logger;

    public RegisteredDumpsiteService(IRegisteredDumpsiteRepository RegisteredDumpsiteRepository, IMapper mapper, ILogger<RegisteredDumpsiteService> logger)
    {
        _registeredDumpsiteRepository = RegisteredDumpsiteRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResultDTO<List<RegisteredDumpsiteDTO>>> GetAllRegisteredDumpsitesDTOs()
    {
        try
        {
            ResultDTO<IEnumerable<RegisteredDumpsite>> resultGetAllEntites =
                await _registeredDumpsiteRepository.GetAll(includeProperties: "CreatedBy,RegisteredDumpsiteWasteType,RegisteredDumpsiteRiskLevel");

            if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                return ResultDTO<List<RegisteredDumpsiteDTO>>.Fail(resultGetAllEntites.ErrMsg!);
            if (resultGetAllEntites.Data == null)
                return ResultDTO<List<RegisteredDumpsiteDTO>>.Fail("Registered dumpsite listlist not found");

            List<RegisteredDumpsiteDTO>? dtos = _mapper.Map<List<RegisteredDumpsiteDTO>>(resultGetAllEntites.Data);
            if (dtos == null)
                return ResultDTO<List<RegisteredDumpsiteDTO>>.Fail("Mapping registered dumpsite failed");

            return ResultDTO<List<RegisteredDumpsiteDTO>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<List<RegisteredDumpsiteDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<RegisteredDumpsiteDTO?>> GetRegisteredDumpsiteById(Guid id)
    {
        try
        {
            ResultDTO<RegisteredDumpsite?> resultGetEntity = await _registeredDumpsiteRepository.GetById(id, includeProperties: "CreatedBy");
            if (resultGetEntity.IsSuccess == false && !resultGetEntity.HandleError())
                return ResultDTO<RegisteredDumpsiteDTO?>.Fail(resultGetEntity.ErrMsg!);
            if (resultGetEntity.Data == null)
                return ResultDTO<RegisteredDumpsiteDTO?>.Fail("Registered dumpsite not found");

            RegisteredDumpsiteDTO dto = _mapper.Map<RegisteredDumpsiteDTO>(resultGetEntity.Data);
            if (dto == null)
                return ResultDTO<RegisteredDumpsiteDTO?>.Fail("Mapping  registered dumpsite failed");

            return ResultDTO<RegisteredDumpsiteDTO?>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<RegisteredDumpsiteDTO?>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO> CreateRegisteredDumpsiteFromDTO(RegisteredDumpsiteDTO dto)
    {
        try
        {
            if (dto is null)
                return ResultDTO.Fail("DTO Object is null");

            RegisteredDumpsite registeredDumpsite = _mapper.Map<RegisteredDumpsite>(dto);
            if (registeredDumpsite is null)
                return ResultDTO.Fail("DTO not mapped");
            registeredDumpsite.RegisteredDumpsiteStatusId = (int)RegisteredDumpsiteStatusId.Detected;

            ResultDTO resultCreate = await _registeredDumpsiteRepository.Create(registeredDumpsite);
            if (resultCreate.IsSuccess == false && resultCreate.HandleError())
                return ResultDTO.Fail(resultCreate.ErrMsg!);

            return resultCreate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO> UpdateRegisteredDumpsiteFromDTO(RegisteredDumpsiteDTO dto)
    {
        try
        {
            if (dto is null)
                return ResultDTO.Fail("DTO Object is null");

            if (dto.Id is null)
                return ResultDTO.Fail("DTO Id is null");

            RegisteredDumpsite RegisteredDumpsite = _mapper.Map<RegisteredDumpsite>(dto);
            if (RegisteredDumpsite is null)
                return ResultDTO.Fail("DTO not mapped");

            ResultDTO<RegisteredDumpsite?> resultGetById =
                await _registeredDumpsiteRepository.GetById(RegisteredDumpsite.Id, track: true);
            if (resultGetById.IsSuccess == false && resultGetById.HandleError())
                return ResultDTO.Fail(resultGetById.ErrMsg!);
            if (resultGetById.Data == null)
                return ResultDTO.Fail(" Registered dumpsite not found");

            if (dto.Geom == null)
            {
                dto.Geom = resultGetById.Data?.Geom;
            }
            dto.CreatedOn = resultGetById.Data?.CreatedOn;
            dto.CreatedById = resultGetById.Data?.CreatedById;
            _mapper.Map(dto, resultGetById.Data);

            ResultDTO resultUpdate = await _registeredDumpsiteRepository.Update(resultGetById.Data!);
            if (resultUpdate.IsSuccess == false && resultUpdate.HandleError())
                return ResultDTO.Fail(resultUpdate.ErrMsg!);

            return resultUpdate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO> DeleteRegisteredDumpsiteFromDTO(RegisteredDumpsiteDTO dto)
    {
        try
        {
            if (dto is null)
                return ResultDTO.Fail("DTO Object is null");

            if (dto.Id is null)
                return ResultDTO.Fail("DTO Id is null");

            RegisteredDumpsite RegisteredDumpsite = _mapper.Map<RegisteredDumpsite>(dto);
            if (RegisteredDumpsite is null)
                return ResultDTO.Fail("DTO not mapped");

            ResultDTO<RegisteredDumpsite?> resultGetById =
                await _registeredDumpsiteRepository.GetById(RegisteredDumpsite.Id, track: true);
            if (resultGetById.IsSuccess == false && resultGetById.HandleError())
                return ResultDTO.Fail(resultGetById.ErrMsg!);
            if (resultGetById.Data == null)
                return ResultDTO.Fail(" Registered dumpsite not found");

            ResultDTO resultDelete = await _registeredDumpsiteRepository.Delete(resultGetById.Data!);
            if (resultDelete.IsSuccess == false && resultDelete.HandleError())
                return ResultDTO.Fail(resultDelete.ErrMsg!);

            return resultDelete;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO> MergeRegisteredDumpsites(CreateRegisteredDumpsiteDTO newDumpsiteData, List<Guid> existingIds)
    {
        try
        {
            if (newDumpsiteData == null || existingIds == null || !existingIds.Any())
                return ResultDTO.Fail("Invalid merge data");

            // Get existing dumpsites
            var existingDumpsites = new List<RegisteredDumpsite>();
            foreach (var id in existingIds)
            {
                var result = await _registeredDumpsiteRepository.GetById(id, track: true);
                if (result.IsSuccess && result.Data != null)
                    existingDumpsites.Add(result.Data);
            }

            if (!existingDumpsites.Any())
                return ResultDTO.Fail("No existing dumpsites found to merge");

            // Convert new dumpsite GeoJSON to Polygon
            var geoJsonReader = new NetTopologySuite.IO.GeoJsonReader();
            var feature = geoJsonReader.Read<NetTopologySuite.Features.Feature>(newDumpsiteData.EnteredZonePolygon);
            var newGeometry = feature.Geometry;

            // Perform union operation using NetTopologySuite
            var unionGeometry = newGeometry;
            foreach (var existingDumpsite in existingDumpsites)
            {
                unionGeometry = (Polygon)unionGeometry.Union(existingDumpsite.Geom);
            }

            if (unionGeometry is not Polygon polygonResult)
                return ResultDTO.Fail("The merged geometry is not a single polygon. Please ensure the input zones are contiguous or simplify them.");

            // Use the first existing dumpsite as the base and update its geometry
            var baseDumpsite = existingDumpsites.First();
            baseDumpsite.Geom = polygonResult;

            // Delete other existing dumpsites
            var dumpsitesToDelete = existingDumpsites.Skip(1);
            foreach (var dumpsite in dumpsitesToDelete)
            {
                var deleteResult = await _registeredDumpsiteRepository.Delete(dumpsite);
                if (!deleteResult.IsSuccess && deleteResult.HandleError())
                    return ResultDTO.Fail($"Failed to delete dumpsite: {deleteResult.ErrMsg}");
            }

            // Update the base dumpsite with merged geometry
            var updateResult = await _registeredDumpsiteRepository.Update(baseDumpsite);
            if (!updateResult.IsSuccess && updateResult.HandleError())
                return ResultDTO.Fail(updateResult.ErrMsg!);

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }
}
