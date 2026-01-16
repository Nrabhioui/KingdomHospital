namespace KingdomHospital.Application.DTOs.Medicaments;

public record MedicamentUpdateDto(
    string Name,
    string DosageForm,
    string Strength,
    string? AtcCode
);
