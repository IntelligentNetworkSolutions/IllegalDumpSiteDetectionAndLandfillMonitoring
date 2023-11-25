using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services;
using Tests.Fixtures;

namespace Tests.ServicesTests
{
    public class LayoutServiceTests : IClassFixture<LayoutServiceFixture>
    {
        private readonly LayoutServiceFixture _fixture;

        public LayoutServiceTests(LayoutServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task LayoutServiceTest_GetLayout_WrongSettings_ReturnsNotNull()
        {
            // Arrange
            var wrongLayoutService = new LayoutService(_fixture.WrongConfiguration, _fixture.ContextAccessor);

            // Act
            var returnedWrongLayout = await wrongLayoutService.GetLayout();

            // Assert
            Assert.NotNull(returnedWrongLayout);
        }

        [Fact]
        public async Task LayoutServiceTest_GetLayout_WrongSettings_ReturnsEmpty()
        {
            // Arrange
            var wrongLayoutService = new LayoutService(_fixture.WrongConfiguration, _fixture.ContextAccessor);

            // Act
            var returnedWrongLayout = await wrongLayoutService.GetLayout();

            // Assert
            Assert.Empty(returnedWrongLayout);
        }

        [Fact]
        public async Task LayoutServiceTest_GetLayout_BothSettingModes_ReturnNotNullNotEmpty()
        {
            // Arrange
            var intranetLayoutService = new LayoutService(_fixture.PublicConfiguration, _fixture.ContextAccessor);
            var publicLayoutService = new LayoutService(_fixture.PublicConfiguration, _fixture.ContextAccessor);

            // Act
            var returnedIntranetLayout = await intranetLayoutService.GetLayout();
            var returnedPublicLayout = await publicLayoutService.GetLayout();

            // Assert
            Assert.NotNull(returnedIntranetLayout);
            Assert.NotNull(returnedPublicLayout);
            Assert.NotEmpty(returnedIntranetLayout);
            Assert.NotEmpty(returnedPublicLayout);
        }

        [Fact]
        public async Task LayoutServiceTest_GetLayout_BothSettingModes_ContainsAppStartupMode()
        {
            // Arrange
            string intranetAppStartupMode = SD.ApplicationStartModes.IntranetPortal;
            string publicAppStartupMode = SD.ApplicationStartModes.PublicPortal;

            var intranetLayoutService = new LayoutService(_fixture.IntranetConfiguration, _fixture.ContextAccessor);
            var publicLayoutService = new LayoutService(_fixture.PublicConfiguration, _fixture.ContextAccessor);

            // Act
            var returnedPublicLayout = await publicLayoutService.GetLayout();
            var returnedIntranetLayout = await intranetLayoutService.GetLayout();

            // Assert
            if (string.IsNullOrEmpty(returnedPublicLayout))
                Assert.Fail("Returned Layout Is Null or Empty");

            Assert.Contains(publicAppStartupMode, returnedPublicLayout);

            if (string.IsNullOrEmpty(returnedIntranetLayout))
                Assert.Fail("Returned Layout Is Null or Empty");

            Assert.Contains(intranetAppStartupMode, returnedIntranetLayout);
        }
    }
}
