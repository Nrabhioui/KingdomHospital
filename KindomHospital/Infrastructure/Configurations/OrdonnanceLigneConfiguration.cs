using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

public class OrdonnanceLigneConfiguration : IEntityTypeConfiguration<OrdonnanceLigne>
{
    public void Configure(EntityTypeBuilder<OrdonnanceLigne> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Dosage)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.Frequency)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.Duration)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(l => l.Quantity)
            .IsRequired();

        builder.Property(l => l.Instructions)
            .HasMaxLength(255);

        builder.HasOne(l => l.Ordonnance)
            .WithMany(o => o.Lignes)
            .HasForeignKey(l => l.OrdonnanceId);

        builder.HasOne(l => l.Medicament)
            .WithMany(m => m.OrdonnanceLignes)
            .HasForeignKey(l => l.MedicamentId);
    }
}
