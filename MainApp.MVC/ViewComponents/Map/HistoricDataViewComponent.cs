using DocumentFormat.OpenXml.Spreadsheet;
using DTOs.MainApp.BL.DetectionDTOs;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using SD.Helpers;

namespace MainApp.MVC.ViewComponents.Map
{
    public class HistoricDataViewComponent : ViewComponent
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        public HistoricDataViewComponent(ModulesAndAuthClaimsHelper modulesAndAuthClaims)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_modulesAndAuthClaims.HasModule(SD.Modules.HistoricData) && User.HasAuthClaim(SD.AuthClaims.ViewHistoricData))
            {
                //for real scenario: var detectionRuns = _historicDataService.GetAllDetectionRuns();
                var detectionRuns = new List<DetectionRunsDTO>
                {
                    new DetectionRunsDTO
                    {
                        Id = Guid.NewGuid(),
                        CreatedBy = "User1",
                        CreatedOn = DateTime.UtcNow.AddDays(-1),
                        Geom = new NetTopologySuite.Geometries.Polygon(new LinearRing(new[]
                        {
                            new Coordinate(21.8103333, 42.1488008),
                            new Coordinate(21.7663880, 42.1243467),
                            new Coordinate(21.7862663, 42.1084947),
                            new Coordinate(21.8338165, 42.1296431),
                            new Coordinate(21.8102775, 42.1487827),
                            new Coordinate(21.8103333, 42.1488008)
                        }))
                    },
                    new DetectionRunsDTO
                    {
                        Id = Guid.NewGuid(),
                        CreatedBy = "User2",
                        CreatedOn = DateTime.UtcNow.AddDays(-2),
                         Geom = new NetTopologySuite.Geometries.Polygon(new LinearRing(new[]
                         {
                            new Coordinate(21.5516052, 41.9969070),
                            new Coordinate(21.4973602, 41.9708620),
                            new Coordinate(21.5200195, 41.9376520),
                            new Coordinate(21.5900574, 41.9534928),
                            new Coordinate(21.5529785, 41.9958858),
                            new Coordinate(21.5516052, 41.9969070)
                         }))
                    },
                    new DetectionRunsDTO
                    {
                        Id = Guid.NewGuid(),
                        CreatedBy = "User3",
                        CreatedOn = DateTime.UtcNow.AddDays(-3),
                        Geom = new NetTopologySuite.Geometries.Polygon(new LinearRing(new[]
                        {
                            new Coordinate(21.7312317, 41.8878556),
                            new Coordinate(21.6680603, 41.8714870),
                            new Coordinate(21.6879730, 41.8356659),
                            new Coordinate(21.7545776, 41.8428317),
                            new Coordinate(21.7298584, 41.8878556),
                            new Coordinate(21.7312317, 41.8878556)
                        }))
                    }
                };

                return View(detectionRuns);
            }
            else
            {
                return Task.FromResult<IViewComponentResult>(Content(string.Empty)).Result;
            }
        }
    }
}
