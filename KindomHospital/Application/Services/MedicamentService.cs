using KingdomHospital.Application.DTOs.Medicaments;
using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

public class MedicamentService
{
    private readonly MedicamentRepository _repository;
    private readonly MedicamentMapper _mapper;
    private readonly OrdonnanceMapper _ordonnanceMapper;

    public MedicamentService(
        MedicamentRepository repository,
        MedicamentMapper mapper,
        OrdonnanceMapper ordonnanceMapper)
    {
        _repository = repository;
        _mapper = mapper;
        _ordonnanceMapper = ordonnanceMapper;
    }

    public async Task<IEnumerable<MedicamentDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(_mapper.ToDto);
    }

    public async Task<MedicamentDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.ToDto(entity);
    }

    public async Task<MedicamentDto> CreateAsync(MedicamentCreateDto dto)
    {
        var name = dto.Name.Trim();
        var dosageForm = dto.DosageForm.Trim();
        var strength = dto.Strength.Trim();
        var atcCode = dto.AtcCode?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(dto.Name));

        if (string.IsNullOrWhiteSpace(dosageForm))
            throw new ArgumentException("DosageForm is required.", nameof(dto.DosageForm));

        if (string.IsNullOrWhiteSpace(strength))
            throw new ArgumentException("Strength is required.", nameof(dto.Strength));

        
        if (name.Length > 100) name = name[..100];
        if (dosageForm.Length > 30) dosageForm = dosageForm[..30];
        if (strength.Length > 30) strength = strength[..30];
        if (!string.IsNullOrEmpty(atcCode) && atcCode.Length > 20)
            atcCode = atcCode[..20];

        var exists = await _repository.ExistsByNameAsync(name);
        if (exists)
            throw new InvalidOperationException("A medicament with the same name already exists.");

        var entity = _mapper.ToEntity(new MedicamentCreateDto(
            name,
            dosageForm,
            strength,
            atcCode
        ));

        await _repository.AddAsync(entity);

        return _mapper.ToDto(entity);
    }

    

    public async Task<IEnumerable<OrdonnanceDto>> GetOrdonnancesByMedicamentAsync(int medicamentId)
    {
        var exists = await _repository.MedicamentExistsAsync(medicamentId);
        if (!exists)
            throw new InvalidOperationException("Medicament not found.");

        var ordos = await _repository.GetOrdonnancesByMedicamentAsync(medicamentId);
        return ordos.Select(_ordonnanceMapper.ToDto);
    }
}
