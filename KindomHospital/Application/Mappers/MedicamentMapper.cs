using KingdomHospital.Application.DTOs.Medicaments;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

[Mapper]
public partial class MedicamentMapper
{
    public partial MedicamentDto ToDto(Medicament entity);

    public partial Medicament ToEntity(MedicamentCreateDto dto);

    public partial void UpdateEntity(MedicamentUpdateDto dto, Medicament entity); 
}
