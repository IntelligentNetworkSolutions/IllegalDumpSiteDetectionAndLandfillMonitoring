using System.Text.RegularExpressions;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Html;
using Westwind.Globalization;

namespace Tests.MainAppMVCTests.Helpers
{
    public class DbResHtmlTests
    {
        // Weird Stuff
        // Somewhere between Unit and Integration Test, but actualy tests the framework, since static classes
        // I don't know how to do this without complete integration test => !!! Test return Localized to one lang, different value
        // WriteResource does not work with this set up
        // If we can make it work and set culture then we can test return of localized resource
        // DbRes.T does not accept context so i don't know on which context it runs
        // i don't think we can set culture on a new context and expect that langue/cookie to apply

        // Test Returns null when null

        // No Resource & Set No Lang 
        // Test Returns empty when empty
        // Test Return same as resourceId
        [Theory]
        [InlineData("", "")]
        [InlineData("test", "test")]
        [InlineData("test-resource", "test-resource")]
        [InlineData("test-{0}-formated", "test-hehe-formated")]
        [InlineData("test-one-{0}-formated-test-two-{1}-formated", "test-one-hehe-formated-test-two-hehe-formated")]
        [InlineData("{0}", "hehe")] // weird
        public void T_ResourceIdOnly_ReturnsExpected(string resourceId, string? hardCodedExpectation)
        {
            string regexPatternForFormatingMatchingStr = "{[0-9]+}";
            Regex regexForFormating = new Regex(regexPatternForFormatingMatchingStr);
            string? expectedResourceStr = resourceId switch
            {
                "" => string.Empty,
                string s when regexForFormating.IsMatch(s) => regexForFormating.Replace(resourceId, "hehe"),
                _ => resourceId // default case
            };
            HtmlString expectedResource = new HtmlString(expectedResourceStr);

            // Act
            HtmlString? returnedResource = DbResHtml.T(resourceId);
            string? returnedResourceStr;

            // Assert
            if (returnedResource is not null && Regex.IsMatch(resourceId, regexPatternForFormatingMatchingStr))
                returnedResourceStr = regexForFormating.Replace(returnedResource.ToString(), "hehe");
            else
                returnedResourceStr = returnedResource is null ? null : returnedResource.ToString();
            
            Assert.Equal(expectedResourceStr, returnedResourceStr);
            Assert.Equal(expectedResourceStr, hardCodedExpectation);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public void T_NullResourceId_ReturnsEmptyHtmlString(bool hasResourceSet, bool hasLang)
        {
            HtmlString returnedResource = (hasResourceSet, hasLang) switch
            {
                (false, false) => returnedResource = DbResHtml.T(null),
                (true, false) => returnedResource = DbResHtml.T(null, null),
                (false, true) => returnedResource = DbResHtml.T(null, lang: null),
                (true, true) => returnedResource = DbResHtml.T(null, null, null)
            };

            Assert.Equal("", returnedResource.ToString());
        }
    }
}
