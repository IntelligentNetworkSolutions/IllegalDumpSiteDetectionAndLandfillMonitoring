﻿namespace MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement
{
    public class LegalLandfillTruckViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Registration { get; set; }
        public double? UnladenWeight { get; set; }
        public double? PayloadWeight { get; set; }
        public double? Capacity { get; set; }
        public bool IsEnabled { get; set; } = false;
        public string? Description { get; set; }
    }
}
