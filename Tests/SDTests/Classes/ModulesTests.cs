using SD;

namespace Tests.SDTests.Classes
{
    public class ModulesTests
    {
        [Fact]
        public void GetAll_ReturnsAllModules()
        {
            // Arrange
            // Act
            var modules = Modules.GetAll();

            // Assert
            Assert.NotEmpty(modules);
            Assert.Equal(4, modules.Count());
            Assert.Contains(modules, m => m.Value == "UserManagement");
            Assert.Contains(modules, m => m.Value == "AuditLog");
            Assert.Contains(modules, m => m.Value == "Admin");
            Assert.Contains(modules, m => m.Value == "SpecialActions");
        }

        [Fact]
        public void CheckModuleValuesForDuplicates_DoesNotThrowExceptionForNoDuplicates()
        {
            // Arrange
            bool hasException = false;

            // Act
            try
            {
                Modules.CheckModuleValuesForDuplicates();
            }
            catch(Exception ex)
            {
                hasException = true;
            }

            // Assert
            Assert.False(hasException);
        }

        [Fact]
        public void NullModules_NoExceptionThrown()
        {
            Modules.CheckModuleValuesForDuplicates(null);
        }

        [Fact]
        public void EmptyModules_NoExceptionThrown()
        {
            var modules = new List<Module>();
            Modules.CheckModuleValuesForDuplicates(modules);
        }

        [Fact]
        public void SingleModule_NoExceptionThrown()
        {
            var module = new Module { Value = "unique value" };
            var modules = new List<Module> { module };
            Modules.CheckModuleValuesForDuplicates(modules);
        }

        [Fact]
        public void TwoDistinctModules_NoExceptionThrown()
        {
            var module1 = new Module { Value = "unique value 1" };
            var module2 = new Module { Value = "unique value 2" };
            var modules = new List<Module> { module1, module2 };
            Modules.CheckModuleValuesForDuplicates(modules);
        }

        [Fact]
        public void TwoEqualModules_ExceptionThrown()
        {
            var module1 = new Module { Value = "duplicate value" };
            var module2 = new Module { Value = "duplicate value" };
            var modules = new List<Module> { module1, module2 };
            Assert.Throws<Exception>(() => Modules.CheckModuleValuesForDuplicates(modules));
        }

        [Fact]
        public void ManyModulesWithDuplicates_ExceptionThrown()
        {
            var module1 = new Module { Value = "unique value 1" };
            var module2 = new Module { Value = "unique value 2" };
            var module3 = new Module { Value = "duplicate value" };
            var module4 = new Module { Value = "duplicate value" };
            var modules = new List<Module> { module1, module2, module3, module4 };
            Assert.Throws<Exception>(() => Modules.CheckModuleValuesForDuplicates(modules));
        }
    }
}
