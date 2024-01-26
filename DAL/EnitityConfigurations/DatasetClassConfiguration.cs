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
    public class DatasetClassConfiguration : IEntityTypeConfiguration<DatasetClass>
    {
        public void Configure(EntityTypeBuilder<DatasetClass> builder)
        {
            builder.Property(dc => dc.ClassId).IsRequired();
            builder.Property(dc => dc.ClassName).IsRequired();
            builder.Property(dc => dc.CreatedById).IsRequired();

            builder.Property(dc => dc.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.HasOne(dc => dc.Dataset)
                .WithOne()
                .HasForeignKey<DatasetClass>(dc => dc.DatasetId);

            builder.HasOne(dc => dc.CreatedBy)
                .WithOne()
                .HasForeignKey<DatasetClass>(dc => dc.CreatedById);
        }
    }
}
