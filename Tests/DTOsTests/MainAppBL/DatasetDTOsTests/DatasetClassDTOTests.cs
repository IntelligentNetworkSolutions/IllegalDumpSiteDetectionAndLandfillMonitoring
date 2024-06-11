using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DatasetDTOsTests
{
    public class DatasetClassDTOTests
    {
        [Fact]
        public void DatasetClassDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var parentClassId = Guid.NewGuid();
            var parentClass = new DatasetClassDTO();
            var className = "ClassName";
            var createdById = "CreatedById";
            var createdOn = new DateTime(2023, 1, 1);
            var createdBy = new UserDTO();
            var datasets = new List<Dataset_DatasetClassDTO>();

            // Act
            var dto = new DatasetClassDTO
            {
                Id = id,
                ParentClassId = parentClassId,
                ParentClass = parentClass,
                ClassName = className,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                Datasets = datasets
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(parentClassId, dto.ParentClassId);
            Assert.Equal(parentClass, dto.ParentClass);
            Assert.Equal(className, dto.ClassName);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(datasets, dto.Datasets);
        }

        [Fact]
        public void DatasetClassDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new DatasetClassDTO();

            // Assert
            Assert.Equal(default(Guid), dto.Id);
            Assert.Null(dto.ParentClassId);
            Assert.Null(dto.ParentClass);
            Assert.Null(dto.ClassName);
            Assert.Null(dto.CreatedById);
            Assert.Equal(DateTime.UtcNow.Date, dto.CreatedOn.Date);
            Assert.Null(dto.CreatedBy);
            Assert.Null(dto.Datasets);
        }

        [Fact]
        public void DatasetClassDTO_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var dto = new DatasetClassDTO();
            var id = Guid.NewGuid();
            var parentClassId = Guid.NewGuid();
            var parentClass = new DatasetClassDTO();
            var className = "ClassName";
            var createdById = "CreatedById";
            var createdOn = new DateTime(2023, 1, 1);
            var createdBy = new UserDTO();
            var datasets = new List<Dataset_DatasetClassDTO>();

            // Act
            dto = dto with
            {
                Id = id,
                ParentClassId = parentClassId,
                ParentClass = parentClass,
                ClassName = className,
                CreatedById = createdById,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                Datasets = datasets
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(parentClassId, dto.ParentClassId);
            Assert.Equal(parentClass, dto.ParentClass);
            Assert.Equal(className, dto.ClassName);
            Assert.Equal(createdById, dto.CreatedById);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdBy, dto.CreatedBy);
            Assert.Equal(datasets, dto.Datasets);
        }
    }
}
