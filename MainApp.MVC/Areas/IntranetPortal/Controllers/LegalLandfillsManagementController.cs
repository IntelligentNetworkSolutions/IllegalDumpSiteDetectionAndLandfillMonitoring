using AutoMapper;
using CliWrap;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;
using Microsoft.AspNetCore.Mvc;
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
            var dtoList = await _legalLandfillService.GetAllLegalLandfills() ?? throw new Exception("Object not found");
            var vmList = _mapper.Map<List<LegalLandfillViewModel>>(dtoList.Data) ?? throw new Exception("Object not found");
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
            if(dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultCreate = await _legalLandfillService.CreateLegalLandfill(dto);
            if (resultCreate.IsSuccess == false && resultCreate.HandleError())
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
            if (resultEdit.IsSuccess == false && resultEdit.HandleError())
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
            if (resultCheckForFiles.IsSuccess == false && resultCheckForFiles.HandleError())
            {
                return ResultDTO.Fail(resultCheckForFiles.ErrMsg!);
            }

            if(resultCheckForFiles.Data.Count() > 0)
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
            if (resultDelete.IsSuccess == false && resultDelete.HandleError())
            {
                return ResultDTO.Fail(resultDelete.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        public async Task<ResultDTO<LegalLandfillDTO>> GetLegalLandfillById(Guid legalLandfillId)
        {
            ResultDTO<LegalLandfillDTO> resultGetEntity = await _legalLandfillService.GetLegalLandfillById(legalLandfillId);
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
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
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
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
            var dtoList = await _legalLandfillPointCloudFileService.GetAllLegalLandfillPointCloudFiles() ?? throw new Exception("Object not found");
            var vmList = _mapper.Map<List<LegalLandfillPointCloudFileViewModel>>(dtoList.Data) ?? throw new Exception("Object not found");
            return View(vmList);
        }

        [HttpPost]
        public async Task<ResultDTO<List<LegalLandfillDTO>>> GetAllLegalLandfills()
        {
            ResultDTO<List<LegalLandfillDTO>> resultGetEntity = await _legalLandfillService.GetAllLegalLandfills();
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
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
            if(viewModel.FileUpload == null)
            {
                return ResultDTO.Fail("Please insert file");
            }

            var fileUploadExtension = Path.GetExtension(viewModel.FileUpload.FileName);
            
            var supportedExtensionsAppSetting = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("SupportedPointCloudFileExtensions", ".las, .laz");
            List<string> listOfSupportedExtensions = new List<string>(supportedExtensionsAppSetting.Data.Split(','));

            for (int i = 0; i < listOfSupportedExtensions.Count; i++)
            {
                listOfSupportedExtensions[i] = listOfSupportedExtensions[i].Trim();
            }

            if (!listOfSupportedExtensions.Contains(fileUploadExtension))
            {
                return ResultDTO.Fail($"Not supported file extension. Supported extensions are {supportedExtensionsAppSetting.Data}");
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

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudUploadFolder.Data, resultGetEntity.Data.Id.ToString());
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
                if (resultCreateEntity.IsSuccess == false && resultCreateEntity.HandleError())
                {
                    return ResultDTO.Fail(resultCreateEntity.ErrMsg!);
                }

                //Upload the file in folder
                var fileName = string.Format("{0}{1}", resultCreateEntity.Data.Id.ToString(), fileUploadExtension);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.FileUpload.CopyToAsync(stream);
                }

                //Convert file to potree format
                string covertsFolder = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudConvertFolder.Data, resultGetEntity.Data.Id.ToString(), resultCreateEntity.Data.Id.ToString());
                if (!Directory.Exists(covertsFolder))
                {
                    Directory.CreateDirectory(covertsFolder);
                }

                var result = await Cli.Wrap(potreeConverterFilePath.Data)
                .WithArguments($"{filePath} -o {covertsFolder}")
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

                if (!result.IsSuccess)
                {
                    return ResultDTO.Fail("Converting failed");
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }

        }

        //[HttpPost]
        //[HasAuthClaim(nameof(SD.AuthClaims.PreviewLegalLandfillPointCloudFiles))]
        //public async Task<ResultDTO<string>> ProcessSelectedFiles([FromBody] List<Guid> selectedFiles)
        //{

        //    if (selectedFiles == null)
        //    {
        //        return ResultDTO<string>.Fail("No files selected");
        //    }
        //    TempData["SelectedFiles"] = JsonConvert.SerializeObject(selectedFiles);
        //    if (selectedFiles == null || !selectedFiles.Any())
        //    {
        //        return ResultDTO<string>.Fail("No files selected");
        //    }

        //    var redirectUrl = Url.Action("Preview", "LegalLandfillsManagement", new { Area = "IntranetPortal" });
        //    if (redirectUrl == null)
        //    {
        //        return ResultDTO<string>.Fail("Redirect url is null");
        //    }
        //    return ResultDTO<string>.Ok(redirectUrl);
        //}

        //[HttpGet]
        //[HasAuthClaim(nameof(SD.AuthClaims.PreviewLegalLandfillPointCloudFiles))]
        //public async Task<IActionResult> Preview()
        //{
        //    var selectedFilesJson = TempData["SelectedFiles"] as string;
        //    if (selectedFilesJson == null)
        //    {
        //        var errorPath = _configuration["ErrorViewsPath:Error"];
        //        return Redirect(errorPath);
        //    }

        //    var selectedFiles = JsonConvert.DeserializeObject<List<Guid>>(selectedFilesJson);
        //    if (selectedFiles == null)
        //    {
        //        var errorPath = _configuration["ErrorViewsPath:Error"];
        //        return Redirect(errorPath);
        //    }

        //    List<PreviewViewModel> listVM = [];
        //    foreach (var item in selectedFiles)
        //    {
        //        var resultGetEntity = await _legalLandfillPointCloudFileService.GetLegalLandfillPointCloudFilesById(item);
        //        if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
        //        {
        //            var errorPath = _configuration["ErrorViewsPath:Error404"];
        //            return Redirect(errorPath);
        //        }
        //        if (resultGetEntity.Data == null)
        //        {
        //            var errorPath = _configuration["ErrorViewsPath:Error404"];
        //            return Redirect(errorPath);
        //        }

        //        string? fileNameWithoutExtension = Path.GetFileNameWithoutExtension(resultGetEntity.Data.FileName);

        //        PreviewViewModel vm = new()
        //        {
        //            FileId = resultGetEntity.Data.Id,
        //            LandfillId = resultGetEntity.Data.LegalLandfillId,
        //            FileName = fileNameWithoutExtension ?? resultGetEntity.Data.FileName
        //        };
        //        listVM.Add(vm);
        //    }

        //    return View(listVM);
        //}

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewLegalLandfillPointCloudFiles))]
        public async Task<ResultDTO<string>> ProcessSelectedFiles([FromBody] List<Guid> selectedFiles)
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
                return ResultDTO<string>.Fail(ex.Message);
            }            
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.PreviewLegalLandfillPointCloudFiles))]
        public async Task<IActionResult> Preview(List<string> selectedFiles)
        {
            if (selectedFiles == null || selectedFiles.Count == 0)
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
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
                    if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                    {
                        var errorPath = _configuration["ErrorViewsPath:Error404"];
                        return Redirect(errorPath);
                    }
                    if (resultGetEntity.Data == null)
                    {
                        var errorPath = _configuration["ErrorViewsPath:Error404"];
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
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
            {
                return ResultDTO.Fail(resultGetEntity.ErrMsg!);
            }

            if (resultGetEntity.Data == null)
            {
                var error = DbResHtml.T("Object not found", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            try
            {
                //delete from uploads
                await DeleteFilesFromUploads(resultGetEntity);
                //delete from converts
                await DeleteFilesFromConverts(resultGetEntity);
                //delete from db
                ResultDTO resultDelete = await _legalLandfillPointCloudFileService.DeleteLegalLandfillPointCloudFile(legalLandfillPointCloudFileViewModel.Id);
                if (resultDelete.IsSuccess == false && resultDelete.HandleError())
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
        [HasAuthClaim(nameof(SD.AuthClaims.EditLegalLandfillPointCloudFile))]
        public async Task<ResultDTO> EditLegalLandfillPointCloudFileConfirmed(LegalLandfillPointCloudFileViewModel legalLandfillPointCloudFileViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            if(legalLandfillPointCloudFileViewModel.FilePath == null)
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

            try
            {
                ResultDTO<LegalLandfillPointCloudFileDTO> resultEdit = await _legalLandfillPointCloudFileService.EditLegalLandfillPointCloudFile(dto);
                if (resultEdit.IsSuccess == false && resultEdit.HandleError())
                {
                    return ResultDTO.Fail(resultEdit.ErrMsg!);
                }

                if (legalLandfillPointCloudFileViewModel.OldLegalLandfillId != resultEdit.Data.LegalLandfillId)
                {
                    //edit upload folder
                    await EditFileInUploads(legalLandfillPointCloudFileViewModel, resultEdit);

                    //edit convert folder
                    await EditFileConverts(legalLandfillPointCloudFileViewModel, resultEdit);
                }

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {

                return ResultDTO.Fail(ex.Message);
            }
        }

        private async Task EditFileInUploads(LegalLandfillPointCloudFileViewModel legalLandfillPointCloudFileViewModel, ResultDTO<LegalLandfillPointCloudFileDTO> resultEdit)
        {
            var pointCloudUploadFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileUploads", "Uploads\\LegalLandfillUploads\\PointCloudUploads");
            var fileNameUpload = resultEdit.Data.Id + Path.GetExtension(resultEdit.Data.FileName);
            var oldFileUploadPath = Path.Combine(_webHostEnvironment.WebRootPath, legalLandfillPointCloudFileViewModel.FilePath, fileNameUpload);

            var newFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudUploadFolder.Data, resultEdit.Data.LegalLandfillId.ToString());
            var newFileUploadPath = Path.Combine(newFolderPath, fileNameUpload);

            if (!System.IO.Directory.Exists(newFolderPath))
            {
                System.IO.Directory.CreateDirectory(newFolderPath);
            }

            System.IO.File.Move(oldFileUploadPath, newFileUploadPath);

            string oldDirectory = Path.Combine(_webHostEnvironment.WebRootPath, legalLandfillPointCloudFileViewModel.FilePath);
            if (System.IO.Directory.GetFiles(oldDirectory).Length == 0 && System.IO.Directory.GetDirectories(oldDirectory).Length == 0)
            {
                System.IO.Directory.Delete(oldDirectory);
            }
        }
        private async Task EditFileConverts(LegalLandfillPointCloudFileViewModel legalLandfillPointCloudFileViewModel, ResultDTO<LegalLandfillPointCloudFileDTO> resultEdit)
        {
            var pointCloudConvertFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts");
            var oldConvertedLandfillSubfolderPath = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudConvertFolder.Data, legalLandfillPointCloudFileViewModel.OldLegalLandfillId.ToString(), resultEdit.Data.Id.ToString());
            var oldConvertedLandfillFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudConvertFolder.Data, legalLandfillPointCloudFileViewModel.OldLegalLandfillId.ToString());

            var newConvertedLandfillSubfolderPath = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudConvertFolder.Data, resultEdit.Data.LegalLandfillId.ToString(), resultEdit.Data.Id.ToString());
            var newConvertedLandfillFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudConvertFolder.Data, resultEdit.Data.LegalLandfillId.ToString());

            if (!Directory.Exists(newConvertedLandfillFolderPath))
            {
                Directory.CreateDirectory(newConvertedLandfillFolderPath);
            }

            if (!Directory.Exists(newConvertedLandfillSubfolderPath))
            {
                Directory.CreateDirectory(newConvertedLandfillSubfolderPath);
            }

            foreach (var file in Directory.GetFiles(oldConvertedLandfillSubfolderPath))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(newConvertedLandfillSubfolderPath, fileName);
                System.IO.File.Move(file, destFile);
            }

            if (System.IO.Directory.GetFiles(oldConvertedLandfillSubfolderPath).Length == 0 && System.IO.Directory.GetDirectories(oldConvertedLandfillSubfolderPath).Length == 0)
            {
                System.IO.Directory.Delete(oldConvertedLandfillSubfolderPath);
            }

            if (System.IO.Directory.GetFiles(oldConvertedLandfillFolderPath).Length == 0 && System.IO.Directory.GetDirectories(oldConvertedLandfillFolderPath).Length == 0)
            {
                System.IO.Directory.Delete(oldConvertedLandfillFolderPath);
            }
        }
        private async Task DeleteFilesFromUploads(ResultDTO<LegalLandfillPointCloudFileDTO>? resultGetEntity)
        {           
            string fileName = resultGetEntity.Data.Id + Path.GetExtension(resultGetEntity.Data.FileName);
            string currentFilePathUpload = Path.Combine(_webHostEnvironment.WebRootPath, resultGetEntity.Data.FilePath, fileName);
            string currentLandfillFilesDir = Path.Combine(_webHostEnvironment.WebRootPath, resultGetEntity.Data.FilePath);

            if (System.IO.Directory.Exists(currentLandfillFilesDir))
            {
                string[] files = Directory.GetFiles(currentLandfillFilesDir);
                if (files.Length == 1)
                {
                    if (System.IO.File.Exists(currentFilePathUpload))
                    {
                        System.IO.File.Delete(currentFilePathUpload);
                        Directory.Delete(currentLandfillFilesDir, true);
                    }
                }
                else
                {
                    if (System.IO.File.Exists(currentFilePathUpload))
                    {
                        System.IO.File.Delete(currentFilePathUpload);
                    }
                }
            }
        }
        private async Task DeleteFilesFromConverts(ResultDTO<LegalLandfillPointCloudFileDTO>? resultGetEntity)
        {
            var pointCloudConvertFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileConverts", "Uploads\\LegalLandfillUploads\\PointCloudConverts");
            string currentFolderOfConvertedFilePath = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudConvertFolder.Data, resultGetEntity.Data.LegalLandfillId.ToString(), resultGetEntity.Data.Id.ToString());
            string currentLandfillFilesConvertedDir = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudConvertFolder.Data, resultGetEntity.Data.LegalLandfillId.ToString());

            if (System.IO.Directory.Exists(currentLandfillFilesConvertedDir))
            {
                string[] subDir = Directory.GetDirectories(currentLandfillFilesConvertedDir);
                if (subDir.Length == 1)
                {
                    string subDirName = new DirectoryInfo(subDir[0]).Name;
                    if (subDirName == resultGetEntity.Data.Id.ToString())
                    {
                        Directory.Delete(currentLandfillFilesConvertedDir, true);
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(currentFolderOfConvertedFilePath))
                    {
                        Directory.Delete(currentFolderOfConvertedFilePath, true);
                    }
                }
            }
        }
    }
}
