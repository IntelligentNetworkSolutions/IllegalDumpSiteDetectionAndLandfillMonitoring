using AutoMapper;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using SD;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class RegisteredDumpsitesController : Controller
    {
        private readonly IRegisteredDumpsiteService _registeredDumpsiteService;
        private readonly IRegisteredDumpsiteWasteTypeService _registeredDumpsiteWasteTypeService;
        private readonly IRegisteredDumpsiteRiskLevelService _registeredDumpsiteRiskLevelService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public RegisteredDumpsitesController(IRegisteredDumpsiteService registeredDumpsiteService, IRegisteredDumpsiteWasteTypeService registeredDumpsiteWasteTypeService, IRegisteredDumpsiteRiskLevelService registeredDumpsiteRiskLevelService, IConfiguration configuration, IMapper mapper)
        {
            _registeredDumpsiteService = registeredDumpsiteService;
            _registeredDumpsiteWasteTypeService = registeredDumpsiteWasteTypeService;
            _registeredDumpsiteRiskLevelService = registeredDumpsiteRiskLevelService;
            _configuration = configuration;
            _mapper = mapper;
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

                var result = await _registeredDumpsiteService.MergeRegisteredDumpsites(request.NewDumpsiteData, request.ExistingDumpsiteIds);

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
                // Use your existing service call
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
                // Use your existing service call for risk levels
                var resultDtoList = await _registeredDumpsiteRiskLevelService.GetAllRegisteredDumpsiteRiskLevels(); // Assuming similar service exists

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
    }
}
