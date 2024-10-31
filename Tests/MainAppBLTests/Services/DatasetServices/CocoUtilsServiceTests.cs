using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.ObjectDetection.API.CocoFormatDTOs;
using MainApp.BL.Services.DatasetServices;
using SD;
using SD.Helpers;

namespace Tests.MainAppBLTests.Services.DatasetServices
{
    public class CocoUtilsServiceTests
    {
        private readonly CocoUtilsService _service;

        public CocoUtilsServiceTests()
        {
            _service = new CocoUtilsService();
        }

        #region IsValidCocoFormatedUploadDirectory Tests
        [Fact]
        public void IsValidCocoFormatedUploadDirectory_WithNullPath_ReturnsFalse()
        {
            // Arrange
            string? path = null;

            // Act
            ResultDTO result = _service.IsValidCocoFormatedUploadDirectory(path!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid upload directory path", result.ErrMsg);
        }

        [Fact]
        public void IsValidCocoFormatedUploadDirectory_WithNonExistentPath_ReturnsFalse()
        {
            // Arrange
            string path = "nonexistent/path";

            // Act
            ResultDTO result = _service.IsValidCocoFormatedUploadDirectory(path);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Upload directory does not exist at path", result.ErrMsg);
        }

        [Fact]
        public void IsValidCocoFormatedUploadDirectory_WithEmptyDirectory_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            try
            {
                // Act
                ResultDTO result = _service.IsValidCocoFormatedUploadDirectory(tempPath);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Equal("Upload Directory is Empty", result.ErrMsg);
            }
            finally
            {
                Directory.Delete(tempPath);
            }
        }

        // TODO: Uncoment when fix_coco_utils is merged
        [Fact]
        public void IsValidCocoFormatedUploadDirectory_WithValidSplitDirectories_ReturnsTrue()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);
            string trainDir = Path.Combine(tempPath, "train");
            string validDir = Path.Combine(tempPath, "valid");
            string testDir = Path.Combine(tempPath, "test");

            try
            {
                // Create directories and sample files
                Directory.CreateDirectory(trainDir);
                Directory.CreateDirectory(validDir);
                Directory.CreateDirectory(testDir);

                File.WriteAllBytes(Path.Combine(trainDir, "image1.jpg"), new byte[] { 0 });
                File.WriteAllBytes(Path.Combine(validDir, "image2.jpg"), new byte[] { 0 });
                File.WriteAllBytes(Path.Combine(testDir, "image3.jpg"), new byte[] { 0 });
                File.WriteAllText(Path.Combine(trainDir, "annotations_coco.json"), "{}");
                File.WriteAllText(Path.Combine(validDir, "annotations_coco.json"), "{}");
                File.WriteAllText(Path.Combine(testDir, "annotations_coco.json"), "{}");

                // Act
                ResultDTO result = _service.IsValidCocoFormatedUploadDirectory(tempPath, false, true, true);

                // Assert
                Assert.True(result.IsSuccess);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void IsValidCocoFormatedUploadDirectory_WithInvalidDirectoryCount_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);
            string trainDir = Path.Combine(tempPath, "train");

            try
            {
                Directory.CreateDirectory(trainDir);
                File.WriteAllBytes(Path.Combine(trainDir, "image1.jpg"), new byte[] { 0 });

                // Act
                ResultDTO result = _service.IsValidCocoFormatedUploadDirectory(tempPath, false, true);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("Invalid Number of directories", result.ErrMsg);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }
        #endregion

        #region IsValidCocoFormatedSplitDirectory Tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValidCocoFormatedSplitDirectory_WithInvalidPath_ReturnsFalse(string path)
        {
            // Act
            ResultDTO result = _service.IsValidCocoFormatedSplitDirectory(path, true);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid Directory Path", result.ErrMsg);
        }

