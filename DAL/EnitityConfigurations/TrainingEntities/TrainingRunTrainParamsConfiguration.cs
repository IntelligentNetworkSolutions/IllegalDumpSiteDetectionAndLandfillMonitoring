using Entities.TrainingEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.TrainingEntities
{
    public class TrainingRunTrainParamsConfiguration : IEntityTypeConfiguration<TrainingRunTrainParams>
    {
        public void Configure(EntityTypeBuilder<TrainingRunTrainParams> builder)
        {
            builder.HasOne(tp => tp.TrainingRun)
                   .WithOne(tr => tr.TrainParams)
                   .HasForeignKey<TrainingRunTrainParams>(tp => tp.TrainingRunId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
