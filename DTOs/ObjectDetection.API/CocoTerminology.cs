using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ObjectDetection.API
{
    public class CocoTerminology
    {
        public const string trainCocoFormatDirName = "train";
        public const string trainingCocoFormatDirName = "training";

        public const string valCocoFormatDirName = "val";
        public const string validCocoFormatDirName = "valid";
        public const string validationCocoFormatDirName = "validation";

        public const string testCocoFormatDirName = "test";
        public const string testingCocoFormatDirName = "testing";

        public const string allowedImageFormatsConcatenated = ".jpg, .jpeg, .png, .tiff, .tif";
        public const string allowedAnnotationsFileNamesConcatenated = "annotations, annotations_coco, coco_annotations, _annotations.coco";
        public static readonly List<string> allowedAnnotationsFileNamesList = 
            ["annotations.json", "annotations_coco.json", "coco_annotations.json", "_annotations.coco.json"];
    }
}
