using Entities.DetectionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.DetectionEntities
{
    public class DetectionRunConfiguration : IEntityTypeConfiguration<DetectionRun>
    {
        public void Configure(EntityTypeBuilder<DetectionRun> builder)
        {
            // Define properties with specific database constraints
            builder.Property(dr => dr.Name).IsRequired();
            builder.Property(dr => dr.Description); // Nullable
            builder.Property(dr => dr.IsCompleted).IsRequired();

            // Define properties for storing paths
            builder.Property(ia => ia.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            // Define fields related to user creation
            builder.Property(dr => dr.CreatedById).IsRequired();
            builder.Property(dr => dr.CreatedOn).IsRequired();

            // Configure the relationship with DetectionInputImage
            builder.HasOne(dr => dr.DetectionInputImage)
                .WithMany()
                .HasForeignKey(dr => dr.DetectionInputImageId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Configure the relationship with TrainedModel
            builder.HasOne(dr => dr.TrainedModel)
                .WithMany()
                .HasForeignKey(dr => dr.TrainedModelId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Configure the relationship with ApplicationUser
            builder.HasOne(dr => dr.CreatedBy)
                   .WithMany() // Assuming ApplicationUser has no navigation property back to DetectionRun
                   .HasForeignKey(dr => dr.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict) // Prevent CASCADE delete if ApplicationUser is deleted
                   .IsRequired();

        }
    }
}
