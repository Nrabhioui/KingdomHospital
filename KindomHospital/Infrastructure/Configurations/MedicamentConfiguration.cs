using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

public class MedicamentConfiguration : IEntityTypeConfiguration<Medicament>
{
    public void Configure(EntityTypeBuilder<Medicament> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(m => m.Name).IsUnique();

        builder.Property(m => m.DosageForm)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(m => m.Strength)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(m => m.AtcCode)
            .HasMaxLength(20);
    }
}
