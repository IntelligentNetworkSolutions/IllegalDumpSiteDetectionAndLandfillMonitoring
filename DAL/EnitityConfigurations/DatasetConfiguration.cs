using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations
{
    public  class DatasetConfiguration : IEntityTypeConfiguration<Dataset>
    {
        public void Configure(EntityTypeBuilder<Dataset> builder)
        {
            builder.Property(c => c.Name).IsRequired();

            builder.Property(c => c.Description).IsRequired();

            builder.Property(d => d.IsPublished).HasDefaultValue(false);

            builder.Property(d => d.CreatedById).IsRequired();
            builder.Property(d => d.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.Property(d => d.UpdatedById);
            builder.Property(d => d.UpdatedOn);

            builder.HasOne<Dataset>(d => d.ParentDataset)
                    .WithOne()
                    .HasForeignKey<Dataset>(d => d.ParentDatasetId);

            builder.HasOne<ApplicationUser>(d => d.CreatedBy)
                    .WithOne()
                    .HasForeignKey<Dataset>(d => d.CreatedById);

            builder.HasOne<ApplicationUser>(d => d.UpdatedBy)
                    .WithOne()
                    .HasForeignKey<Dataset>(d => d.UpdatedById);
        }
    }
}
