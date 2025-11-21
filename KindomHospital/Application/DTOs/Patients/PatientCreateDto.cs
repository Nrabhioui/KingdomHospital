namespace KingdomHospital.Application.DTOs.Patients;

public record PatientCreateDto(
    string LastName,
    string FirstName,
    DateOnly BirthDate
);