        [Fact]
        public void IsValidCocoFormatedSplitDirectory_WithNoAnnotationsWhenRequired_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            try
            {
                File.WriteAllBytes(Path.Combine(tempPath, "image1.jpg"), new byte[] { 0 });

                // Act
                ResultDTO result = _service.IsValidCocoFormatedSplitDirectory(tempPath, true);

                // Assert
                Assert.False(result.IsSuccess);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void IsValidCocoFormatedSplitDirectory_WithOnlyInvalidImageFormats_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            try
            {
                File.WriteAllBytes(Path.Combine(tempPath, "image1.txt"), new byte[] { 0 });
                File.WriteAllText(Path.Combine(tempPath, "annotations_coco.json"), "{}");

                // Act
                ResultDTO result = _service.IsValidCocoFormatedSplitDirectory(tempPath, true);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("No images in the allowed formats", result.ErrMsg);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void IsValidCocoFormatedSplitDirectory_WithEmptyDirectory_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            try
            {
                // Act
                ResultDTO result = _service.IsValidCocoFormatedSplitDirectory(tempPath, true);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("Empty directory at path", result.ErrMsg);
            }
            finally
            {
                Directory.Delete(tempPath);
            }
        }

        [Fact]
        public void IsValidCocoFormatedSplitDirectory_WithValidImagesAndAnnotations_ReturnsTrue()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            try
            {
                // Create sample files
                File.WriteAllBytes(Path.Combine(tempPath, "image1.jpg"), new byte[] { 0 });
                File.WriteAllText(Path.Combine(tempPath, "annotations_coco.json"), "{}");

                // Act
                ResultDTO result = _service.IsValidCocoFormatedSplitDirectory(tempPath, true);

                // Assert
                Assert.True(result.IsSuccess);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }
        #endregion

        #region AreAllAnnotationsForPresentImageIdsInAnnotationFile Tests
        [Fact]
        public void AreAllAnnotationsForPresentImageIdsInAnnotationFile_WithMatchingIds_ReturnsTrue()
        {
            // Arrange
            var dataset = new CocoDatasetDTO
            {
                Images = [ new CocoImageDTO { Id = 1, FileName = "image1.jpg", Width = 1280, Height = 1280 },
                            new CocoImageDTO { Id = 2, FileName = "image2.jpg", Width = 1280, Height = 1280 } ],
                Annotations = [ new CocoAnnotationDTO { ImageId = 1, Bbox = [] }, new CocoAnnotationDTO { ImageId = 2, Bbox = [] } ],
                Categories = []
            };

            // Act
            bool result = _service.AreAllAnnotationsForPresentImageIdsInAnnotationFile(dataset);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AreAllAnnotationsForPresentImageIdsInAnnotationFile_WithMismatchedIds_ReturnsFalse()
        {
            // Arrange
            var dataset = new CocoDatasetDTO
            {
                Images = [ new CocoImageDTO { Id = 1, FileName = "image1.jpg", Width = 1280, Height = 1280 },
                            new CocoImageDTO { Id = 2, FileName = "image2.jpg", Width = 1280, Height = 1280 } ],
                Annotations = [ new CocoAnnotationDTO { ImageId = 1,Bbox = [] },
                                new CocoAnnotationDTO { ImageId = 3, Bbox = [] } ],
                Categories = []
            };

            // Act
            bool result = _service.AreAllAnnotationsForPresentImageIdsInAnnotationFile(dataset);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AreAllAnnotationsForPresentImageIdsInAnnotationFile_WithEmptyDataset_ReturnsFalse()
        {
            // Arrange
            var dataset = new CocoDatasetDTO
            {
                Images = [],
                Annotations = [],
                Categories = []
            };

            // Act
            bool result = _service.AreAllAnnotationsForPresentImageIdsInAnnotationFile(dataset);

            // Assert
            Assert.True(result); // Empty lists should be considered equal
        }
        #endregion

        #region AreMatchedDirectoryImagePathsAndAnnotationFileImageFilePaths Tests
        [Fact]
        public void AreMatchedDirectoryImagePathsAndAnnotationFileImageFilePaths_WithMatchingPaths_ReturnsTrue()
        {
            // Arrange
            var dataset = new CocoDatasetDTO
            {
                Images = [ new CocoImageDTO { FileName = "image1.jpg", Width = 1280, Height = 1280 },
                            new CocoImageDTO { FileName = "image2.jpg", Width = 1280, Height = 1280 } ],
                Annotations = [],
                Categories = []
            };
            string[] imagePaths = new[]
            {
            Path.Combine("path", "to", "image1.jpg"),
            Path.Combine("path", "to", "image2.jpg")
        };

            // Act
            bool result = _service.AreMatchedDirectoryImagePathsAndAnnotationFileImageFilePaths(dataset, imagePaths);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AreMatchedDirectoryImagePathsAndAnnotationFileImageFilePaths_WithMismatchedPaths_ReturnsFalse()
        {
            // Arrange
            var dataset = new CocoDatasetDTO
            {
                Images = new List<CocoImageDTO>
                {
                    new CocoImageDTO { FileName = "image1.jpg", Width = 1280, Height = 1280 },
                    new CocoImageDTO { FileName = "image2.jpg", Width = 1280, Height = 1280 }
                },
                Annotations = [],
                Categories = []
            };
            string[] imagePaths = new[]
            {
            Path.Combine("path", "to", "image1.jpg"),
            Path.Combine("path", "to", "image3.jpg")
        };

            // Act
            bool result = _service.AreMatchedDirectoryImagePathsAndAnnotationFileImageFilePaths(dataset, imagePaths);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region HasAllowedImageFormats Tests
        [Fact]
        public void HasAllowedImageFormats_WithValidFormats_ReturnsTrue()
        {
            // Arrange
            string[] paths = new[]
            {
            "image1.jpg",
            "image2.png",
            "annotations_coco.json"
        };

            // Act
            bool result = _service.HasAllowedImageFormats(paths);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasAllowedImageFormats_WithNoValidFormats_ReturnsFalse()
        {
            // Arrange
            string[] paths = new[]
            {
            "image1.txt",
            "image2.doc",
            "annotations_coco.json"
        };

            // Act
            bool result = _service.HasAllowedImageFormats(paths);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region GetImagePaths Tests
        [Fact]
        public void GetImagePaths_ReturnsOnlyImagePaths()
        {
            // Arrange
            string[] paths = new[]
            {
            "image1.jpg",
            "image2.png",
            "annotations_coco.json",
            "document.txt"
        };

            // Act
            string[] result = _service.GetImagePaths(paths);

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Contains("image1.jpg", result);
            Assert.Contains("image2.png", result);
        }
        #endregion

        #region HasAllowedAnnotationsFileNameFormat Tests
        [Fact]
        public void HasAllowedAnnotationsFileNameFormat_WithValidAnnotationFile_ReturnsTrue()
        {
            // Arrange
            string[] paths = new[]
            {
            "image1.jpg",
            "annotations_coco.json"
        };

            // Act
            bool result = _service.HasAllowedAnnotationsFileNameFormat(paths);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasAllowedAnnotationsFileNameFormat_WithNoValidAnnotationFile_ReturnsFalse()
        {
            // Arrange
            string[] paths = new[]
            {
            "image1.jpg",
            "invalid_annotations.json"
        };

            // Act
            bool result = _service.HasAllowedAnnotationsFileNameFormat(paths);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region GetAnnotationFilePathInAllowedFormats Tests
        [Fact]
        public void GetAnnotationFilePathInAllowedFormats_ReturnsCorrectPath()
        {
            // Arrange
            string[] paths = new[]
            {
            "image1.jpg",
            "annotations_coco.json",
            "document.txt"
        };

            // Act
            string? result = _service.GetAnnotationFilePathInAllowedFormats(paths);

            // Assert
            Assert.Equal("annotations_coco.json", result);
        }

        [Fact]
        public void GetAnnotationFilePathInAllowedFormats_WithNoValidAnnotationFile_ReturnsNull()
        {
            // Arrange
            string[] paths = new[]
            {
            "image1.jpg",
            "invalid_annotations.json",
            "document.txt"
        };

            // Act
            string? result = _service.GetAnnotationFilePathInAllowedFormats(paths);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync Tests
        // TODO: Fix for GitHub Actions Runner
        [Fact]
        public async Task GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPath_WithValidData_ReturnsCocoDataset()
        {
            // Arrange
            string tempPath = CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            Directory.CreateDirectory(tempPath);

            try
            {
                File.WriteAllBytes(CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(tempPath, "image1.jpg")), new byte[] { 0 });
                string validJson = @"{
                ""images"": [{""id"": 1, ""file_name"": ""image1.jpg""}],
                ""annotations"": [{""image_id"": 1, ""id"": 1}],
                ""categories"": [{""id"": 1, ""name"": ""test""}]
            }";
                File.WriteAllText(CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(tempPath, "annotations_coco.json")), validJson);

                // Act
                ResultDTO<CocoDatasetDTO> result = await _service.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(tempPath);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);
                Assert.Single(result.Data.Images);
                Assert.Single(result.Data.Annotations);
                Assert.Single(result.Data.Categories);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public async Task GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPath_WithInvalidJson_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            try
            {
                File.WriteAllBytes(Path.Combine(tempPath, "image1.jpg"), new byte[] { 0 });
                File.WriteAllText(Path.Combine(tempPath, "annotations_coco.json"), "invalid json");

                // Act
                ResultDTO<CocoDatasetDTO> result = await _service.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(tempPath);

                // Assert
                Assert.False(result.IsSuccess);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public async Task GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPath_WithMissingImages_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            try
            {
                string validJson = @"{
                ""images"": [],
                ""annotations"": [{""image_id"": 1, ""id"": 1}],
                ""categories"": [{""id"": 1, ""name"": ""test""}]
            }";
                File.WriteAllText(Path.Combine(tempPath, "annotations_coco.json"), validJson);

                // Act
                ResultDTO<CocoDatasetDTO> result = await _service.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(tempPath);

                // Assert
                Assert.False(result.IsSuccess);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public async Task GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPath_WithMissingAnnotations_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            try
            {
                File.WriteAllBytes(Path.Combine(tempPath, "image1.jpg"), new byte[] { 0 });
                string validJson = @"{
                ""images"": [{""id"": 1, ""file_name"": ""image1.jpg""}],
                ""annotations"": [],
                ""categories"": [{""id"": 1, ""name"": ""test""}]
            }";
                File.WriteAllText(Path.Combine(tempPath, "annotations_coco.json"), validJson);

                // Act
                ResultDTO<CocoDatasetDTO> result = await _service.GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(tempPath);

                // Assert
                Assert.False(result.IsSuccess);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }
        #endregion

        #region IsOnlyFilesCocoDatasetDirectory Tests
        [Fact]
        public void IsOnlyFilesCocoDatasetDirectory_WithSubdirectories_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);
            Directory.CreateDirectory(Path.Combine(tempPath, "subdir"));

            try
            {
                File.WriteAllBytes(Path.Combine(tempPath, "image1.jpg"), new byte[] { 0 });

                // Act
                bool result = _service.IsOnlyFilesCocoDatasetDirectory(tempPath, false);

                // Assert
                Assert.False(result);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void IsOnlyFilesCocoDatasetDirectory_WithAnnotationsRequired_AndNoAnnotations_ReturnsFalse()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            try
            {
                File.WriteAllBytes(Path.Combine(tempPath, "image1.jpg"), new byte[] { 0 });

                // Act
                bool result = _service.IsOnlyFilesCocoDatasetDirectory(tempPath, true);

                // Assert
                Assert.False(result);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        // TODO: Fix for GitHub Actions Runner
        [Fact]
        public void IsOnlyFilesCocoDatasetDirectory_WithValidFiles_ReturnsTrue()
        {
            // Arrange
            string tempPath = CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            Directory.CreateDirectory(tempPath);

            try
            {
                File.WriteAllBytes(CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(tempPath, "image1.jpg")), new byte[] { 0 });
                File.WriteAllText(CommonHelper.PathToLinuxRegexSlashReplace(Path.Combine(tempPath, "annotations_coco.json")), "{}");

                // Act
                bool result = _service.IsOnlyFilesCocoDatasetDirectory(tempPath, true);

                // Assert
                Assert.True(result);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }
        #endregion
    }
}
