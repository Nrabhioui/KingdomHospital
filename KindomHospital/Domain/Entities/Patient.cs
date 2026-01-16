namespace KingdomHospital.Domain.Entities;

public class Patient
{
    public int Id { get; set; }

    public string LastName { get; set; } = default!;
    public string FirstName { get; set; } = default!;

   
    public DateOnly BirthDate { get; set; }

 
    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
    public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
}
