using AutoMapper;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainApp.MVC.Mappers;

namespace Tests.MainAppMVCTests.Mappers
{
    public class DatasetProfileTests
    {
        private readonly IMapper _mapper;

        public DatasetProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DatasetProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void DatasetDTO_To_DatasetViewModel_Should_Map_Properties()
        {
            var dto = new DatasetDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Dataset",
                Description = "Test Description",
                IsPublished = true,
                CreatedById = "User1",
                CreatedOn = DateTime.UtcNow,
                UpdatedById = "User2",
                UpdatedOn = DateTime.UtcNow,
                AnnotationsPerSubclass = true
            };

            var viewModel = _mapper.Map<DatasetViewModel>(dto);

            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.Name, viewModel.Name);
            Assert.Equal(dto.Description, viewModel.Description);
            Assert.Equal(dto.IsPublished, viewModel.IsPublished);
            Assert.Equal(dto.CreatedById, viewModel.CreatedById);
            Assert.Equal(dto.CreatedOn, viewModel.CreatedOn);
            Assert.Equal(dto.UpdatedById, viewModel.UpdatedById);
            Assert.Equal(dto.UpdatedOn, viewModel.UpdatedOn);
            Assert.Equal(dto.AnnotationsPerSubclass, viewModel.AnnotationsPerSubclass);
        }

        [Fact]
        public void CreateDatasetDTO_To_CreateDatasetViewModel_Should_Map_Properties()
        {
            var dto = new CreateDatasetDTO
            {
                Id = Guid.NewGuid(),
                Name = "New Dataset",
                Description = "New Description",
                IsPublished = false,
                CreatedById = "CreatorId",
                CreatedOn = DateTime.UtcNow,
                AllDatasetClasses = new List<DatasetClassDTO>
            {
                new DatasetClassDTO { Id = Guid.NewGuid(), ClassName = "Class 1" }
            },
                InsertedDatasetClasses = new List<Guid> { Guid.NewGuid() }
            };

            var viewModel = _mapper.Map<CreateDatasetViewModel>(dto);

            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.Name, viewModel.Name);
            Assert.Equal(dto.Description, viewModel.Description);
            Assert.Equal(dto.IsPublished, viewModel.IsPublished);
            Assert.Equal(dto.CreatedById, viewModel.CreatedById);
            Assert.Equal(dto.CreatedOn, viewModel.CreatedOn);
            Assert.Equal(dto.AllDatasetClasses, viewModel.AllDatasetClasses);
            Assert.Equal(dto.InsertedDatasetClasses, viewModel.InsertedDatasetClasses);
        }

        [Fact]
        public void EditDatasetDTO_To_EditDatasetViewModel_Should_Map_Properties()
        {
            var editDatasetDTO = new EditDatasetDTO
            {
                UninsertedDatasetRootClasses = new List<DatasetClassDTO> { new DatasetClassDTO { Id = Guid.NewGuid(), ClassName = "Class A" } },
                UninsertedDatasetSubclasses = new List<DatasetClassDTO> { new DatasetClassDTO { Id = Guid.NewGuid(), ClassName = "Class B" } },
                ClassesByDatasetId = new List<DatasetClassDTO> { new DatasetClassDTO { Id = Guid.NewGuid(), ClassName = "Class C" } },
                ListOfDatasetImages = new List<DatasetImageDTO> { new DatasetImageDTO { Id = Guid.NewGuid(), FileName = "Image1.png" } },
                NumberOfDatasetClasses = 5,
                NumberOfChildrenDatasets = 3,
                InsertedDatasetClass = Guid.NewGuid(),
                NumberOfDatasetImages = 10,
                NumberOfEnabledImages = 8,
                NumberOfAnnotatedImages = 6,
                AllEnabledImagesHaveAnnotations = true
            };

            var result = _mapper.Map<EditDatasetViewModel>(editDatasetDTO);

            Assert.Equal(editDatasetDTO.UninsertedDatasetRootClasses, result.UninsertedDatasetRootClasses);
            Assert.Equal(editDatasetDTO.UninsertedDatasetSubclasses, result.UninsertedDatasetSubclasses);
            Assert.Equal(editDatasetDTO.ClassesByDatasetId, result.ClassesByDatasetId);
            Assert.Equal(editDatasetDTO.ListOfDatasetImages, result.ListOfDatasetImages);
            Assert.Equal(editDatasetDTO.NumberOfDatasetClasses, result.NumberOfDatasetClasses);
            Assert.Equal(editDatasetDTO.NumberOfChildrenDatasets, result.NumberOfChildrenDatasets);
            Assert.Equal(editDatasetDTO.InsertedDatasetClass, result.InsertedDatasetClass);
            Assert.Equal(editDatasetDTO.NumberOfDatasetImages, result.NumberOfDatasetImages);
            Assert.Equal(editDatasetDTO.NumberOfEnabledImages, result.NumberOfEnabledImages);
            Assert.Equal(editDatasetDTO.NumberOfAnnotatedImages, result.NumberOfAnnotatedImages);
            Assert.Equal(editDatasetDTO.AllEnabledImagesHaveAnnotations, result.AllEnabledImagesHaveAnnotations);
        }

        [Fact]
        public void DatasetClassDTO_To_DatasetClassViewModel_Should_Map_Properties()
        {
            var datasetClassDTO = new DatasetClassDTO
            {
                Id = Guid.NewGuid(),
                ParentClassId = Guid.NewGuid(),
                ClassName = "Class 1",
                CreatedById = "User123",
                CreatedOn = DateTime.UtcNow,
                Datasets = new List<Dataset_DatasetClassDTO> { new Dataset_DatasetClassDTO { DatasetClassId = Guid.NewGuid() } }
            };

            var result = _mapper.Map<DatasetClassViewModel>(datasetClassDTO);

            Assert.Equal(datasetClassDTO.Id, result.Id);
            Assert.Equal(datasetClassDTO.ParentClassId, result.ParentClassId);
            Assert.Equal(datasetClassDTO.ClassName, result.ClassName);
            Assert.Equal(datasetClassDTO.CreatedById, result.CreatedById);
            Assert.Equal(datasetClassDTO.CreatedOn, result.CreatedOn);
            Assert.Equal(datasetClassDTO.Datasets.Count, result.Datasets.Count);
        }

        [Fact]
        public void CreateDatasetViewModel_To_DatasetDTO_Should_Map_CreatedOn_To_Current_DateTime()
        {
            var createDatasetViewModel = new CreateDatasetViewModel
            {
                Name = "New Dataset",
                Description = "Description of new dataset",
            };

            var result = _mapper.Map<DatasetDTO>(createDatasetViewModel);

            Assert.Equal(createDatasetViewModel.Name, result.Name);
            Assert.Equal(createDatasetViewModel.Description, result.Description);
            Assert.Equal(DateTime.Now.Date, result.CreatedOn.Date); 
        }

        [Fact]
        public void Dataset_DatasetClassDTO_To_Dataset_DatasetClassViewModel_Should_Map_Properties()
        {
            var datasetDatasetClassDTO = new Dataset_DatasetClassDTO
            {
                DatasetId = Guid.NewGuid(),
                DatasetClassId = Guid.NewGuid()
            };

            var result = _mapper.Map<Dataset_DatasetClassViewModel>(datasetDatasetClassDTO);

            Assert.Equal(datasetDatasetClassDTO.DatasetId, result.DatasetId);
            Assert.Equal(datasetDatasetClassDTO.DatasetClassId, result.DatasetClassId);
        }
    }
}
