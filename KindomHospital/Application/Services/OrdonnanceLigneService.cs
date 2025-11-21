using KingdomHospital.Application.DTOs.OrdonnanceLignes;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

public class OrdonnanceLigneService
{
    private readonly OrdonnanceLigneRepository _repository;
    private readonly OrdonnanceLigneMapper _mapper;

    public OrdonnanceLigneService(OrdonnanceLigneRepository repository, OrdonnanceLigneMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrdonnanceLigneDto>> GetForOrdonnanceAsync(int ordonnanceId)
    {
        var lignes = await _repository.GetForOrdonnanceAsync(ordonnanceId);
        return lignes.Select(_mapper.ToDto);
    }

    public async Task<OrdonnanceLigneDto?> GetByIdAsync(int ordonnanceId, int ligneId)
    {
        var ligne = await _repository.GetByIdAsync(ordonnanceId, ligneId);
        return ligne is null ? null : _mapper.ToDto(ligne);
    }

    public async Task<OrdonnanceLigneDto> CreateAsync(int ordonnanceId, OrdonnanceLigneCreateDto dto)
    {
        var ordonnanceExists = await _repository.OrdonnanceExistsAsync(ordonnanceId);
        if (!ordonnanceExists)
            throw new InvalidOperationException("Ordonnance does not exist.");

        var medicamentExists = await _repository.MedicamentExistsAsync(dto.MedicamentId);
        if (!medicamentExists)
            throw new InvalidOperationException("Medicament does not exist.");

        ValidateBusiness(dto.Dosage, dto.Frequency, dto.Duration, dto.Quantity, dto.Instructions);

        var clean = CleanDto(dto);

        var existsSame = await _repository.ExistsSameAsync(
            ordonnanceId,
            clean.MedicamentId,
            clean.Dosage,
            clean.Frequency,
            clean.Duration);

        if (existsSame)
            throw new InvalidOperationException("An identical prescription line already exists for this ordonnance.");

        var entity = _mapper.ToEntity(clean);
        entity.OrdonnanceId = ordonnanceId;

        await _repository.AddAsync(entity);

        return _mapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int ordonnanceId, int ligneId, OrdonnanceLigneUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(ordonnanceId, ligneId);
        if (entity is null)
            return false;

        ValidateBusiness(dto.Dosage, dto.Frequency, dto.Duration, dto.Quantity, dto.Instructions);

        var clean = CleanDto(dto);

        _mapper.UpdateEntity(clean, entity);

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int ordonnanceId, int ligneId)
    {
        var entity = await _repository.GetByIdAsync(ordonnanceId, ligneId);
        if (entity is null)
            return false;

        await _repository.RemoveAsync(entity);
        return true;
    }

    private static void ValidateBusiness(
        string dosage,
        string frequency,
        string duration,
        int quantity,
        string? instructions)
    {
        if (string.IsNullOrWhiteSpace(dosage))
            throw new ArgumentException("Dosage is required.", nameof(dosage));

        if (string.IsNullOrWhiteSpace(frequency))
            throw new ArgumentException("Frequency is required.", nameof(frequency));

        if (string.IsNullOrWhiteSpace(duration))
            throw new ArgumentException("Duration is required.", nameof(duration));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

        if (dosage.Trim().Length > 50)
            throw new ArgumentException("Dosage must be at most 50 characters.", nameof(dosage));

        if (frequency.Trim().Length > 50)
            throw new ArgumentException("Frequency must be at most 50 characters.", nameof(frequency));

        if (duration.Trim().Length > 30)
            throw new ArgumentException("Duration must be at most 30 characters.", nameof(duration));

        if (!string.IsNullOrWhiteSpace(instructions) && instructions.Trim().Length > 255)
            throw new ArgumentException("Instructions must be at most 255 characters.", nameof(instructions));
    }

    private static OrdonnanceLigneCreateDto CleanDto(OrdonnanceLigneCreateDto dto)
    {
        return new OrdonnanceLigneCreateDto(
            dto.MedicamentId,
            dto.Dosage.Trim(),
            dto.Frequency.Trim(),
            dto.Duration.Trim(),
            dto.Quantity,
            dto.Instructions?.Trim()
        );
    }

    private static OrdonnanceLigneUpdateDto CleanDto(OrdonnanceLigneUpdateDto dto)
    {
        return new OrdonnanceLigneUpdateDto(
            dto.Dosage.Trim(),
            dto.Frequency.Trim(),
            dto.Duration.Trim(),
            dto.Quantity,
            dto.Instructions?.Trim()
        );
    }
}
