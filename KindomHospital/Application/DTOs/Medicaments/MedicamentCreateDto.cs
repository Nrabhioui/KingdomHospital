namespace KingdomHospital.Application.DTOs.Medicaments;

public record MedicamentCreateDto(
    string Name,
    string DosageForm,
    string Strength,
    string? AtcCode
);
