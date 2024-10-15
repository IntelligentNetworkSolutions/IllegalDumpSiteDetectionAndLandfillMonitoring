using AutoMapper;
using MainApp.BL.Interfaces.Services.TrainingServices;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class TrainingRunsController : Controller
    {
        private readonly ITrainingRunService _trainingRunService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TrainingRunsController(ITrainingRunService trainingRunService, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _trainingRunService = trainingRunService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var getTrainingRunResultsLogResult = await _trainingRunService.GetBestEpochForTrainingRun(Guid.Parse("e6684e31-1dd9-489a-9163-524f134e397a"));

            //var res = await _trainingRunService.ExecuteDummyTrainingRunProcess();

            return View();
        }
    }
}
