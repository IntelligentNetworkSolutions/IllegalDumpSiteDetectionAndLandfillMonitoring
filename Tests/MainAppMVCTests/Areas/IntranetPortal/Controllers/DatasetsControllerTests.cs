using AutoMapper;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services.DatasetServices;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class DatasetsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfDatasets()
        {
            // Arrange
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<List<DatasetViewModel>>(It.IsAny<List<DatasetDTO>>()))
                .Returns(new List<DatasetViewModel>());

            var mockDatasetService = new Mock<IDatasetService>();
            mockDatasetService.Setup(service => service.GetAllDatasets())
                .ReturnsAsync(new List<DatasetDTO>());

            var controller = new DatasetsController(null, mockMapper.Object, mockDatasetService.Object, null, null, null);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<DatasetViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task GetAllDatasets_ReturnsListOfDatasets()
        {
            // Arrange
            var expectedDatasets = new List<DatasetDTO>
            {
                new DatasetDTO { Id = Guid.NewGuid(), Name = "Dataset 1" },
                new DatasetDTO { Id = Guid.NewGuid(), Name = "Dataset 2" }
            };

            var mockDatasetService = new Mock<IDatasetService>();
            mockDatasetService.Setup(service => service.GetAllDatasets()).ReturnsAsync(expectedDatasets);
            var controller = new DatasetsController(null, null, mockDatasetService.Object, null, null, null);

            // Act
            var result = await controller.GetAllDatasets();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<DatasetDTO>>(result);
            Assert.Equal(expectedDatasets.Count, result.Count);
        }

    }
}
