namespace KingdomHospital.Application.DTOs.Medicaments;
//Pas demandé, facultatif
public record MedicamentUpdateDto(
    string Name,
    string DosageForm,
    string Strength,
    string? AtcCode
);
