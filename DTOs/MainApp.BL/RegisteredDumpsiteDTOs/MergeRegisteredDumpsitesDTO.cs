namespace DTOs.MainApp.BL.RegisteredDumpsiteDTOs
{
    public class MergeRegisteredDumpsitesDTO
    {
        public CreateRegisteredDumpsiteDTO NewDumpsiteData { get; set; }
        public List<Guid> ExistingDumpsiteIds { get; set; }
    }
}
