using Entities.TrainingEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.TrainingEntities
{
    public class TrainedModelStatisticsConfiguration : IEntityTypeConfiguration<TrainedModelStatistics>
    {
        public void Configure(EntityTypeBuilder<TrainedModelStatistics> builder)
        {
            // Relationships
            // Configuring the one-to-one relationship with TrainedModel
            builder.HasOne(tms => tms.TrainedModel)
                   .WithOne(tm => tm.TrainedModelStatistics)
                   .HasForeignKey<TrainedModelStatistics>(tms => tms.TrainedModelId);

            // Configuring nullable properties
            builder.Property(tms => tms.TrainingDuration).IsRequired(false);
            builder.Property(tms => tms.TotalParameters).IsRequired(false);
            builder.Property(tms => tms.NumEpochs).IsRequired(false);
            builder.Property(tms => tms.LearningRate).IsRequired(false);
            builder.Property(tms => tms.AvgBoxLoss).IsRequired(false);
            builder.Property(tms => tms.mApp).IsRequired(false);
        }
    }
}
