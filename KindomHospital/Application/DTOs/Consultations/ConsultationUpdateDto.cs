namespace KingdomHospital.Application.DTOs.Consultations;

public record ConsultationUpdateDto(
    int DoctorId,
    int PatientId,
    DateOnly Date,
    TimeOnly Hour,
    string? Reason
);
