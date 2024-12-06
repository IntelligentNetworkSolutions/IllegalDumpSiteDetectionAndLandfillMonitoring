using AutoMapper;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using Microsoft.AspNetCore.Mvc;
using SD;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DatasetClassesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IDatasetClassesService _datasetClassesService;
        private readonly IDataset_DatasetClassService _datasetDatasetClassService;
        private readonly IDatasetService _datasetSerivce;

        public DatasetClassesController(IConfiguration configuration,
                                        IMapper mapper,
                                        IDatasetClassesService datasetClassesService,
                                        IDataset_DatasetClassService datasetDatasetClassService,
                                        IDatasetService datasetService)
        {
            _configuration = configuration;
            _mapper = mapper;
            _datasetClassesService = datasetClassesService;
            _datasetDatasetClassService = datasetDatasetClassService;
            _datasetSerivce = datasetService;
        }

        [HasAuthClaim(nameof(SD.AuthClaims.ViewDatasetClasses))]
        public async Task<IActionResult> Index()
        {
            try
            {
                var allClassesList = await _datasetClassesService.GetAllDatasetClasses();
                if (!allClassesList.IsSuccess && allClassesList.HandleError())
                {
                    var errorPath = _configuration["ErrorViewsPath:Error"];
                    if (errorPath == null)
                    {
                        return BadRequest();
                    }
                    return Redirect(errorPath);
                }
                if (allClassesList.Data == null)
                {
                    var errorPath = _configuration["ErrorViewsPath:Error404"];
                    if (errorPath == null)
                    {
                        return NotFound();
                    }
                    return Redirect(errorPath);
                }

                var model = _mapper.Map<List<DatasetClassViewModel>>(allClassesList.Data);

                if (model == null)
                {
                    var errorPath = _configuration["ErrorViewsPath:Error404"];
                    if (errorPath == null)
                    {
                        return NotFound();
                    }
                    return Redirect(errorPath);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (errorPath == null)
                {
                    return BadRequest();
                }
                return Redirect(errorPath);
            }
        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDatasetClass))]
        public async Task<IActionResult> CreateClass(CreateDatasetClassDTO model)
        {
            try
            {
                string? userId = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { responseError = DbResHtml.T("User ID is not valid", "Resources") });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { responseError = DbResHtml.T("Model is not valid", "Resources") });
                }

                model.CreatedById = userId;
                var isAdded = await _datasetClassesService.AddDatasetClass(model);

                if (isAdded.IsSuccess == false && isAdded.HandleError())
                {
                    return Json(new { responseError = DbResHtml.T("An error occurred. Dataset class was not added.", "Resources") });
                }

                return isAdded.Data switch
                {
                    1 when isAdded.IsSuccess && string.IsNullOrEmpty(isAdded.ErrMsg) =>
                        Json(new { responseSuccess = DbResHtml.T("Successfully added dataset class", "Resources") }),

                    2 => Json(new { responseError = DbResHtml.T(isAdded.ErrMsg, "Resources") }),

                    3 => Json(new { responseError = DbResHtml.T(isAdded.ErrMsg, "Resources") }),

                    _ => Json(new { responseError = DbResHtml.T("Dataset class was not added", "Resources") })
                };
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T("An unexpected error occurred. Please try again later.", "Resources") });
            }
        }



        //[HttpPost]
        //[HasAuthClaim(nameof(SD.AuthClaims.EditDatasetClass))]
        //public async Task<IActionResult> EditClass(EditDatasetClassDTO model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new { responseError = DbResHtml.T("Model is not valid", "Resources") });
        //    }

        //    var isUpdated = await _datasetClassesService.EditDatasetClass(model);
        //    var allClasses = await _datasetClassesService.GetAllDatasetClasses();
        //    if (allClasses.IsSuccess == false && allClasses.HandleError())
        //        return Json(new { responseError = DbResHtml.T(allClasses.ErrMsg!, resourceSet: "Resources") });

        //    var childrenClassesList = allClasses.Data.Where(x => x.ParentClassId == model.Id).ToList();
        //    var all_dataset_datasetClasses_for_class = await _datasetDatasetClassService.GetDataset_DatasetClassByClassId(model.Id) ?? throw new Exception("Object not found");
        //    var selectDatasetsId = all_dataset_datasetClasses_for_class.Data.Select(x => x.DatasetId).ToList() ?? throw new Exception("Object not found");
        //    var datasets = await _datasetSerivce.GetAllDatasets();
        //    if (datasets.IsSuccess == false && datasets.HandleError())
        //        return Json(new { responseError = DbResHtml.T(datasets.ErrMsg!, resourceSet: "Resources") });
        //    if (datasets.Data.Any() == false)
        //        return Json(new { responseError = DbResHtml.T("No datasets found", resourceSet: "Resources") });


        //    var datasetsWhereClassIsUsed = datasets.Data.Where(x => selectDatasetsId.Contains(x.Id)).ToList() ?? throw new Exception("Object not found");


        //    if (isUpdated.IsSuccess == true && isUpdated.Data == 1 && string.IsNullOrEmpty(isUpdated.ErrMsg))
        //    {
        //        return Json(new { responseSuccess = DbResHtml.T("Successfully updated dataset class", "Resources") });
        //    }
        //    if (!string.IsNullOrEmpty(isUpdated.ErrMsg))
        //    {
        //        if (isUpdated.Data == 2)
        //        {
        //            return Json(new { responseError = DbResHtml.T(isUpdated.ErrMsg, "Resources"), childrenClassesList = childrenClassesList });
        //        }
        //        if (isUpdated.Data == 3)
        //        {
        //            return Json(new { responseError = DbResHtml.T(isUpdated.ErrMsg, "Resources"), datasetsWhereClassIsUsed = datasetsWhereClassIsUsed });
        //        }
        //        if (isUpdated.Data == 4)
        //        {
        //            return Json(new { responseError = DbResHtml.T(isUpdated.ErrMsg, "Resources") });
        //        }
        //        if (isUpdated.Data == 5)
        //        {
        //            return Json(new { responseError = DbResHtml.T(isUpdated.ErrMsg, "Resources") });
        //        }
        //    }
        //    return Json(new { responseError = DbResHtml.T("Dataset class was not updated", "Resources") });
        //}

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditDatasetClass))]
        public async Task<IActionResult> EditClass(EditDatasetClassDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { responseError = DbResHtml.T("Model is not valid", "Resources") });
                }

                var result = await _datasetClassesService.EditDatasetClass(model);

                if (result.IsSuccess == false)
                {
                    var allClasses = await _datasetClassesService.GetAllDatasetClasses();
                    if (allClasses.IsSuccess == false && allClasses.HandleError())
                    {
                        return Json(new { responseError = DbResHtml.T(allClasses.ErrMsg!, resourceSet: "Resources") });
                    }
                    var childrenClassesList = allClasses.Data.Where(x => x.ParentClassId == model.Id).ToList();

                    var allDatasetDatasetClasses = await _datasetDatasetClassService.GetDataset_DatasetClassByClassId(model.Id);
                    var selectDatasetIds = allDatasetDatasetClasses?.Data?.Select(x => x.DatasetId).ToList();

                    var datasets = await _datasetSerivce.GetAllDatasets();
                    if (datasets.IsSuccess == false && datasets.HandleError())
                    {
                        return Json(new { responseError = DbResHtml.T(datasets.ErrMsg!, resourceSet: "Resources") });
                    }

                    var datasetsWhereClassIsUsed = datasets.Data
                        .Where(x => selectDatasetIds != null && selectDatasetIds.Contains(x.Id))
                        .ToList();

                    return result.Data switch
                    {
                        2 => Json(new { responseError = DbResHtml.T(result.ErrMsg, "Resources"), childrenClassesList }),
                        3 => Json(new { responseError = DbResHtml.T(result.ErrMsg, "Resources"), datasetsWhereClassIsUsed }),
                        4 => Json(new { responseError = DbResHtml.T(result.ErrMsg, "Resources") }),
                        5 => Json(new { responseError = DbResHtml.T(result.ErrMsg, "Resources") }),
                        _ => Json(new { responseError = DbResHtml.T("Dataset class was not updated", "Resources") })
                    };
                }

                return Json(new { responseSuccess = DbResHtml.T("Successfully updated dataset class", "Resources") });
            }
            catch (Exception ex)
            {
                return Json(new { responseError = DbResHtml.T("An unexpected error occurred. Please try again later.", "Resources") });
            }
        }

        [HttpPost]
        public async Task<DatasetClassDTO> GetClassById(Guid classId)
        {
            try
            {
                if (classId == Guid.Empty)
                {
                    return new DatasetClassDTO();
                }

                var resultGetEntity = await _datasetClassesService.GetDatasetClassById(classId);

                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                {
                    return new DatasetClassDTO();
                }

                return resultGetEntity.Data;
            }
            catch (Exception ex)
            {
                // Check this 
                return new DatasetClassDTO();
            }
        }


        //[HttpPost]
        //[HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetClass))]
        //public async Task<IActionResult> DeleteClass(Guid classId)
        //{
        //    var isDeleted = await _datasetClassesService.DeleteDatasetClass(classId);

        //    if (isDeleted.IsSuccess == false && isDeleted.HandleError())
        //    {
        //        switch (isDeleted.Data)
        //        {
        //            case 2: // Subclasses exist
        //                var allClasses = await _datasetClassesService.GetAllDatasetClasses();
        //                if (allClasses.IsSuccess)
        //                {
        //                    var childrenClassesList = allClasses.Data
        //                        .Where(x => x.ParentClassId == classId)
        //                        .ToList();
        //                    return Json(new
        //                    {
        //                        responseError = DbResHtml.T(isDeleted.ErrMsg, "Resources"),
        //                        childrenClassesList
        //                    });
        //                }
        //                else if (allClasses.IsSuccess == false && allClasses.HandleError())
        //                {
        //                    return Json(new
        //                    {
        //                        responseError = DbResHtml.T(allClasses.ErrMsg, "Resources")
        //                    });
        //                }
        //                return Json(new
        //                {
        //                    responseError = DbResHtml.T("Failed to retrieve child classes.", "Resources")
        //                });

        //            case 3: // Class in use
        //                var allDatasetDatasetClasses = await _datasetDatasetClassService.GetDataset_DatasetClassByClassId(classId);
        //                if (allDatasetDatasetClasses.IsSuccess)
        //                {
        //                    var selectDatasetsId = allDatasetDatasetClasses.Data.Select(x => x.DatasetId).ToList();
        //                    var datasets = await _datasetSerivce.GetAllDatasets();
        //                    if (datasets.IsSuccess)
        //                    {
        //                        var datasetsWhereClassIsUsed = datasets.Data
        //                            .Where(x => selectDatasetsId.Contains(x.Id))
        //                            .ToList();
        //                        return Json(new
        //                        {
        //                            responseError = DbResHtml.T(isDeleted.ErrMsg, "Resources"),
        //                            datasetsWhereClassIsUsed
        //                        });
        //                    }
        //                    else if (datasets.IsSuccess == false && datasets.HandleError())
        //                    {
        //                        return Json(new
        //                        {
        //                            responseError = DbResHtml.T(datasets.ErrMsg, "Resources")
        //                        });
        //                    }
        //                    return Json(new
        //                    {
        //                        responseError = DbResHtml.T("No datasets found where this class is used.", "Resources")
        //                    });
        //                }
        //                else if (allDatasetDatasetClasses.IsSuccess == false && allDatasetDatasetClasses.HandleError())
        //                {
        //                    return Json(new
        //                    {
        //                        responseError = DbResHtml.T(allDatasetDatasetClasses.ErrMsg, "Resources")
        //                    });
        //                }
        //                return Json(new
        //                {
        //                    responseError = DbResHtml.T("Failed to retrieve dataset relationships.", "Resources")
        //                });

        //            case 4: // General deletion error
        //                return Json(new
        //                {
        //                    responseError = DbResHtml.T(isDeleted.ErrMsg, "Resources")
        //                });

        //            default: // Other errors
        //                return Json(new
        //                {
        //                    responseError = DbResHtml.T("Dataset class was not deleted due to another error.", "Resources")
        //                });
        //        }
        //    }
        //    If deletion succeeds
        //    return Json(new
        //    {
        //        responseSuccess = DbResHtml.T("Successfully deleted dataset class", "Resources")
        //    });
        //}

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetClass))]
        public async Task<IActionResult> DeleteClass(Guid classId)
        {
            try
            {
                var isDeleted = await _datasetClassesService.DeleteDatasetClass(classId);

                // Check if deletion was successful
                if (isDeleted.IsSuccess)
                {
                    return Json(new
                    {
                        responseSuccess = DbResHtml.T("Successfully deleted dataset class", "Resources")
                    });
                }

                if (isDeleted.IsSuccess == false && isDeleted.HandleError())
                {
                    return isDeleted.Data switch
                    {
                        2 => await HandleSubclassDeletion(classId),
                        3 => await HandleClassInUse(classId),
                        4 => CreateErrorResponse(isDeleted.ErrMsg),
                        _ => CreateErrorResponse("Dataset class was not deleted due to another error.")
                    };
                }

                return CreateErrorResponse("Dataset class was not deleted due to an unexpected error.");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse($"An error occurred while deleting the class: {ex.Message}");
            }
        }

        private async Task<IActionResult> HandleSubclassDeletion(Guid classId)
        {
            try
            {
                var allClasses = await _datasetClassesService.GetAllDatasetClasses();

                if (allClasses.IsSuccess == false)
                {
                    return HandleAllClassesError(allClasses);
                }

                var childrenClassesList = allClasses.Data
                    .Where(x => x.ParentClassId == classId)
                    .ToList();

                return Json(new
                {
                    responseError = DbResHtml.T("Subclasses for this class exist. Please delete them first.", "Resources"),
                    childrenClassesList
                });
            }
            catch (Exception ex)
            {
                return CreateErrorResponse($"An error occurred while handling subclass deletion: {ex.Message}");
            }
        }

        private async Task<IActionResult> HandleClassInUse(Guid classId)
        {
            try
            {
                var allDatasetDatasetClasses = await _datasetDatasetClassService.GetDataset_DatasetClassByClassId(classId);

                if (allDatasetDatasetClasses.IsSuccess == false)
                {
                    return HandleAllDatasetRelationshipsError(allDatasetDatasetClasses);
                }

                var selectDatasetsId = allDatasetDatasetClasses.Data.Select(x => x.DatasetId).ToList();
                var datasets = await _datasetSerivce.GetAllDatasets();

                if (datasets.IsSuccess == false)
                {
                    return HandleAllDatasetsError(datasets);
                }

                var datasetsWhereClassIsUsed = datasets.Data
                    .Where(x => selectDatasetsId.Contains(x.Id))
                    .ToList();

                return Json(new
                {
                    responseError = DbResHtml.T("Class is currently in use in datasets.", "Resources"),
                    datasetsWhereClassIsUsed
                });
            }
            catch (Exception ex)
            {
                return CreateErrorResponse($"An error occurred while handling class in use: {ex.Message}");
            }
        }

        private IActionResult CreateErrorResponse(string message)
        {
            return Json(new
            {
                responseError = DbResHtml.T(message, "Resources")
            });
        }

        private IActionResult HandleAllClassesError(ResultDTO<List<DatasetClassDTO>> allClasses)
        {
            try
            {
                return allClasses.HandleError()
                    ? CreateErrorResponse(allClasses.ErrMsg)
                    : CreateErrorResponse("Failed to retrieve child classes.");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse($"An error occurred while handling all classes: {ex.Message}");
            }
        }

        private IActionResult HandleAllDatasetsError(ResultDTO<List<DatasetDTO>> datasets)
        {
            try
            {
                return datasets.HandleError()
                    ? CreateErrorResponse(datasets.ErrMsg)
                    : CreateErrorResponse("No datasets found where this class is used.");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse($"An error occurred while handling all datasets: {ex.Message}");
            }
        }

        private IActionResult HandleAllDatasetRelationshipsError(ResultDTO<List<Dataset_DatasetClassDTO>> allDatasetDatasetClasses)
        {
            try
            {
                return allDatasetDatasetClasses.HandleError()
                    ? CreateErrorResponse(allDatasetDatasetClasses.ErrMsg)
                    : CreateErrorResponse("Failed to retrieve dataset relationships.");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse($"An error occurred while handling dataset relationships: {ex.Message}");
            }
        }

    }
}
