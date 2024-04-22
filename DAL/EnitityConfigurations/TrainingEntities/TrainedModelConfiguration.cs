using Entities.TrainingEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.TrainingEntities
{
    public class TrainedModelConfiguration : IEntityTypeConfiguration<TrainedModel>
    {
        public void Configure(EntityTypeBuilder<TrainedModel> builder)
        {
            // String properties
            builder.Property(tm => tm.Name).IsRequired();
            builder.Property(tm => tm.ModelFilePath).IsRequired();

            // Boolean property
            builder.Property(tm => tm.IsPublished).IsRequired();

            // Relationships
            // Dataset
            builder.HasOne(tm => tm.Dataset)
                   .WithMany() // Assuming Dataset does not have a navigation property pointing back to TrainedModel
                   .HasForeignKey(tm => tm.DatasetId)
                   .OnDelete(DeleteBehavior.Restrict) // Prevents cascade delete
                   .IsRequired();

            // Required TrainingRun navigation property
            builder.HasOne(tm => tm.TrainingRun)
                   .WithOne(tr => tr.TrainedModel)
                   .HasForeignKey<TrainedModel>(tm => tm.TrainingRunId);

            // TrainedModelStatistics
            builder.HasOne(tm => tm.TrainedModelStatistics)
                   .WithOne() // Assuming a one-to-one relationship
                   .HasForeignKey<TrainedModel>(tm => tm.TrainedModelStatisticsId)
                   .OnDelete(DeleteBehavior.SetNull) // Set null on delete
                   ;

            // Configuring the self-referencing relationship
            builder.HasOne(tm => tm.BaseModel)
                   .WithMany()  // This might need to be adjusted if you decide to track derived models
                   .HasForeignKey(tm => tm.BaseModelId)
                   .OnDelete(DeleteBehavior.SetNull); // Ensures deletion of a base model does not delete derived models

            // User-related properties
            builder.Property(tm => tm.CreatedById).IsRequired();
            builder.Property(ia => ia.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.HasOne(tm => tm.CreatedBy)
                   .WithMany() // Assuming ApplicationUser does not have a navigation property back to TrainedModel
                   .HasForeignKey(tm => tm.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}
