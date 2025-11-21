namespace KingdomHospital.Application.DTOs.Patients;

public record PatientUpdateDto(
    string LastName,
    string FirstName,
    DateOnly BirthDate
);
