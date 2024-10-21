using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.BL.Services.TrainingServices;
using MainApp.MVC.Filters;
using Microsoft.AspNetCore.Mvc;
using SD;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class TrainedModelsController : Controller
    {

        private readonly ITrainedModelService _trainedModelService;
        public TrainedModelsController(ITrainedModelService trainedModelService)
        {
            _trainedModelService = trainedModelService;
        }
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModels()
        {
            ResultDTO<List<TrainedModelDTO>> resultGetEntities = await _trainedModelService.GetAllTrainedModels();

            if (!resultGetEntities.IsSuccess && resultGetEntities.HandleError())
            {
                return ResultDTO<List<TrainedModelDTO>>.Fail(resultGetEntities.ErrMsg!);
            }

            if (resultGetEntities.Data == null)
            {
                return ResultDTO<List<TrainedModelDTO>>.Fail("Trained models are not found");
            }

            return ResultDTO<List<TrainedModelDTO>>.Ok(resultGetEntities.Data);
        }
    }
}
