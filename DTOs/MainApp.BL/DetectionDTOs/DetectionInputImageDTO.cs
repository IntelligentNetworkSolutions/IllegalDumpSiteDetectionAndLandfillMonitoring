namespace DTOs.MainApp.BL.DetectionDTOs
{
    public class DetectionInputImageDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DateTaken { get; set; }

        public string? ImagePath { get; set; }
        public string? ImageFileName { get; set; }

        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual UserDTO? CreatedBy { get; set; }

        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public virtual UserDTO? UpdatedBy { get; set; }
    }
}
