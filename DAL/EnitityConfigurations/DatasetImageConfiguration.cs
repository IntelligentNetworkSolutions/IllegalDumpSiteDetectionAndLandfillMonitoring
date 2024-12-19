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
            builder.Property(di => di.Width).IsRequired();
            builder.Property(di => di.Height).IsRequired();

            builder.HasOne(di => di.Dataset)
                .WithMany(ds => ds.DatasetImages)
                .HasForeignKey(di => di.DatasetId);

            builder.Property(di => di.CreatedById).IsRequired();
            builder.Property(di => di.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.Property(di => di.UpdatedById);
            builder.Property(di => di.UpdatedOn);

            builder.HasOne(di => di.CreatedBy)
                .WithMany()
                .HasForeignKey(di => di.CreatedById);

            builder.HasOne(di => di.UpdatedBy)
                .WithMany()
                .HasForeignKey(di => di.UpdatedById);
        }
    }
}