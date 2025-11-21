namespace KingdomHospital.Application.DTOs.Consultations;

public record ConsultationCreateDto(
    int DoctorId,
    int PatientId,
    DateOnly Date,
    TimeOnly Hour,
    string? Reason
);
