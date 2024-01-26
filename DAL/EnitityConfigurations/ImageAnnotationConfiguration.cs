using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations
{
    public class ImageAnnotationConfiguration : IEntityTypeConfiguration<ImageAnnotation>
    {
        public void Configure(EntityTypeBuilder<ImageAnnotation> builder)
        {
            builder.Property(ia => ia.AnnotationsGeoJson).IsRequired();
            builder.Property(ia => ia.Geom).IsRequired();
            builder.Property(ia => ia.IsEnabled).HasDefaultValue(false);

            builder.HasOne(ia => ia.DatasetImage)
                .WithOne()
                .HasForeignKey<ImageAnnotation>(ia => ia.DatasetImageId);

            builder.Property(ia => ia.CreatedById).IsRequired();
            builder.Property(ia => ia.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.Property(ia => ia.UpdatedById);
            builder.Property(ia => ia.UpdatedOn);

            builder.HasOne(ia => ia.CreatedBy)
                .WithOne()
                .HasForeignKey<ImageAnnotation>(ia => ia.CreatedById);

            builder.HasOne(ia => ia.UpdatedBy)
                .WithOne()
                .HasForeignKey<ImageAnnotation>(ia => ia.UpdatedById);
        }
    }
}
