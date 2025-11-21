namespace KingdomHospital.Application.DTOs.Consultations;

public record ConsultationDto(
    int Id,
    int DoctorId,
    int PatientId,
    DateOnly Date,
    TimeOnly Hour,
    string? Reason
);
