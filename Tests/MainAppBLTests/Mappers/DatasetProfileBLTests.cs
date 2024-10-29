using AutoMapper;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests.Mappers
{
    public class DatasetProfileBLTests
    {
        private readonly IMapper _mapper;

        public DatasetProfileBLTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DatasetProfileBL>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Should_Map_DatasetDTO_To_Dataset()
        {
            // Arrange
            var dto = new DatasetDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test Dataset",
                Description = "A sample dataset description",
                IsPublished = true,
                CreatedById = "User123",
                CreatedOn = DateTime.UtcNow,
                UpdatedById = "User456",
                UpdatedOn = DateTime.UtcNow
            };

            // Act
            var result = _mapper.Map<Dataset>(dto);

            // Assert
            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.IsPublished, result.IsPublished);
            Assert.Equal(dto.CreatedById, result.CreatedById);
            Assert.Equal(dto.CreatedOn, result.CreatedOn);
            Assert.Equal(dto.UpdatedById, result.UpdatedById);
            Assert.Equal(dto.UpdatedOn, result.UpdatedOn);
        }

        [Fact]
        public void Should_Map_CreateDatasetClassDTO_To_DatasetClass_With_CurrentDate()
        {
            // Arrange
            var createDto = new CreateDatasetClassDTO
            {
                ClassName = "Test Class",
                ParentClassId = Guid.NewGuid(),
                CreatedById = "User123"
            };

            // Act
            var result = _mapper.Map<DatasetClass>(createDto);

            // Assert
            Assert.Equal(createDto.ClassName, result.ClassName);
            Assert.Equal(createDto.ParentClassId, result.ParentClassId);
            Assert.Equal(createDto.CreatedById, result.CreatedById);
            Assert.True(result.CreatedOn <= DateTime.UtcNow);
        }

        [Fact]
        public void Should_Map_EditDatasetImageDTO_To_DatasetImage_With_CurrentUpdatedOnDate()
        {
            // Arrange
            var editDto = new EditDatasetImageDTO
            {
                Id = Guid.NewGuid(),
                DatasetId = Guid.NewGuid(),
                IsEnabled = true,
                Name = "Updated Image",
                FileName = "image.png",
                UpdatedById = "User123"
            };

            // Act
            var result = _mapper.Map<DatasetImage>(editDto);

            // Assert
            Assert.Equal(editDto.Id, result.Id);
            Assert.Equal(editDto.DatasetId, result.DatasetId);
            Assert.Equal(editDto.IsEnabled, result.IsEnabled);
            Assert.Equal(editDto.Name, result.Name);
            Assert.Equal(editDto.FileName, result.FileName);
            Assert.Equal(editDto.UpdatedById, result.UpdatedById);
            Assert.True(result.UpdatedOn <= DateTime.UtcNow);
        }

        [Fact]
        public void Should_Map_DatasetClassDTO_To_DatasetClass()
        {
            // Arrange
            var dto = new DatasetClassDTO
            {
                Id = Guid.NewGuid(),
                ClassName = "Class Name",
                CreatedById = "User123",
                CreatedOn = DateTime.UtcNow,
                ParentClassId = Guid.NewGuid()
            };

            // Act
            var result = _mapper.Map<DatasetClass>(dto);

            // Assert
            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(dto.ClassName, result.ClassName);
            Assert.Equal(dto.CreatedById, result.CreatedById);
            Assert.Equal(dto.CreatedOn, result.CreatedOn);
            Assert.Equal(dto.ParentClassId, result.ParentClassId);
        }

        [Fact]
        public void Should_Map_ImageAnnotationDTO_To_ImageAnnotation()
        {
            // Arrange
            var annotationDto = new ImageAnnotationDTO
            {
                Id = Guid.NewGuid(),
                AnnotationJson = "{}",
                IsEnabled = true,
                DatasetImageId = Guid.NewGuid(),
                DatasetClassId = Guid.NewGuid(),
                CreatedById = "User123",
                CreatedOn = DateTime.UtcNow,
                UpdatedById = "User456",
                UpdatedOn = DateTime.UtcNow
            };

            // Act
            var result = _mapper.Map<ImageAnnotation>(annotationDto);

            // Assert
            Assert.Equal(annotationDto.Id, result.Id);
            Assert.Equal(annotationDto.AnnotationJson, result.AnnotationJson);
            Assert.Equal(annotationDto.IsEnabled, result.IsEnabled);
            Assert.Equal(annotationDto.DatasetImageId, result.DatasetImageId);
            Assert.Equal(annotationDto.DatasetClassId, result.DatasetClassId);
            Assert.Equal(annotationDto.CreatedById, result.CreatedById);
            Assert.Equal(annotationDto.CreatedOn, result.CreatedOn);
            Assert.Equal(annotationDto.UpdatedById, result.UpdatedById);
            Assert.Equal(annotationDto.UpdatedOn, result.UpdatedOn);
        }
    }
}
