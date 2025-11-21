namespace KingdomHospital.Application.DTOs.Ordonnances;

public record OrdonnanceCreateDto(
    int DoctorId,
    int PatientId,
    int? ConsultationId,
    DateOnly Date,
    string? Notes
);
