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
    public class MapLayerGroupConfigConfiguration : IEntityTypeConfiguration<MapLayerGroupConfiguration>
    {
        public void Configure(EntityTypeBuilder<MapLayerGroupConfiguration> builder)
        {
            builder.Property(c => c.GroupName).IsRequired();
            builder.Property(c => c.LayerGroupTitleJson).IsRequired();
            builder.Property(c => c.LayerGroupDescriptionJson).IsRequired();
            builder.Property(c => c.Order).IsRequired();
            builder.Property(c => c.Opacity).IsRequired();
            builder.Property(c => c.GroupJs).IsRequired();

            builder.Property(c => c.MapConfigurationId);

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
                   .WithMany(d => d.MapLayerGroupConfigurations)
                   .HasForeignKey(d => d.MapConfigurationId);

        }
    }
}
