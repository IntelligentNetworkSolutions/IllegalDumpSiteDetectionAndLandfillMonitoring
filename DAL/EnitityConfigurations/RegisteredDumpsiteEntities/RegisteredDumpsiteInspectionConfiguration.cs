using Entities.RegisteredDumpsiteEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteInspectionConfiguration : IEntityTypeConfiguration<RegisteredDumpsiteInspection>
{
    public void Configure(EntityTypeBuilder<RegisteredDumpsiteInspection> builder)
    {
        builder.Property(i => i.InspectionDate).IsRequired();

        builder.Property(i => i.CreatedById).IsRequired();
        builder.Property(i => i.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

        builder.HasOne(d => d.RegisteredDumpsiteInspectionStatus)
                .WithMany()
                .HasForeignKey(d => d.RegisteredDumpsiteInspectionStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

        builder.HasOne(i => i.CreatedBy)
               .WithMany()
               .HasForeignKey(i => i.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.InspectionFiles)
               .WithOne(f => f.RegisteredDumpsiteInspection)
               .HasForeignKey(f => f.RegisteredDumpsiteInspectionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Assignments)
               .WithOne(a => a.RegisteredDumpsiteInspection)
               .HasForeignKey(a => a.RegisteredDumpsiteInspectionId)
               .OnDelete(DeleteBehavior.Cascade);

    }
}
