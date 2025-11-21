namespace KingdomHospital.Application.DTOs.OrdonnanceLignes;

public record OrdonnanceLigneCreateDto(
    int MedicamentId,
    string Dosage,
    string Frequency,
    string Duration,
    int Quantity,
    string? Instructions
);



//on ne met pas OrdonnanceId ici, parce qu’on le passera dans l’URL (POST /api/ordonnances/{id}/lignes) et le service le fixera.