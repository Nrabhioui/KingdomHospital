namespace KingdomHospital.Domain.Entities;

public class OrdonnanceLigne
{
    public int Id { get; set; }

    public int OrdonnanceId { get; set; }
    public int MedicamentId { get; set; }

    public string Dosage { get; set; } = default!;      
    public string Frequency { get; set; } = default!;  
    public string Duration { get; set; } = default!;    

    public int Quantity { get; set; }                  

    public string? Instructions { get; set; }           

    
    public Ordonnance Ordonnance { get; set; } = default!;
    public Medicament Medicament { get; set; } = default!;
}
