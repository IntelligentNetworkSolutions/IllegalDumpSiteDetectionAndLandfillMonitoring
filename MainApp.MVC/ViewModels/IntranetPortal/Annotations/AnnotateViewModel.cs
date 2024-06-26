﻿using DTOs.MainApp.BL.DatasetDTOs;

namespace MainApp.MVC.ViewModels.IntranetPortal.Annotations
{
    public class AnnotateViewModel
    {
        public DatasetImageDTO DatasetImage { get; set; }
        public DatasetDTO Dataset { get; set; }        
        public List<DatasetClassDTO> DatasetClasses { get; set; }
        public int TotalImagesCount { get; set; }
        public int CurrentImagePositionInDataset { get; set; }
        public DatasetImageDTO? NextImage { get; set; }
        public DatasetImageDTO? PreviousImage { get; set; }

        /*
        public List<ImageAnnotationDTO> ImageAnnotations { get; set; }

        public AnnotateViewModel() { 
            ImageAnnotations = new List<ImageAnnotationDTO>();
        }
        */
    }
}
