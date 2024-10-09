using DTOs.ObjectDetection.API.CocoFormatDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.DatasetServices
{
    public interface ICocoUtilsService
    {
        ResultDTO IsValidCocoFormatedUploadDirectory(string uploadDirPath, bool mustHaveAnnotations = false,
            bool mustBeSplit = false, bool mustHaveTestDir = true);

        ResultDTO IsValidCocoFormatedSplitDirectory(string splitDirPath, bool mustHaveAnnotations);

        Task<ResultDTO<CocoDatasetDTO>> GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(string uploadDirPath, bool allowUnannotatedImages = false);

        bool AreAllAnnotationsForPresentImageIdsInAnnotationFile(CocoDatasetDTO cocoDatasetDTO);

        bool AreMatchedDirectoryImagePathsAndAnnotationFileImageFilePaths(CocoDatasetDTO dataset, string[] imagePaths);

        bool IsOnlyFilesCocoDatasetDirectory(string dirPath, bool mustHaveAnnotations);

        bool HasAllowedImageFormats(string[] filePaths);

        string[] GetImagePaths(string[] filePaths);

        bool HasAllowedAnnotationsFileNameFormat(string[] filePaths);

        string? GetAnnotationFilePathInAllowedFormats(string[] filePaths);
    }
}
