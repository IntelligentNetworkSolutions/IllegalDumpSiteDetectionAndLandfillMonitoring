using DTOs.MainApp.BL.DatasetDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL.DatasetDTOsTests
{
    public class EditDatasetClassDTOTests
    {
        [Fact]
        public void EditDatasetClassDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var className = "Test Class";
            var parentClassId = Guid.NewGuid();

            // Act
            var dto = new EditDatasetClassDTO
            {
                Id = id,
                ClassName = className,
                ParentClassId = parentClassId
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(className, dto.ClassName);
            Assert.Equal(parentClassId, dto.ParentClassId);
        }

        [Fact]
        public void EditDatasetClassDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new EditDatasetClassDTO();

            // Assert
            Assert.Equal(default(Guid), dto.Id);
            Assert.Null(dto.ClassName);
            Assert.Null(dto.ParentClassId);
        }

        [Fact]
        public void EditDatasetClassDTO_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var dto = new EditDatasetClassDTO();
            var id = Guid.NewGuid();
            var className = "Test Class";
            var parentClassId = Guid.NewGuid();

            // Act
            dto.Id = id;
            dto.ClassName = className;
            dto.ParentClassId = parentClassId;

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(className, dto.ClassName);
            Assert.Equal(parentClassId, dto.ParentClassId);
        }

    }
}
