namespace KingdomHospital.Application.DTOs.OrdonnanceLignes;

public record OrdonnanceLigneUpdateDto(
    string Dosage,
    string Frequency,
    string Duration,
    int Quantity,
    string? Instructions
);
