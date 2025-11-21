namespace KingdomHospital.Application.DTOs.Patients;

public record PatientDto(
    int Id,
    string LastName,
    string FirstName,
    DateOnly BirthDate
);
