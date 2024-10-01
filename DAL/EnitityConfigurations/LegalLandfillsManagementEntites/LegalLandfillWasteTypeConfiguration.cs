using Entities.LegalLandfillsManagementEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.LegalLandfillsManagementEntites
{
    public class LegalLandfillWasteTypeConfiguration : IEntityTypeConfiguration<LegalLandfillWasteType>
    {
        public void Configure(EntityTypeBuilder<LegalLandfillWasteType> builder)
        {
            builder.Property(x => x.Name).IsRequired();
        }
    }
}
