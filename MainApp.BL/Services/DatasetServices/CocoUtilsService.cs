using DTOs.ObjectDetection.API;
using DTOs.ObjectDetection.API.CocoFormatDTOs;
using MainApp.BL.Interfaces.Services.DatasetServices;
using Newtonsoft.Json;
using SD;

namespace MainApp.BL.Services.DatasetServices
{
    public class CocoUtilsService : ICocoUtilsService
    {
        public string[] TrainDirAllowedNames =
            new string[] { CocoTerminology.trainCocoFormatDirName, CocoTerminology.trainingCocoFormatDirName };
        public string[] ValidDirAllowedNames =
            new string[] { CocoTerminology.valCocoFormatDirName, CocoTerminology.validCocoFormatDirName, CocoTerminology.validationCocoFormatDirName };
        public string[] TestDirAllowedNames =
            new string[] { CocoTerminology.testCocoFormatDirName, CocoTerminology.testingCocoFormatDirName };

        /// <summary>
        /// Validate the Format of the COCO Dataset Upload Directory
        /// </summary>
        /// <param name="uploadDirPath">directory path for the COCO Formated Dataset being validated</param>
        /// <param name="mustHaveAnnotations">true if annotations must be present, false for training/unannotated import </param>
        /// <param name="mustBeSplit">true if dataset Must already be Split in Coco Format Directories, false for training import</param>
        /// <param name="mustHaveTestDir">true if the dataset is already split and the test directory must be present</param>
        /// <returns></returns>
        public ResultDTO IsValidCocoFormatedUploadDirectory(string uploadDirPath, bool mustHaveAnnotations = false,
            bool mustBeSplit = false, bool mustHaveTestDir = true)
        {
            if (string.IsNullOrWhiteSpace(uploadDirPath))
                return ResultDTO.Fail("Invalid upload directory path");

            if (Directory.Exists(uploadDirPath) == false)
                return ResultDTO.Fail("Upload directory does not exist at path");

            string[] filePaths = Directory.GetFiles(uploadDirPath);
            string[] directoriesPaths = Directory.GetDirectories(uploadDirPath);

            // Empty
            if (directoriesPaths.Length == 0 && filePaths.Length == 0)
                return ResultDTO.Fail("Upload Directory is Empty");

            // Is Only Files
            if (IsOnlyFilesCocoDatasetDirectory(uploadDirPath, false))
            {
                ResultDTO isValidCocoFormatDirectoryResult = IsValidCocoFormatedSplitDirectory(uploadDirPath, mustHaveAnnotations);
                if (isValidCocoFormatDirectoryResult.IsSuccess == false)
                    return ResultDTO.Fail(isValidCocoFormatDirectoryResult.ErrMsg!);

                return ResultDTO.Ok();
            }

            if (((directoriesPaths.Length == 2 && mustHaveTestDir == false) || (directoriesPaths.Length == 3 && mustHaveTestDir)) == false)
                return ResultDTO.Fail($"Invalid Number of directories in directory: {uploadDirPath}, " +
                                        $"allowed number of directories: {(mustHaveTestDir ? "3" : "2")}");

            // No Training Dir
            if (TrainDirAllowedNames.Any(allowName => directoriesPaths.Any(dp => dp.EndsWith(allowName))) == false)
                return ResultDTO.Fail($"No valid training directory found in directory: {uploadDirPath}, " +
                                        $"allowed training directory names: {CocoTerminology.trainCocoFormatDirName}, " +
                                        $"{CocoTerminology.trainingCocoFormatDirName}");

            // No Validation Dir
            //if (directoriesPaths.Any(path => ValidDirAllowedNames.Contains(path)) == false)
            if (ValidDirAllowedNames.Any(allowName => directoriesPaths.Any(dp => dp.EndsWith(allowName))) == false)
                    return ResultDTO.Fail($"No valid Validation directory found in directory: {uploadDirPath}, " +
                                        $"Allowed Validation directory names: {CocoTerminology.valCocoFormatDirName}, " +
                                        $"{CocoTerminology.validCocoFormatDirName}, " +
                                        $"{CocoTerminology.validationCocoFormatDirName}.");

            // No Test Dir
            //if (mustHaveTestDir && directoriesPaths.Any(path => TrainDirAllowedNames.Contains(path)) == false)
            if (mustHaveTestDir && TestDirAllowedNames.Any(allowName => directoriesPaths.Any(dp => dp.EndsWith(allowName))) == false)
                return ResultDTO.Fail($"No valid Test directory found in directory: {uploadDirPath}, " +
                                        $"Allowed Test directory names: {CocoTerminology.testCocoFormatDirName}, " +
                                        $"{CocoTerminology.testingCocoFormatDirName}");

            string trainDirPath = directoriesPaths.Where(path => TrainDirAllowedNames.Any(x => path.EndsWith(x))).First();
            string validDirPath = directoriesPaths.Where(path => ValidDirAllowedNames.Any(x => path.EndsWith(x))).First();
            string? testDirPath = directoriesPaths.Where(path => mustHaveTestDir && TestDirAllowedNames.Any(x => path.EndsWith(x))).FirstOrDefault();

            ResultDTO isValidTrainDirResult = IsValidCocoFormatedSplitDirectory(trainDirPath, mustHaveAnnotations);
            if (isValidTrainDirResult.IsSuccess == false)
                return ResultDTO.Fail(isValidTrainDirResult.ErrMsg!);

            ResultDTO isValidValidDirResult = IsValidCocoFormatedSplitDirectory(validDirPath, mustHaveAnnotations);
            if (isValidValidDirResult.IsSuccess == false)
                return ResultDTO.Fail(isValidValidDirResult.ErrMsg!);

            if (mustHaveTestDir == false)
                return ResultDTO.Ok();

            ResultDTO isValidTestDirResult = IsValidCocoFormatedSplitDirectory(testDirPath, mustHaveAnnotations);
            if (isValidTestDirResult.IsSuccess == false)
                return ResultDTO.Fail(isValidTestDirResult.ErrMsg!);

            return ResultDTO.Ok();
        }

