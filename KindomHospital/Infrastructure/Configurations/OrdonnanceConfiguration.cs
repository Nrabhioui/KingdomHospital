using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

public class OrdonnanceConfiguration : IEntityTypeConfiguration<Ordonnance>
{
    public void Configure(EntityTypeBuilder<Ordonnance> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Date)
            .IsRequired();

        builder.Property(o => o.Notes)
            .HasMaxLength(255);

        builder.HasOne(o => o.Doctor)
            .WithMany(d => d.Ordonnances)
            .HasForeignKey(o => o.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);    // ⬅  FOREIGN KEY constraint 'FK_Ordonnances_Doctors_DoctorId'

        builder.HasOne(o => o.Patient)
            .WithMany(p => p.Ordonnances)
            .HasForeignKey(o => o.PatientId)
            .OnDelete(DeleteBehavior.Restrict);    // ⬅  FOREIGN KEY constraint 'FK_Ordonnances_Doctors_DoctorId'

        builder.HasOne(o => o.Consultation)
            .WithMany(c => c.Ordonnances)
            .HasForeignKey(o => o.ConsultationId)
            .OnDelete(DeleteBehavior.SetNull);     // OK pour “ordonnance sans consultation”
    }
}
