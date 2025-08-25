using Entities.RegisteredDumpsiteEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteStatusConfiguration : IEntityTypeConfiguration<RegisteredDumpsiteStatus>
{
    public void Configure(EntityTypeBuilder<RegisteredDumpsiteStatus> builder)
    {
        builder.Property(rd => rd.Name).IsRequired();

        builder.HasData(
            new RegisteredDumpsiteStatus
            {
                Id = 1,
                Name = "Detected",
                Description = "Waste has been detected.",
                Color = "#FF0000"
            },
            new RegisteredDumpsiteStatus
            {
                Id = 2,
                Name = "InProcess",
                Description = "Cleanup is in progress.",
                Color = "#FFA500"
            },
            new RegisteredDumpsiteStatus
            {
                Id = 3,
                Name = "Resolved",
                Description = "Waste has been resolved.",
                Color = "#008000"
            }
        );
    }
}
