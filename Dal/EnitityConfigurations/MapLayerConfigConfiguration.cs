using Entities.MapConfigurationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EnitityConfigurations
{
    public class MapLayerConfigConfiguration : IEntityTypeConfiguration<MapLayerConfiguration>
    {
        public void Configure(EntityTypeBuilder<MapLayerConfiguration> builder)
        {
            builder.Property(c => c.LayerName).IsRequired();
            builder.Property(c => c.LayerTitleJson).IsRequired();
            builder.Property(c => c.LayerDescriptionJson).IsRequired();
            builder.Property(c => c.Order).IsRequired();
            builder.Property(c => c.LayerJs).IsRequired();

            builder.Property(c => c.MapConfigurationId);
            builder.Property(c => c.MapLayerGroupConfigurationId);

            builder.Property(d => d.CreatedById).IsRequired();
            builder.Property(d => d.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.Property(d => d.UpdatedById);
            builder.Property(d => d.UpdatedOn);

            builder.HasOne(d => d.CreatedBy)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedById);

            builder.HasOne(d => d.UpdatedBy)
                    .WithMany()
                    .HasForeignKey(d => d.UpdatedById);

            builder.HasOne(d => d.MapConfiguration)
                    .WithMany(d => d.MapLayerConfigurations)
                    .HasForeignKey(d => d.MapConfigurationId);

            builder.HasOne(d => d.MapLayerGroupConfiguration)
                    .WithMany(d => d.MapLayerConfigurations)
                    .HasForeignKey(d => d.MapLayerGroupConfigurationId);

        }
    }
}
