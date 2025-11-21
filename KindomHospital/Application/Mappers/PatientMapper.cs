using KingdomHospital.Application.DTOs.Patients;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

[Mapper]
public partial class PatientMapper
{
    public partial PatientDto ToDto(Patient entity);

    public partial Patient ToEntity(PatientCreateDto dto);

    public partial void UpdateEntity(PatientUpdateDto dto, Patient entity);
}
