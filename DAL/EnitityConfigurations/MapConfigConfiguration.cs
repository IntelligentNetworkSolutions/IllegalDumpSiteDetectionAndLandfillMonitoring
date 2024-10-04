using Entities;
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
    public class MapConfigConfiguration : IEntityTypeConfiguration<MapConfiguration>
    {
        public void Configure(EntityTypeBuilder<MapConfiguration> builder)
        {
            builder.Property(c => c.MapName).IsRequired();
            builder.Property(c => c.Projection).IsRequired();
            builder.Property(c => c.TileGridJs).IsRequired();
            builder.Property(c => c.MaxX).IsRequired();
            builder.Property(c => c.MaxY).IsRequired();
            builder.Property(c => c.MinY).IsRequired();
            builder.Property(c => c.MinX).IsRequired();
            builder.Property(c => c.CenterX).IsRequired();
            builder.Property(c => c.CenterY).IsRequired();
            builder.Property(c => c.DefaultResolution).IsRequired();
            builder.Property(c => c.Resolutions).IsRequired();

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

        }
    }
}
