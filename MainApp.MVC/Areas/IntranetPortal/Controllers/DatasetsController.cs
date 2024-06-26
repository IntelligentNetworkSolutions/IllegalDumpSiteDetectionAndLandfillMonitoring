﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.ApplicationStorage;
using Entities.DatasetEntities;
using DAL.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Services.Interfaces.Services;
using AutoMapper;
using DAL.Interfaces.Helpers;
using MainApp.MVC.Helpers;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using DocumentFormat.OpenXml.Spreadsheet;
using DTOs.MainApp.BL;
using Services;
using DTOs.MainApp.BL.DatasetDTOs;
using Humanizer;
using System.Drawing;
using Microsoft.Extensions.Options;
using System.Data;
using NuGet.Packaging;
using DocumentFormat.OpenXml.Drawing.Charts;
using Westwind.Globalization;
using DAL.Helpers;
using System.Text;
using X.PagedList;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml.Wordprocessing;
using MainApp.MVC.Filters;
using System.Security.Claims;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DatasetsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IDatasetService _datasetService;
        private readonly IDatasetImagesService _datasetImagesService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAppSettingsAccessor _appSettingsAccessor;

        public DatasetsController(IConfiguration configuration,
                                  IMapper mapper, 
                                  IDatasetService datasetService, 
                                  IDatasetImagesService datasetImagesService, 
                                  IWebHostEnvironment webHostEnvironment,
                                  IAppSettingsAccessor appSettingsAccessor)
        {
            _configuration = configuration;
            _mapper = mapper;
            _datasetService = datasetService;
            _datasetImagesService = datasetImagesService;
            _webHostEnvironment = webHostEnvironment;
            _appSettingsAccessor = appSettingsAccessor;
        }

        [HasAuthClaim(nameof(SD.AuthClaims.ViewDatasets))]
        public async Task<IActionResult> Index()
        {
            var datasetsList = await _datasetService.GetAllDatasets();
            var model = _mapper.Map<List<DatasetViewModel>>(datasetsList) ?? throw new Exception("Model not found");

            List<DatasetViewModel> parentDatasetsList = model.Where(d => d.ParentDatasetId == null).OrderBy(x => x.Name).ToList();
            //string optionsHtml = GenerateDatasetOptionsHtml(parentDatasetsList, model,"");
            //ViewBag.OptionsHtml = optionsHtml;

            return View(model);
        }

        public async Task<List<DatasetDTO>> GetAllDatasets()
        {
            var datasets =  await _datasetService.GetAllDatasets() ?? throw new Exception("Object not found");
            return datasets;
        }
        [HttpPost]
        public async Task<IActionResult> GetParentAndChildrenDatasets(Guid currentDatasetId)
        {
            if (currentDatasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }
            var allDatasetsDb = await _datasetService.GetAllDatasets() ?? throw new Exception("Object not found");
            var currentDatasetDb = await _datasetService.GetDatasetById(currentDatasetId) ?? throw new Exception("Object not found");
            if (currentDatasetDb == null)
            {
                return Json(new { responseError = DbResHtml.T("Invalid current dataset", "Resources") });
            }
            var childrenDatasets = allDatasetsDb.Where(x => x.ParentDatasetId == currentDatasetId).ToList() ?? throw new Exception("Object not found");
            var parentDataset = currentDatasetDb.ParentDataset;
            if(parentDataset == null)
            {
                DatasetDTO datasetDTO = new();
                return Json(new {parent = datasetDTO, childrenList = childrenDatasets, currentDataset = currentDatasetDb});
            }

            return Json(new { parent = parentDataset, childrenList = childrenDatasets, currentDataset = currentDatasetDb });
        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDataset))]
        public async Task<IActionResult> CreateConfirmed(CreateDatasetViewModel datasetViewModel)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { responseError = DbResHtml.T("Dataset model is not valid", "Resources") });
            }
            DatasetDTO selectedParentDataset;
            if(datasetViewModel.ParentDatasetId != null)
            {
                selectedParentDataset = await _datasetService.GetDatasetById(datasetViewModel.ParentDatasetId.Value);
                if(selectedParentDataset.IsPublished == false)
                {
                    return Json(new { responseError = DbResHtml.T("Selected parent dataset is not published. You can not add subdataset for unpublished datasets!", "Resources") });
                }
            }

            datasetViewModel.CreatedById = userId;
            DatasetDTO dataSetDto = _mapper.Map<DatasetDTO>(datasetViewModel) ?? throw new Exception("Model not found");            
            var insertedDatasetDTO = await _datasetService.CreateDataset(dataSetDto) ?? throw new Exception("Model not found");
            if(datasetViewModel.ParentDatasetId != null)
            {
                await _datasetService.AddInheritedParentClasses(insertedDatasetDTO.Id, datasetViewModel.ParentDatasetId.Value);
            }
           
            return Json(new { id = insertedDatasetDTO.Id });
        }

        [HasAuthClaim(nameof(SD.AuthClaims.ManageDataset))]
        public async Task<IActionResult> Edit(Guid datasetId, int? page, string? SearchByImageName, bool? SearchByIsAnnotatedImage, bool? SearchByIsEnabledImage, int? SearchByShowNumberOfImages, string? OrderByImages)
        {
            if (datasetId == Guid.Empty)
            {
                return NotFound();
            }
            var datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Object not found");
            if (datasetDb.IsPublished == true)
            {
                TempData["ErrorDatasetIsPublished"] = DbRes.T("This dataset is published and cannot be edited.", "Resources");
                return RedirectToAction(nameof(Index));
            }

            var pageSize = SearchByShowNumberOfImages ?? 20;
            var pageNumber = page ?? 1;

            var dto = await _datasetService.GetObjectForEditDataset(datasetId, SearchByImageName, SearchByIsAnnotatedImage, SearchByIsEnabledImage, OrderByImages);
            var pagedImagesList = dto.ListOfDatasetImages.ToPagedList(pageNumber, pageSize);
            var model = _mapper.Map<EditDatasetViewModel>(dto);
            model.PagedImagesList = pagedImagesList;
            model.SearchByImageName = SearchByImageName;
            model.SearchByIsAnnotatedImage = SearchByIsAnnotatedImage;
            model.SearchByIsEnabledImage = SearchByIsEnabledImage;
            model.SearchByShowNumberOfImages = SearchByShowNumberOfImages;
            model.OrderByImages = OrderByImages;
            return View(model);
        }


        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.PublishDataset))]
        public async Task<IActionResult> PublishDataset(Guid datasetId)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });
            }

            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Incorect dataset id", "Resources") });
            }
            var datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Dataset not found");
            var nubmerOfImagesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfImagesNeededToPublishDataset", 100);
            var nubmerOfClassesNeededToPublishDataset = await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("NumberOfClassesNeededToPublishDataset", 1);
            if(datasetDb.IsPublished == true)
            {
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });
            }

            var isPublished = await _datasetService.PublishDataset(datasetId, userId);
            if(isPublished.IsSuccess == true && isPublished.Data == 1 && string.IsNullOrEmpty(isPublished.ErrMsg))
            {
                return Json(new {responseSuccess = DbResHtml.T("Successfully published dataset","Resources")});
            }

            if(isPublished.IsSuccess == false && isPublished.Data == 2)
            {
                return Json(new { responseError = DbResHtml.T($"Insert at least {nubmerOfImagesNeededToPublishDataset.Data} images, {nubmerOfClassesNeededToPublishDataset.Data} class/es and annotate all enabled images to publish the dataset", "Resources") });
            }
            else
            {
                return Json(new {responseError = DbResHtml.T("Dataset was not published", "Resources")});
            }            
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AddDatasetClass))]
        public async Task<IActionResult> AddDatasetClass(Guid selectedClassId, Guid datasetId)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });
            }

            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }
            if (selectedClassId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset class id", "Resources") });
            }

            var datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Object not found");
            if (datasetDb.IsPublished == true)
            {
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });
            }

            var isClassAdded = await _datasetService.AddDatasetClassForDataset(selectedClassId,datasetId, userId);
            if (isClassAdded.IsSuccess == true && isClassAdded.Data == 1 && string.IsNullOrEmpty(isClassAdded.ErrMsg))
            {
                return Json(new { responseSuccess = DbResHtml.T("Successfully added dataset class", "Resources") });
            }
            else
            {
                if (!string.IsNullOrEmpty(isClassAdded.ErrMsg))
                {
                    return Json(new { responseError = DbResHtml.T(isClassAdded.ErrMsg, "Resources") });
                }
                else
                {
                    return Json(new { responseError = DbResHtml.T("Dataset class was not added", "Resources") });
                }
               
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetClass))]
        public async Task<IActionResult> DeleteDatasetClass(Guid datasetClassId, Guid datasetId)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });
            }

            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }
            if (datasetClassId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset class id", "Resources") });
            }

            var datasetDb = await _datasetService.GetDatasetById(datasetId) ?? throw new Exception("Object not found");
            if (datasetDb.IsPublished == true)
            {
                return Json(new { responseErrorAlreadyPublished = DbResHtml.T("Dataset is already published. No changes allowed", "Resources") });
            }

            var isClassDeleted = await _datasetService.DeleteDatasetClassForDataset(datasetClassId, datasetId, userId);
            if (isClassDeleted.IsSuccess == true && isClassDeleted.Data == 1 && string.IsNullOrEmpty(isClassDeleted.ErrMsg))
            {
                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset class", "Resources") });
            }
            else
            {
                if (!string.IsNullOrEmpty(isClassDeleted.ErrMsg))
                {
                    return Json(new { responseError = DbResHtml.T(isClassDeleted.ErrMsg, "Resources") });
                }
                else
                {
                    return Json(new { responseError = DbResHtml.T("Some error occured", "Resources") });
                }
                
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.DeleteDatasetClass))]
        public async Task<IActionResult> DeleteDatasetConfirmed(Guid datasetId)
        {
            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }

            var isDatasetDeleted = await _datasetService.DeleteDataset(datasetId);
            var listOfAllDatasets = await _datasetService.GetAllDatasets() ?? throw new Exception("Datasets not found");
            var childrenDatasetsList = listOfAllDatasets.Where(x => x.ParentDatasetId == datasetId).ToList() ?? throw new Exception("Object not found");
            if (isDatasetDeleted.IsSuccess ==  true && isDatasetDeleted.Data == 1 && string.IsNullOrEmpty(isDatasetDeleted.ErrMsg))
            {
                var datasetImagesFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("DatasetImagesFolder", "DatasetImages");
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, datasetImagesFolder.Data, datasetId.ToString());
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
                return Json(new { responseSuccess = DbResHtml.T("Successfully deleted dataset", "Resources") });
            }
            else
            {
                if (!string.IsNullOrEmpty(isDatasetDeleted.ErrMsg))
                {
                    return Json(new { responseError = DbResHtml.T(isDatasetDeleted.ErrMsg, "Resources") });
                }
                else
                {
                    return Json(new { responseError = DbResHtml.T("Some error occured", "Resources") });
                }
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.ChooseDatasetClassType))]
        public async Task<IActionResult> ChooseDatasetClassType(Guid datasetId, bool annotationsPerSubclass)
        {
            string? userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { responseError = DbResHtml.T("User id is not valid", "Resources") });
            }

            if (datasetId == Guid.Empty)
            {
                return Json(new { responseError = DbResHtml.T("Invalid dataset id", "Resources") });
            }

            var isSetAnnontationsPerSubclass = await _datasetService.SetAnnotationsPerSubclass(datasetId, annotationsPerSubclass, userId);
            if (isSetAnnontationsPerSubclass.IsSuccess == true && isSetAnnontationsPerSubclass.Data == 1 && string.IsNullOrEmpty(isSetAnnontationsPerSubclass.ErrMsg))
            {
                return Json(new { responseSuccess = DbResHtml.T("Now you can add classes for this dataset", "Resources") });
            }           
            else
            {
                if (!string.IsNullOrEmpty(isSetAnnontationsPerSubclass.ErrMsg))
                {
                    return Json(new { responseError = DbResHtml.T(isSetAnnontationsPerSubclass.ErrMsg, "Resources") });
                }
                else
                {
                    return Json(new { responseError = DbResHtml.T("Choosed option was not saved", "Resources") });
                }
                
            }
        }


        //private string GetOptionDisplayName(DatasetViewModel item, string prefix)
        //{
        //    return $"{prefix}{item.Name}<br>";
        //}

        //private string GenerateDatasetOptionsHtml(List<DatasetViewModel> parentDatasetsList, List<DatasetViewModel> allDatasets,string prefix)
        //{
        //    StringBuilder options = new StringBuilder();
        //    int count = 1;

        //    foreach (var dataset in parentDatasetsList)
        //    {
        //        string optionPrefix = $"{prefix}{count}. ";
        //        options.AppendLine($"<option value=\"{dataset.Id}\">{GetOptionDisplayName(dataset, optionPrefix)}</option>");

        //        var children = allDatasets.Where(d => d.ParentDatasetId == dataset.Id).ToList();
        //        if (children.Any())
        //        {
        //            options.AppendLine(GenerateDatasetOptionsHtml(children, allDatasets, $"{optionPrefix}"));
        //        }
        //        count++;
        //    }

        //    return options.ToString();
        //}


    }
}
