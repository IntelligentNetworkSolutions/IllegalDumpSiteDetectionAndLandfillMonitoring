using DocumentFormat.OpenXml.Spreadsheet;
using DTOs.MainApp.BL.DetectionDTOs;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.BL.Interfaces.Services.DetectionServices;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using SD.Helpers;

namespace MainApp.MVC.ViewComponents.Map
{
    public class HistoricDataViewComponent : ViewComponent
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        private readonly IDetectionRunService _detectionRunService;
        public HistoricDataViewComponent(ModulesAndAuthClaimsHelper modulesAndAuthClaims , IDetectionRunService detectionRunService)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
            _detectionRunService = detectionRunService;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid? detectionRunId)
        {
            if (_modulesAndAuthClaims.HasModule(SD.Modules.HistoricData) && User.HasAuthClaim(SD.AuthClaims.ViewHistoricData))
            {
                var listOfHistoricDataLayerDto = await _detectionRunService.GetDetectionRunsWithClassesHistoricDataLayer();
                HistoricDataDTO model = new();
                model.DetectionRuns = listOfHistoricDataLayerDto;
                model.SelectedDetectionRunId = detectionRunId;
                
                return View(model);
            }
            else
            {
                return Task.FromResult<IViewComponentResult>(Content(string.Empty)).Result;
            }
        }
    }
}
