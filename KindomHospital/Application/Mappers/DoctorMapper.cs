using KingdomHospital.Application.DTOs.Doctors;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

[Mapper]
public partial class DoctorMapper
{
    public partial DoctorDto ToDto(Doctor entity);

    public partial Doctor ToEntity(DoctorCreateDto dto);

    public partial void UpdateEntity(DoctorUpdateDto dto, Doctor entity);
}
