using Entities.RegisteredDumpsiteEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.RegisteredDumpsiteEntities;
public class InspectionAssignmentConfiguration : IEntityTypeConfiguration<InspectionAssignment>
{
    public void Configure(EntityTypeBuilder<InspectionAssignment> builder)
    {
        builder.HasKey(x => new { x.RegisteredDumpsiteInspectionId, x.InspectorId });

        builder.HasOne(x => x.RegisteredDumpsiteInspection)
               .WithMany(x => x.Assignments)
               .HasForeignKey(x => x.RegisteredDumpsiteInspectionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Inspector)
               .WithMany()
               .HasForeignKey(x => x.InspectorId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
