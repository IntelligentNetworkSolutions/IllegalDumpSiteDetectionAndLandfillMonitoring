using AutoMapper;
using CliWrap;
using CliWrap.Buffered;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Humanizer;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SD;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class LegalLandfillsManagementController : Controller
    {
        private readonly ILegalLandfillService _legalLandfillService;
        private readonly ILegalLandfillPointCloudFileService _legalLandfillPointCloudFileService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LegalLandfillsManagementController(ILegalLandfillService legalLandfillService,
                                                  ILegalLandfillPointCloudFileService legalLandfillPointCloudFileService,
                                                  IConfiguration configuration,
                                                  IMapper mapper,
                                                  IAppSettingsAccessor appSettingsAccessor,
                                                  IWebHostEnvironment webHostEnvironment)
        {
            _legalLandfillService = legalLandfillService;
            _legalLandfillPointCloudFileService = legalLandfillPointCloudFileService;
            _configuration = configuration;
            _mapper = mapper;
            _appSettingsAccessor = appSettingsAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewLegalLandfills))]
        public async Task<IActionResult> ViewLegalLandfills()
        {
            var resultDtoList = await _legalLandfillService.GetAllLegalLandfills();
            if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (errorPath == null)
                {
                    return BadRequest();
                }
                return Redirect(errorPath);
            }
            if (resultDtoList.Data == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (errorPath == null)
                {
                    return NotFound();
                }
                return Redirect(errorPath);
            }

            var vmList = _mapper.Map<List<LegalLandfillViewModel>>(resultDtoList.Data);
            if (vmList == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (errorPath == null)
                {
                    return NotFound();
                }
                return Redirect(errorPath);
            }

            return View(vmList);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddLegalLandfill))]
        public async Task<ResultDTO> CreateLegalLandfillConfirmed(LegalLandfillViewModel legalLandfillViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var dto = _mapper.Map<LegalLandfillDTO>(legalLandfillViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultCreate = await _legalLandfillService.CreateLegalLandfill(dto);
            if (!resultCreate.IsSuccess && resultCreate.HandleError())
            {
                return ResultDTO.Fail(resultCreate.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditLegalLandfill))]
        public async Task<ResultDTO> EditLegalLandfillConfirmed(LegalLandfillViewModel legalLandfillViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var dto = _mapper.Map<LegalLandfillDTO>(legalLandfillViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultEdit = await _legalLandfillService.EditLegalLandfill(dto);
            if (!resultEdit.IsSuccess && resultEdit.HandleError())
            {
                return ResultDTO.Fail(resultEdit.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteLegalLandfill))]
        public async Task<ResultDTO> DeleteLegalLandfillConfirmed(LegalLandfillViewModel legalLandfillViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var resultCheckForFiles = await _legalLandfillPointCloudFileService.GetLegalLandfillPointCloudFilesByLandfillId(legalLandfillViewModel.Id);
            if (!resultCheckForFiles.IsSuccess && resultCheckForFiles.HandleError())
            {
                return ResultDTO.Fail(resultCheckForFiles.ErrMsg!);
            }
            if(resultCheckForFiles.Data == null)
            {
                return ResultDTO.Fail("Data is null");
            }

            if (resultCheckForFiles.Data.Count > 0)
            {
                var error = DbResHtml.T("There are point cloud files for this landfill. Delete first the files!", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            var dto = _mapper.Map<LegalLandfillDTO>(legalLandfillViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultDelete = await _legalLandfillService.DeleteLegalLandfill(dto);
            if (!resultDelete.IsSuccess && resultDelete.HandleError())
            {
                return ResultDTO.Fail(resultDelete.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        public async Task<ResultDTO<LegalLandfillDTO>> GetLegalLandfillById(Guid legalLandfillId)
        {
            ResultDTO<LegalLandfillDTO> resultGetEntity = await _legalLandfillService.GetLegalLandfillById(legalLandfillId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<LegalLandfillDTO>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<LegalLandfillDTO>.Fail("Landfill is null");

            }
            return ResultDTO<LegalLandfillDTO>.Ok(resultGetEntity.Data);
        }

        [HttpPost]
        public async Task<ResultDTO<LegalLandfillPointCloudFileDTO>> GetLegalLandfillPointCloudFileById(Guid fileId)
        {
            ResultDTO<LegalLandfillPointCloudFileDTO> resultGetEntity = await _legalLandfillPointCloudFileService.GetLegalLandfillPointCloudFilesById(fileId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("File is null");

            }
            return ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(resultGetEntity.Data);
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewLegalLandfillPointCloudFiles))]
        public async Task<IActionResult> ViewPointCloudFiles()
        {
            var resultDtoList = await _legalLandfillPointCloudFileService.GetAllLegalLandfillPointCloudFiles();
            if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (errorPath == null)
                {
                    return BadRequest();
                }
                return Redirect(errorPath);
            }
            if (resultDtoList.Data == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (errorPath == null)
                {
                    return NotFound();
                }
                return Redirect(errorPath);
            }

            var vmList = _mapper.Map<List<LegalLandfillPointCloudFileViewModel>>(resultDtoList.Data);
            if (vmList == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (errorPath == null)
                {
                    return NotFound();
                }
                return Redirect(errorPath);
            }

            return View(vmList);
        }

        [HttpPost]
        public async Task<ResultDTO<List<LegalLandfillDTO>>> GetAllLegalLandfills()
        {
            ResultDTO<List<LegalLandfillDTO>> resultGetEntity = await _legalLandfillService.GetAllLegalLandfills();
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<List<LegalLandfillDTO>>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<List<LegalLandfillDTO>>.Fail("Object is null");

            }
            return ResultDTO<List<LegalLandfillDTO>>.Ok(resultGetEntity.Data);
        }

        [HttpPost]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        [HasAuthClaim(nameof(SD.AuthClaims.UploadConvertLegalLandfillPointCloudFile))]
        public async Task<ResultDTO> UploadConvertLegalLandfillPointCloudFileConfirmed(LegalLandfillPointCloudFileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }
            if (viewModel.FileUpload == null)
            {
                return ResultDTO.Fail("Please insert file");
            }

            //check file support
            var fileUploadExtension = Path.GetExtension(viewModel.FileUpload.FileName);
            ResultDTO resultCheckSuportingFiles = await _legalLandfillPointCloudFileService.CheckSupportingFiles(fileUploadExtension);
            if (!resultCheckSuportingFiles.IsSuccess && resultCheckSuportingFiles.HandleError())
            {
                return ResultDTO.Fail(resultCheckSuportingFiles.ErrMsg!);
            }

            ResultDTO<LegalLandfillDTO> resultGetEntity = await _legalLandfillService.GetLegalLandfillById(viewModel.LegalLandfillId);
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
            {
                return ResultDTO.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO.Fail("Object is null");
            }

            try
            {
                var pointCloudUploadFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileUploads", "Uploads\\LegalLandfillUploads\\PointCloudUploads");
                var pointCloudConvertFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts");
                var potreeConverterFilePath = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("PotreeConverterFilePath", "C:\\PotreeConverter\\PotreeConverter.exe");
                var pdalAbsPath = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("PdalExePath", string.Empty);
                var pipelineJsonTemplate = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("PdalPipelineTemplate", string.Empty);

                if((!pointCloudUploadFolder.IsSuccess && pointCloudUploadFolder.HandleError()) || 
                    (!pointCloudConvertFolder.IsSuccess && pointCloudConvertFolder.HandleError()) || 
                    (!potreeConverterFilePath.IsSuccess && potreeConverterFilePath.HandleError()) || 
                    (!pdalAbsPath.IsSuccess && pdalAbsPath.HandleError()) ||
                    (!pipelineJsonTemplate.IsSuccess && pipelineJsonTemplate.HandleError()))
                {
                    return ResultDTO.Fail("Can not get some of the application settings");
                }
                if (pointCloudUploadFolder.Data == null || pointCloudConvertFolder.Data == null || potreeConverterFilePath.Data == null || pdalAbsPath.Data == null || pipelineJsonTemplate.Data == null)
                {
                    return ResultDTO.Fail("Some of the paths are null");
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudUploadFolder.Data, resultGetEntity.Data.Id.ToString());
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                LegalLandfillPointCloudFileDTO dto = new()
                {
                    Id = Guid.NewGuid(),
                    FileName = $"{viewModel.FileName}{fileUploadExtension}",
                    LegalLandfillId = resultGetEntity.Data.Id,
                    FilePath = string.Format("{0}\\{1}\\", pointCloudUploadFolder.Data, resultGetEntity.Data.Id.ToString()),
                    ScanDateTime = viewModel.ScanDateTime
                };

                //Create the file in database
                var resultCreateEntity = await _legalLandfillPointCloudFileService.CreateLegalLandfillPointCloudFile(dto);
                if (!resultCreateEntity.IsSuccess && resultCreateEntity.HandleError())
                {
                    return ResultDTO.Fail(resultCreateEntity.ErrMsg!);
                }
                if (resultCreateEntity.Data == null)
                {
                    return ResultDTO.Fail("Created file is null");
                }

                //Upload the file in folder                
                var fileName = string.Format("{0}{1}", resultCreateEntity.Data.Id.ToString(), fileUploadExtension);
                var filePath = Path.Combine(uploadsFolder, fileName);
                var uploadResult = await _legalLandfillPointCloudFileService.UploadFile(viewModel.FileUpload, uploadsFolder, resultCreateEntity.Data.Id.ToString() + fileUploadExtension);
                if (!uploadResult.IsSuccess && uploadResult.HandleError())
                {
                    return ResultDTO.Fail("File upload failed");
                }
                if (uploadResult.Data == null)
                {
                    return ResultDTO.Fail("Uploaded file path is null");
                }

                //Convert file to potree format
                var convertsFolder = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudConvertFolder.Data, resultGetEntity.Data.Id.ToString(), resultCreateEntity.Data.Id.ToString());
                var convertResult = await _legalLandfillPointCloudFileService.ConvertToPointCloud(potreeConverterFilePath.Data, uploadResult.Data, convertsFolder, filePath);
                if (!convertResult.IsSuccess && convertResult.HandleError())
                {
                    return ResultDTO.Fail("Converting to point cloud failed");
                }

                //create tif file                
                var tifFileName = string.Format("{0}{1}", resultCreateEntity.Data.Id.ToString(), "_dsm.tif");
                var tifFilePath = Path.Combine(uploadsFolder, tifFileName);
                var tiffResult = await _legalLandfillPointCloudFileService.CreateTifFile(pipelineJsonTemplate.Data, pdalAbsPath.Data, filePath, tifFilePath);
                if (!tiffResult.IsSuccess)
                {
                    return ResultDTO.Fail("Creating TIF file failed");
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }

        }               
       
        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewLegalLandfillPointCloudFiles))]
        public ResultDTO<string> ProcessSelectedFiles([FromBody] List<Guid> selectedFiles)
        {
            if (selectedFiles == null)
            {
                return ResultDTO<string>.Fail("No files selected");
            }

            try
            {
                List<string> encryptedGuids = new List<string>();
                foreach (var guid in selectedFiles)
                {
                    string encryptedGuid = GuidEncryptionHelper.EncryptGuid(guid);
                    encryptedGuids.Add(encryptedGuid);
                }

                var redirectUrl = Url.Action("Preview", "LegalLandfillsManagement", new { Area = "IntranetPortal", selectedFiles = encryptedGuids });
                if (redirectUrl == null)
                {
                    return ResultDTO<string>.Fail("Redirect url is null");
                }
                return ResultDTO<string>.Ok(redirectUrl);
            }
            catch (Exception ex)
            {
                return ResultDTO<string>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewLegalLandfillPointCloudFiles))]
        public async Task<IActionResult> Preview(List<string> selectedFiles)
        {
            if (selectedFiles == null || selectedFiles.Count == 0)
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
                if(errorPath == null)
                {
                    return BadRequest();
                }
                return Redirect(errorPath);
            }

            try
            {
                List<Guid> depryptedGuids = new List<Guid>();
                foreach (var item in selectedFiles)
                {
                    Guid decryptedGuid = GuidEncryptionHelper.DecryptGuid(item);
                    depryptedGuids.Add(decryptedGuid);
                }

                List<PreviewViewModel> listVM = new();
                foreach (var item in depryptedGuids)
                {
                    var resultGetEntity = await _legalLandfillPointCloudFileService.GetLegalLandfillPointCloudFilesById(item);
                    if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                    {
                        var errorPath = _configuration["ErrorViewsPath:Error404"];
                        if (errorPath == null)
                        {
                            return NotFound();
                        }
                        return Redirect(errorPath);
                    }
                    if (resultGetEntity.Data == null)
                    {
                        var errorPath = _configuration["ErrorViewsPath:Error404"];
                        if (errorPath == null)
                        {
                            return NotFound();
                        }
                        return Redirect(errorPath);
                    }

                    string? fileNameWithoutExtension = Path.GetFileNameWithoutExtension(resultGetEntity.Data.FileName);

                    PreviewViewModel vm = new()
                    {
                        FileId = resultGetEntity.Data.Id,
                        LandfillId = resultGetEntity.Data.LegalLandfillId,
                        FileName = fileNameWithoutExtension ?? resultGetEntity.Data.FileName
                    };

                    listVM.Add(vm);
                }

                return View(listVM);
            }
            catch (Exception)
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (errorPath == null)
                {
                    return BadRequest();
                }
                return Redirect(errorPath);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteLegalLandfillPointCloudFile))]
        public async Task<ResultDTO> DeleteLegalLandfillPointCloudFileConfirmed(LegalLandfillPointCloudFileViewModel legalLandfillPointCloudFileViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var resultGetEntity = await _legalLandfillPointCloudFileService.GetLegalLandfillPointCloudFilesById(legalLandfillPointCloudFileViewModel.Id);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                var error = DbResHtml.T("Object not found", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            //delete from uploads
            ResultDTO resultDeletingFilesFromUploads = await _legalLandfillPointCloudFileService.DeleteFilesFromUploads(resultGetEntity.Data, _webHostEnvironment.WebRootPath);
            if (!resultDeletingFilesFromUploads.IsSuccess && resultDeletingFilesFromUploads.HandleError())
            {
                return ResultDTO.Fail(resultDeletingFilesFromUploads.ErrMsg!);
            }
            //delete from converts
            ResultDTO resultDeletingFilesFromConverts = await _legalLandfillPointCloudFileService.DeleteFilesFromConverts(resultGetEntity.Data, _webHostEnvironment.WebRootPath);
            if (!resultDeletingFilesFromConverts.IsSuccess && resultDeletingFilesFromConverts.HandleError())
            {
                return ResultDTO.Fail(resultDeletingFilesFromConverts.ErrMsg!);
            }
            //delete from db
            ResultDTO resultDelete = await _legalLandfillPointCloudFileService.DeleteLegalLandfillPointCloudFile(legalLandfillPointCloudFileViewModel.Id);
            if (!resultDelete.IsSuccess && resultDelete.HandleError())
            {
                return ResultDTO.Fail(resultDelete.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditLegalLandfillPointCloudFile))]
        public async Task<ResultDTO> EditLegalLandfillPointCloudFileConfirmed(LegalLandfillPointCloudFileViewModel legalLandfillPointCloudFileViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            if (legalLandfillPointCloudFileViewModel.FilePath == null)
            {
                var error = DbResHtml.T("File path is null", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            var dto = _mapper.Map<LegalLandfillPointCloudFileDTO>(legalLandfillPointCloudFileViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            //edit in db
            ResultDTO<LegalLandfillPointCloudFileDTO> resultEdit = await _legalLandfillPointCloudFileService.EditLegalLandfillPointCloudFile(dto);
            if (!resultEdit.IsSuccess && resultEdit.HandleError())
            {
                return ResultDTO.Fail(resultEdit.ErrMsg!);
            }
            if (resultEdit.Data == null)
            {
                var error = DbResHtml.T("No result data", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            if (legalLandfillPointCloudFileViewModel.OldLegalLandfillId != resultEdit.Data.LegalLandfillId)
            {
                //edit upload folder
                ResultDTO resultEditFileInUploads = await _legalLandfillPointCloudFileService.EditFileInUploads(_webHostEnvironment.WebRootPath, legalLandfillPointCloudFileViewModel.FilePath, resultEdit.Data);
                if (!resultEditFileInUploads.IsSuccess && resultEditFileInUploads.HandleError())
                {
                    return ResultDTO.Fail(resultEditFileInUploads.ErrMsg!);
                }
                //edit convert folder
                ResultDTO resultEditFileConverts = await _legalLandfillPointCloudFileService.EditFileConverts(_webHostEnvironment.WebRootPath, legalLandfillPointCloudFileViewModel.OldLegalLandfillId, resultEdit.Data);
                if (!resultEditFileConverts.IsSuccess && resultEditFileConverts.HandleError())
                {
                    return ResultDTO.Fail(resultEditFileConverts.ErrMsg!);
                }
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewWasteVolumeDiffAnalysis))]
        public async Task<ResultDTO<WasteVolumeComparisonDTO>> WasteVolumeDiffAnalysis([FromBody] List<Guid> selectedFiles)
        {
            var resultGetEntities = await _legalLandfillPointCloudFileService.GetFilteredLegalLandfillPointCloudFiles(selectedFiles);
            if (!resultGetEntities.IsSuccess && resultGetEntities.HandleError())
            {
                return ResultDTO<WasteVolumeComparisonDTO>.Fail(resultGetEntities.ErrMsg!);
            }
            if (resultGetEntities.Data == null || resultGetEntities.Data.Count < 2 || resultGetEntities.Data.Count > 2)
            {
                return ResultDTO<WasteVolumeComparisonDTO>.Fail("Expected data is null or does not have required number of elements.");
            }

            var orderedList = resultGetEntities.Data.OrderBy(x => x.ScanDateTime).ToList();
            var resultCreatingFile = await _legalLandfillPointCloudFileService.CreateDiffWasteVolumeComparisonFile(orderedList, _webHostEnvironment.WebRootPath);
            if (!resultCreatingFile.IsSuccess && resultCreatingFile.HandleError())
            {
                return ResultDTO<WasteVolumeComparisonDTO>.Fail(resultCreatingFile.ErrMsg!);
            }
            if (resultCreatingFile.Data == null)
            {
                return ResultDTO<WasteVolumeComparisonDTO>.Fail("Differential file path is null");
            }

            var resultReadAndDeleteFile = await _legalLandfillPointCloudFileService.ReadAndDeleteDiffWasteVolumeComparisonFile(resultCreatingFile.Data);
            if (!resultReadAndDeleteFile.IsSuccess && resultReadAndDeleteFile.HandleError())
            {
                return ResultDTO<WasteVolumeComparisonDTO>.Fail(resultReadAndDeleteFile.ErrMsg!);
            }
            if (resultReadAndDeleteFile.Data == null)
            {
                return ResultDTO<WasteVolumeComparisonDTO>.Fail("Difference was not calculated properly!");
            }

            WasteVolumeComparisonDTO dto = new()
            {
                FileAName = Path.GetFileNameWithoutExtension(orderedList[0].FileName),
                FileBName = Path.GetFileNameWithoutExtension(orderedList[1].FileName),
                ScanDateFileA = orderedList[0].ScanDateTime.ToString("dd.MM.yyyy"),
                ScanDateFileB = orderedList[1].ScanDateTime.ToString("dd.MM.yyyy"),
                Difference = resultReadAndDeleteFile.Data
            };

            return ResultDTO<WasteVolumeComparisonDTO>.Ok(dto);
        }    

    }
}
