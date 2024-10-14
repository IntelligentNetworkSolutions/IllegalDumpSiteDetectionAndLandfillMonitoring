using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;

namespace MainApp.MVC.ViewModels.IntranetPortal.ScheduleRuns
{
    public class ViewScheduledRunsViewModel
    {
        public List<DetectionRunDTO> DetectionRuns { get; set; }
        public List<TrainingRunDTO> TrainingRuns { get; set; }
    }
}
