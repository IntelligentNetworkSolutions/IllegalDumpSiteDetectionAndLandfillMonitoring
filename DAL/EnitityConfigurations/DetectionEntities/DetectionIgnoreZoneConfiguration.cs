using Entities.DetectionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.DetectionEntities
{
    public class DetectionIgnoreZoneConfiguration : IEntityTypeConfiguration<DetectionIgnoreZone>
    {
        public void Configure(EntityTypeBuilder<DetectionIgnoreZone> builder)
        {
            // Configuring properties
            builder.Property(diz => diz.Name).IsRequired();
            builder.Property(diz => diz.Description); // Optional

            // Configuring the spatial data type for the Polygon
            builder.Property(diz => diz.Geom)
                   .IsRequired()
                   .HasColumnType("geometry(Polygon)");

            // Configure the ignored properties
            builder.Ignore(diz => diz.GeoJson);

            // Configuring user-related properties
            builder.Property(diz => diz.CreatedById).IsRequired();
            builder.Property(ia => ia.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            // Configure the relationship with ApplicationUser
            builder.HasOne(diz => diz.CreatedBy)
                   .WithMany() // Assuming there's no inverse navigation property
                   .HasForeignKey(diz => diz.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict) // This prevents CASCADE delete when ApplicationUser is deleted
                   .IsRequired();
        }
    }
}
