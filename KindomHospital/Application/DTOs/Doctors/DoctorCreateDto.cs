namespace KingdomHospital.Application.DTOs.Doctors;

public record DoctorCreateDto(
    string LastName,
    string FirstName,
    int SpecialtyId
);
