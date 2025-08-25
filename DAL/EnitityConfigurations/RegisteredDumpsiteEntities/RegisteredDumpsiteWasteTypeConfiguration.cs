using Entities.RegisteredDumpsiteEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EnitityConfigurations.RegisteredDumpsiteEntities;

public class RegisteredDumpsiteWasteTypeConfiguration : IEntityTypeConfiguration<RegisteredDumpsiteWasteType>
{
    public void Configure(EntityTypeBuilder<RegisteredDumpsiteWasteType> builder)
    {
        builder.Property(wt => wt.Name).IsRequired();
        // Configuring user-related properties
        builder.Property(diz => diz.CreatedById).IsRequired();
        builder.Property(ia => ia.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

        // Configure the relationship with ApplicationUser
        builder.HasOne(diz => diz.CreatedBy)
               .WithMany() // Assuming there's no inverse navigation property
               .HasForeignKey(diz => diz.CreatedById)
               .OnDelete(DeleteBehavior.Restrict) // This prevents CASCADE delete when ApplicationUser is deleted
               .IsRequired();
    }
}
