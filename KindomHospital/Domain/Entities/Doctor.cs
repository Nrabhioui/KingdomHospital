namespace KingdomHospital.Domain.Entities;

public class Doctor
{
    public int Id { get; set; }

    public int SpecialtyId { get; set; }

    public string LastName { get; set; } = default!;
    public string FirstName { get; set; } = default!;

    public Specialty Specialty { get; set; } = default!;

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
    public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
}
