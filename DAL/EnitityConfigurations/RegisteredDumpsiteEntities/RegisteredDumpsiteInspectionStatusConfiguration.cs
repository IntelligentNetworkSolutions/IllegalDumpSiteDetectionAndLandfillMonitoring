using Entities.RegisteredDumpsiteEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteInspectionStatusConfiguration : IEntityTypeConfiguration<RegisteredDumpsiteInspectionStatus>
{
    public void Configure(EntityTypeBuilder<RegisteredDumpsiteInspectionStatus> builder)
    {
        builder.Property(rd => rd.Name).IsRequired();

        builder.HasData(
            new RegisteredDumpsiteInspectionStatus
            {
                Id = 1,
                Name = "Scheduled",
                Description = "Cleanup has been scheduled.",
                Color = "#1E90FF"
            },
            new RegisteredDumpsiteInspectionStatus
            {
                Id = 2,
                Name = "Assigned",
                Description = "Cleanup task has been assigned.",
                Color = "#800080"
            },
            new RegisteredDumpsiteInspectionStatus
            {
                Id = 3,
                Name = "InProgress",
                Description = "Cleanup is currently in progress.",
                Color = "#FFD700"
            },
            new RegisteredDumpsiteInspectionStatus
            {
                Id = 4,
                Name = "Completed",
                Description = "Cleanup has been completed.",
                Color = "#228B22"
            },
            new RegisteredDumpsiteInspectionStatus
            {
                Id = 5,
                Name = "Cancelled",
                Description = "Cleanup has been cancelled.",
                Color = "#A9A9A9"
            }
        );
    }
}
