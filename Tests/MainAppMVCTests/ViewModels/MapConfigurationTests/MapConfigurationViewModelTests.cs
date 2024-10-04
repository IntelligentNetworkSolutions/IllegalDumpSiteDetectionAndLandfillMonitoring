using MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration;

namespace Tests.MainAppMVCTests.ViewModels.MapConfigurationTests
{
    public class MapConfigurationViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new MapConfigurationViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void MapNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var mapName = "Test Map";
            var viewModel = new MapConfigurationViewModel
            {
                MapName = mapName
            };

            // Act & Assert
            Assert.Equal(mapName, viewModel.MapName);
        }

        [Fact]
        public void ProjectionProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var projection = "EPSG:4326";
            var viewModel = new MapConfigurationViewModel
            {
                Projection = projection
            };

            // Act & Assert
            Assert.Equal(projection, viewModel.Projection);
        }

        [Fact]
        public void CenterXProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var centerX = 100.5;
            var viewModel = new MapConfigurationViewModel
            {
                CenterX = centerX
            };

            // Act & Assert
            Assert.Equal(centerX, viewModel.CenterX);
        }

        [Fact]
        public void CenterYProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var centerY = 200.5;
            var viewModel = new MapConfigurationViewModel
            {
                CenterY = centerY
            };

            // Act & Assert
            Assert.Equal(centerY, viewModel.CenterY);
        }
        [Fact]
        public void MaxXProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var maxX = 100.5;
            var viewModel = new MapConfigurationViewModel
            {
                MaxX = maxX
            };

            // Act & Assert
            Assert.Equal(maxX, viewModel.MaxX);
        }

        [Fact]
        public void MaxYProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var maxY = 200.5;
            var viewModel = new MapConfigurationViewModel
            {
                MaxY = maxY
            };

            // Act & Assert
            Assert.Equal(maxY, viewModel.MaxY);
        }
        [Fact]
        public void MinXProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var minX = 100.5;
            var viewModel = new MapConfigurationViewModel
            {
                MinX = minX
            };

            // Act & Assert
            Assert.Equal(minX, viewModel.MinX);
        }

        [Fact]
        public void MinYProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var minY = 200.5;
            var viewModel = new MapConfigurationViewModel
            {
                MinY = minY
            };

            // Act & Assert
            Assert.Equal(minY, viewModel.MinY);
        }

        [Fact]
        public void CreatedOnProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var viewModel = new MapConfigurationViewModel
            {
                CreatedOn = createdOn
            };

            // Act & Assert
            Assert.Equal(createdOn, viewModel.CreatedOn);
        }

        [Fact]
        public void DefaultResolutionProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var defaultResolution = 0.5;
            var viewModel = new MapConfigurationViewModel
            {
                DefaultResolution = defaultResolution
            };

            // Act & Assert
            Assert.Equal(defaultResolution, viewModel.DefaultResolution);
        }
    }


}
