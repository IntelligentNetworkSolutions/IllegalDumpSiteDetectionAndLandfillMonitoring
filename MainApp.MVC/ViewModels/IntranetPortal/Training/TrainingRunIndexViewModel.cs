using DTOs.MainApp.BL;

namespace MainApp.MVC.ViewModels.IntranetPortal.Training
{
    public class TrainingRunIndexViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string? Status { get; set; }
        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual UserDTO? CreatedBy { get; set; }
    }
}
