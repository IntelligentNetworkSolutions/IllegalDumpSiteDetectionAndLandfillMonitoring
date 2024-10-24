using DTOs.MainApp.BL;

namespace MainApp.MVC.ViewModels.IntranetPortal.Training
{
    public class TrainedModelViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? ModelFilePath { get; set; }
        public string? ModelConfigPath { get; set; }
        public bool IsPublished { get; set; } = false;
        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual UserDTO? CreatedBy { get; set; }
    }
}
