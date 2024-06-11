using DTOs.MainApp.BL.DatasetDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DatasetDTOsTests
{
    public class CreateDatasetClassDTOTests
    {
        [Fact]
        public void CreateDatasetClassDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var className = "TestClassName";
            var parentClassId = Guid.NewGuid();
            var createdOn = DateTime.UtcNow;
            var createdById = "TestUser";

            // Act
            var dto = new CreateDatasetClassDTO
            {
                ClassName = className,
                ParentClassId = parentClassId,
                CreatedOn = createdOn,
                CreatedById = createdById
            };

            // Assert
            Assert.Equal(className, dto.ClassName);
            Assert.Equal(parentClassId, dto.ParentClassId);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdById, dto.CreatedById);
        }

        [Fact]
        public void CreateDatasetClassDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new CreateDatasetClassDTO();

            // Assert
            Assert.Null(dto.ClassName);
            Assert.Null(dto.ParentClassId);
            Assert.Null(dto.CreatedOn);
            Assert.Null(dto.CreatedById);
        }

        [Fact]
        public void CreateDatasetClassDTO_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var dto = new CreateDatasetClassDTO();
            var className = "TestClassName";
            var parentClassId = Guid.NewGuid();
            var createdOn = DateTime.UtcNow;
            var createdById = "TestUser";

            // Act
            dto.ClassName = className;
            dto.ParentClassId = parentClassId;
            dto.CreatedOn = createdOn;
            dto.CreatedById = createdById;

            // Assert
            Assert.Equal(className, dto.ClassName);
            Assert.Equal(parentClassId, dto.ParentClassId);
            Assert.Equal(createdOn, dto.CreatedOn);
            Assert.Equal(createdById, dto.CreatedById);
        }
    }
}
