using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using MainApp.BL.Services.DetectionServices;
using MainApp.MVC.Filters;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using SD;
using System.Collections.Generic;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DetectionIgnoreZonesController : Controller
    {
        private readonly IDetectionIgnoreZoneService _detectionIgnoreZoneService;
        public DetectionIgnoreZonesController(IDetectionIgnoreZoneService detectionIgnoreZoneService)
        {
           _detectionIgnoreZoneService = detectionIgnoreZoneService;
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ManageDetectionIgnoreZones))]
        public async Task<ResultDTO<List<DetectionIgnoreZoneDTO>>> GetAlllIgnoreZones()
        {
            try
            {
                ResultDTO<List<DetectionIgnoreZoneDTO>> resultGetEntites = await _detectionIgnoreZoneService.GetAllIgnoreZonesDTOs();
                if (!resultGetEntites.IsSuccess && resultGetEntites.HandleError())
                {
                    return ResultDTO<List<DetectionIgnoreZoneDTO>>.Fail(resultGetEntites.ErrMsg!);
                }
                if (resultGetEntites.Data == null)
                {
                    return ResultDTO<List<DetectionIgnoreZoneDTO>>.Fail("Igonre zones are not found");

                }
                return ResultDTO<List<DetectionIgnoreZoneDTO>>.Ok(resultGetEntites.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<DetectionIgnoreZoneDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ManageDetectionIgnoreZones))]
        public async Task<ResultDTO<DetectionIgnoreZoneDTO?>> GetIgnoreZoneById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return ResultDTO<DetectionIgnoreZoneDTO?>.Fail("Invalid zone id");
                }


                ResultDTO<DetectionIgnoreZoneDTO?> resultGetEntity = await _detectionIgnoreZoneService.GetIgnoreZoneById(id);
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                {
                    return ResultDTO<DetectionIgnoreZoneDTO?>.Fail(resultGetEntity.ErrMsg!);
                }

                return ResultDTO<DetectionIgnoreZoneDTO?>.Ok(resultGetEntity.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<DetectionIgnoreZoneDTO?>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ManageDetectionIgnoreZones))]
        public async Task<ResultDTO> AddIgnoreZone([FromBody] DetectionIgnoreZoneDTO dto)
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
                ResultDTO resultCreate = await _detectionIgnoreZoneService.CreateDetectionIgnoreZoneFromDTO(dto);
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
        [HasAuthClaim(nameof(SD.AuthClaims.ManageDetectionIgnoreZones))]
        public async Task<ResultDTO> DeleteIgnoreZone(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return ResultDTO.Fail("Invalid zone id");
                }

                ResultDTO<DetectionIgnoreZoneDTO?> resultGetEntity = await _detectionIgnoreZoneService.GetIgnoreZoneById(id);
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                {
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);
                }
                if (resultGetEntity.Data == null)
                {
                    return ResultDTO.Fail("Ignore zone does not exist");
                }


                ResultDTO resultDeleteEntity = await _detectionIgnoreZoneService.DeleteDetectionIgnoreZoneFromDTO(resultGetEntity.Data);
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
        [HasAuthClaim(nameof(SD.AuthClaims.ManageDetectionIgnoreZones))]
        public async Task<ResultDTO> UpdateIgnoreZone([FromBody] DetectionIgnoreZoneDTO dto)
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

                ResultDTO resultUpdate = await _detectionIgnoreZoneService.UpdateDetectionIgnoreZoneFromDTO(dto);
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

        
    }
}
