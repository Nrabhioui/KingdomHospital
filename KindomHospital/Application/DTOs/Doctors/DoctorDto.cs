namespace KingdomHospital.Application.DTOs.Doctors;

public record DoctorDto(
    int Id,
    string LastName,
    string FirstName,
    int SpecialtyId
);
