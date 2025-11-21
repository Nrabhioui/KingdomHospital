using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.BirthDate)
            .IsRequired();

        builder.HasMany(p => p.Consultations)
            .WithOne(c => c.Patient)
            .HasForeignKey(c => c.PatientId);

        builder.HasMany(p => p.Ordonnances)
            .WithOne(o => o.Patient)
            .HasForeignKey(o => o.PatientId);
    }
}
