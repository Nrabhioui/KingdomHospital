namespace KingdomHospital.Domain.Entities;

public class Medicament
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
    public string DosageForm { get; set; } = default!;
    public string Strength { get; set; } = default!;

    public string? AtcCode { get; set; }

    // Navigation : 1 Medicament → N OrdonnanceLignes
    public ICollection<OrdonnanceLigne> OrdonnanceLignes { get; set; } = new List<OrdonnanceLigne>();
}
