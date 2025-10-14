using AutoMapper;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using Entities.RegisteredDumpsiteEntities;
using MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using SD;
using SD.Enums;

namespace MainApp.BL.Services.RegisteredDumpsiteServices;

public class RegisteredDumpsiteService : IRegisteredDumpsiteService
{
    private readonly IRegisteredDumpsiteRepository _registeredDumpsiteRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisteredDumpsiteService> _logger;
    private readonly IRegisteredDumpsiteFileRepository _registeredDumpsiteFileRepository;
    private readonly IRegisteredDumpsiteInspectionRepository _inspectionRepository;
    private readonly IRegisteredDumpsiteInspectionFileRepository _inspectionFileRepository;



    public RegisteredDumpsiteService(IRegisteredDumpsiteRepository RegisteredDumpsiteRepository, IMapper mapper, ILogger<RegisteredDumpsiteService> logger, IRegisteredDumpsiteFileRepository registeredDumpsiteFileRepository, IRegisteredDumpsiteInspectionRepository inspectionRepository, IRegisteredDumpsiteInspectionFileRepository inspectionFileRepository)
    {
        _registeredDumpsiteRepository = RegisteredDumpsiteRepository;
        _mapper = mapper;
        _logger = logger;
        _registeredDumpsiteFileRepository = registeredDumpsiteFileRepository;
        _inspectionRepository = inspectionRepository;
        _inspectionFileRepository = inspectionFileRepository;
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
            ResultDTO<RegisteredDumpsite?> resultGetEntity = await _registeredDumpsiteRepository.GetById(id, includeProperties: "CreatedBy,RegisteredDumpsiteWasteType,RegisteredDumpsiteRiskLevel");
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

    public async Task<ResultDTO> MergeRegisteredDumpsites(CreateRegisteredDumpsiteDTO newDumpsiteData, List<Guid> existingIds, Guid? targetDumpsiteId = null)
    {
        try
        {
            if (newDumpsiteData == null || existingIds == null || !existingIds.Any())
                return ResultDTO.Fail("Invalid merge data");

            var existingDumpsites = new List<RegisteredDumpsite>();
            foreach (var id in existingIds)
            {
                var result = await _registeredDumpsiteRepository.GetById(id, track: true);
                if (result.IsSuccess && result.Data != null)
                    existingDumpsites.Add(result.Data);
            }

            if (!existingDumpsites.Any())
                return ResultDTO.Fail("No existing dumpsites found to merge");

            RegisteredDumpsite targetDumpsite = null;
            List<RegisteredDumpsite> dumpsitesToDelete = new List<RegisteredDumpsite>();

            if (targetDumpsiteId.HasValue)
            {
                targetDumpsite = existingDumpsites.FirstOrDefault(d => d.Id == targetDumpsiteId.Value);
                if (targetDumpsite == null)
                    return ResultDTO.Fail("Specified target dumpsite not found");

                dumpsitesToDelete = existingDumpsites.Where(d => d.Id != targetDumpsiteId.Value).ToList();
            }
            else
            {
                targetDumpsite = new RegisteredDumpsite
                {
                    Id = Guid.NewGuid(),
                    Name = newDumpsiteData.Name,
                    Description = newDumpsiteData.Description,
                    IsEnabled = newDumpsiteData.IsEnabled,
                    RegisteredDumpsiteWasteTypeId = (Guid)newDumpsiteData.RegisteredDumpsiteWasteTypeId,
                    RegisteredDumpsiteRiskLevelId = (Guid)newDumpsiteData.RegisteredDumpsiteRiskLevelId,
                    CreatedById = newDumpsiteData.CreatedById ?? "system",
                    CreatedOn = DateTime.UtcNow
                };
                dumpsitesToDelete = existingDumpsites.ToList();
            }

            var geoJsonReader = new NetTopologySuite.IO.GeoJsonReader();
            var feature = geoJsonReader.Read<Feature>(newDumpsiteData.EnteredZonePolygon);
            var newGeometry = feature.Geometry;

            var unionGeometry = newGeometry;
            foreach (var existingDumpsite in existingDumpsites)
            {
                unionGeometry = (Polygon)unionGeometry.Union(existingDumpsite.Geom);
            }

            if (unionGeometry is not Polygon polygonResult)
                return ResultDTO.Fail("The merged geometry is not a single polygon. Please ensure the input zones are contiguous or simplify them.");

            targetDumpsite.Geom = polygonResult;
            targetDumpsite.RegisteredDumpsiteStatusId = (int)RegisteredDumpsiteStatusId.Detected;
            ResultDTO saveResult;
            if (targetDumpsiteId.HasValue)
            {
                saveResult = await _registeredDumpsiteRepository.Update(targetDumpsite);
            }
            else
            {
                saveResult = await _registeredDumpsiteRepository.Create(targetDumpsite);
            }

            if (!saveResult.IsSuccess && saveResult.HandleError())
                return ResultDTO.Fail(saveResult.ErrMsg!);

            foreach (var dumpsite in dumpsitesToDelete)
            {
                var deleteResult = await _registeredDumpsiteRepository.Delete(dumpsite);
                if (!deleteResult.IsSuccess && deleteResult.HandleError())
                    return ResultDTO.Fail($"Failed to delete dumpsite: {deleteResult.ErrMsg}");
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }
    public async Task<ResultDTO<List<RegisteredDumpsiteFileDTO>>> GetFilesByDumpsiteId(Guid dumpsiteId)
    {
        try
        {
            var result = await _registeredDumpsiteFileRepository.GetAll(
                filter: f => f.RegisteredDumpsiteId == dumpsiteId,
                includeProperties: "CreatedBy");

            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<List<RegisteredDumpsiteFileDTO>>.Fail(result.ErrMsg!);

            if (result.Data == null)
                return ResultDTO<List<RegisteredDumpsiteFileDTO>>.Fail("Files not found");

            var dtos = _mapper.Map<List<RegisteredDumpsiteFileDTO>>(result.Data);
            if (dtos == null)
                return ResultDTO<List<RegisteredDumpsiteFileDTO>>.Fail("Mapping files failed");

            return ResultDTO<List<RegisteredDumpsiteFileDTO>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<List<RegisteredDumpsiteFileDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<RegisteredDumpsiteFileDTO>> GetSingleFileById(Guid fileId)
    {
        try
        {
            var result = await _registeredDumpsiteFileRepository.GetById(fileId, includeProperties: "CreatedBy");
            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail(result.ErrMsg!);

            if (result.Data == null)
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail("Files not found");

            var dtos = _mapper.Map<RegisteredDumpsiteFileDTO>(result.Data);
            if (dtos == null)
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail("Mapping files failed");

            return ResultDTO<RegisteredDumpsiteFileDTO>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<RegisteredDumpsiteFileDTO>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<RegisteredDumpsiteFileDTO>> UploadFile(RegisteredDumpsiteFileDTO dumpsiteFileDto)
    {
        try
        {
            var dumpsiteFile = _mapper.Map<RegisteredDumpsiteFile>(dumpsiteFileDto);

            var createResult = await _registeredDumpsiteFileRepository.Create(dumpsiteFile);
            if (!createResult.IsSuccess && createResult.HandleError())
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail(createResult.ErrMsg!);

            return ResultDTO<RegisteredDumpsiteFileDTO>.Ok(dumpsiteFileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving dumpsite file");
            return ResultDTO<RegisteredDumpsiteFileDTO>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO> DeleteFile(Guid fileId)
    {
        try
        {
            var getResult = await _registeredDumpsiteFileRepository.GetById(fileId);
            if (!getResult.IsSuccess && getResult.HandleError())
                return ResultDTO.Fail(getResult.ErrMsg!);

            if (getResult.Data == null)
                return ResultDTO.Fail("File not found");

            // Only delete from DB now
            var deleteResult = await _registeredDumpsiteFileRepository.Delete(getResult.Data);
            if (!deleteResult.IsSuccess && deleteResult.HandleError())
                return ResultDTO.Fail(deleteResult.ErrMsg!);

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from DB");
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<List<RegisteredDumpsiteInspectionDTO>>> GetInspectionsByDumpsiteId(Guid dumpsiteId)
    {
        try
        {
            var result = await _inspectionRepository.GetAll(
                filter: i => i.RegisteredDumpsiteId == dumpsiteId,
                includeProperties: "Assignments.Inspector,CreatedBy,InspectionFiles,RegisteredDumpsiteInspectionStatus");

            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Fail(result.ErrMsg!);

            if (result.Data == null)
                return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Fail("Inspections not found");

            var dtos = _mapper.Map<List<RegisteredDumpsiteInspectionDTO>>(result.Data);
            if (dtos == null)
                return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Fail("Mapping inspections failed");

            return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<RegisteredDumpsiteInspectionDTO?>> GetInspectionById(Guid id)
    {
        try
        {
            var result = await _inspectionRepository.GetById(id,
                includeProperties: "CreatedBy,InspectionFiles");

            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<RegisteredDumpsiteInspectionDTO?>.Fail(result.ErrMsg!);

            if (result.Data == null)
                return ResultDTO<RegisteredDumpsiteInspectionDTO?>.Fail("Inspection not found");

            var dto = _mapper.Map<RegisteredDumpsiteInspectionDTO>(result.Data);
            return ResultDTO<RegisteredDumpsiteInspectionDTO?>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<RegisteredDumpsiteInspectionDTO?>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO> CreateInspection(RegisteredDumpsiteInspectionDTO dto)
    {
        try
        {
            if (dto == null)
                return ResultDTO.Fail("DTO Object is null");

            var inspection = _mapper.Map<RegisteredDumpsiteInspection>(dto);
            if (inspection == null)
                return ResultDTO.Fail("DTO not mapped");
            inspection.RegisteredDumpsiteInspectionStatusId = (int)SD.Enums.RegisteredDumpsiteInspectionStatus.Scheduled;

            var result = await _inspectionRepository.Create(inspection);
            if (!result.IsSuccess && result.HandleError())
                return ResultDTO.Fail(result.ErrMsg!);

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO> UpdateInspection(RegisteredDumpsiteInspectionDTO dto)
    {
        try
        {
            if (dto == null || dto.Id == null)
                return ResultDTO.Fail("DTO Object or Id is null");

            var getResult = await _inspectionRepository.GetById((Guid)dto.Id, track: true);
            if (!getResult.IsSuccess && getResult.HandleError())
                return ResultDTO.Fail(getResult.ErrMsg!);

            if (getResult.Data == null)
                return ResultDTO.Fail("Inspection not found");

            dto.CreatedOn = getResult.Data.CreatedOn;
            dto.CreatedById = getResult.Data.CreatedById;

            _mapper.Map(dto, getResult.Data);

            var updateResult = await _inspectionRepository.Update(getResult.Data);
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

    public async Task<ResultDTO> DeleteInspection(Guid id)
    {
        try
        {
            var getResult = await _inspectionRepository.GetById(id, track: true);
            if (!getResult.IsSuccess && getResult.HandleError())
                return ResultDTO.Fail(getResult.ErrMsg!);

            if (getResult.Data == null)
                return ResultDTO.Fail("Inspection not found");

            var deleteResult = await _inspectionRepository.Delete(getResult.Data);
            if (!deleteResult.IsSuccess && deleteResult.HandleError())
                return ResultDTO.Fail(deleteResult.ErrMsg!);

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO> AssignInspector(Guid inspectionId, string inspectorId)
    {
        try
        {
            var getResult = await _inspectionRepository.GetById(inspectionId, track: true, includeProperties: "Assignments");
            if (!getResult.IsSuccess && getResult.HandleError())
                return ResultDTO.Fail(getResult.ErrMsg!);

            if (getResult.Data == null)
                return ResultDTO.Fail("Inspection not found");

            var alreadyAssigned = getResult.Data.Assignments
                .Any(a => a.InspectorId == inspectorId);

            if (alreadyAssigned)
                return ResultDTO.Fail("Inspector is already assigned to this inspection");

            var assignment = new InspectionAssignment
            {
                RegisteredDumpsiteInspectionId = inspectionId,
                InspectorId = inspectorId,
                AssignedOn = DateTime.UtcNow
            };

            getResult.Data.Assignments.Add(assignment);

            var updateResult = await _inspectionRepository.Update(getResult.Data);
            await _inspectionRepository.SaveChangesAsync();

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


    public async Task<ResultDTO> CompleteInspection(CompleteInspectionDTO inspectionDTO)
    {
        try
        {
            var getResult = await _inspectionRepository.GetById(inspectionDTO.InspectionId, track: true);
            if (!getResult.IsSuccess && getResult.HandleError())
                return ResultDTO.Fail(getResult.ErrMsg!);

            if (getResult.Data == null)
                return ResultDTO.Fail("Inspection not found");

            getResult.Data.RegisteredDumpsiteInspectionStatusId = (int)SD.Enums.RegisteredDumpsiteInspectionStatus.Completed;
            getResult.Data.Findings = inspectionDTO.Findings;
            getResult.Data.Recommendations = inspectionDTO.Recommendations;
            getResult.Data.InspectionDate = DateTime.UtcNow;
            getResult.Data.Notes = inspectionDTO.Notes;

            var updateResult = await _inspectionRepository.Update(getResult.Data);
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
    public async Task<ResultDTO<RegisteredDumpsiteDTO?>> ConvertDetectedDumpsiteAsync(ConvertDetectedDumpsiteRequest request)
    {
        try
        {
            // Validate the request
            var validationResult = ValidateConvertRequest(request);
            if (!validationResult.IsSuccess)
            {
                return ResultDTO<RegisteredDumpsiteDTO?>.Fail(validationResult.ErrMsg!);
            }

            // Parse GeoJSON to Polygon using NetTopologySuite
            var polygon = ParseGeoJsonToPolygon(request.EnteredZonePolygon);
            if (polygon == null)
            {
                return ResultDTO<RegisteredDumpsiteDTO?>.Fail("Invalid geometry data provided");
            }

            // Check for overlaps with existing registered dumpsites
            var existingDumpsites = await _registeredDumpsiteRepository.GetAll();
            var overlappingDumpsites = existingDumpsites.Data.Where(d => d.Geom.Intersects(polygon)).ToList();

            if (overlappingDumpsites.Any())
            {
                return ResultDTO<RegisteredDumpsiteDTO?>.Fail("The converted dumpsite overlaps with existing registered dumpsites. Please handle overlaps first.");
            }

            // Create new registered dumpsite
            var newDumpsite = new RegisteredDumpsite
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                RegisteredDumpsiteWasteTypeId = request.RegisteredDumpsiteWasteTypeId,
                RegisteredDumpsiteRiskLevelId = request.RegisteredDumpsiteRiskLevelId,
                Geom = polygon,
                IsEnabled = request.IsEnabled,
                CreatedById = "b9e7defd-4f17-428a-813b-211c2521bfa8",
                CreatedOn = DateTime.UtcNow,
                RegisteredDumpsiteStatusId = 1 // Set default status (you might need to adjust this)
            };

            // Add conversion metadata to description if provided
            if (!string.IsNullOrEmpty(request.ConversionNotes))
            {
                newDumpsite.Description += $"\n\n[Conversion Notes: {request.ConversionNotes}]";
            }

            await _registeredDumpsiteRepository.Create(newDumpsite);

            // Save changes
            var saveResult = await _registeredDumpsiteRepository.SaveChangesAsync();
            //if (saveResult > 0)
            //{
            var dto = _mapper.Map<RegisteredDumpsiteDTO>(newDumpsite);
            return ResultDTO<RegisteredDumpsiteDTO?>.Ok(dto);
            //}

            //return ResultDTO<RegisteredDumpsiteDTO?>.Fail("Failed to save the converted dumpsite");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<RegisteredDumpsiteDTO?>.ExceptionFail(ex.Message, ex);
        }
    }

    private ResultDTO<RegisteredDumpsiteDTO?> ValidateConvertRequest(ConvertDetectedDumpsiteRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Name is required");

        if (request.RegisteredDumpsiteWasteTypeId == Guid.Empty)
            errors.Add("Waste Type is required");

        if (request.RegisteredDumpsiteRiskLevelId == Guid.Empty)
            errors.Add("Risk Level is required");

        if (string.IsNullOrWhiteSpace(request.EnteredZonePolygon))
            errors.Add("Geometry data is required");

        if (errors.Any())
        {
            return ResultDTO<RegisteredDumpsiteDTO?>.Fail(string.Join(", ", errors));
        }

        return ResultDTO<RegisteredDumpsiteDTO?>.Ok(null);
    }

    private Polygon? ParseGeoJsonToPolygon(string geoJsonString)
    {
        try
        {
            // Using NetTopologySuite GeoJSON reader
            var geoJsonReader = new NetTopologySuite.IO.GeoJsonReader();
            var geometry = geoJsonReader.Read<Geometry>(geoJsonString);

            // Ensure it's a Polygon
            if (geometry is Polygon polygon)
            {
                return polygon;
            }

            _logger.LogWarning("GeoJSON does not represent a valid Polygon geometry. Only Polygon geometries are supported.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return null;
        }
    }


    public async Task<ResultDTO<RegisteredDumpsiteInspectionFileDTO>> UploadInspectionFile(RegisteredDumpsiteInspectionFileDTO inspectionFileDto)
    {
        try
        {
            var inspectionFile = _mapper.Map<RegisteredDumpsiteInspectionFile>(inspectionFileDto);

            var createResult = await _inspectionFileRepository.Create(inspectionFile);
            if (!createResult.IsSuccess && createResult.HandleError())
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail(createResult.ErrMsg!);

            return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Ok(inspectionFileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving inspection file");
            return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO> DeleteInspectionFile(Guid fileId)
    {
        try
        {
            var getResult = await _inspectionFileRepository.GetById(fileId);
            if (!getResult.IsSuccess && getResult.HandleError())
                return ResultDTO.Fail(getResult.ErrMsg!);

            if (getResult.Data == null)
                return ResultDTO.Fail("File not found");

            var deleteResult = await _inspectionFileRepository.Delete(getResult.Data);
            if (!deleteResult.IsSuccess && deleteResult.HandleError())
                return ResultDTO.Fail(deleteResult.ErrMsg!);

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inspection file from DB");
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<RegisteredDumpsiteInspectionFileDTO>> GetSingleInspectionFileById(Guid fileId)
    {
        try
        {
            var result = await _inspectionFileRepository.GetById(fileId);
            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail(result.ErrMsg!);

            if (result.Data == null)
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail("File not found");

            var dto = _mapper.Map<RegisteredDumpsiteInspectionFileDTO>(result.Data);
            return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>> GetInspectionFilesByInspectionId(Guid inspectionId)
    {
        try
        {
            var result = await _inspectionFileRepository.GetAll(
                filter: f => f.RegisteredDumpsiteInspectionId == inspectionId,
                includeProperties: "CreatedBy");

            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Fail(result.ErrMsg!);

            if (result.Data == null)
                return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Fail("Files not found");

            var dtos = _mapper.Map<List<RegisteredDumpsiteInspectionFileDTO>>(result.Data);
            if (dtos == null)
                return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Fail("Mapping files failed");

            return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<bool>> IsUserAssignedToInspection(Guid inspectionId, string userId)
    {
        try
        {
            var inspectionResult = await _inspectionRepository.GetAll(
                        filter: i => i.Id == inspectionId && i.Assignments.Any(x => x.InspectorId == userId),
                        includeProperties: "Assignments");

            if (!inspectionResult.IsSuccess && inspectionResult.HandleError())
                return ResultDTO<bool>.Fail(inspectionResult.ErrMsg!);

            bool isAssigned = inspectionResult.Data != null && inspectionResult.Data.Any();
            return ResultDTO<bool>.Ok(isAssigned);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<bool>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>> GetInspectionFilesByUserId(string userId)
    {
        try
        {
            var result = await _inspectionFileRepository.GetAll(
                filter: f => f.CreatedById == userId,
                includeProperties: "CreatedBy");

            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Fail(result.ErrMsg!);

            if (result.Data == null)
                return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Fail("Files not found");

            var dtos = _mapper.Map<List<RegisteredDumpsiteInspectionFileDTO>>(result.Data);
            if (dtos == null)
                return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Fail("Mapping files failed");

            return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    public async Task<ResultDTO<List<RegisteredDumpsiteInspectionDTO>>> GetMyAssignedInspections(string userId)
    {
        try
        {
            var result = await _inspectionRepository.GetAll(includeProperties: "RegisteredDumpsite,InspectionFiles,Assignments");

            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Fail(result.ErrMsg!);

            if (result.Data == null)
                return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Fail("Inspections not found");

            var filteredList = result.Data.Where(x => x.Assignments.Any(a => a.InspectorId == userId)).ToList();

            var dtos = _mapper.Map<List<RegisteredDumpsiteInspectionDTO>>(filteredList);
            if (dtos == null)
                return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Fail("Mapping inspections failed");

            return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

}
