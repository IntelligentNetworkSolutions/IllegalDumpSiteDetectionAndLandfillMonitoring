using AutoMapper;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Humanizer;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using MainApp.BL.Services.DetectionServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Host;
using SD;
using Services.Interfaces.Services;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class LegalLandfillsManagementController : Controller
    {
        private readonly ILegalLandfillService _legalLandfillService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public LegalLandfillsManagementController(ILegalLandfillService legalLandfillService, IConfiguration configuration, IMapper mapper)
        {
            _legalLandfillService = legalLandfillService;
            _configuration = configuration;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
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
                ResultDTO.Fail(error.ToString());
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
                ResultDTO.Fail(error.ToString());
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

            var dto = _mapper.Map<LegalLandfillDTO>(legalLandfillViewModel);
            if (dto == null)
            {
                var error = DbResHtml.T("Mapping failed", "Resources");
                ResultDTO.Fail(error.ToString());
            }

            ResultDTO resultEdit = await _legalLandfillService.DeleteLegalLandfill(dto);
            if (resultEdit.IsSuccess == false && resultEdit.HandleError())
            {
                return ResultDTO.Fail(resultEdit.ErrMsg!);
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
    }
}
