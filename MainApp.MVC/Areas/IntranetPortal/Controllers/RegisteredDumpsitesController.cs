using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using SD;
using Services.Interfaces.Services;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers;

[Area("IntranetPortal")]
public class RegisteredDumpsitesController : Controller
{
    private readonly IRegisteredDumpsiteService _registeredDumpsiteService;
    private readonly IRegisteredDumpsiteWasteTypeService _registeredDumpsiteWasteTypeService;
    private readonly IRegisteredDumpsiteRiskLevelService _registeredDumpsiteRiskLevelService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IAppSettingsAccessor _appSettingsAccessor;
    private readonly ILogger<RegisteredDumpsitesController> _logger;
    private readonly IUserManagementService _userManagementService;
    private readonly IDetectionRunService _detectionRunService;

    public RegisteredDumpsitesController(IRegisteredDumpsiteService registeredDumpsiteService, IRegisteredDumpsiteWasteTypeService registeredDumpsiteWasteTypeService, IRegisteredDumpsiteRiskLevelService registeredDumpsiteRiskLevelService, IConfiguration configuration, IMapper mapper, IWebHostEnvironment webHostEnvironment, IAppSettingsAccessor appSettingsAccessor, ILogger<RegisteredDumpsitesController> logger, IUserManagementService userManagementService, IDetectionRunService detectionRunService)
    {
        _registeredDumpsiteService = registeredDumpsiteService;
        _registeredDumpsiteWasteTypeService = registeredDumpsiteWasteTypeService;
        _registeredDumpsiteRiskLevelService = registeredDumpsiteRiskLevelService;
        _configuration = configuration;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
        _appSettingsAccessor = appSettingsAccessor;
        _logger = logger;
        _userManagementService = userManagementService;
        _detectionRunService = detectionRunService;
    }

