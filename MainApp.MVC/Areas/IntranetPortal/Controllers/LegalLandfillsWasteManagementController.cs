using AutoMapper;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;
using Microsoft.AspNetCore.Mvc;
using SD;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class LegalLandfillsWasteManagementController : Controller
    {
        private readonly ILegalLandfillTruckService _legalLandfillTruckService;
        private readonly ILegalLandfillWasteTypeService _legalLandfillWasteTypeService;
        private readonly ILegalLandfillWasteImportService _legalLandfillWasteImportService;
        private readonly ILegalLandfillService _legalLandfillService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;



        public LegalLandfillsWasteManagementController(ILegalLandfillTruckService legalLandfillTruckService,
                                                        ILegalLandfillWasteTypeService legalLandfillWasteTypeService,
                                                        ILegalLandfillWasteImportService legalLandfillWasteImportService,
                                                        ILegalLandfillService legalLandfillService,
                                                        IMapper mapper, IConfiguration configuration)
        {
            _legalLandfillTruckService = legalLandfillTruckService;
            _legalLandfillWasteTypeService = legalLandfillWasteTypeService;
            _legalLandfillWasteImportService = legalLandfillWasteImportService;
            _configuration = configuration;
            _mapper = mapper;
            _legalLandfillService = legalLandfillService;
        }

        #region LegalLandfillTrucks

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewLegalLandfillTrucks))]
        public async Task<IActionResult> ViewLegalLandfillTrucks()
        {
            var resultDtoList = await _legalLandfillTruckService.GetAllLegalLandfillTrucks();
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

            var vmList = _mapper.Map<List<LegalLandfillTruckViewModel>>(resultDtoList.Data);
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
        //[ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.AddLegalLandfillTruck))]
        public async Task<ResultDTO> CreateLegalLandfillTruckConfirmed(LegalLandfillTruckViewModel legalLandfillTruckViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var dto = _mapper.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }
            if (dto.Capacity is null || dto.UnladenWeight is null || dto.PayloadWeight is null)
            {
                var error = DbResHtml.T("Capacity, Unladen Weight and Payload Weight must be provided.");
                return ResultDTO.Fail(error.ToString());
            }

            dto.IsEnabled = true;

            ResultDTO resultCreate = await _legalLandfillTruckService.CreateLegalLandfillTruck(dto);
            if (!resultCreate.IsSuccess && resultCreate.HandleError())
            {
                return ResultDTO.Fail(resultCreate.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteLegalLandfillTruck))]
        public async Task<ResultDTO> DeleteLegalLandfillTruckConfirmed(LegalLandfillTruckViewModel legalLandfillTruckViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var resultCheckForFiles = await _legalLandfillTruckService.GetLegalLandfillTruckById(legalLandfillTruckViewModel.Id);
            if (!resultCheckForFiles.IsSuccess && resultCheckForFiles.HandleError())
            {
                return ResultDTO.Fail(resultCheckForFiles.ErrMsg!);
            }
            if (resultCheckForFiles.Data == null)
            {
                return ResultDTO.Fail("Data is null");
            }

            var dto = _mapper.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultDelete = await _legalLandfillTruckService.DeleteLegalLandfillTruck(dto);
            if (!resultDelete.IsSuccess && resultDelete.HandleError())
            {
                return ResultDTO.Fail(resultDelete.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteLegalLandfillTruck))]
        public async Task<ResultDTO> DisableLegalLandfillTruckConfirmed(LegalLandfillTruckViewModel legalLandfillTruckViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var dto = _mapper.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultDisable = await _legalLandfillTruckService.DisableLegalLandfillTruck(dto);

            if (!resultDisable.IsSuccess && resultDisable.HandleError())
            {
                return ResultDTO.Fail(resultDisable.ErrMsg!);
            }

            return ResultDTO.Ok();
        }



        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ResultDTO<LegalLandfillTruckDTO>> GetLegalLandfillTruckById(Guid legalLandfillTruckId)
        {
            ResultDTO<LegalLandfillTruckDTO> resultGetEntity = await _legalLandfillTruckService.GetLegalLandfillTruckById(legalLandfillTruckId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<LegalLandfillTruckDTO>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<LegalLandfillTruckDTO>.Fail("Landfill is null");

            }
            return ResultDTO<LegalLandfillTruckDTO>.Ok(resultGetEntity.Data);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.EditLegalLandfillTruck))]
        public async Task<ResultDTO> EditLegalLandfillTruckConfirmed(LegalLandfillTruckViewModel legalLandfillTruckViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var dto = _mapper.Map<LegalLandfillTruckDTO>(legalLandfillTruckViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            if (dto.Capacity is null || dto.UnladenWeight is null || dto.PayloadWeight is null)
            {
                var error = DbResHtml.T("Capacity, Unladen Weight and Payload Weight must be provided.");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultEdit = await _legalLandfillTruckService.EditLegalLandfillTruck(dto);
            if (!resultEdit.IsSuccess && resultEdit.HandleError())
            {
                return ResultDTO.Fail(resultEdit.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        #endregion

        #region LegalLandfillWasteType

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewLegalLandfillWasteTypes))]
        public async Task<IActionResult> ViewLegalLandfillWasteTypes()
        {
            var resultDtoList = await _legalLandfillWasteTypeService.GetAllLegalLandfillWasteTypes();
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

            var vmList = _mapper.Map<List<LegalLandfillWasteTypeViewModel>>(resultDtoList.Data);
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
        //[ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.AddLegalLandfillWasteType))]
        public async Task<ResultDTO> CreateLegalLandfillWasteTypeConfirmed(LegalLandfillWasteTypeViewModel legalLandfillWasteTypeViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var dto = _mapper.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultCreate = await _legalLandfillWasteTypeService.CreateLegalLandfillWasteType(dto);
            if (!resultCreate.IsSuccess && resultCreate.HandleError())
            {
                return ResultDTO.Fail(resultCreate.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteLegalLandfillWasteType))]
        public async Task<ResultDTO> DeleteLegalLandfillWasteTypeConfirmed(LegalLandfillWasteTypeViewModel legalLandfillWasteTypeViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var resultCheckForFiles = await _legalLandfillWasteTypeService.GetLegalLandfillWasteTypeById(legalLandfillWasteTypeViewModel.Id);
            if (!resultCheckForFiles.IsSuccess && resultCheckForFiles.HandleError())
            {
                return ResultDTO.Fail(resultCheckForFiles.ErrMsg!);
            }
            if (resultCheckForFiles.Data == null)
            {
                return ResultDTO.Fail("Data is null");
            }

            var dto = _mapper.Map<LegalLandfillWasteTypeDTO>(legalLandfillWasteTypeViewModel);

            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultDelete = await _legalLandfillWasteTypeService.DeleteLegalLandfillWasteType(dto);
            if (!resultDelete.IsSuccess && resultDelete.HandleError())
            {
                return ResultDTO.Fail(resultDelete.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.EditLegalLandfillWasteType))]
        public async Task<ResultDTO<LegalLandfillWasteTypeDTO>> GetLegalLandfillWasteTypeById(Guid legalLandfillWasteTypeId)
        {
            ResultDTO<LegalLandfillWasteTypeDTO> resultGetEntity = await _legalLandfillWasteTypeService.GetLegalLandfillWasteTypeById(legalLandfillWasteTypeId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<LegalLandfillWasteTypeDTO>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<LegalLandfillWasteTypeDTO>.Fail("Landfill is null");

            }
            return ResultDTO<LegalLandfillWasteTypeDTO>.Ok(resultGetEntity.Data);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditLegalLandfillWasteType))]
        public async Task<ResultDTO> EditLegalLandfillWasteTypeConfirmed(LegalLandfillWasteTypeViewModel LegalLandfillWasteTypeViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var dto = _mapper.Map<LegalLandfillWasteTypeDTO>(LegalLandfillWasteTypeViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultEdit = await _legalLandfillWasteTypeService.EditLegalLandfillWasteType(dto);
            if (!resultEdit.IsSuccess && resultEdit.HandleError())
            {
                return ResultDTO.Fail(resultEdit.ErrMsg!);
            }

            return ResultDTO.Ok();
        }
        #endregion

        #region LegalLandfillWasteImport

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewLegalLandfillWasteImports))]
        public async Task<IActionResult> ViewLegalLandfillWasteImports()
        {
            var resultDtoList = await _legalLandfillWasteImportService.GetAllLegalLandfillWasteImports();
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

            var vmList = _mapper.Map<List<LegalLandfillWasteImportViewModel>>(resultDtoList.Data);
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
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteLegalLandfillTruck))]
        public async Task<ResultDTO> DeleteLegalLandfillWasteImportConfirmed(LegalLandfillWasteImportViewModel legalLandfillWasteImportViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return ResultDTO.Fail(error);
            }

            var resultCheckForFiles = await _legalLandfillWasteImportService.GetLegalLandfillWasteImportById(legalLandfillWasteImportViewModel.Id);
            if (!resultCheckForFiles.IsSuccess && resultCheckForFiles.HandleError())
            {
                return ResultDTO.Fail(resultCheckForFiles.ErrMsg!);
            }
            if (resultCheckForFiles.Data == null)
            {
                return ResultDTO.Fail("Data is null");
            }

            var dto = _mapper.Map<LegalLandfillWasteImportDTO>(legalLandfillWasteImportViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultDelete = await _legalLandfillWasteImportService.DeleteLegalLandfillWasteImport(dto);
            if (!resultDelete.IsSuccess && resultDelete.HandleError())
            {
                return ResultDTO.Fail(resultDelete.ErrMsg!);
            }

            return ResultDTO.Ok();
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditLegalLandfillWasteImports))]
        public async Task<ResultDTO<LegalLandfillWasteImportDTO>> GetLegalLandfillWasteImportById(Guid legalLandfillWasteImportId)
        {
            ResultDTO<LegalLandfillWasteImportDTO> resultGetEntity = await _legalLandfillWasteImportService.GetLegalLandfillWasteImportById(legalLandfillWasteImportId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                return ResultDTO<LegalLandfillWasteImportDTO>.Fail(resultGetEntity.ErrMsg!);
            }
            if (resultGetEntity.Data == null)
            {
                return ResultDTO<LegalLandfillWasteImportDTO>.Fail("Landfill is null");

            }
            return ResultDTO<LegalLandfillWasteImportDTO>.Ok(resultGetEntity.Data);
        }


        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.AddLegalLandfillWasteImports))]
        public async Task<IActionResult> CreateLegalLandfillWasteImport()
        {
            var legalLandfillTrucks = await _legalLandfillTruckService.GetAllLegalLandfillTrucks();
            var legalLandfills = await _legalLandfillService.GetAllLegalLandfills();
            var legalLandfillWasteTypes = await _legalLandfillWasteTypeService.GetAllLegalLandfillWasteTypes();
            var id = User.FindFirstValue("UserId");

            if (!legalLandfillTrucks.IsSuccess || !legalLandfills.IsSuccess || !legalLandfillWasteTypes.IsSuccess)
            {
                return RedirectToAction("ViewLegalLandfillWasteImports", "LegalLandfillsWasteManagement");
            }

            var viewModel = new LegalLandfillWasteImportViewModel
            {
                LegalLandfillTrucks = legalLandfillTrucks.Data,
                LegalLandfills = legalLandfills.Data,
                LegalLandfillWasteTypes = legalLandfillWasteTypes.Data,
                CreatedById = id,
            };

            return View(viewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.AddLegalLandfillWasteImports))]
        public async Task<IActionResult> CreateLegalLandfillWasteImport(LegalLandfillWasteImportViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, errorMessage: "Model state is invalid.");
                return View(viewModel);
            }

            var dto = new LegalLandfillWasteImportDTO
            {
                ImportExportStatus = viewModel.ImportExportStatus,
                LegalLandfillId = viewModel.LegalLandfillId,
                LegalLandfillTruckId = viewModel.LegalLandfillTruckId,
                LegalLandfillWasteTypeId = viewModel.LegalLandfillWasteTypeId,
                Capacity = viewModel.Capacity,
                Weight = viewModel.Weight,
                CreatedOn = DateTime.UtcNow,
                CreatedById = viewModel.CreatedById,
                ImportedOn = DateTime.UtcNow,
            };

            if (dto is null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                return RedirectToAction("ViewLegalLandfillWasteImports", "LegalLandfillsWasteManagement");
            }
            ResultDTO resultAdd = await _legalLandfillWasteImportService.CreateLegalLandfillWasteImport(dto);

            if (!resultAdd.IsSuccess && resultAdd.HandleError())
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return View(viewModel);
            }

            return RedirectToAction("ViewLegalLandfillWasteImports", "LegalLandfillsWasteManagement");
        }



        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.EditLegalLandfillWasteImports))]
        public async Task<IActionResult> EditLegalLandfillWasteImport(Guid legalLandfillWasteImportId)
        {
            var entity = await _legalLandfillWasteImportService.GetLegalLandfillWasteImportById(legalLandfillWasteImportId);
            var legalLandfillTrucks = await _legalLandfillTruckService.GetAllLegalLandfillTrucks();
            var legalLandfills = await _legalLandfillService.GetAllLegalLandfills();
            var legalLandfillWasteTypes = await _legalLandfillWasteTypeService.GetAllLegalLandfillWasteTypes();


            var userId = User.FindFirstValue("UserId");

            if (!entity.IsSuccess || !legalLandfillTrucks.IsSuccess || !legalLandfills.IsSuccess || !legalLandfillWasteTypes.IsSuccess)
            {
                return RedirectToAction("ViewLegalLandfillWasteImports", "LegalLandfillsWasteManagement");
            }

            if (entity.Data == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (errorPath == null)
                {
                    return NotFound();
                }
                return Redirect(errorPath);
            }

            LegalLandfillWasteImportViewModel viewModel = new LegalLandfillWasteImportViewModel()
            {
                Id = entity.Data.Id,
                Capacity = entity.Data.Capacity,
                Weight = entity.Data.Weight,
                ImportExportStatus = entity.Data.ImportExportStatus,
                LegalLandfillId = entity.Data.LegalLandfillId,
                LegalLandfillTruckId = entity.Data.LegalLandfillTruckId,
                LegalLandfillWasteTypeId = entity.Data.LegalLandfillWasteTypeId,
                LegalLandfills = legalLandfills.Data,
                LegalLandfillTrucks = legalLandfillTrucks.Data,
                LegalLandfillWasteTypes = legalLandfillWasteTypes.Data,
                CreatedById = userId,
                CreatedOn = entity.Data.CreatedOn
            };

            return View(viewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.EditLegalLandfillWasteImports))]
        public async Task<IActionResult> EditLegalLandfillWasteImport(LegalLandfillWasteImportViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (errorPath == null)
                {
                    return BadRequest();
                }
                return Redirect(errorPath);
            }

            LegalLandfillWasteImportDTO dto = _mapper.Map<LegalLandfillWasteImportDTO>(viewModel);

            await _legalLandfillWasteImportService.EditLegalLandfillWasteImport(dto);

            return RedirectToAction("ViewLegalLandfillWasteImports");
        }
    }

    #endregion

}

