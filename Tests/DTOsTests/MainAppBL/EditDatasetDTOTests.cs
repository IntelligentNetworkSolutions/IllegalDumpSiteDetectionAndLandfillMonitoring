using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class EditDatasetDTOTests
    {
        [Fact]
        public void EditDatasetDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var uninsertedDatasetRootClasses = new List<DatasetClassDTO> { new DatasetClassDTO { ClassName = "RootClass1" } };
            var uninsertedDatasetSubclasses = new List<DatasetClassDTO> { new DatasetClassDTO { ClassName = "Subclass1" } };
            var classesByDatasetId = new List<DatasetClassDTO> { new DatasetClassDTO { ClassName = "DatasetClass1" } };
            var parentDatasetClasses = new List<DatasetClassDTO> { new DatasetClassDTO { ClassName = "ParentClass1" } };
            var listOfDatasetImages = new List<DatasetImageDTO> { new DatasetImageDTO { FileName = "Image1.png" } };
            var listOfAllDatasetImagesUnFiltered = new List<DatasetImageDTO> { new DatasetImageDTO { FileName = "Image2.png" } };
            var numberOfDatasetClasses = 5;
            var currentDataset = new DatasetDTO { Name = "TestDataset" };
            var numberOfChildrenDatasets = 3;
            var insertedDatasetClass = Guid.NewGuid();
            var numberOfDatasetImages = 10;
            var allImageAnnotations = new List<ImageAnnotationDTO> { new ImageAnnotationDTO { AnnotationJson = "Annotation1" } };
            var numberOfEnabledImages = 7;
            var numberOfAnnotatedImages = 4;
            var allEnabledImagesHaveAnnotations = true;
            var numberOfClassesNeededToPublishDataset = 2;
            var numberOfImagesNeededToPublishDataset = 5;

            // Act
            var dto = new EditDatasetDTO
            {
                UninsertedDatasetRootClasses = uninsertedDatasetRootClasses,
                UninsertedDatasetSubclasses = uninsertedDatasetSubclasses,
                ClassesByDatasetId = classesByDatasetId,
                ParentDatasetClasses = parentDatasetClasses,
                ListOfDatasetImages = listOfDatasetImages,
                ListOfAllDatasetImagesUnFiltered = listOfAllDatasetImagesUnFiltered,
                NumberOfDatasetClasses = numberOfDatasetClasses,
                CurrentDataset = currentDataset,
                NumberOfChildrenDatasets = numberOfChildrenDatasets,
                InsertedDatasetClass = insertedDatasetClass,
                NumberOfDatasetImages = numberOfDatasetImages,
                AllImageAnnotations = allImageAnnotations,
                NumberOfEnabledImages = numberOfEnabledImages,
                NumberOfAnnotatedImages = numberOfAnnotatedImages,
                AllEnabledImagesHaveAnnotations = allEnabledImagesHaveAnnotations,
                NumberOfClassesNeededToPublishDataset = numberOfClassesNeededToPublishDataset,
                NumberOfImagesNeededToPublishDataset = numberOfImagesNeededToPublishDataset
            };

            // Assert
            Assert.Equal(uninsertedDatasetRootClasses, dto.UninsertedDatasetRootClasses);
            Assert.Equal(uninsertedDatasetSubclasses, dto.UninsertedDatasetSubclasses);
            Assert.Equal(classesByDatasetId, dto.ClassesByDatasetId);
            Assert.Equal(parentDatasetClasses, dto.ParentDatasetClasses);
            Assert.Equal(listOfDatasetImages, dto.ListOfDatasetImages);
            Assert.Equal(listOfAllDatasetImagesUnFiltered, dto.ListOfAllDatasetImagesUnFiltered);
            Assert.Equal(numberOfDatasetClasses, dto.NumberOfDatasetClasses);
            Assert.Equal(currentDataset, dto.CurrentDataset);
            Assert.Equal(numberOfChildrenDatasets, dto.NumberOfChildrenDatasets);
            Assert.Equal(insertedDatasetClass, dto.InsertedDatasetClass);
            Assert.Equal(numberOfDatasetImages, dto.NumberOfDatasetImages);
            Assert.Equal(allImageAnnotations, dto.AllImageAnnotations);
            Assert.Equal(numberOfEnabledImages, dto.NumberOfEnabledImages);
            Assert.Equal(numberOfAnnotatedImages, dto.NumberOfAnnotatedImages);
            Assert.Equal(allEnabledImagesHaveAnnotations, dto.AllEnabledImagesHaveAnnotations);
            Assert.Equal(numberOfClassesNeededToPublishDataset, dto.NumberOfClassesNeededToPublishDataset);
            Assert.Equal(numberOfImagesNeededToPublishDataset, dto.NumberOfImagesNeededToPublishDataset);
        }

        [Fact]
        public void EditDatasetDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new EditDatasetDTO();

            // Assert
            Assert.Null(dto.UninsertedDatasetRootClasses);
            Assert.Null(dto.UninsertedDatasetSubclasses);
            Assert.Null(dto.ClassesByDatasetId);
            Assert.Null(dto.ParentDatasetClasses);
            Assert.Null(dto.ListOfDatasetImages);
            Assert.Null(dto.ListOfAllDatasetImagesUnFiltered);
            Assert.Equal(0, dto.NumberOfDatasetClasses);
            Assert.Null(dto.CurrentDataset);
            Assert.Equal(0, dto.NumberOfChildrenDatasets);
            Assert.Equal(Guid.Empty, dto.InsertedDatasetClass);
            Assert.Equal(0, dto.NumberOfDatasetImages);
            Assert.Null(dto.AllImageAnnotations);
            Assert.Equal(0, dto.NumberOfEnabledImages);
            Assert.Equal(0, dto.NumberOfAnnotatedImages);
            Assert.False(dto.AllEnabledImagesHaveAnnotations);
            Assert.Equal(0, dto.NumberOfClassesNeededToPublishDataset);
            Assert.Equal(0, dto.NumberOfImagesNeededToPublishDataset);
        }

        [Fact]
        public void EditDatasetDTO_Properties_ShouldBeInitializedWithProvidedValues()
        {
            // Arrange
            var dto = new EditDatasetDTO
            {
                UninsertedDatasetRootClasses = new List<DatasetClassDTO> { new DatasetClassDTO { ClassName = "Class1" } },
                UninsertedDatasetSubclasses = new List<DatasetClassDTO> { new DatasetClassDTO { ClassName = "Class2" } },
                ClassesByDatasetId = new List<DatasetClassDTO> { new DatasetClassDTO { ClassName = "Class3" } },
                ParentDatasetClasses = new List<DatasetClassDTO> { new DatasetClassDTO { ClassName = "Class4" } },
                ListOfDatasetImages = new List<DatasetImageDTO> { new DatasetImageDTO { FileName = "Image1.png" } },
                ListOfAllDatasetImagesUnFiltered = new List<DatasetImageDTO> { new DatasetImageDTO { FileName = "Image2.png" } },
                NumberOfDatasetClasses = 5,
                CurrentDataset = new DatasetDTO { Name = "TestDataset" },
                NumberOfChildrenDatasets = 3,
                InsertedDatasetClass = Guid.NewGuid(),
                NumberOfDatasetImages = 10,
                AllImageAnnotations = new List<ImageAnnotationDTO> { new ImageAnnotationDTO { AnnotationJson = "Annotation1" } },
                NumberOfEnabledImages = 7,
                NumberOfAnnotatedImages = 4,
                AllEnabledImagesHaveAnnotations = true,
                NumberOfClassesNeededToPublishDataset = 2,
                NumberOfImagesNeededToPublishDataset = 5
            };

            // Assert
            Assert.Equal("Class1", dto.UninsertedDatasetRootClasses[0].ClassName);
            Assert.Equal("Class2", dto.UninsertedDatasetSubclasses[0].ClassName);
            Assert.Equal("Class3", dto.ClassesByDatasetId[0].ClassName);
            Assert.Equal("Class4", dto.ParentDatasetClasses[0].ClassName);
            Assert.Equal("Image1.png", dto.ListOfDatasetImages[0].FileName);
            Assert.Equal("Image2.png", dto.ListOfAllDatasetImagesUnFiltered[0].FileName);
            Assert.Equal(5, dto.NumberOfDatasetClasses);
            Assert.Equal("TestDataset", dto.CurrentDataset.Name);
            Assert.Equal(3, dto.NumberOfChildrenDatasets);
            Assert.NotEqual(Guid.Empty, dto.InsertedDatasetClass);
            Assert.Equal(10, dto.NumberOfDatasetImages);
            Assert.Equal("Annotation1", dto.AllImageAnnotations[0].AnnotationJson);
            Assert.Equal(7, dto.NumberOfEnabledImages);
            Assert.Equal(4, dto.NumberOfAnnotatedImages);
            Assert.True(dto.AllEnabledImagesHaveAnnotations);
            Assert.Equal(2, dto.NumberOfClassesNeededToPublishDataset);
            Assert.Equal(5, dto.NumberOfImagesNeededToPublishDataset);
        }
    }
}