    #region RegisteredDumpsites
    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO<List<RegisteredDumpsiteDTO>>> GetAlllRegisteredDumpsites()
    {
        try
        {
            ResultDTO<List<RegisteredDumpsiteDTO>> resultGetEntites = await _registeredDumpsiteService.GetAllRegisteredDumpsitesDTOs();
            if (!resultGetEntites.IsSuccess && resultGetEntites.HandleError())
            {
                return ResultDTO<List<RegisteredDumpsiteDTO>>.Fail(resultGetEntites.ErrMsg!);
            }
            if (resultGetEntites.Data == null)
            {
                return ResultDTO<List<RegisteredDumpsiteDTO>>.Fail("Igonre zones are not found");

            }
            return ResultDTO<List<RegisteredDumpsiteDTO>>.Ok(resultGetEntites.Data);
        }
        catch (Exception ex)
        {
            return ResultDTO<List<RegisteredDumpsiteDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO<RegisteredDumpsiteDTO?>> GetRegisteredDumpsiteById(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return ResultDTO<RegisteredDumpsiteDTO?>.Fail("Invalid zone id");
            }


            ResultDTO<RegisteredDumpsiteDTO?> resultGetEntity = await _registeredDumpsiteService.GetRegisteredDumpsiteById(id);
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
            {
                return ResultDTO<RegisteredDumpsiteDTO?>.Fail(resultGetEntity.ErrMsg!);
            }

            return ResultDTO<RegisteredDumpsiteDTO?>.Ok(resultGetEntity.Data);
        }
        catch (Exception ex)
        {
            return ResultDTO<RegisteredDumpsiteDTO?>.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO> AddRegisteredDumpsite([FromBody] RegisteredDumpsiteDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }
            if (string.IsNullOrEmpty(dto.EnteredZonePolygon))
            {
                return ResultDTO.Fail("Invalid entered polygon");
            }
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return ResultDTO.Fail("User not found");
            }

            dto.CreatedById = userId;
            dto.CreatedOn = DateTime.UtcNow;
            dto.Geom = (Polygon)DTOs.Helpers.GeoJsonHelpers.GeoJsonFeatureToGeometry(dto.EnteredZonePolygon);
            ResultDTO resultCreate = await _registeredDumpsiteService.CreateRegisteredDumpsiteFromDTO(dto);
            if (resultCreate.IsSuccess == false && resultCreate.HandleError())
            {
                return ResultDTO.Fail(resultCreate.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO> DeleteRegisteredDumpsite(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return ResultDTO.Fail("Invalid zone id");
            }

            ResultDTO<RegisteredDumpsiteDTO?> resultGetEntity = await _registeredDumpsiteService.GetRegisteredDumpsiteById(id);
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
            {
                return ResultDTO.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO.Fail("Ignore zone does not exist");
            }


            ResultDTO resultDeleteEntity = await _registeredDumpsiteService.DeleteRegisteredDumpsiteFromDTO(resultGetEntity.Data);
            if (resultDeleteEntity.IsSuccess == false && resultDeleteEntity.HandleError())
            {
                return ResultDTO.Fail(resultDeleteEntity.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO> UpdateRegisteredDumpsite([FromBody] RegisteredDumpsiteDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }
            if (!string.IsNullOrEmpty(dto.EnteredZonePolygon))
            {
                dto.Geom = (Polygon)DTOs.Helpers.GeoJsonHelpers.GeoJsonFeatureToGeometry(dto.EnteredZonePolygon);
            }

            ResultDTO resultUpdate = await _registeredDumpsiteService.UpdateRegisteredDumpsiteFromDTO(dto);
            if (resultUpdate.IsSuccess == false && resultUpdate.HandleError())
            {
                return ResultDTO.Fail(resultUpdate.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO> MergeRegisteredDumpsites([FromBody] MergeRegisteredDumpsitesDTO request)
    {
        try
        {
            if (request?.NewDumpsiteData == null || request.ExistingDumpsiteIds == null || !request.ExistingDumpsiteIds.Any())
            {
                return ResultDTO.Fail("Invalid merge request data");
            }

            // Set the current user as creator for new dumpsites
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return ResultDTO.Fail("User not found");
            }
            request.NewDumpsiteData.CreatedById = userId;

            var result = await _registeredDumpsiteService.MergeRegisteredDumpsites(
                request.NewDumpsiteData,
                request.ExistingDumpsiteIds,
                request.TargetDumpsiteId);

            if (!result.IsSuccess && result.HandleError())
            {
                return ResultDTO.Fail(result.ErrMsg!);
            }
            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }
    #endregion

    #region RegisteredDumpsiteWasteType

    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.ViewRegisteredDumpsiteWasteTypes))]
    public async Task<IActionResult> ViewRegisteredDumpsiteWasteTypes()
    {
        try
        {
            var resultDtoList = await _registeredDumpsiteWasteTypeService.GetAllRegisteredDumpsiteWasteTypes();
            if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);

            if (resultDtoList.Data == null)
                return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

            var vmList = _mapper.Map<List<RegisteredDumpsiteWasteTypeViewModel>>(resultDtoList.Data);
            if (vmList == null)
                return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

            return View(vmList);
        }
        catch (Exception ex)
        {
            return HandleErrorRedirect("ErrorViewsPath:Error", 400);
        }
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [HasAuthClaim(nameof(SD.AuthClaims.AddRegisteredDumpsiteWasteType))]
    public async Task<ResultDTO> CreateRegisteredDumpsiteWasteTypeConfirmed(RegisteredDumpsiteWasteTypeViewModel registeredDumpsiteWasteTypeViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }
            var userId = User.FindFirstValue("UserId");
            registeredDumpsiteWasteTypeViewModel.CreatedById = userId;

            var dto = _mapper.Map<RegisteredDumpsiteWasteTypeDTO>(registeredDumpsiteWasteTypeViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultCreate = await _registeredDumpsiteWasteTypeService.CreateRegisteredDumpsiteWasteType(dto);
            if (!resultCreate.IsSuccess && resultCreate.HandleError())
            {
                return ResultDTO.Fail(resultCreate.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [HasAuthClaim(nameof(SD.AuthClaims.DeleteRegisteredDumpsiteWasteType))]
    public async Task<ResultDTO> DeleteRegisteredDumpsiteWasteTypeConfirmed(RegisteredDumpsiteWasteTypeViewModel registeredDumpsiteWasteTypeViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var resultCheckForFiles = await _registeredDumpsiteWasteTypeService.GetRegisteredDumpsiteWasteTypeById(registeredDumpsiteWasteTypeViewModel.Id);
            if (!resultCheckForFiles.IsSuccess && resultCheckForFiles.HandleError())
            {
                return ResultDTO.Fail(resultCheckForFiles.ErrMsg!);
            }
            if (resultCheckForFiles.Data == null)
            {
                return ResultDTO.Fail("Data is null");
            }

            var dto = _mapper.Map<RegisteredDumpsiteWasteTypeDTO>(registeredDumpsiteWasteTypeViewModel);

            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultDelete = await _registeredDumpsiteWasteTypeService.DeleteRegisteredDumpsiteWasteType(dto);
            if (!resultDelete.IsSuccess && resultDelete.HandleError())
            {
                return ResultDTO.Fail(resultDelete.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [HasAuthClaim(nameof(SD.AuthClaims.EditRegisteredDumpsiteWasteType))]
    public async Task<ResultDTO<RegisteredDumpsiteWasteTypeDTO>> GetRegisteredDumpsiteWasteTypeById(Guid registeredDumpsiteWasteTypeId)
    {
        try
        {
            ResultDTO<RegisteredDumpsiteWasteTypeDTO> resultGetEntity = await _registeredDumpsiteWasteTypeService.GetRegisteredDumpsiteWasteTypeById(registeredDumpsiteWasteTypeId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<RegisteredDumpsiteWasteTypeDTO>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<RegisteredDumpsiteWasteTypeDTO>.Fail("Waste Type is null");
            }
            var userId = User.FindFirstValue("UserId");
            resultGetEntity.Data.CreatedById = userId;

            return ResultDTO<RegisteredDumpsiteWasteTypeDTO>.Ok(resultGetEntity.Data);
        }
        catch (Exception ex)
        {
            return ResultDTO<RegisteredDumpsiteWasteTypeDTO>.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.EditRegisteredDumpsiteWasteType))]
    public async Task<ResultDTO> EditRegisteredDumpsiteWasteTypeConfirmed(RegisteredDumpsiteWasteTypeViewModel registeredDumpsiteWasteTypeViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var dto = _mapper.Map<RegisteredDumpsiteWasteTypeDTO>(registeredDumpsiteWasteTypeViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultEdit = await _registeredDumpsiteWasteTypeService.EditRegisteredDumpsiteWasteType(dto);
            if (!resultEdit.IsSuccess && resultEdit.HandleError())
            {
                return ResultDTO.Fail(resultEdit.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetWasteTypes()
    {
        try
        {
            var resultDtoList = await _registeredDumpsiteWasteTypeService.GetAllRegisteredDumpsiteWasteTypes();

            if (!resultDtoList.IsSuccess)
            {
                return Json(new
                {
                    isSuccess = false,
                    errMsg = "Failed to load waste types"
                });
            }

            if (resultDtoList.Data == null)
            {
                return Json(new
                {
                    isSuccess = false,
                    errMsg = "No waste types found"
                });
            }

            var dropdownData = resultDtoList.Data.Select(x => new
            {
                id = x.Id,
                name = x.Name
            }).ToList();

            return Json(new
            {
                isSuccess = true,
                data = dropdownData
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                isSuccess = false,
                errMsg = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRiskLevels()
    {
        try
        {
            var resultDtoList = await _registeredDumpsiteRiskLevelService.GetAllRegisteredDumpsiteRiskLevels();

            if (!resultDtoList.IsSuccess)
            {
                return Json(new
                {
                    isSuccess = false,
                    errMsg = "Failed to load risk levels"
                });
            }

            if (resultDtoList.Data == null)
            {
                return Json(new
                {
                    isSuccess = false,
                    errMsg = "No risk levels found"
                });
            }

            var dropdownData = resultDtoList.Data.Select(x => new
            {
                id = x.Id,
                name = x.Name
            }).ToList();

            return Json(new
            {
                isSuccess = true,
                data = dropdownData
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                isSuccess = false,
                errMsg = ex.Message
            });
        }
    }


    #endregion

    #region RegisteredDumpsiteRiskLevel

    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.ViewRegisteredDumpsiteRiskLevels))]
    public async Task<IActionResult> ViewRegisteredDumpsiteRiskLevels()
    {
        try
        {
            var resultDtoList = await _registeredDumpsiteRiskLevelService.GetAllRegisteredDumpsiteRiskLevels();
            if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);

            if (resultDtoList.Data == null)
                return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

            var vmList = _mapper.Map<List<RegisteredDumpsiteRiskLevelViewModel>>(resultDtoList.Data);
            if (vmList == null)
                return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

            return View(vmList);
        }
        catch (Exception ex)
        {
            return HandleErrorRedirect("ErrorViewsPath:Error", 400);
        }
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [HasAuthClaim(nameof(SD.AuthClaims.AddRegisteredDumpsiteRiskLevel))]
    public async Task<ResultDTO> CreateRegisteredDumpsiteRiskLevelConfirmed(RegisteredDumpsiteRiskLevelViewModel registeredDumpsiteRiskLevelViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }
            var userId = User.FindFirstValue("UserId");
            registeredDumpsiteRiskLevelViewModel.CreatedById = userId;

            var dto = _mapper.Map<RegisteredDumpsiteRiskLevelDTO>(registeredDumpsiteRiskLevelViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultCreate = await _registeredDumpsiteRiskLevelService.CreateRegisteredDumpsiteRiskLevel(dto);
            if (!resultCreate.IsSuccess && resultCreate.HandleError())
            {
                return ResultDTO.Fail(resultCreate.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [HasAuthClaim(nameof(SD.AuthClaims.DeleteRegisteredDumpsiteRiskLevel))]
    public async Task<ResultDTO> DeleteRegisteredDumpsiteRiskLevelConfirmed(RegisteredDumpsiteRiskLevelViewModel registeredDumpsiteRiskLevelViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var resultCheckForFiles = await _registeredDumpsiteRiskLevelService.GetRegisteredDumpsiteRiskLevelById(registeredDumpsiteRiskLevelViewModel.Id);
            if (!resultCheckForFiles.IsSuccess && resultCheckForFiles.HandleError())
            {
                return ResultDTO.Fail(resultCheckForFiles.ErrMsg!);
            }
            if (resultCheckForFiles.Data == null)
            {
                return ResultDTO.Fail("Data is null");
            }

            var dto = _mapper.Map<RegisteredDumpsiteRiskLevelDTO>(registeredDumpsiteRiskLevelViewModel);

            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultDelete = await _registeredDumpsiteRiskLevelService.DeleteRegisteredDumpsiteRiskLevel(dto);
            if (!resultDelete.IsSuccess && resultDelete.HandleError())
            {
                return ResultDTO.Fail(resultDelete.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [HasAuthClaim(nameof(SD.AuthClaims.EditRegisteredDumpsiteRiskLevel))]
    public async Task<ResultDTO<RegisteredDumpsiteRiskLevelDTO>> GetRegisteredDumpsiteRiskLevelById(Guid registeredDumpsiteRiskLevelId)
    {
        try
        {
            ResultDTO<RegisteredDumpsiteRiskLevelDTO> resultGetEntity = await _registeredDumpsiteRiskLevelService.GetRegisteredDumpsiteRiskLevelById(registeredDumpsiteRiskLevelId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<RegisteredDumpsiteRiskLevelDTO>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<RegisteredDumpsiteRiskLevelDTO>.Fail("Risk Level is null");
            }
            var userId = User.FindFirstValue("UserId");
            resultGetEntity.Data.CreatedById = userId;

            return ResultDTO<RegisteredDumpsiteRiskLevelDTO>.Ok(resultGetEntity.Data);
        }
        catch (Exception ex)
        {
            return ResultDTO<RegisteredDumpsiteRiskLevelDTO>.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.EditRegisteredDumpsiteRiskLevel))]
    public async Task<ResultDTO> EditRegisteredDumpsiteRiskLevelConfirmed(RegisteredDumpsiteRiskLevelViewModel registeredDumpsiteRiskLevelViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var dto = _mapper.Map<RegisteredDumpsiteRiskLevelDTO>(registeredDumpsiteRiskLevelViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultEdit = await _registeredDumpsiteRiskLevelService.EditRegisteredDumpsiteRiskLevel(dto);
            if (!resultEdit.IsSuccess && resultEdit.HandleError())
            {
                return ResultDTO.Fail(resultEdit.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }
    #endregion

    private IActionResult HandleErrorRedirect(string configKey, int statusCode)
    {
        string? errorPath = _configuration[configKey];
        if (string.IsNullOrEmpty(errorPath))
        {
            return statusCode switch
            {
                404 => NotFound(),
                403 => Forbid(),
                405 => StatusCode(405),
                _ => BadRequest()
            };
        }
        return Redirect(errorPath);
    }

    #region File Management
    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    [RequestSizeLimit(int.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<ResultDTO<RegisteredDumpsiteFileDTO>> UploadDumpsiteFile(Guid dumpsiteId, IFormFile file, string? description)
    {
        try
        {
            if (file == null || file.Length == 0)
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail("No file provided");

            if (dumpsiteId == Guid.Empty)
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail("Invalid dumpsite id");

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail("User not found");

            // Get upload folder path from app settings
            var uploadFolderResult = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>(
                "RegisteredDumpsiteFilesFolder",
                "Uploads\\RegisteredDumpsites\\Files"
            );

            if (!uploadFolderResult.IsSuccess && uploadFolderResult.HandleError())
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail("Cannot get the application settings");

            if (string.IsNullOrEmpty(uploadFolderResult.Data))
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail("Upload folder path not found");

            // Resolve physical path
            string saveDirectory = Path.Combine(_webHostEnvironment.WebRootPath, uploadFolderResult.Data);
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            // Save file
            string fileExtension = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(saveDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create entity
            var dumpsiteFile = new RegisteredDumpsiteFileDTO
            {
                RegisteredDumpsiteId = dumpsiteId,
                FileName = fileName,
                OriginalFileName = file.FileName,
                FilePath = Path.Combine(uploadFolderResult.Data, fileName),
                ContentType = file.ContentType,
                FileExtension = fileExtension,
                Description = description,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            };

            // Save in DB
            var createResult = await _registeredDumpsiteService.UploadFile(dumpsiteFile);
            if (!createResult.IsSuccess && createResult.HandleError())
                return ResultDTO<RegisteredDumpsiteFileDTO>.Fail(createResult.ErrMsg!);

            var dto = _mapper.Map<RegisteredDumpsiteFileDTO>(dumpsiteFile);
            return ResultDTO<RegisteredDumpsiteFileDTO>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while uploading file");
            return ResultDTO<RegisteredDumpsiteFileDTO>.ExceptionFail(ex.Message, ex);
        }
    }


    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO<List<RegisteredDumpsiteFileDTO>>> GetDumpsiteFiles(Guid dumpsiteId)
    {
        try
        {
            if (dumpsiteId == Guid.Empty)
                return ResultDTO<List<RegisteredDumpsiteFileDTO>>.Fail("Invalid dumpsite id");

            var result = await _registeredDumpsiteService.GetFilesByDumpsiteId(dumpsiteId);
            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<List<RegisteredDumpsiteFileDTO>>.Fail(result.ErrMsg!);

            return ResultDTO<List<RegisteredDumpsiteFileDTO>>.Ok(result.Data ?? new List<RegisteredDumpsiteFileDTO>());
        }
        catch (Exception ex)
        {
            return ResultDTO<List<RegisteredDumpsiteFileDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<IActionResult> DownloadDumpsiteFile(Guid fileId)
    {
        try
        {
            var fileInfo = await _registeredDumpsiteService.GetSingleFileById(fileId);
            if (!fileInfo.IsSuccess || fileInfo.Data == null)
                return NotFound("File information not found");

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, fileInfo.Data.FilePath);
            if (!System.IO.File.Exists(fullPath))
                return NotFound("Physical file not found");

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            return File(fileBytes, fileInfo.Data.ContentType, fileInfo.Data.OriginalFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading dumpsite file");
            return BadRequest(ex.Message);
        }
    }


    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO> DeleteDumpsiteFile(Guid fileId)
    {
        try
        {
            var fileInfo = await _registeredDumpsiteService.GetSingleFileById(fileId);
            if (!fileInfo.IsSuccess || fileInfo.Data == null)
                return ResultDTO.Fail("File not found");

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, fileInfo.Data.FilePath);
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            return await _registeredDumpsiteService.DeleteFile(fileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting dumpsite file");
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }
    #endregion

    #region Inspection Management
    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO<List<RegisteredDumpsiteInspectionDTO>>> GetDumpsiteInspections(Guid dumpsiteId)
    {
        try
        {
            if (dumpsiteId == Guid.Empty)
                return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Fail("Invalid dumpsite id");

            var result = await _registeredDumpsiteService.GetInspectionsByDumpsiteId(dumpsiteId);
            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Fail(result.ErrMsg!);

            return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.Ok(result.Data ?? new List<RegisteredDumpsiteInspectionDTO>());
        }
        catch (Exception ex)
        {
            return ResultDTO<List<RegisteredDumpsiteInspectionDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<IActionResult> CreateInspection([FromBody] RegisteredDumpsiteInspectionDTO dto)
    {
        try
        {
            if (dto == null)
                return Json(new { isSuccess = false, errMsg = "Invalid inspection data" });

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { isSuccess = false, errMsg = "User not found" });
            dto.CreatedById = userId;

            var result = await _registeredDumpsiteService.CreateInspection(dto);

            if (result.IsSuccess)
            {
                return Json(new { isSuccess = true, message = "Inspection scheduled successfully" });
            }

            return Json(new { isSuccess = false, errMsg = result.ErrMsg });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inspection");
            return Json(new { isSuccess = false, errMsg = "An error occurred while scheduling the inspection" });
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO> UpdateInspection([FromBody] RegisteredDumpsiteInspectionDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var result = await _registeredDumpsiteService.UpdateInspection(dto);
            if (!result.IsSuccess && result.HandleError())
                return ResultDTO.Fail(result.ErrMsg!);

            return ResultDTO.Ok();
        }
        catch (Exception ex)
        {
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<IActionResult> AssignInspector([FromBody] AssignInspectorRequest request)
    {
        try
        {
            if (request == null || request.InspectionId == Guid.Empty || string.IsNullOrEmpty(request.InspectorId))
                return Json(new { isSuccess = false, errMsg = "Invalid request data" });

            var result = await _registeredDumpsiteService.AssignInspector(request.InspectionId, request.InspectorId);

            if (result.IsSuccess)
            {
                return Json(new { isSuccess = true, message = "Inspector assigned successfully" });
            }

            return Json(new { isSuccess = false, errMsg = result.ErrMsg });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning inspector");
            return Json(new { isSuccess = false, errMsg = "An error occurred while assigning the inspector" });
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<IActionResult> CompleteInspection([FromBody] CompleteInspectionRequest request)
    {
        try
        {
            if (request == null || request.InspectionId == Guid.Empty || string.IsNullOrEmpty(request.Findings))
                return Json(new { isSuccess = false, errMsg = "Invalid request data" });

            var result = await _registeredDumpsiteService.CompleteInspection(
                request.InspectionId,
                request.Findings,
                request.Recommendations,
                request.Notes);

            if (result.IsSuccess)
            {
                return Json(new { isSuccess = true, message = "Inspection completed successfully" });
            }

            return Json(new { isSuccess = false, errMsg = result.ErrMsg });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing inspection");
            return Json(new { isSuccess = false, errMsg = "An error occurred while completing the inspection" });
        }
    }

    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO<RegisteredDumpsiteInspectionDTO?>> GetInspectionById(Guid inspectionId)
    {
        try
        {
            if (inspectionId == Guid.Empty)
                return ResultDTO<RegisteredDumpsiteInspectionDTO?>.Fail("Invalid inspection id");

            var result = await _registeredDumpsiteService.GetInspectionById(inspectionId);
            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<RegisteredDumpsiteInspectionDTO?>.Fail(result.ErrMsg!);

            return ResultDTO<RegisteredDumpsiteInspectionDTO?>.Ok(result.Data);
        }
        catch (Exception ex)
        {
            return ResultDTO<RegisteredDumpsiteInspectionDTO?>.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<IActionResult> GetDumpsiteDetailsPartial(Guid dumpsiteId)
    {
        try
        {
            var dumpsiteResult = await _registeredDumpsiteService.GetRegisteredDumpsiteById(dumpsiteId);
            if (!dumpsiteResult.IsSuccess || dumpsiteResult.Data == null)
                return PartialView("_DumpsiteDetailsPartial", null);

            var filesResult = await _registeredDumpsiteService.GetFilesByDumpsiteId(dumpsiteId);
            var inspectionsResult = await _registeredDumpsiteService.GetInspectionsByDumpsiteId(dumpsiteId);

            var userId = User.FindFirstValue("UserId");

            var viewModel = new DumpsiteDetailsViewModel
            {
                Dumpsite = dumpsiteResult.Data,
                Files = filesResult.Data ?? new List<RegisteredDumpsiteFileDTO>(),
                Inspections = inspectionsResult.Data ?? new List<RegisteredDumpsiteInspectionDTO>(),
                CurrentUserId = userId
            };

            // Load inspection files for each inspection
            if (viewModel.Inspections.Any())
            {
                foreach (var inspection in viewModel.Inspections)
                {
                    if (inspection.Id.HasValue)
                    {
                        var inspectionFilesResult = await _registeredDumpsiteService.GetInspectionFilesByInspectionId(inspection.Id.Value);
                        if (inspectionFilesResult.IsSuccess && inspectionFilesResult.Data != null)
                        {
                            viewModel.InspectionFiles[inspection.Id.Value] = inspectionFilesResult.Data;
                            inspection.InspectionFiles = inspectionFilesResult.Data;
                        }
                    }
                }
            }

            return PartialView("_DumpsiteDetailsPartial", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dumpsite details");
            return PartialView("_DumpsiteDetailsPartial", null);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableInspectors()
    {
        try
        {
            var resultGetUsers = await _userManagementService.GetAllIntanetPortalUsers();

            if (!resultGetUsers.IsSuccess || resultGetUsers.Data == null)
            {
                return Json(new { isSuccess = false, errMsg = "Failed to load inspectors" });
            }

            var inspectors = resultGetUsers.Data
                .Select(u => new
                {
                    id = u.Id,
                    userName = u.UserName,
                    fullName = $"{u.FirstName} {u.LastName}".Trim(),
                    email = u.Email
                })
                .OrderBy(u => u.fullName)
                .ToList();

            return Json(new { isSuccess = true, data = inspectors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading inspectors");
            return Json(new { isSuccess = false, errMsg = "An error occurred while loading inspectors" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetInspectionDetails(Guid inspectionId)
    {
        try
        {
            var result = await _registeredDumpsiteService.GetInspectionById(inspectionId);

            if (result.IsSuccess)
            {
                return Json(new { isSuccess = true, data = result.Data });
            }

            return Json(new { isSuccess = false, errMsg = result.ErrMsg });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inspection details");
            return Json(new { isSuccess = false, errMsg = "An error occurred while loading inspection details" });
        }
    }
    #endregion


    #region Inspection File Management

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    [RequestSizeLimit(int.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<ResultDTO<RegisteredDumpsiteInspectionFileDTO>> UploadInspectionFile(Guid inspectionId, IFormFile file, string? description)
    {
        try
        {
            if (file == null || file.Length == 0)
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail("No file provided");

            if (inspectionId == Guid.Empty)
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail("Invalid inspection id");

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail("User not found");

            // Check if user is assigned to this inspection
            var assignmentCheck = await _registeredDumpsiteService.IsUserAssignedToInspection(inspectionId, userId);
            if (!assignmentCheck.IsSuccess || !assignmentCheck.Data)
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail("You are not assigned to this inspection");

            // Get upload folder path from app settings
            var uploadFolderResult = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>(
                "RegisteredDumpsiteInspectionFilesFolder",
                "Uploads\\RegisteredDumpsites\\InspectionFiles"
            );

            if (!uploadFolderResult.IsSuccess && uploadFolderResult.HandleError())
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail("Cannot get the application settings");

            if (string.IsNullOrEmpty(uploadFolderResult.Data))
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail("Upload folder path not found");

            // Resolve physical path
            string saveDirectory = Path.Combine(_webHostEnvironment.WebRootPath, uploadFolderResult.Data);
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            // Save file
            string fileExtension = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(saveDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create entity
            var inspectionFile = new RegisteredDumpsiteInspectionFileDTO
            {
                RegisteredDumpsiteInspectionId = inspectionId,
                FileName = fileName,
                OriginalFileName = file.FileName,
                FilePath = Path.Combine(uploadFolderResult.Data, fileName),
                ContentType = file.ContentType,
                FileExtension = fileExtension,
                Description = description,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            };

            // Save in DB
            var createResult = await _registeredDumpsiteService.UploadInspectionFile(inspectionFile);
            if (!createResult.IsSuccess && createResult.HandleError())
                return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Fail(createResult.ErrMsg!);

            var dto = _mapper.Map<RegisteredDumpsiteInspectionFileDTO>(inspectionFile);
            return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while uploading inspection file");
            return ResultDTO<RegisteredDumpsiteInspectionFileDTO>.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>> GetInspectionFiles(Guid inspectionId)
    {
        try
        {
            if (inspectionId == Guid.Empty)
                return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Fail("Invalid inspection id");

            var result = await _registeredDumpsiteService.GetInspectionFilesByInspectionId(inspectionId);
            if (!result.IsSuccess && result.HandleError())
                return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Fail(result.ErrMsg!);

            return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.Ok(result.Data ?? new List<RegisteredDumpsiteInspectionFileDTO>());
        }
        catch (Exception ex)
        {
            return ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<IActionResult> DownloadInspectionFile(Guid fileId)
    {
        try
        {
            var fileInfo = await _registeredDumpsiteService.GetSingleInspectionFileById(fileId);
            if (!fileInfo.IsSuccess || fileInfo.Data == null)
                return NotFound("File information not found");

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, fileInfo.Data.FilePath);
            if (!System.IO.File.Exists(fullPath))
                return NotFound("Physical file not found");

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            return File(fileBytes, fileInfo.Data.ContentType, fileInfo.Data.OriginalFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading inspection file");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO> DeleteInspectionFile(Guid fileId)
    {
        try
        {
            var fileInfo = await _registeredDumpsiteService.GetSingleInspectionFileById(fileId);
            if (!fileInfo.IsSuccess || fileInfo.Data == null)
                return ResultDTO.Fail("File not found");

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return ResultDTO.Fail("User not found");

            // Check if user is assigned to this inspection or is the file creator
            var assignmentCheck = await _registeredDumpsiteService.IsUserAssignedToInspection(fileInfo.Data.RegisteredDumpsiteInspectionId, userId);
            if (!assignmentCheck.IsSuccess || (!assignmentCheck.Data && fileInfo.Data.CreatedById != userId))
                return ResultDTO.Fail("You don't have permission to delete this file");

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, fileInfo.Data.FilePath);
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            return await _registeredDumpsiteService.DeleteInspectionFile(fileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inspection file");
            return ResultDTO.ExceptionFail(ex.Message, ex);
        }
    }

    [HttpGet]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    public async Task<ResultDTO<bool>> CheckInspectionAssignment(Guid inspectionId)
    {
        try
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return ResultDTO<bool>.Fail("User not found");

            return await _registeredDumpsiteService.IsUserAssignedToInspection(inspectionId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking inspection assignment");
            return ResultDTO<bool>.ExceptionFail(ex.Message, ex);
        }
    }

    #endregion

    // last

    [HttpPost]
    public async Task<IActionResult> ConvertDetectedDumpsite([FromBody] ConvertDetectedDumpsiteRequest request)
    {
        try
        {
            var result = await _registeredDumpsiteService.ConvertDetectedDumpsiteAsync(request);

            if (result.IsSuccess)
            {
                return Ok(new { isSuccess = true, data = result.Data });
            }

            return Ok(new { isSuccess = false, errMsg = result.ErrMsg });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting detected dumpsite");
            return Ok(new { isSuccess = false, errMsg = "An error occurred while converting the dumpsite" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDetectedDumpsiteDetails(Guid detectedDumpsiteId)
    {
        try
        {
            var result = await _detectionRunService.GetDetectionRunById(detectedDumpsiteId);

            if (result.IsSuccess)
            {
                return Ok(new { isSuccess = true, data = result.Data });
            }

            return Ok(new { isSuccess = false, errMsg = result.ErrMsg });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving detected dumpsite details");
            return Ok(new { isSuccess = false, errMsg = "An error occurred while retrieving dumpsite details" });
        }
    }

}
