namespace KingdomHospital.Domain.Entities;

public class Ordonnance
{
    public int Id { get; set; }

    public int DoctorId { get; set; }
    public int PatientId { get; set; }

    public int? ConsultationId { get; set; }

    public DateOnly Date { get; set; }

    public string? Notes { get; set; }

    public Doctor Doctor { get; set; } = default!;
    public Patient Patient { get; set; } = default!;
    public Consultation? Consultation { get; set; }

    public ICollection<OrdonnanceLigne> Lignes { get; set; } = new List<OrdonnanceLigne>();
}
