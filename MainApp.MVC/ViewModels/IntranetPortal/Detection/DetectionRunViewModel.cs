﻿using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL;

namespace MainApp.MVC.ViewModels.IntranetPortal.Detection
{
    public class DetectionRunViewModel
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string? ImagePath { get; set; }
        public string? ImageFileName { get; set; }

        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual UserDTO? CreatedBy { get; set; }

        public virtual ICollection<DetectedDumpSiteViewModel>? DetectedDumpSites { get; set; }

        public DetectionRunViewModel()
        {
            DetectedDumpSites = new List<DetectedDumpSiteViewModel>();
        }
    }
}
