
using KingdomHospital.Application.DTOs.Specialties;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

[Mapper]
public partial class SpecialtyMapper
{
    public partial SpecialtyDto ToDto(Specialty entity);
    public partial Specialty ToEntity(SpecialtyCreateDto dto);
    public partial void UpdateEntity(SpecialtyUpdateDto dto, Specialty entity);
}
