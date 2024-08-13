using Entities.LegalLandfillsManagementEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EnitityConfigurations.LegalLandfillsManagementEntites
{
    public class LegalLandfillPointCloudFileConfiguration : IEntityTypeConfiguration<LegalLandfillPointCloudFile>
    {
        public void Configure(EntityTypeBuilder<LegalLandfillPointCloudFile> builder)
        {
            builder.Property(x => x.FileName).IsRequired();
            builder.Property(x => x.FilePath).IsRequired();
            builder.Property(x => x.ScanDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");           
            builder.HasOne(x => x.LegalLandfill)
            .WithMany(l => l.LegalLandfillPointCloudFiles)
            .HasForeignKey(x => x.LegalLandfillId);
        }
    }
}
