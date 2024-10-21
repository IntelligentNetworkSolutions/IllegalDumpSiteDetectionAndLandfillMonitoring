using SD.Helpers;

namespace Tests.SDTests.Helpers
{
    public class CommonHelperTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData(42, 42)]
        [InlineData("Hello", "Hello")]
        [InlineData(3.14, 3.14)]
        [InlineData(true, true)]
        public void CastTo_ReturnsSameValue_WhenInputAndOutputTypesAreSame<ReturnType>(object inputValue, ReturnType expectedValue)
        {
            // Arrange
            // Act
            var actualValue = CommonHelper.CastTo<ReturnType>(inputValue);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Theory]
        [InlineData(42, "42")]
        [InlineData("3.14", 3.14)]
        [InlineData(true, 1)]
        [InlineData(0, false)]
        public void CastTo_ReturnsConvertedValue_WhenInputAndOutputTypesAreConvertible<ReturnType>(object inputValue, ReturnType expectedValue)
        {
            // Arrange
            // Act
            var actualValue = CommonHelper.CastTo<ReturnType>(inputValue);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Theory]
        [InlineData("Hello", 42)]
        [InlineData(3.14, new string[] { "3.14" })]
        [InlineData(false, new string[] { "false" })]
        public void CastTo_ReturnsNull_WhenInputAndOutputTypesAreNotConvertible<ReturnType>(object inputValue, ReturnType expectedValue)
        {
            // Arrange
            // Act
            var actualValue = CommonHelper.CastTo<ReturnType>(inputValue);

            // Assert
            Assert.Null(actualValue);
        }

        [Fact]
        public void EnumerableHasDuplicatesByProperty_EmptyEnumerable_ReturnsFalse()
        {
            var collection = Enumerable.Empty<int>();
            Assert.False(CommonHelper.EnumerableHasDuplicatesByProperty(collection));
        }

        [Fact]
        public void EnumerableHasDuplicatesByProperty_SingleElementEnumerable_ReturnsFalse()
        {
            var collection = new[] { 1 };
            Assert.False(CommonHelper.EnumerableHasDuplicatesByProperty(collection));
        }

        [Fact]
        public void EnumerableHasDuplicatesByProperty_TwoDistinctElements_ReturnsFalse()
        {
            var collection = new[] { 1, 2 };
            Assert.False(CommonHelper.EnumerableHasDuplicatesByProperty(collection));
        }

        [Fact]
        public void EnumerableHasDuplicatesByProperty_TwoEqualElements_ReturnsTrue()
        {
            var collection = new[] { 1, 1 };
            Assert.True(CommonHelper.EnumerableHasDuplicatesByProperty(collection));
        }

        [Fact]
        public void EnumerableHasDuplicatesByProperty_ManyElements_SomeDuplicates_ReturnsTrue()
        {
            var collection = new[] { 1, 2, 2, 3, 3, 3 };
            Assert.True(CommonHelper.EnumerableHasDuplicatesByProperty(collection));
        }

        [Fact]
        public void EnumerableHasDuplicatesByProperty_ManyElements_NoDuplicates_ReturnsFalse()
        {
            var collection = new[] { 1, 2, 3, 4, 5 };
            Assert.False(CommonHelper.EnumerableHasDuplicatesByProperty(collection));
        }

        [Fact]
        public void EnumerableHasDuplicatesByPropertyValue_TwoKeyValueDistinctValueElements_ReturnsFalse()
        {
            var collection = new KeyValuePair<string, int>[] {
                KeyValuePair.Create<string, int>("1", 1),
                KeyValuePair.Create<string, int>("2", 2)
            };
            Assert.False(CommonHelper.EnumerableHasDuplicatesByProperty(collection, x => x.Value));
        }

        [Fact]
        public void EnumerableHasDuplicatesByPropertyValue_TwoKeyValueEqualValueElements_ReturnsTrue()
        {
            var collection = new KeyValuePair<string, int>[] {
                KeyValuePair.Create<string, int>("1", 1),
                KeyValuePair.Create<string, int>("2", 1)
            };
            Assert.True(CommonHelper.EnumerableHasDuplicatesByProperty(collection, x => x.Value));
        }

        [Fact]
        public void EnumerableHasDuplicatesByPropertyValue_ManyKeyValueElementsSomeDuplicates_ReturnsTrue()
        {
            var collection = new KeyValuePair<string, int>[] {
                KeyValuePair.Create<string, int>("1", 1),
                KeyValuePair.Create<string, int>("2", 2),
                KeyValuePair.Create<string, int>("3", 2),
                KeyValuePair.Create<string, int>("4", 3),
                KeyValuePair.Create<string, int>("5", 3),
                KeyValuePair.Create<string, int>("6", 3)
            };
            Assert.True(CommonHelper.EnumerableHasDuplicatesByProperty(collection, x => x.Value));
        }

        [Fact]
        public void EnumerableHasDuplicatesByPropertyValue_ManyKeyValueElementsNoDuplicates_ReturnsFalse()
        {
            var collection = new KeyValuePair<string, int>[] {
                KeyValuePair.Create<string, int>("1", 1),
                KeyValuePair.Create<string, int>("2", 2),
                KeyValuePair.Create<string, int>("3", 3),
                KeyValuePair.Create<string, int>("4", 4),
                KeyValuePair.Create<string, int>("5", 5)
            };
            Assert.False(CommonHelper.EnumerableHasDuplicatesByProperty(collection, x => x.Value));
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithBackslashes_ReturnsForwardSlashes()
        {
            string windowsPath = "C:\\Users\\JohnDoe\\Documents\\file.txt";
            string expected = "C:/Users/JohnDoe/Documents/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(windowsPath);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithMixedSlashes_ReturnsForwardSlashes()
        {
            string mixedPath = "C:\\Users/JohnDoe\\Documents/file.txt";
            string expected = "C:/Users/JohnDoe/Documents/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(mixedPath);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithNetworkPath_ReturnsForwardSlashes()
        {
            string networkPath = "\\\\ServerName\\SharedFolder\\file.txt";
            string expected = "/ServerName/SharedFolder/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(networkPath);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithAlreadyLinuxPath_ReturnsSamePath()
        {
            string linuxPath = "/home/user/documents/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(linuxPath);

            Assert.Equal(linuxPath, result);
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithEmptyString_ReturnsEmptyString()
        {
            string emptyPath = "";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(emptyPath);

            Assert.Equal(emptyPath, result);
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithNullInput_ReturnsEmptyString()
        {
            string nullPath = null;

            string result = CommonHelper.PathToLinuxRegexSlashReplace(nullPath);

            Assert.Equal(null, result);
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithEmptyInput_ReturnsEmptyString()
        {
            string nullPath = string.Empty;

            string result = CommonHelper.PathToLinuxRegexSlashReplace(nullPath);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithOnlyBackslashes_ReturnsAllForwardSlashes()
        {
            string backslashesOnly = "\\\\\\\\";
            string expected = "/";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(backslashesOnly);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithPathContainingSpaces_PreservesSpaces()
        {
            string pathWithSpaces = "C:\\Program Files\\My App\\file with spaces.txt";
            string expected = "C:/Program Files/My App/file with spaces.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(pathWithSpaces);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertWindowsPathToLinuxPathReplaceAllDashes_WithRelativePath_ConvertsToPosixStyle()
        {
            string relativePath = "..\\ParentFolder\\ChildFolder\\file.txt";
            string expected = "../ParentFolder/ChildFolder/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(relativePath);

            Assert.Equal(expected, result);
        }
    }
}
