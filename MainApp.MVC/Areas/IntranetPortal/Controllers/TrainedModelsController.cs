using AutoMapper;
using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.BL.Interfaces.Services.TrainingServices;
using MainApp.BL.Services.TrainingServices;
using MainApp.MVC.Filters;
using MainApp.MVC.ViewModels.IntranetPortal.Training;
using Microsoft.AspNetCore.Mvc;
using SD;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class TrainedModelsController : Controller
    {

        private readonly ITrainedModelService _trainedModelService;
        private readonly ITrainingRunService _trainingRunService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public TrainedModelsController(ITrainedModelService trainedModelService, ITrainingRunService trainingRunService, IMapper mapper, IConfiguration configuration)
        {
            _trainedModelService = trainedModelService;
            _trainingRunService = trainingRunService;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<IActionResult> Index()
        {
            try
            {
                ResultDTO<List<TrainedModelDTO>> resultDtoList = await _trainedModelService.GetAllTrainedModels();

                if (!resultDtoList.IsSuccess && resultDtoList.HandleError())
                {
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                }
                if (resultDtoList.Data == null)
                {
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);
                }

                var vmList = _mapper.Map<List<TrainedModelViewModel>>(resultDtoList.Data);
                if (vmList == null)
                {
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);
                }

                return View(vmList);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModels()
        {
            try
            {
                ResultDTO<List<TrainedModelDTO>> resultDtoList = await _trainedModelService.GetAllTrainedModels();

                if (resultDtoList.IsSuccess == false && resultDtoList.HandleError())
                    return ResultDTO<List<TrainedModelDTO>>.Fail(resultDtoList.ErrMsg!);

                if (resultDtoList.Data == null)
                    return ResultDTO<List<TrainedModelDTO>>.Fail("Trained models are not found");

                return ResultDTO<List<TrainedModelDTO>>.Ok(resultDtoList.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<TrainedModelDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<ResultDTO<TrainedModelDTO>> GetTrainedModelById(Guid trainedModelId)
        {
            try
            {
                ResultDTO<TrainedModelDTO> resultGetEntity = await _trainedModelService.GetTrainedModelById(trainedModelId);
                if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                    return ResultDTO<TrainedModelDTO>.Fail(resultGetEntity.ErrMsg!);

                if (resultGetEntity.Data == null)
                    return ResultDTO<TrainedModelDTO>.Fail("Trained model not found");

                return ResultDTO<TrainedModelDTO>.Ok(resultGetEntity.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<TrainedModelDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditTrainingRun))]
        public async Task<ResultDTO> EditTrainedModelById(TrainedModelViewModel trainedModelViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return ResultDTO.Fail(error);
                }

                ResultDTO resultEdit = await _trainedModelService.EditTrainedModelById(trainedModelViewModel.Id, trainedModelViewModel.Name, trainedModelViewModel.IsPublished);

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
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteTrainingRun))]
        public async Task<ResultDTO> DeleteTrainedModelById(Guid trainedModelId)
        {
            try
            {
                if (trainedModelId == Guid.Empty)
                    return ResultDTO.Fail("Invalid training run id");

                ResultDTO resultDelete = await _trainedModelService.DeleteTrainedModelById(trainedModelId);

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
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainedModelStatistics))]
        public async Task<ResultDTO<TrainingRunResultsDTO>> GetTrainedModelStatistics(Guid trainedModelId)
        {
            try
            {
                ResultDTO<TrainedModelDTO>? resultGetTrainedModel = await _trainedModelService.GetTrainedModelById(trainedModelId);
                if (resultGetTrainedModel.IsSuccess == false && resultGetTrainedModel.HandleError())
                    return ResultDTO<TrainingRunResultsDTO>.Fail(resultGetTrainedModel.ErrMsg!);
                if (resultGetTrainedModel.Data == null)
                    return ResultDTO<TrainingRunResultsDTO>.Fail("Trained model not found");

                ResultDTO<TrainingRunResultsDTO>? resultGetBestEpoch = _trainingRunService.GetBestEpochForTrainingRun(resultGetTrainedModel.Data.TrainingRunId.Value);
                if (resultGetBestEpoch.IsSuccess == false && resultGetBestEpoch.HandleError())
                    return ResultDTO<TrainingRunResultsDTO>.Fail(resultGetBestEpoch.ErrMsg!);
                if (resultGetBestEpoch.Data == null)
                    return ResultDTO<TrainingRunResultsDTO>.Fail("Failed to get training run statistics");

                return ResultDTO<TrainingRunResultsDTO>.Ok(resultGetBestEpoch.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<TrainingRunResultsDTO>.ExceptionFail(ex.Message, ex);
            }
        }

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
