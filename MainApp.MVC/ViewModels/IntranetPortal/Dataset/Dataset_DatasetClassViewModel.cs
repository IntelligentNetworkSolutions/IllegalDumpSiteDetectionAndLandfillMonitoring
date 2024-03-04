using DTOs.MainApp.BL.DatasetDTOs;

namespace MainApp.MVC.ViewModels.IntranetPortal.Dataset
{
    public class Dataset_DatasetClassViewModel
    {
        public Guid? DatasetId { get; set; }
        public DatasetViewModel? Dataset { get; set; }

        public Guid? DatasetClassId { get; set; }
        public DatasetClassViewModel? DatasetClass { get; set; }
    }
}
