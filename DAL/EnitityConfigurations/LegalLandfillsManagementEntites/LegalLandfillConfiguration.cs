using Entities.DatasetEntities;
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
    public class LegalLandfillConfiguration : IEntityTypeConfiguration<LegalLandfill>
    {
        public void Configure(EntityTypeBuilder<LegalLandfill> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Description).IsRequired();
        }
    }
}
