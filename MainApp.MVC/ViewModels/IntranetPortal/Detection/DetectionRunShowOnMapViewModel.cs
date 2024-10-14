using DTOs.MainApp.BL.DetectionDTOs;

namespace MainApp.MVC.ViewModels.IntranetPortal.Detection
{
    public class DetectionRunShowOnMapViewModel
    {
        public List<Guid>? selectedDetectionRunsIds { get; set; }
        public List<ConfidenceRateDTO>? selectedConfidenceRates { get; set; }
    }
}
