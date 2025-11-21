namespace KingdomHospital.Application.DTOs.Ordonnances;

public record OrdonnanceDto(
    int Id,
    int DoctorId,
    int PatientId,
    int? ConsultationId,
    DateOnly Date,
    string? Notes
);
