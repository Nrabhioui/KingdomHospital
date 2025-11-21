using KingdomHospital.Application.DTOs.Specialties;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

public class SpecialtyService
{
    private readonly SpecialtyRepository _repository;
    private readonly SpecialtyMapper _mapper;

    public SpecialtyService(SpecialtyRepository repository, SpecialtyMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SpecialtyDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(_mapper.ToDto);
    }

    public async Task<SpecialtyDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.ToDto(entity);
    }

    public async Task<SpecialtyDto> CreateAsync(SpecialtyCreateDto dto)
    {
        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(dto.Name));

        var exists = await _repository.ExistsByNameAsync(name);
        if (exists)
            throw new InvalidOperationException("A specialty with the same name already exists.");

        // On recrée un DTO "propre" (Name déjà trimé)
        var entity = _mapper.ToEntity(new SpecialtyCreateDto(name));

        await _repository.AddAsync(entity);

        return _mapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, SpecialtyUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(dto.Name));

        var exists = await _repository.ExistsByNameAsync(name, excludeId: id);
        if (exists)
            throw new InvalidOperationException("A specialty with the same name already exists.");

        // On met à jour l'entité suivie par le contexte
        _mapper.UpdateEntity(new SpecialtyUpdateDto(name), entity);

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        // Règle métier de la checklist : pas supprimer si médecins associés 
        var hasDoctors = await _repository.HasDoctorsAsync(id);
        if (hasDoctors)
            throw new InvalidOperationException("Cannot delete a specialty with doctors.");

        await _repository.RemoveAsync(entity);
        return true;
    }
}
