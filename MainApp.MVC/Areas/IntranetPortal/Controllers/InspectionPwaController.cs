using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;
using MainApp.MVC.Filters;
using MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SD;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers;

[Area("IntranetPortal")]
public class InspectionPwaController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IRegisteredDumpsiteService _registeredDumpsiteService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<InspectionPwaController> _logger;
    private readonly IMapper _mapper;
    private readonly IAppSettingsAccessor _appSettingsAccessor;


    public InspectionPwaController(
        IConfiguration configuration,
        IRegisteredDumpsiteService registeredDumpsiteService,
        IWebHostEnvironment webHostEnvironment,
        ILogger<InspectionPwaController> logger,
        IMapper mapper,
        IAppSettingsAccessor appSettingsAccessor)
    {
        _configuration = configuration;
        _registeredDumpsiteService = registeredDumpsiteService;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
        _mapper = mapper;
        _appSettingsAccessor = appSettingsAccessor;
    }

    public IActionResult Index()
    {
        var domain = _configuration.GetValue<string>("DomainSettings:MainDomain");
        var appPath = _configuration.GetValue<string>("DomainSettings:PwaAppPath", "");
        var title = _configuration.GetValue<string>("AppInstanceSettings:Title", "");
        var area = _configuration.GetValue<string>("ApplicationStartupMode", "");
        appPath = appPath != "" ? "/" + appPath : "";
        area = area != "" ? "/" + area : "";
        ViewData["AppPath"] = appPath;
        ViewData["FullAppUrl"] = "https://" + domain + area + appPath;
        ViewData["Title"] = title;
        ViewData["Area"] = area;
        ViewData["Domain"] = domain;
        ViewData["InstanceName"] = _configuration.GetValue<string>("AppInstanceSettings:InstanceName", "");
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ManifestJson()
    {
        var domain = _configuration.GetValue<string>("DomainSettings:MainDomain");
        var appPath = _configuration.GetValue<string>("DomainSettings:PwaAppPath", "");
        var manifestFile = _configuration.GetValue<string>("AppInstanceSettings:InstanceName", "");
        var area = _configuration.GetValue<string>("ApplicationStartupMode", "");
        var mainAppPath = _configuration.GetValue<string>("DomainSettings:MainAppPath", ""); // On server
        appPath = appPath != "" ? "/" + appPath : "";
        area = area != "" ? "/" + area : "";
        mainAppPath = mainAppPath != "" ? "/" + mainAppPath : "";
        ViewData["AppPath"] = "/InspectionPwa";
        ViewData["FullAppUrl"] = "https://" + domain + mainAppPath + area + appPath;
        ViewData["InstanceName"] = manifestFile;
        ViewData["Area"] = area;
        ViewData["Domain"] = domain;
        HttpContext.Response.Headers["Content-Type"] = "application/json";
        return View($"Manifest/{manifestFile}");
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("/IntranetPortal/InspectionPwa/ServiceWorker")]
    public IActionResult ServiceWorker()
    {
        var domain = _configuration.GetValue<string>("DomainSettings:MainDomain");
        var appPath = _configuration.GetValue<string>("DomainSettings:PwaAppPath", "");
        var area = _configuration.GetValue<string>("ApplicationStartupMode", "");
        var mainAppPath = _configuration.GetValue<string>("DomainSettings:MainAppPath", "");
        appPath = !string.IsNullOrEmpty(appPath) ? "/" + appPath : "";
        area = !string.IsNullOrEmpty(area) ? "/" + area : "";
        mainAppPath = !string.IsNullOrEmpty(mainAppPath) ? "/" + mainAppPath : "";
        ViewData["AppPath"] = appPath;
        ViewData["FullAppUrl"] = "https://" + domain + mainAppPath + area + appPath;
        ViewData["Area"] = area;
        ViewData["Domain"] = domain;

        HttpContext.Response.Headers["Content-Type"] = "application/javascript";
        HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        HttpContext.Response.Headers["Pragma"] = "no-cache";
        HttpContext.Response.Headers["Expires"] = "0";
        HttpContext.Response.Headers["Service-Worker-Allowed"] = "/IntranetPortal/InspectionPwa/";
        return View();
    }

    #region Inspection File Management for PWA

    [HttpGet]
    public async Task<IActionResult> GetMyAssignedInspections()
    {
        try
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { isSuccess = false, errMsg = "User not found" });

            var result = await _registeredDumpsiteService.GetMyAssignedInspections(userId);

            if (result.IsSuccess)
            {
                // Transform the data to include file counts and other useful info
                var inspectionsWithCounts = result.Data?.Select(inspection => new
                {
                    id = inspection.Id,
                    registeredDumpsiteInspectionId = inspection.Id,
                    registeredDumpsiteId = inspection.RegisteredDumpsiteId,
                    status = inspection.Status,
                    description = inspection.RegisteredDumpsite.Description ?? "Inspection",
                    inspectionType = inspection.Status,
                    //priority = inspection.Status,
                    //priorityText = GetPriorityText(inspection.Priority),
                    scheduledDate = inspection.ScheduledDate?.ToString("yyyy-MM-dd"),
                    completedOn = inspection.InspectionFiles.Select(assignment => assignment.CreatedOn)/*?.ToString("yyyy-MM-dd")*/,
                    findings = inspection.Findings,
                    recommendations = inspection.Recommendations,
                    createdOn = inspection.CreatedOn,
                    filesCount = inspection.InspectionFiles?.Count ?? 0,
                    dumpsiteName = inspection.RegisteredDumpsite?.Name ?? "Unknown Location",
                    assignedBy = inspection.CreatedBy?.FirstName ?? "System"
                });

                return Json(new { isSuccess = true, data = inspectionsWithCounts });
            }

            return Json(new { isSuccess = false, errMsg = result.ErrMsg });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assigned inspections for user");
            return Json(new { isSuccess = false, errMsg = "An error occurred while loading your inspections" });
        }
    }

    [HttpPost]
    [HasAuthClaim(nameof(SD.AuthClaims.MapToolRegisterDumpsites))]
    [RequestSizeLimit(int.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> UploadInspectionFile(Guid inspectionId, IFormFile file, string? description)
    {
        try
        {
            if (file == null || file.Length == 0)
                return Json(new { isSuccess = false, errMsg = "No file provided" });

            if (inspectionId == Guid.Empty)
                return Json(new { isSuccess = false, errMsg = "Invalid inspection id" });

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { isSuccess = false, errMsg = "User not found" });

            // Check if user is assigned to this inspection
            var assignmentCheck = await _registeredDumpsiteService.IsUserAssignedToInspection(inspectionId, userId);
            if (!assignmentCheck.IsSuccess || !assignmentCheck.Data)
                return Json(new { isSuccess = false, errMsg = "You are not assigned to this inspection" });

            // Get upload folder path from app settings
            var uploadFolderResult = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>(
                "RegisteredDumpsiteInspectionFilesFolder",
                "Uploads\\RegisteredDumpsites\\InspectionFiles"
            );

            if (!uploadFolderResult.IsSuccess && uploadFolderResult.HandleError())
                return Json(new { isSuccess = false, errMsg = "Cannot get the application settings" });

            if (string.IsNullOrEmpty(uploadFolderResult.Data))
                return Json(new { isSuccess = false, errMsg = "Upload folder path not found" });

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
                return Json(new { isSuccess = false, errMsg = createResult.ErrMsg });

            var dto = _mapper.Map<RegisteredDumpsiteInspectionFileDTO>(inspectionFile);
            return Json(new { isSuccess = true, message = "File uploaded successfully", data = dto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while uploading inspection file");
            return Json(new { isSuccess = false, errMsg = "Error while uploading inspection file" });
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetInspectionFiles(Guid inspectionId)
    {
        try
        {
            if (inspectionId == Guid.Empty)
                return Json(new { isSuccess = false, errMsg = "Invalid inspection id" });

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { isSuccess = false, errMsg = "User not found" });

            // Check if user is assigned to this inspection
            var assignmentCheck = await _registeredDumpsiteService.IsUserAssignedToInspection(inspectionId, userId);
            if (!assignmentCheck.IsSuccess || !assignmentCheck.Data)
                return Json(new { isSuccess = false, errMsg = "You are not assigned to this inspection" });

            var result = await _registeredDumpsiteService.GetInspectionFilesByInspectionId(inspectionId);

            if (result.IsSuccess)
            {
                return Json(new { isSuccess = true, data = result.Data });
            }

            return Json(new { isSuccess = false, errMsg = result.ErrMsg });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inspection files");
            return Json(new { isSuccess = false, errMsg = "Error loading files" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> DownloadInspectionFile(Guid fileId)
    {
        try
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User not found");

            var fileInfo = await _registeredDumpsiteService.GetSingleInspectionFileById(fileId);
            if (!fileInfo.IsSuccess || fileInfo.Data == null)
                return NotFound("File not found");

            // Check if user is assigned to the inspection
            var assignmentCheck = await _registeredDumpsiteService.IsUserAssignedToInspection(
                fileInfo.Data.RegisteredDumpsiteInspectionId, userId);

            if (!assignmentCheck.IsSuccess || !assignmentCheck.Data)
                return Forbid("You don't have access to this file");

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, fileInfo.Data.FilePath);
            if (!System.IO.File.Exists(fullPath))
                return NotFound("Physical file not found");

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(fileBytes, fileInfo.Data.ContentType, fileInfo.Data.OriginalFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading inspection file");
            return BadRequest("Error downloading file");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteInspectionFile(Guid fileId)
    {
        try
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { isSuccess = false, errMsg = "User not found" });

            var fileInfo = await _registeredDumpsiteService.GetSingleInspectionFileById(fileId);
            if (!fileInfo.IsSuccess || fileInfo.Data == null)
                return Json(new { isSuccess = false, errMsg = "File not found" });

            // Check if user is assigned to inspection or is the file creator
            var assignmentCheck = await _registeredDumpsiteService.IsUserAssignedToInspection(
                fileInfo.Data.RegisteredDumpsiteInspectionId, userId);

            if (!assignmentCheck.IsSuccess || (!assignmentCheck.Data && fileInfo.Data.CreatedById != userId))
                return Json(new { isSuccess = false, errMsg = "You don't have permission to delete this file" });

            var result = await _registeredDumpsiteService.DeleteInspectionFile(fileId);

            if (result.IsSuccess)
            {
                // Also delete physical file
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, fileInfo.Data.FilePath);
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                return Json(new { isSuccess = true, message = "File deleted successfully" });
            }

            return Json(new { isSuccess = false, errMsg = result.ErrMsg });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inspection file");
            return Json(new { isSuccess = false, errMsg = "Error deleting file" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CompleteInspection([FromBody] CompleteInspectionViewModel viewModel)
    {
        try
        {
            if (viewModel == null || viewModel.InspectionId == Guid.Empty || string.IsNullOrEmpty(viewModel.Findings))
                return Json(new { isSuccess = false, errMsg = "Invalid viewModel data" });

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { isSuccess = false, errMsg = "User not found" });

            // Check if user is assigned to this inspection
            var assignmentCheck = await _registeredDumpsiteService.IsUserAssignedToInspection(viewModel.InspectionId, userId);
            if (!assignmentCheck.IsSuccess || !assignmentCheck.Data)
                return Json(new { isSuccess = false, errMsg = "You are not assigned to this inspection" });

            var inspectionDTO = _mapper.Map<CompleteInspectionDTO>(viewModel);

            var result = await _registeredDumpsiteService.CompleteInspection(inspectionDTO);

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
    public async Task<IActionResult> GetInspectionDetails(Guid inspectionId)
    {
        try
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { isSuccess = false, errMsg = "User not found" });

            // Check if user is assigned to this inspection
            var assignmentCheck = await _registeredDumpsiteService.IsUserAssignedToInspection(inspectionId, userId);
            if (!assignmentCheck.IsSuccess || !assignmentCheck.Data)
                return Json(new { isSuccess = false, errMsg = "You are not assigned to this inspection" });

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
            return Json(new { isSuccess = false, errMsg = "Error loading inspection details" });
        }
    }

    #endregion
}
