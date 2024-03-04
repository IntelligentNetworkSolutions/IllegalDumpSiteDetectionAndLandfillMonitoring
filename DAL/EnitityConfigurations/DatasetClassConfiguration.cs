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
    public class DatasetClassConfiguration : IEntityTypeConfiguration<DatasetClass>
    {
        public void Configure(EntityTypeBuilder<DatasetClass> builder)
        {
            builder.Property(dc => dc.ParentClassId).IsRequired(false);
            builder.Property(dc => dc.ClassName).IsRequired();
            builder.Property(dc => dc.CreatedById).IsRequired();

            builder.Property(dc => dc.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.HasOne<DatasetClass>(d => d.ParentClass)
                .WithMany()
                .HasForeignKey(d => d.ParentClassId);

            builder.HasOne<ApplicationUser>(d => d.CreatedBy)
                .WithMany()
                .HasForeignKey(d => d.CreatedById);

        }
    }
}
