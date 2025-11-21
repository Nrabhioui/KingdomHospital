namespace KingdomHospital.Application.DTOs.OrdonnanceLignes;

public record OrdonnanceLigneDto(
    int Id,
    int OrdonnanceId,
    int MedicamentId,
    string Dosage,
    string Frequency,
    string Duration,
    int Quantity,
    string? Instructions
);
