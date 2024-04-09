using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using System.ComponentModel.DataAnnotations;

namespace MainApp.MVC.ViewModels.IntranetPortal.Dataset
{
    public class DatasetViewModel
    {
        public Guid Id { get; init; }
        [Required(ErrorMessage = "Dataset name is a required property")]
        public string Name { get; init; }
        [Required(ErrorMessage = "Dataset description is a required property")]
        public string Description { get; init; }

        public bool IsPublished { get; init; } = false;

        public Guid? ParentDatasetId { get; init; }
        public virtual DatasetDTO? ParentDataset { get; init; }

        public string? CreatedById { get; set; }
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public UserDTO? CreatedBy { get; init; }

        public string? UpdatedById { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public UserDTO? UpdatedBy { get; init; }
        public bool? AnnotationsPerSubclass { get; set; }
    }
}
