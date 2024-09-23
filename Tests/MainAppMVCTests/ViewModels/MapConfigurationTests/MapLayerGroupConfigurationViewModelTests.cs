using MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration;

namespace Tests.MainAppMVCTests.ViewModels.MapConfigurationTests
{
    public class MapLayerGroupConfigurationViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void GroupNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var groupName = "Sample Group";
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                GroupName = groupName
            };

            // Act & Assert
            Assert.Equal(groupName, viewModel.GroupName);
        }

        [Fact]
        public void LayerGroupTitleJsonProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var layerGroupTitleJson = "Layer Group Title";
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                LayerGroupTitleJson = layerGroupTitleJson
            };

            // Act & Assert
            Assert.Equal(layerGroupTitleJson, viewModel.LayerGroupTitleJson);
        }

        [Fact]
        public void LayerGroupDescriptionJsonProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var layerGroupDescriptionJson = "Layer Group Description";
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                LayerGroupDescriptionJson = layerGroupDescriptionJson
            };

            // Act & Assert
            Assert.Equal(layerGroupDescriptionJson, viewModel.LayerGroupDescriptionJson);
        }

        [Fact]
        public void OrderProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var order = 2;
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                Order = order
            };

            // Act & Assert
            Assert.Equal(order, viewModel.Order);
        }

        [Fact]
        public void OpacityProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var opacity = 0.75;
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                Opacity = opacity
            };

            // Act & Assert
            Assert.Equal(opacity, viewModel.Opacity);
        }

        [Fact]
        public void GroupJsProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var groupJs = "function() { /* JS code */ }";
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                GroupJs = groupJs
            };

            // Act & Assert
            Assert.Equal(groupJs, viewModel.GroupJs);
        }

        [Fact]
        public void MapConfigurationIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var mapConfigurationId = Guid.NewGuid();
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                MapConfigurationId = mapConfigurationId
            };

            // Act & Assert
            Assert.Equal(mapConfigurationId, viewModel.MapConfigurationId);
        }

        [Fact]
        public void CreatedOnProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                CreatedOn = createdOn
            };

            // Act & Assert
            Assert.Equal(createdOn, viewModel.CreatedOn);
        }

        [Fact]
        public void CreatedByIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdById = "user456";
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                CreatedById = createdById
            };

            // Act & Assert
            Assert.Equal(createdById, viewModel.CreatedById);
        }

        [Fact]
        public void UpdatedByIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var updatedById = "user789";
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                UpdatedById = updatedById
            };

            // Act & Assert
            Assert.Equal(updatedById, viewModel.UpdatedById);
        }

        [Fact]
        public void UpdatedOnProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var updatedOn = DateTime.UtcNow;
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                UpdatedOn = updatedOn
            };

            // Act & Assert
            Assert.Equal(updatedOn, viewModel.UpdatedOn);
        }

        [Fact]
        public void MapLayerConfigurationsProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var mapLayerConfigurations = new List<MapLayerConfigurationViewModel>
        {
            new MapLayerConfigurationViewModel { Id = Guid.NewGuid() }
        };
            var viewModel = new MapLayerGroupConfigurationViewModel
            {
                MapLayerConfigurations = mapLayerConfigurations
            };

            // Act & Assert
            Assert.Equal(mapLayerConfigurations, viewModel.MapLayerConfigurations);
        }
    }

}
