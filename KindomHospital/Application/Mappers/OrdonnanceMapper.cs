using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

[Mapper]
public partial class OrdonnanceMapper
{
    public partial OrdonnanceDto ToDto(Ordonnance entity);

    public partial Ordonnance ToEntity(OrdonnanceCreateDto dto);

    public partial void UpdateEntity(OrdonnanceUpdateDto dto, Ordonnance entity);
}
