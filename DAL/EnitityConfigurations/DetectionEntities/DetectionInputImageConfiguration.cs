using Entities.DetectionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.DetectionEntities
{
    public class DetectionInputImageConfiguration : IEntityTypeConfiguration<DetectionInputImage>
    {
        public void Configure(EntityTypeBuilder<DetectionInputImage> builder)
        {
            // Define properties with specific database constraints
            builder.Property(di => di.Name).IsRequired();
            builder.Property(di => di.Description);

            // Define properties for storing paths
            builder.Property(di => di.ImagePath).IsRequired();
            builder.Property(di => di.ImageFileName).IsRequired();
            builder.Property(ia => ia.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
            builder.Property(di => di.DateTaken).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            // Define fields related to user creation
            builder.Property(di => di.CreatedById).IsRequired();
            builder.Property(di => di.CreatedOn).IsRequired();

            builder.Property(di => di.UpdatedById);
            builder.Property(di => di.UpdatedOn);

            // Configure the relationship with ApplicationUser
            builder.HasOne(di => di.CreatedBy)
                .WithMany()
                .HasForeignKey(di => di.CreatedById)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(di => di.UpdatedBy)
                .WithMany()
                .HasForeignKey(di => di.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
