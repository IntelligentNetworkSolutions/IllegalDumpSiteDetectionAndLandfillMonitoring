using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations
{
    public class DatasetImageConfiguration : IEntityTypeConfiguration<DatasetImage>
    {
        public void Configure(EntityTypeBuilder<DatasetImage> builder)
        {
            builder.Property(di => di.FileName).IsRequired();
            builder.Property(di => di.ImagePath).IsRequired();
            builder.Property(di => di.IsEnabled).HasDefaultValue(false);

            builder.HasOne(di => di.Dataset)
                .WithOne()
                .HasForeignKey<DatasetImage>(di => di.DatasetId);

            builder.Property(di => di.CreatedById).IsRequired();
            builder.Property(di => di.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.Property(di => di.UpdatedById);
            builder.Property(di => di.UpdatedOn);

            builder.HasOne(di => di.CreatedBy)
                .WithOne()
                .HasForeignKey<DatasetImage>(di => di.CreatedById);

            builder.HasOne(di => di.UpdatedBy)
                .WithOne()
                .HasForeignKey<DatasetImage>(di => di.UpdatedById);
        }
    }
}
