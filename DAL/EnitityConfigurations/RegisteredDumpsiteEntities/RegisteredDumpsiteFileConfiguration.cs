using Entities.RegisteredDumpsiteEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteFileConfiguration : IEntityTypeConfiguration<RegisteredDumpsiteFile>
{
    public void Configure(EntityTypeBuilder<RegisteredDumpsiteFile> builder)
    {
        builder.Property(f => f.FileName).IsRequired();
        builder.Property(f => f.FilePath).IsRequired();
        builder.Property(f => f.ContentType).IsRequired();
        builder.Property(f => f.FileExtension).IsRequired();

        builder.Property(f => f.CreatedById).IsRequired();
        builder.Property(f => f.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

        builder.HasOne(f => f.CreatedBy)
               .WithMany()
               .HasForeignKey(f => f.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

