using Entities.DetectionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.DetectionEntities
{
    public class DetectedDumpSiteConfiguration : IEntityTypeConfiguration<DetectedDumpSite>
    {
        public void Configure(EntityTypeBuilder<DetectedDumpSite> builder)
        {
            builder.Property(d => d.ConfidenceRate).IsRequired();

            builder.Property(d => d.DatasetClassId).IsRequired();

            // Configure the relationship to DatasetClass
            builder.HasOne(d => d.DatasetClass)
                .WithMany() // Assuming no navigation property back to DetectedDumpSites in DatasetClass
                .HasForeignKey(d => d.DatasetClassId)
                .OnDelete(DeleteBehavior.Restrict) // Avoid CASCADE delete
                .IsRequired();

            builder.HasOne(dds => dds.DetectionRun)
               .WithMany() // Configure the inverse navigation if necessary
               .HasForeignKey(dds => dds.DetectionRunId)
               .OnDelete(DeleteBehavior.Restrict) // CASCADE delete when DetectionRun is deleted
               .IsRequired();

            // Special configuration for the Geometry type
            builder.Property(d => d.Geom)
                .IsRequired()
                .HasColumnType("geometry(Polygon)");

            // Since GeoJson is not mapped, EF Core will ignore this property
            builder.Ignore(d => d.GeoJson);
        }
    }
}
