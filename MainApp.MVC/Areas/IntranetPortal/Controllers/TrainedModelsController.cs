using AutoMapper;
using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.BL.Interfaces.Services.TrainingServices;
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
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public TrainedModelsController(ITrainedModelService trainedModelService, IMapper mapper, IConfiguration configuration)
        {
            _trainedModelService = trainedModelService;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<IActionResult> Index()
        {
            ResultDTO<List<TrainedModelDTO>> resultDtoList = await _trainedModelService.GetAllTrainedModels();

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

            var vmList = _mapper.Map<List<TrainedModelViewModel>>(resultDtoList.Data);
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

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<ResultDTO<List<TrainedModelDTO>>> GetAllTrainedModels()
        {
            ResultDTO<List<TrainedModelDTO>> resultDtoList = await _trainedModelService.GetAllTrainedModels();

            if (resultDtoList.IsSuccess == false && resultDtoList.HandleError())
                return ResultDTO<List<TrainedModelDTO>>.Fail(resultDtoList.ErrMsg!);

            if (resultDtoList.Data == null)
                return ResultDTO<List<TrainedModelDTO>>.Fail("Trained models are not found");

            return ResultDTO<List<TrainedModelDTO>>.Ok(resultDtoList.Data);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ViewTrainingRuns))]
        public async Task<ResultDTO<TrainedModelDTO>> GetTrainedModelById(Guid trainedModelId)
        {
            ResultDTO<TrainedModelDTO> resultGetEntity = await _trainedModelService.GetTrainedModelById(trainedModelId);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
                return ResultDTO<TrainedModelDTO>.Fail(resultGetEntity.ErrMsg!);

            if (resultGetEntity.Data == null)
                return ResultDTO<TrainedModelDTO>.Fail("Trained model not found");

            return ResultDTO<TrainedModelDTO>.Ok(resultGetEntity.Data);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditTrainingRun))]
        public async Task<ResultDTO> EditTrainedModelById(TrainedModelViewModel trainedModelViewModel)
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

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteTrainingRun))]
        public async Task<ResultDTO> DeleteTrainedModelById(Guid trainedModelId)
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
    }
}
