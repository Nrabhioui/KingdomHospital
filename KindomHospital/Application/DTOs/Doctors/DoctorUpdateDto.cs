namespace KingdomHospital.Application.DTOs.Doctors;

public record DoctorUpdateDto(
    string LastName,
    string FirstName,
    int SpecialtyId
);
