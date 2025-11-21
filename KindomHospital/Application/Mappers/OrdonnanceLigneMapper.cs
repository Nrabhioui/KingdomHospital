using KingdomHospital.Application.DTOs.OrdonnanceLignes;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

[Mapper]
public partial class OrdonnanceLigneMapper
{
    public partial OrdonnanceLigneDto ToDto(OrdonnanceLigne entity);

    public partial OrdonnanceLigne ToEntity(OrdonnanceLigneCreateDto dto);

    public partial void UpdateEntity(OrdonnanceLigneUpdateDto dto, OrdonnanceLigne entity);
}
