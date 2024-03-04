using DTOs.MainApp.BL.DatasetDTOs;

namespace MainApp.MVC.ViewModels.IntranetPortal.Dataset
{
    public class CreateDatasetViewModel : DatasetViewModel
    {
        public List<DatasetClassDTO>? AllDatasetClasses { get; set; }
        public List<Guid>? InsertedDatasetClasses { get; set; }
    }
}
