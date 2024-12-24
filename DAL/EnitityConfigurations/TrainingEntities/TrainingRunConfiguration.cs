using Entities.TrainingEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.TrainingEntities
{
    public class TrainingRunConfiguration : IEntityTypeConfiguration<TrainingRun>
    {
        public void Configure(EntityTypeBuilder<TrainingRun> builder)
        {
            // Basic property configurations
            builder.Property(tr => tr.Name).IsRequired();
            builder.Property(tr => tr.IsCompleted).IsRequired();

            // Relationships
            // Dataset relationship
            builder.HasOne(tr => tr.Dataset)
                   .WithMany() // Assuming no navigation property in Dataset pointing back
                   .HasForeignKey(tr => tr.DatasetId)
                   .OnDelete(DeleteBehavior.SetNull); // Choose appropriate delete behavior

            // Optional TrainedModel navigation property
            builder.HasOne(tr => tr.TrainedModel)
                   .WithOne(tm => tm.TrainingRun)
                   .HasForeignKey<TrainedModel>(tm => tm.TrainingRunId);

            // BaseModel relationship - Assuming it's another TrainedModel instance
            builder.HasOne(tr => tr.BaseModel)
                   .WithMany() // Assuming no navigation property in TrainedModel pointing back
                   .HasForeignKey(tr => tr.BaseModelId)
                   .OnDelete(DeleteBehavior.SetNull); // Set null on delete

            // TrainingRunTrainParams relationship
            builder.HasOne(tr => tr.TrainParams)
                   .WithOne(tp => tp.TrainingRun)
                   .HasForeignKey<TrainingRun>(tr => tr.TrainParamsId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Configuring user-related properties
            builder.Property(tr => tr.CreatedById).IsRequired();
            builder.Property(ia => ia.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.HasOne(tr => tr.CreatedBy)
                   .WithMany() // Assuming there's no collection of TrainingRuns in ApplicationUser
                   .HasForeignKey(tr => tr.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict) // Prevent CASCADE delete when ApplicationUser is deleted
                   .IsRequired();
        }
    }
}
