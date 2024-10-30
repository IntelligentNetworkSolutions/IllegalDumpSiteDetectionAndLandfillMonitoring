using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Tests.MainAppMVCTests.Helpers
{
    public class LocalizationQueryProviderTests
    {
        private readonly LocalizationQueryProvider _localizationQueryProvider;

        public LocalizationQueryProviderTests()
        {
            _localizationQueryProvider = new LocalizationQueryProvider();
        }

        [Fact]
        public async Task DetermineProviderCultureResult_ShouldReturnNull_WhenHttpContextIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _localizationQueryProvider.DetermineProviderCultureResult(null!));
        }

        [Fact]
        public async Task DetermineProviderCultureResult_ShouldReturnNull_WhenCultureNotInQuery()
        {
            var context = new DefaultHttpContext();
            var result = await _localizationQueryProvider.DetermineProviderCultureResult(context);
            Assert.Null(result);
        }

        [Fact]
        public async Task DetermineProviderCultureResult_ShouldSetCultureFromQueryParameter()
        {
            var context = new DefaultHttpContext();
            context.Request.Query = new QueryCollection(new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { LocalizationQueryProvider.DefaultParamterName, "en-US" }
            });

            var result = await _localizationQueryProvider.DetermineProviderCultureResult(context);
            Assert.NotNull(result);
            Assert.Equal("en-US", result.Cultures[0].Value);
        }

        [Fact]
        public async Task DetermineProviderCultureResult_ShouldSetCultureFromReturnUrlParameter()
        {
            var context = new DefaultHttpContext();
            context.Request.Query = new QueryCollection(new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "returnUrl", "/?culture=fr-FR" }
            });

            var result = await _localizationQueryProvider.DetermineProviderCultureResult(context);
            Assert.NotNull(result);
            Assert.Equal("fr-FR", result.Cultures[0].Value);
        }

        [Fact]
        public void ParseDefaultParamterValue_ShouldReturnNull_WhenValueIsNullOrWhiteSpace()
        {
            var result = LocalizationQueryProvider.ParseDefaultParamterValue(null);
            Assert.Null(result);

            result = LocalizationQueryProvider.ParseDefaultParamterValue("   ");
            Assert.Null(result);
        }

        [Fact]
        public void ParseDefaultParamterValue_ShouldReturnCultureResult_WhenValueIsProvided()
        {
            var result = LocalizationQueryProvider.ParseDefaultParamterValue("de-DE");
            Assert.NotNull(result);
            Assert.Equal("de-DE", result.Cultures[0].Value);
        }

        [Fact]
        public void ParseDefaultParamterValue_ShouldReturnNull_WhenValueIsEmptyString()
        {
            var result = LocalizationQueryProvider.ParseDefaultParamterValue("");
            Assert.Null(result);
        }

        [Fact]
        public void ParseDefaultParamterValue_ShouldReturnCultureResult_WhenDifferentValidValuesAreProvided()
        {
            var result = LocalizationQueryProvider.ParseDefaultParamterValue("fr-FR");
            Assert.NotNull(result);
            Assert.Equal("fr-FR", result.Cultures[0].Value);

            result = LocalizationQueryProvider.ParseDefaultParamterValue("en-GB");
            Assert.NotNull(result);
            Assert.Equal("en-GB", result.Cultures[0].Value);
        }

        [Fact]
        public void ParseDefaultParamterValue_ShouldReturnCultureResult_WhenCultureAndUICultureAreDifferent()
        {
            var cultureResult = new ProviderCultureResult("fr-FR", "fr-FR");
            var result = LocalizationQueryProvider.ParseDefaultParamterValue("fr-FR");
            Assert.NotNull(result);
            Assert.Equal(cultureResult.Cultures[0].Value, result.Cultures[0].Value);
        }

        [Fact]
        public void ParseDefaultParamterValue_ShouldHandleInvalidCultureStrings()
        {
            var result = LocalizationQueryProvider.ParseDefaultParamterValue("invalid-culture");
            Assert.NotNull(result);
            Assert.Equal("invalid-culture", result.Cultures[0].Value);
        }

    }
}
