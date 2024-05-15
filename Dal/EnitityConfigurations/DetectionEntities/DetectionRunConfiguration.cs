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
            builder.Property(dr => dr.ImagePath).IsRequired();
            builder.Property(ia => ia.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            // Define fields related to user creation
            builder.Property(dr => dr.CreatedById).IsRequired();
            builder.Property(dr => dr.CreatedOn).IsRequired();

            // Configure the relationship with ApplicationUser
            builder.HasOne(dr => dr.CreatedBy)
                   .WithMany() // Assuming ApplicationUser has no navigation property back to DetectionRun
                   .HasForeignKey(dr => dr.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict) // Prevent CASCADE delete if ApplicationUser is deleted
                   .IsRequired();

        }
    }
}
