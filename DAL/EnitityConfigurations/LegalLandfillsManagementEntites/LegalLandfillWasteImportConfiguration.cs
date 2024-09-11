using Entities.LegalLandfillsManagementEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.LegalLandfillsManagementEntites
{
    public class LegalLandfillWasteImportConfiguration : IEntityTypeConfiguration<LegalLandfillWasteImport>
    {
        public void Configure(EntityTypeBuilder<LegalLandfillWasteImport> builder)
        {
            // Configuring ImportedOn as required
            builder.Property(e => e.ImportedOn).IsRequired();

            // Configuring ImportExportStatus with a range constraint (-1 to 1)
            builder.Property(e => e.ImportExportStatus)
                    .IsRequired()
                    .HasDefaultValue(1); //1 is for imported waste

            // Configuring ApplicationUser relationship
            builder.Property(e => e.CreatedById).IsRequired();
            builder.Property(e => e.CreatedOn).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            // Configuring optional relationships with LegalLandfillTruck
            builder.HasOne(e => e.LegalLandfillTruck)
                .WithMany()
                .HasForeignKey(e => e.LegalLandfillTruckId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade if truck is deleted 

            // Configuring required relationships with LegalLandfill
            builder.HasOne(e => e.LegalLandfill)
                .WithMany()
                .HasForeignKey(e => e.LegalLandfillId)
                .OnDelete(DeleteBehavior.Cascade); // Maybe restrict ?

            // Configuring required relationship with LegalLandfillWasteType
            builder.HasOne(e => e.LegalLandfillWasteType)
                .WithMany()
                .HasForeignKey(e => e.LegalLandfillWasteTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
