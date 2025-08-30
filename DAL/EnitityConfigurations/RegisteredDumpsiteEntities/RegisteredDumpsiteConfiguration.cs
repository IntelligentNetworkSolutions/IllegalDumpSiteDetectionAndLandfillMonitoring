using Entities.RegisteredDumpsiteEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteConfiguration : IEntityTypeConfiguration<RegisteredDumpsite>
{
    public void Configure(EntityTypeBuilder<RegisteredDumpsite> builder)
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

        builder.HasOne(d => d.RegisteredDumpsiteStatus)
                .WithMany()
                .HasForeignKey(d => d.RegisteredDumpsiteStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

        builder.HasOne(d => d.RegisteredDumpsiteRiskLevel)
                .WithMany()
                .HasForeignKey(d => d.RegisteredDumpsiteRiskLevelId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

        builder.HasOne(d => d.RegisteredDumpsiteWasteType)
                .WithMany()
                .HasForeignKey(d => d.RegisteredDumpsiteWasteTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

        builder.HasMany(d => d.DumpsiteFiles)
               .WithOne(f => f.RegisteredDumpsite)
               .HasForeignKey(f => f.RegisteredDumpsiteId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Inspections)
               .WithOne(i => i.RegisteredDumpsite)
               .HasForeignKey(i => i.RegisteredDumpsiteId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}