using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;

namespace MainApp.MVC.ViewModels.IntranetPortal.Dataset
{
    public class DatasetClassViewModel
    {
        public Guid Id { get; init; }
        public Guid? ParentClassId { get; init; }
        public  DatasetClassDTO? ParentClass { get; init; }
        public string ClassName { get; init; }

        public string CreatedById { get; init; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }
        public virtual ICollection<Dataset_DatasetClassViewModel> Datasets { get; set; }
    }
}
