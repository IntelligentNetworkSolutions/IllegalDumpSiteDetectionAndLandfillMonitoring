using Entities.LegalLandfillsManagementEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.LegalLandfillsManagementEntites
{
    public class LegalLandfillTruckConfiguration : IEntityTypeConfiguration<LegalLandfillTruck>
    {
        public void Configure(EntityTypeBuilder<LegalLandfillTruck> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.IsEnabled).IsRequired();

        }
    }
}
