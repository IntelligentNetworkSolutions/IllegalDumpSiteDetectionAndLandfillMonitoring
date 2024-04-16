using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.ApplicationStorage;
using Entities.DatasetEntities;
using AutoMapper;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Services.DatasetServices;
using MainApp.MVC.Helpers;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using MainApp.MVC.Filters;
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
            var allClassesList = await _datasetClassesService.GetAllDatasetClasses() ?? throw new Exception("Object not found");
            var model = _mapper.Map<List<DatasetClassViewModel>>(allClassesList);
            return View(model);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDatasetClass))]
        public async Task<IActionResult> CreateClass(CreateDatasetClassDTO model)
        {
            string? userId = User.FindFirstValue("UserId");
            if(string.IsNullOrEmpty(userId))
            {
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });
            }            

            if (!ModelState.IsValid)
            {
                return Json(new { responseError = DbResHtml.T("Model is not valid", "Resources") });
            }

            model.CreatedById = userId;
            var isAdded = await _datasetClassesService.AddDatasetClass(model);
            if (isAdded.IsSuccess == true && isAdded.Data == 1 && string.IsNullOrEmpty(isAdded.ErrMsg))
            {
                return Json(new { responseSuccess = DbResHtml.T("Successfully added dataset class", "Resources") });
            }
            if(!string.IsNullOrEmpty(isAdded.ErrMsg))
            {
                return Json(new { responseError = DbResHtml.T(isAdded.ErrMsg, "Resources") });
            }

            return Json(new { responseError = DbResHtml.T("Dataset class was not added", "Resources") });            
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.EditDatasetClass))]
        public async Task<IActionResult> EditClass(EditDatasetClassDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { responseError = DbResHtml.T("Model is not valid", "Resources") });
            }

            var isUpdated = await _datasetClassesService.EditDatasetClass(model);
            var allClasses = await _datasetClassesService.GetAllDatasetClasses() ?? throw new Exception("Object not found");
            var childrenClassesList = allClasses.Where(x => x.ParentClassId == model.Id).ToList() ?? throw new Exception("Object not found");

            var all_dataset_datasetClasses_for_class = await _datasetDatasetClassService.GetDataset_DatasetClassByClassId(model.Id) ?? throw new Exception("Object not found");
            var selectDatasetsId = all_dataset_datasetClasses_for_class.Select(x => x.DatasetId).ToList() ?? throw new Exception("Object not found");
            var datasets = await _datasetSerivce.GetAllDatasets() ?? throw new Exception("Object not found");
            var datasetsWhereClassIsUsed = datasets.Where(x => selectDatasetsId.Contains(x.Id)).ToList() ?? throw new Exception("Object not found");


            if(isUpdated.IsSuccess == true && isUpdated.Data == 1 && string.IsNullOrEmpty(isUpdated.ErrMsg))
            {
                return Json(new { responseSuccess = DbResHtml.T("Successfully updated dataset class", "Resources") });
            }
            if(!string.IsNullOrEmpty(isUpdated.ErrMsg))
            {
                if (isUpdated.Data == 2)
                {
                    return Json(new { responseError = DbResHtml.T(isUpdated.ErrMsg, "Resources"), childrenClassesList = childrenClassesList });
                }
                if (isUpdated.Data == 3)
                {
                    return Json(new { responseError = DbResHtml.T(isUpdated.ErrMsg, "Resources"), datasetsWhereClassIsUsed = datasetsWhereClassIsUsed });
                }
                if (isUpdated.Data == 4)
                {
                    return Json(new { responseError = DbResHtml.T(isUpdated.ErrMsg, "Resources") });
                }
                if (isUpdated.Data == 5)
                {
                    return Json(new { responseError = DbResHtml.T(isUpdated.ErrMsg, "Resources") });
                }
            }
            return Json(new { responseError = DbResHtml.T("Dataset class was not updated", "Resources") });   
        }

        [HttpPost]
        public async Task<DatasetClassDTO> GetClassById(Guid classId)
        {
            if(classId == Guid.Empty)
            {
                DatasetClassDTO datasetClassDto = new();
                return datasetClassDto;
            }
            var datasetClass = await _datasetClassesService.GetDatasetClassById(classId) ?? throw new Exception("Object not found");
            return datasetClass;
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetClass))]
        public async Task<IActionResult> DeleteClass(Guid classId)
        {
            var isDeleted = await _datasetClassesService.DeleteDatasetClass(classId);
            var allClasses = await _datasetClassesService.GetAllDatasetClasses() ?? throw new Exception("Object not found");
            var childrenClassesList = allClasses.Where(x => x.ParentClassId == classId).ToList() ?? throw new Exception("Object not found");

            var all_dataset_datasetClasses_for_class = await _datasetDatasetClassService.GetDataset_DatasetClassByClassId(classId) ?? throw new Exception("Object not found");
            var selectDatasetsId = all_dataset_datasetClasses_for_class.Select(x => x.DatasetId).ToList() ?? throw new Exception("Object not found");
            var datasets = await _datasetSerivce.GetAllDatasets() ?? throw new Exception("Object not found");
            var datasetsWhereClassIsUsed = datasets.Where(x => selectDatasetsId.Contains(x.Id)).ToList() ?? throw new Exception("Object not found");

            if (isDeleted.IsSuccess == true && isDeleted.Data == 1 && string.IsNullOrEmpty(isDeleted.ErrMsg))
            {
                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset class", "Resources") });
            }
            if (!string.IsNullOrEmpty(isDeleted.ErrMsg))
            {
                if (isDeleted.Data == 2)
                {
                    return Json(new { responseError = DbResHtml.T(isDeleted.ErrMsg, "Resources"), childrenClassesList = childrenClassesList });
                }
                if (isDeleted.Data == 3)
                {
                    return Json(new { responseError = DbResHtml.T(isDeleted.ErrMsg, "Resources"), datasetsWhereClassIsUsed = datasetsWhereClassIsUsed });
                }
                if (isDeleted.Data == 4)
                {
                    return Json(new { responseError = DbResHtml.T(isDeleted.ErrMsg, "Resources") });
                }
            }
            return Json(new { responseError = DbResHtml.T("Dataset class was not deleted", "Resources") });
        }

    }
}
