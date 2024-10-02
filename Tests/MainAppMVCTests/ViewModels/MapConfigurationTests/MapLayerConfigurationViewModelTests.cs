using MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration;

namespace Tests.MainAppMVCTests.ViewModels.MapConfigurationTests
{
    public class MapLayerConfigurationViewModelTests
    {
        [Fact]
        public void IdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var viewModel = new MapLayerConfigurationViewModel
            {
                Id = id
            };

            // Act & Assert
            Assert.Equal(id, viewModel.Id);
        }

        [Fact]
        public void LayerNameProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var layerName = "Sample Layer";
            var viewModel = new MapLayerConfigurationViewModel
            {
                LayerName = layerName
            };

            // Act & Assert
            Assert.Equal(layerName, viewModel.LayerName);
        }

        [Fact]
        public void LayerTitleJsonProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var layerTitleJson = "Layer Title Test";
            var viewModel = new MapLayerConfigurationViewModel
            {
                LayerTitleJson = layerTitleJson
            };

            // Act & Assert
            Assert.Equal(layerTitleJson, viewModel.LayerTitleJson);
        }
        [Fact]
        public void LayerDescriptionJsonProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var layerDescriptionJson = "Layer  Description Json test";
            var viewModel = new MapLayerConfigurationViewModel
            {
                LayerDescriptionJson = layerDescriptionJson
            };

            // Act & Assert
            Assert.Equal(layerDescriptionJson, viewModel.LayerDescriptionJson);
        }
        [Fact]
        public void LayerJsProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var layerJs = "Layer JS test";
            var viewModel = new MapLayerConfigurationViewModel
            {
                LayerJs = layerJs
            };

            // Act & Assert
            Assert.Equal(layerJs, viewModel.LayerJs);
        }


        [Fact]
        public void OrderProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var order = 1;
            var viewModel = new MapLayerConfigurationViewModel
            {
                Order = order
            };

            // Act & Assert
            Assert.Equal(order, viewModel.Order);
        }

        [Fact]
        public void CreatedOnProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var createdOn = DateTime.UtcNow;
            var viewModel = new MapLayerConfigurationViewModel
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
            var createdById = "user123";
            var viewModel = new MapLayerConfigurationViewModel
            {
                CreatedById = createdById
            };

            // Act & Assert
            Assert.Equal(createdById, viewModel.CreatedById);
        }

        [Fact]
        public void MapConfigurationIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var mapConfigurationId = Guid.NewGuid();
            var viewModel = new MapLayerConfigurationViewModel
            {
                MapConfigurationId = mapConfigurationId
            };

            // Act & Assert
            Assert.Equal(mapConfigurationId, viewModel.MapConfigurationId);
        }

        [Fact]
        public void MapLayerGroupConfigurationIdProperty_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var groupConfigId = Guid.NewGuid();
            var viewModel = new MapLayerConfigurationViewModel
            {
                MapLayerGroupConfigurationId = groupConfigId
            };

            // Act & Assert
            Assert.Equal(groupConfigId, viewModel.MapLayerGroupConfigurationId);
        }
    }

}
