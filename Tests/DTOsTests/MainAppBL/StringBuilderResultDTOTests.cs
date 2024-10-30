using DTOs.MainApp.BL;
using System.Text;

namespace Tests.DTOsTests.MainAppBL
{
    public class StringBuilderResultDTOTests
    {
        [Fact]
        public void StringBuilderResultDTO_ShouldInitializeWithNullValues()
        {
            // Arrange & Act
            var dto = new StringBuilderResultDTO();

            // Assert
            Assert.Null(dto.StringBuilderSuccess);
            Assert.Null(dto.StringBuilderError);
        }

        [Fact]
        public void StringBuilderResultDTO_ShouldStoreSuccessAndErrorMessages()
        {
            // Arrange
            var successMessage = "Operation completed successfully.";
            var errorMessage = "An error occurred during the operation.";

            var successBuilder = new StringBuilder(successMessage);
            var errorBuilder = new StringBuilder(errorMessage);

            // Act
            var dto = new StringBuilderResultDTO
            {
                StringBuilderSuccess = successBuilder,
                StringBuilderError = errorBuilder
            };

            // Assert
            Assert.Equal(successMessage, dto.StringBuilderSuccess.ToString());
            Assert.Equal(errorMessage, dto.StringBuilderError.ToString());
        }
    }
}
