using AutoMapper;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration;
using Microsoft.AspNetCore.Mvc;
using SD;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area(areaName: "IntranetPortal")]

    public class MapConfigurationController : Controller
    {
        private readonly IMapConfigurationService _mapConfigurationService;
        private readonly IMapLayersConfigurationService _mapLayersConfigurationService;
        private readonly IMapLayerGroupsConfigurationService _mapLayerGroupsConfigurationService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public MapConfigurationController(IMapConfigurationService mapConfigurationService, IConfiguration configuration, IMapper mapper, IMapLayersConfigurationService mapLayersConfigurationService, IMapLayerGroupsConfigurationService mapLayerGroupsConfigurationService)
        {
            _mapConfigurationService = mapConfigurationService;
            _configuration = configuration;
            _mapper = mapper;
            _mapLayersConfigurationService = mapLayersConfigurationService;
            _mapLayerGroupsConfigurationService = mapLayerGroupsConfigurationService;
        }
        #region MapConfiguration
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewMapConfigurations))]
        public async Task<IActionResult> Index()
        {
            try
            {
                var resultDtoList = await _mapConfigurationService.GetAllMapConfigurations();

                if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);

                if (resultDtoList.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                var vmList = _mapper.Map<List<MapConfigurationViewModel>>(resultDtoList.Data);
                if(vmList == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                return View(vmList);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewMapConfigurations))]
        public async Task<ResultDTO<MapConfigurationDTO>> GetMapConfigurationById(Guid mapConfigurationId)
        {
            try
            {
                ResultDTO<MapConfigurationDTO> resultGetEntity = await _mapConfigurationService.GetMapConfigurationById(mapConfigurationId);

                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())                
                    return ResultDTO<MapConfigurationDTO>.Fail(resultGetEntity.ErrMsg!);                

                if (resultGetEntity.Data is null)                
                    return ResultDTO<MapConfigurationDTO>.Fail("Map configuration is null");

                return ResultDTO<MapConfigurationDTO>.Ok(resultGetEntity.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<MapConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddMapConfigurations))]
        public async Task<ResultDTO> CreateMapConfiguration(MapConfigurationViewModel mapConfigurationViewModel)
        {
            try
            {
                var userId = User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ResultDTO.Fail("User is not authenticated.");
                }

                mapConfigurationViewModel.CreatedById = userId;

                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }

                var dto = _mapper.Map<MapConfigurationDTO>(mapConfigurationViewModel);

                if (dto is null)
                {
                    var error = DbResHtml.T("Mapping failed", "Resources");
                    return ResultDTO.Fail(error.ToString());
                }

                ResultDTO resultCreate = await _mapConfigurationService.CreateMapConfiguration(dto);
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

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.EditMapConfigurations))]
        public async Task<IActionResult> EditMapConfiguration(Guid mapId)
        {
            try
            {
                var resultDto = await _mapConfigurationService.GetMapConfigurationById(mapId);

                if (!resultDto.IsSuccess && resultDto.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error400", 400);

                if (resultDto.Data == null)
                  return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                var viewModel = _mapper.Map<MapConfigurationViewModel>(resultDto.Data);
                if(viewModel == null)
                  return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                return View(viewModel);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error400", 400);
            }
        }




        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditMapConfigurations))]
        public async Task<ResultDTO> EditMapConfiguration(MapConfigurationViewModel mapConfigurationViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }

                var userId = User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ResultDTO.Fail("User is not authenticated.");
                }
                mapConfigurationViewModel.UpdatedById = userId;
                mapConfigurationViewModel.UpdatedOn = DateTime.UtcNow;

                var dto = _mapper.Map<MapConfigurationDTO>(mapConfigurationViewModel);
                if (dto == null)
                {
                    var error = DbResHtml.T("Mapping failed", "Resources");
                    return ResultDTO.Fail(error.ToString());
                }

                ResultDTO resultEdit = await _mapConfigurationService.EditMapConfiguration(dto);
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

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteMapConfigurations))]
        public async Task<ResultDTO> DeleteMapConfiguration(MapConfigurationViewModel mapConfigurationViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }
                ResultDTO<MapConfigurationDTO> resultGetEntity = await _mapConfigurationService.GetMapConfigurationById(mapConfigurationViewModel.Id);

                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                {
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);
                }

                if (resultGetEntity.Data is null)
                {
                    return ResultDTO.Fail("Data is null");
                }

                ResultDTO resultDelete = await _mapConfigurationService.DeleteMapConfiguration(resultGetEntity.Data);
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
        #endregion

        #region MapLayerGroupConfiguration


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewMapLayerGroupConfigurations))]
        public async Task<ResultDTO<MapLayerGroupConfigurationDTO>> GetMapLayerGroupConfigurationById(Guid mapLayerGroupConfigurationId)
        {
            try
            {
                ResultDTO<MapLayerGroupConfigurationDTO> resultGetEntity = await _mapLayerGroupsConfigurationService.GetMapLayerGroupConfigurationById(mapLayerGroupConfigurationId);

                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                {
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail(resultGetEntity.ErrMsg!);
                }

                if (resultGetEntity.Data is null)
                {
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("Map configuration is null");

                }
                return ResultDTO<MapLayerGroupConfigurationDTO>.Ok(resultGetEntity.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<MapLayerGroupConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewMapLayerGroupConfigurations))]
        public async Task<ResultDTO<MapLayerGroupConfigurationDTO>> GetAllGroupLayersById(Guid mapLayerGroupConfigurationId)
        {
            try
            {
                ResultDTO<MapLayerGroupConfigurationDTO> resultGetEntity = await _mapLayerGroupsConfigurationService.GetAllGroupLayersById(mapLayerGroupConfigurationId);

                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                {
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail(resultGetEntity.ErrMsg!);
                }

                if (resultGetEntity.Data is null)
                {
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("No layers in group ");

                }
                return ResultDTO<MapLayerGroupConfigurationDTO>.Ok(resultGetEntity.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<MapLayerGroupConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddMapLayerGroupConfigurations))]
        public async Task<ResultDTO<MapLayerGroupConfigurationDTO>> AddMapLayerGroupConfiguration(MapLayerGroupConfigurationViewModel mapLayerGroupConfigurationViewModel)
        {
            try
            {
                var userId = User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail("User is not authenticated.");
                }

                mapLayerGroupConfigurationViewModel.CreatedById = userId;

                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail(error);
                }

                var dto = _mapper.Map<MapLayerGroupConfigurationDTO>(mapLayerGroupConfigurationViewModel);

                if (dto is null)
                {
                    var error = DbResHtml.T("Mapping failed", "Resources");
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail(error.ToString());
                }

                ResultDTO<MapLayerGroupConfigurationDTO> resultCreate = await _mapLayerGroupsConfigurationService.CreateMapLayerGroupConfiguration(dto);
                if (!resultCreate.IsSuccess && resultCreate.HandleError())
                {
                    return ResultDTO<MapLayerGroupConfigurationDTO>.Fail(resultCreate.ErrMsg!);
                }

                return ResultDTO<MapLayerGroupConfigurationDTO>.Ok(resultCreate.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<MapLayerGroupConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditMapLayerGroupConfigurations))]
        public async Task<ResultDTO> EditMapLayerGroupConfiguration(MapLayerGroupConfigurationViewModel mapLayerGroupConfigurationViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }

                var userId = User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ResultDTO.Fail("User is not authenticated.");
                }
                mapLayerGroupConfigurationViewModel.UpdatedById = userId;
                mapLayerGroupConfigurationViewModel.UpdatedOn = DateTime.UtcNow;

                var dto = _mapper.Map<MapLayerGroupConfigurationDTO>(mapLayerGroupConfigurationViewModel);
                if (dto == null)
                {
                    var error = DbResHtml.T("Mapping failed", "Resources");
                    return ResultDTO.Fail(error.ToString());
                }

                ResultDTO resultEdit = await _mapLayerGroupsConfigurationService.EditMapLayerGroupConfiguration(dto);
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

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteMapLayerGroupConfigurations))]
        public async Task<ResultDTO> DeleteMapLayerGroupConfiguration(MapLayerGroupConfigurationViewModel mapLayerGroupConfigurationViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }
                ResultDTO<MapLayerGroupConfigurationDTO> resultGetEntity = await _mapLayerGroupsConfigurationService.GetMapLayerGroupConfigurationById(mapLayerGroupConfigurationViewModel.Id);

                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                {
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);
                }

                if (resultGetEntity.Data is null)
                {
                    return ResultDTO.Fail("Data is null");
                }

                ResultDTO resultDelete = await _mapLayerGroupsConfigurationService.DeleteMapLayerGroupConfiguration(resultGetEntity.Data);
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
        #endregion

        #region MapLayerConfiguration

        //[HttpGet]
        //[HasAuthClaim(nameof(SD.AuthClaims.ViewMapLayerConfigurations))]
        //public async Task<IActionResult> Index()
        //{
        //    var resultDtoList = await _mapLayersConfigurationService.GetAllMapLayerConfigurations();

        //    if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
        //    {
        //        var errorPath = _configuration["ErrorViewsPath:Error"];
        //        if (errorPath is null)
        //        {
        //            return BadRequest();
        //        }
        //        return Redirect(errorPath);
        //    }
        //    if (resultDtoList.Data == null)
        //    {
        //        var errorPath = _configuration["ErrorViewsPath:Error404"];
        //        if (errorPath == null)
        //        {
        //            return NotFound();
        //        }
        //        return Redirect(errorPath);
        //    }

        //    var vmList = _mapper.Map<List<MapLayerConfigurationViewModel>>(resultDtoList.Data);
        //    if (vmList == null)
        //    {
        //        var errorPath = _configuration["ErrorViewsPath:Error404"];
        //        if (errorPath == null)
        //        {
        //            return NotFound();
        //        }
        //        return Redirect(errorPath);
        //    }

        //    return View(vmList);
        //}

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewMapLayerConfigurations))]
        public async Task<ResultDTO<MapLayerConfigurationDTO>> GetMapLayerConfigurationById(Guid mapLayerConfigurationId)
        {
            try
            {
                ResultDTO<MapLayerConfigurationDTO> resultGetEntity = await _mapLayersConfigurationService.GetMapLayerConfigurationById(mapLayerConfigurationId);

                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                {
                    return ResultDTO<MapLayerConfigurationDTO>.Fail(resultGetEntity.ErrMsg!);
                }

                if (resultGetEntity.Data is null)
                {
                    return ResultDTO<MapLayerConfigurationDTO>.Fail("Map configuration is null");

                }
                return ResultDTO<MapLayerConfigurationDTO>.Ok(resultGetEntity.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<MapLayerConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddMapLayerConfigurations))]
        public async Task<ResultDTO<MapLayerConfigurationDTO>> AddMapLayerConfiguration(MapLayerConfigurationViewModel mapLayerConfigurationViewModel)
        {
            try
            {
                var userId = User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ResultDTO<MapLayerConfigurationDTO>.Fail("User is not authenticated.");
                }

                mapLayerConfigurationViewModel.CreatedById = userId;

                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO<MapLayerConfigurationDTO>.Fail(error);
                }

                var dto = _mapper.Map<MapLayerConfigurationDTO>(mapLayerConfigurationViewModel);

                if (dto is null)
                {
                    var error = DbResHtml.T("Mapping failed", "Resources");
                    return ResultDTO<MapLayerConfigurationDTO>.Fail(error.ToString());
                }

                ResultDTO<MapLayerConfigurationDTO> resultCreate = await _mapLayersConfigurationService.CreateMapLayerConfiguration(dto);

                if (!resultCreate.IsSuccess && resultCreate.HandleError())
                {
                    return ResultDTO<MapLayerConfigurationDTO>.Fail(resultCreate.ErrMsg!);
                }

                return ResultDTO<MapLayerConfigurationDTO>.Ok(resultCreate.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<MapLayerConfigurationDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditMapLayerConfigurations))]
        public async Task<ResultDTO> EditMapLayerConfiguration(MapLayerConfigurationViewModel mapLayerConfigurationViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }

                var userId = User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ResultDTO.Fail("User is not authenticated.");
                }
                mapLayerConfigurationViewModel.UpdatedById = userId;
                mapLayerConfigurationViewModel.UpdatedOn = DateTime.UtcNow;

                var dto = _mapper.Map<MapLayerConfigurationDTO>(mapLayerConfigurationViewModel);
                if (dto == null)
                {
                    var error = DbResHtml.T("Mapping failed", "Resources");
                    return ResultDTO.Fail(error.ToString());
                }

                ResultDTO resultEdit = await _mapLayersConfigurationService.EditMapLayerConfiguration(dto);
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

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteMapLayerConfigurations))]
        public async Task<ResultDTO> DeleteMapLayerConfiguration(MapLayerConfigurationViewModel mapLayerConfigurationViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }
                ResultDTO<MapLayerConfigurationDTO> resultGetEntity = await _mapLayersConfigurationService.GetMapLayerConfigurationById(mapLayerConfigurationViewModel.Id);

                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                {
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);
                }

                if (resultGetEntity.Data is null)
                {
                    return ResultDTO.Fail("Data is null");
                }

                ResultDTO resultDelete = await _mapLayersConfigurationService.DeleteMapLayerConfiguration(resultGetEntity.Data);
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
