using KingdomHospital.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KingdomHospital.Infrastructure.Configurations;

public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Date).IsRequired();
        builder.Property(c => c.Hour).IsRequired();

        builder.Property(c => c.Reason)
            .HasMaxLength(100);

        builder.HasOne(c => c.Doctor)
            .WithMany(d => d.Consultations)
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);   // ⬅ IMPORTANT Introducing FOREIGN KEY constraint 'FK_Ordonnances_Doctors_DoctorId' on table 'Ordonnances' may cause cycles or multiple cascade paths. Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.  Could not create constraint or index. See previous errors.

                builder.HasOne(c => c.Patient)
            .WithMany(p => p.Consultations)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);   // ⬅ IMPORTANT

        builder.HasIndex(c => new { c.DoctorId, c.Date, c.Hour })
            .IsUnique();
    }
}