        public ResultDTO IsValidCocoFormatedSplitDirectory(string splitDirPath, bool mustHaveAnnotations)
        {
            if (string.IsNullOrEmpty(splitDirPath) == true)
                return ResultDTO.Fail($"Invalid Directory Path: {splitDirPath}");

            string[] splitDirectoryFilePaths = Directory.GetFiles(splitDirPath);
            if (splitDirectoryFilePaths.Length == 0)
                return ResultDTO.Fail($"Empty directory at path: {splitDirPath}");

            if (HasAllowedImageFormats(splitDirectoryFilePaths) == false)
                return ResultDTO.Fail($"No images in the allowed formats {CocoTerminology.allowedImageFormatsConcatenated} " +
                                        $"found at path: {splitDirPath}");

            //IEnumerable<string> imagePaths = 
            //    splitDirectoryFilePaths.Where(path => CocoTerminology.allowedImageFormatsConcatenated.Contains(Path.GetExtension(path)));

            if (HasAllowedAnnotationsFileNameFormat(splitDirectoryFilePaths) == false && mustHaveAnnotations)
                return ResultDTO.Fail($"Missing Annotations File at path: {splitDirPath}");

            return ResultDTO.Ok();
        }

        public async Task<ResultDTO<CocoDatasetDTO>> GetBulkAnnotatedValidParsedCocoDatasetFromDirectoryPathAsync(string uploadDirPath, bool allowUnannotatedImages = false)
        {
            try
            {
                ResultDTO isValidCocoFormatedDatasetResult = IsValidCocoFormatedUploadDirectory(uploadDirPath, mustHaveAnnotations: false, true, true);
                if (isValidCocoFormatedDatasetResult.IsSuccess == false)
                    return ResultDTO<CocoDatasetDTO>.Fail(isValidCocoFormatedDatasetResult.ErrMsg!);

                string[] filePaths = Directory.GetFiles(uploadDirPath);

                if (IsOnlyFilesCocoDatasetDirectory(uploadDirPath, true) == false)
                    return ResultDTO<CocoDatasetDTO>.Fail($"Invalid Directory Structure, only image files and an annotation file can be present in directory: {uploadDirPath}");

                string[] imagePaths = GetImagePaths(filePaths);
                string annotationsFile = GetAnnotationFilePathInAllowedFormats(filePaths)!;

                string annotationsFileContent = await File.ReadAllTextAsync(annotationsFile);

                CocoDatasetDTO? cocoDataset = JsonConvert.DeserializeObject<CocoDatasetDTO>(annotationsFileContent);
                if (cocoDataset is null)
                    return ResultDTO<CocoDatasetDTO>.Fail($"Unable to Parse Annotation file at path: {annotationsFile} in directory : {uploadDirPath}");

                if (cocoDataset.Images.Count == 0)
                    return ResultDTO<CocoDatasetDTO>.Fail($"No Images were Parsed from the Annotation file at path: {annotationsFile} in directory : {uploadDirPath}");

                if (AreMatchedDirectoryImagePathsAndAnnotationFileImageFilePaths(cocoDataset, imagePaths) == false)
                    return ResultDTO<CocoDatasetDTO>.Fail($"Unable to Match the Parsed Image Paths from the Annotation file at path: {annotationsFile} in directory : {uploadDirPath} with the actual allowed physical images in the same directory");

                if (cocoDataset.Annotations.Count == 0)
                    return ResultDTO<CocoDatasetDTO>.Fail($"No Annotations were Parsed from the Annotation file at path: {annotationsFile} in directory : {uploadDirPath}");

                if (allowUnannotatedImages is false)
                {
                    if (AreAllAnnotationsForPresentImageIdsInAnnotationFile(cocoDataset) == false)
                        return ResultDTO<CocoDatasetDTO>.Fail($"Non Matching parsed Annotation Image Ids with parsed Image Ids from the Annotation file at path: {annotationsFile} in directory : {uploadDirPath}");
                }

                if (cocoDataset.Categories.Count == 0)
                    return ResultDTO<CocoDatasetDTO>.Fail($"No Categories were Parsed from the Annotation file at path: {annotationsFile} in directory : {uploadDirPath}");

                return ResultDTO<CocoDatasetDTO>.Ok(cocoDataset);
            }
            catch (Exception ex)
            {
                return ResultDTO<CocoDatasetDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public bool AreAllAnnotationsForPresentImageIdsInAnnotationFile(CocoDatasetDTO cocoDatasetDTO)
            => cocoDatasetDTO.Images.Select(img => img.Id).ToArray()
                                    .SequenceEqual(cocoDatasetDTO.Annotations.Select(ann => ann.ImageId).Distinct().ToArray());

        public bool AreMatchedDirectoryImagePathsAndAnnotationFileImageFilePaths(CocoDatasetDTO dataset, string[] imagePaths)
        {
            if (dataset is null)
                return false;

            if (dataset.Images.Count == 0)
                return false;

            if (imagePaths.Length == 0)
                return false;

            if (dataset.Images.Count != imagePaths.Length)
                return false;

            string[] imageFileNames = imagePaths.Select(path => Path.GetFileName(path)).ToArray();
            foreach (CocoImageDTO cocoImage in dataset.Images)
                if (imageFileNames.Contains(cocoImage.FileName) == false)
                    return false;

            return true;
        }

        public bool IsOnlyFilesCocoDatasetDirectory(string dirPath, bool mustHaveAnnotations)
        {
            string[] filePaths = Directory.GetFiles(dirPath);
            if (filePaths.Length == 0)
                return false;

            if (Directory.GetDirectories(dirPath).Length > 0)
                return false;

            bool hasAnnotationsFile = false;
            foreach (var filePath in filePaths)
            {
                if (Path.GetExtension(filePath) != ".json")
                    break;

                if (!CocoTerminology.allowedAnnotationsFileNamesConcatenated.Contains(Path.GetFileNameWithoutExtension(filePath)))
                    break;

                hasAnnotationsFile = true;
            }

            if (mustHaveAnnotations)
                return hasAnnotationsFile;

            return true;
        }

        public bool HasAllowedImageFormats(string[] filePaths)
            => filePaths.Length > 1
                && filePaths.Any(path => CocoTerminology.allowedImageFormatsConcatenated.Contains(Path.GetExtension(path)));

        public string[] GetImagePaths(string[] filePaths)
            => filePaths.Where(path => CocoTerminology.allowedImageFormatsConcatenated.Contains(Path.GetExtension(path)))
                        .ToArray();

        public bool HasAllowedAnnotationsFileNameFormat(string[] filePaths)
            => filePaths.Any(path => Path.GetExtension(path) == ".json"
                                && CocoTerminology.allowedAnnotationsFileNamesConcatenated.Contains(Path.GetFileNameWithoutExtension(path)));

        public string? GetAnnotationFilePathInAllowedFormats(string[] filePaths)
            => filePaths.FirstOrDefault(path
                => CocoTerminology.allowedAnnotationsFileNamesConcatenated.Contains(Path.GetFileNameWithoutExtension(path)));
    }
}
