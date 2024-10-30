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
        public void PathToLinuxRegexSlashReplace_WithBackslashes_ReturnsForwardSlashes()
        {
            string windowsPath = "C:\\Users\\JohnDoe\\Documents\\file.txt";
            string expected = "C:/Users/JohnDoe/Documents/file.txt";
            if (Path.DirectorySeparatorChar == '/')
                expected = "/Users/JohnDoe/Documents/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(windowsPath);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithMixedSlashes_ReturnsForwardSlashes()
        {
            string mixedPath = "C:\\Users/JohnDoe\\Documents/file.txt";
            string expected = "C:/Users/JohnDoe/Documents/file.txt";
            if (Path.DirectorySeparatorChar == '/')
                expected = "/Users/JohnDoe/Documents/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(mixedPath);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithNetworkPath_ReturnsForwardSlashes()
        {
            string networkPath = "\\\\ServerName\\SharedFolder\\file.txt";
            string expected = "/ServerName/SharedFolder/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(networkPath);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithAlreadyLinuxPath_ReturnsSamePath()
        {
            string linuxPath = "/home/user/documents/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(linuxPath);

            Assert.Equal(linuxPath, result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithEmptyString_ReturnsEmptyString()
        {
            string emptyPath = "";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(emptyPath);

            Assert.Equal(emptyPath, result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithNullInput_ReturnsEmptyString()
        {
            string nullPath = null;

            string result = CommonHelper.PathToLinuxRegexSlashReplace(nullPath);

            Assert.Null(result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithPathContainingSpaces_PreservesSpaces()
        {
            string pathWithSpaces = "C:\\Program Files\\My App\\file with spaces.txt";
            string expected = "C:/Program Files/My App/file with spaces.txt";
            if (Path.DirectorySeparatorChar == '/')
                expected = "/Program Files/My App/file with spaces.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(pathWithSpaces);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithRelativePath_ConvertsToPosixStyle()
        {
            string relativePath = "..\\ParentFolder\\ChildFolder\\file.txt";
            string expected = "../ParentFolder/ChildFolder/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(relativePath);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithWindowsDriveLetter_HandlesCorrectly()
        {
            string windowsPath = "D:\\Projects\\MyProject\\src\\main.cs";
            string expected = "D:/Projects/MyProject/src/main.cs";
            if (Path.DirectorySeparatorChar == '/')
                expected = "/Projects/MyProject/src/main.cs";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(windowsPath);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithConsecutiveBackslashes_ReducesToSingleForwardSlash()
        {
            string pathWithConsecutiveSlashes = "C:\\\\Users\\\\JohnDoe\\\\\\Documents\\\\file.txt";
            string expected = "C:/Users/JohnDoe/Documents/file.txt";
            if (Path.DirectorySeparatorChar == '/')
                expected = "/Users/JohnDoe/Documents/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(pathWithConsecutiveSlashes);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PathToLinuxRegexSlashReplace_WithLinuxRootPath_RetainsLeadingSlash()
        {
            string linuxRootPath = "/root/directory/file.txt";

            string result = CommonHelper.PathToLinuxRegexSlashReplace(linuxRootPath);

            Assert.Equal(linuxRootPath, result);
        }

        [Theory]
        [InlineData("https://example.com/file.txt", "file.txt")]
        [InlineData("https://example.com/path/to/document.pdf", "document.pdf")]
        [InlineData("https://example.com/files/image.jpg", "image.jpg")]
        [InlineData("http://example.com/download/archive.zip", "archive.zip")]
        public void GetFileNameFromUrl_ValidUrls_ReturnsFileName(string url, string expected)
        {
            // Act
            string result = CommonHelper.GetFileNameFromUrl(url);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("https://example.com/file.txt?param=value", "file.txt")]
        [InlineData("https://example.com/document.pdf#page=1", "document.pdf")]
        [InlineData("https://example.com/image.jpg?size=large&format=jpeg", "image.jpg")]
        [InlineData("http://example.com/file.zip?download=true&token=abc123", "file.zip")]
        public void GetFileNameFromUrl_UrlsWithQueryParameters_ReturnsFileName(string url, string expected)
        {
            // Act
            string result = CommonHelper.GetFileNameFromUrl(url);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("https://example.com/path/to/file.with.multiple.dots.txt", "file.with.multiple.dots.txt")]
        [InlineData("https://example.com/path/_special-chars_%20file.pdf", "_special-chars_%20file.pdf")]
        [InlineData("https://example.com/path/file-name_with-special@chars.doc", "file-name_with-special@chars.doc")]
        public void GetFileNameFromUrl_ComplexFileNames_ReturnsFileName(string url, string expected)
        {
            // Act
            string result = CommonHelper.GetFileNameFromUrl(url);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("https://example.com/")]
        [InlineData("https://example.com")]
        [InlineData("https://example.com/path/")]
        [InlineData("https://example.com/path/to/folder/")]
        public void GetFileNameFromUrl_UrlsWithoutFileName_ReturnsEmptyString(string url)
        {
            // Act
            string result = CommonHelper.GetFileNameFromUrl(url);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetFileNameFromUrl_NullOrEmptyUrls_ReturnsInput(string url)
        {
            // Act
            string result = CommonHelper.GetFileNameFromUrl(url);

            // Assert
            Assert.Equal(url, result);
        }

        [Theory]
        [InlineData("not_a_url")]
        [InlineData("file.txt")]
        public void GetFileNameFromUrl_InvalidUrls_ThrowsUriFormatException(string url)
        {
            // Act & Assert
            Assert.Throws<UriFormatException>(() => CommonHelper.GetFileNameFromUrl(url));
        }

        [Theory]
        [InlineData("https://example.com/path%20with%20spaces/file.txt", "file.txt")]
        [InlineData("https://example.com/path%2Fwith%2Fencoded%2Fslashes/file.pdf", "file.pdf")]
        public void GetFileNameFromUrl_UrlEncodedPaths_ReturnsDecodedFileName(string url, string expected)
        {
            // Act
            string result = CommonHelper.GetFileNameFromUrl(url);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetFileNameFromUrl_VeryLongUrl_HandlesCorrectly()
        {
            // Arrange
            string longFileName = new string('a', 100) + ".txt";
            string url = $"https://example.com/path/to/{longFileName}";

            // Act
            string result = CommonHelper.GetFileNameFromUrl(url);

            // Assert
            Assert.Equal(longFileName, result);
        }
    }
}
