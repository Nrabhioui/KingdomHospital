using KingdomHospital.Application.DTOs.Consultations;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

[Mapper]
public partial class ConsultationMapper
{
    public partial ConsultationDto ToDto(Consultation entity);

    public partial Consultation ToEntity(ConsultationCreateDto dto);

    public partial void UpdateEntity(ConsultationUpdateDto dto, Consultation entity);
}
