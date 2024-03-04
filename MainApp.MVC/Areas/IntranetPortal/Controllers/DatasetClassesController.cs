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


        public async Task<IActionResult> Index()
        {
            var allClassesList = await _datasetClassesService.GetAllDatasetClasses() ?? throw new Exception("Object not found");
            var model = _mapper.Map<List<DatasetClassViewModel>>(allClassesList);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass(CreateDatasetClassDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { responseError = DbResHtml.T("Model is not valid", "Resources") });
            }

            var isAdded = await _datasetClassesService.AddDatasetClass(model);
            if (isAdded == 1)
            {
                return Json(new { responseSuccess = DbResHtml.T("Successfully added dataset class", "Resources") });
            }
            else if(isAdded == 2)
            {
                return Json(new { responseError = DbResHtml.T("You can not add this class as a subclass becuse the selected parent class is already set as subclass!", "Resources")});
            }
            else
            {
                return Json(new { responseError = DbResHtml.T("Dataset class was not added", "Resources") });
            }
        }

        [HttpPost]
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

            if (isUpdated == 1)
            {
                return Json(new { responseSuccess = DbResHtml.T("Successfully updated dataset class", "Resources") });
            }
            else if(isUpdated == 2)
            {
                return Json(new { responseError = DbResHtml.T("This dataset class has subclasses and can not be set as a subclass too!", "Resources"), childrenClassesList = childrenClassesList });
            }
            else if (isUpdated == 3)
            {
                return Json(new { responseError = DbResHtml.T("This dataset is already in use in dataset/s, you can only change the class name!", "Resources"), datasetsWhereClassIsUsed = datasetsWhereClassIsUsed });
            }
            else if (isUpdated == 4)
            {
                return Json(new { responseError = DbResHtml.T("You can not add this class as a subclass becuse the selected parent class is already set as subclass!", "Resources") });
            }
            else
            {
                return Json(new { responseError = DbResHtml.T("Dataset class was not updated", "Resources") });
            }
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
        public async Task<IActionResult> DeleteClass(Guid classId)
        {            
            var isDeleted = await _datasetClassesService.DeleteDatasetClass(classId);
            var allClasses = await _datasetClassesService.GetAllDatasetClasses() ?? throw new Exception("Object not found");
            var childrenClassesList = allClasses.Where(x => x.ParentClassId == classId).ToList() ?? throw new Exception("Object not found");

            var all_dataset_datasetClasses_for_class = await _datasetDatasetClassService.GetDataset_DatasetClassByClassId(classId) ?? throw new Exception("Object not found");
            var selectDatasetsId = all_dataset_datasetClasses_for_class.Select(x => x.DatasetId).ToList() ?? throw new Exception("Object not found");
            var datasets = await _datasetSerivce.GetAllDatasets() ?? throw new Exception("Object not found");
            var datasetsWhereClassIsUsed = datasets.Where(x => selectDatasetsId.Contains(x.Id)).ToList() ?? throw new Exception("Object not found");
            if (isDeleted == 1)
            {
                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset class", "Resources") });
            }
            else if (isDeleted == 2)
            {
                return Json(new { responseError = DbResHtml.T("This dataset class can not be deleted because there are subclasses. Delete first the subclasses!", "Resources"), childrenClassesList = childrenClassesList });
            }
            else if(isDeleted == 3)
            {
                return Json(new { responseError = DbResHtml.T("This dataset class can not be deleted because this class is already in use in dataset/s!", "Resources"), datasetsWhereClassIsUsed = datasetsWhereClassIsUsed });
            }
            else
            {
                return Json(new { responseError = DbResHtml.T("Dataset class was not deleted", "Resources") });
            }
        }

    }
}
