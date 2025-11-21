using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasOne(d => d.Specialty)
            .WithMany(s => s.Doctors)
            .HasForeignKey(d => d.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Consultations)
            .WithOne(c => c.Doctor)
            .HasForeignKey(c => c.DoctorId);

        builder.HasMany(d => d.Ordonnances)
            .WithOne(o => o.Doctor)
            .HasForeignKey(o => o.DoctorId);
    }
}
