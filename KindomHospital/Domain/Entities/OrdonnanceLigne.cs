namespace KingdomHospital.Domain.Entities;

public class OrdonnanceLigne
{
    public int Id { get; set; }

    public int OrdonnanceId { get; set; }
    public int MedicamentId { get; set; }

    public string Dosage { get; set; } = default!;      // ≤ 50 chars
    public string Frequency { get; set; } = default!;   // ≤ 50 chars
    public string Duration { get; set; } = default!;    // ≤ 30 chars

    public int Quantity { get; set; }                   // > 0 (on validera côté service/EF)

    public string? Instructions { get; set; }           // ≤ 255 chars

    // Navigation
    public Ordonnance Ordonnance { get; set; } = default!;
    public Medicament Medicament { get; set; } = default!;
}
