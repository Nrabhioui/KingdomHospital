namespace KingdomHospital.Application.DTOs.Ordonnances;

public record OrdonnanceUpdateDto(
    int DoctorId,
    int PatientId,
    int? ConsultationId,
    DateOnly Date,
    string? Notes
);
