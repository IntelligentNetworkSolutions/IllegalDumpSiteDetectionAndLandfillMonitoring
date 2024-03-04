using Entities;
using Entities.DatasetEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EnitityConfigurations
{
    public class Dataset_DatasetClassConfiguration : IEntityTypeConfiguration<Dataset_DatasetClass>
    {
        public void Configure(EntityTypeBuilder<Dataset_DatasetClass> builder)
        {
            builder.Property(c => c.DatasetClassId).IsRequired();
            builder.Property(c => c.DatasetId).IsRequired();
            builder.HasOne(d => d.Dataset)
                    .WithMany(d => d.DatasetClasses)
                    .HasForeignKey(d => d.DatasetId);
            builder.HasOne(d => d.DatasetClass)
                    .WithMany(d => d.Datasets)
                    .HasForeignKey(d => d.DatasetClassId);
        }
    }
}
