namespace KingdomHospital.Application.DTOs.OrdonnanceLignes;

public record OrdonnanceLigneCreateDto(
    int MedicamentId,
    string Dosage,
    string Frequency,
    string Duration,
    int Quantity,
    string? Instructions
);
