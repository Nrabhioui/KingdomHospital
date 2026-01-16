using KingdomHospital.Application.DTOs.Consultations;
using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.DTOs.Patients;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

public class PatientService
{
    private readonly PatientRepository _repository;
    private readonly PatientMapper _mapper;
    private readonly ConsultationMapper _consultationMapper;
    private readonly OrdonnanceMapper _ordonnanceMapper;

    public PatientService(
        PatientRepository repository,
        PatientMapper mapper,
        ConsultationMapper consultationMapper,
        OrdonnanceMapper ordonnanceMapper)
    {
        _repository = repository;
        _mapper = mapper;
        _consultationMapper = consultationMapper;
        _ordonnanceMapper = ordonnanceMapper;
    }


    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(_mapper.ToDto);
    }

    public async Task<PatientDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.ToDto(entity);
    }

    public async Task<PatientDto> CreateAsync(PatientCreateDto dto)
    {
        var lastName = dto.LastName.Trim();
        var firstName = dto.FirstName.Trim();

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName is required.");

        if (!IsValidBirthDate(dto.BirthDate))
            throw new ArgumentException("Invalid BirthDate.");

        var exists = await _repository.ExistsByIdentityAsync(lastName, firstName, dto.BirthDate);
        if (exists)
            throw new InvalidOperationException("A patient with the same name and birth date already exists.");

        var entity = _mapper.ToEntity(new PatientCreateDto(lastName, firstName, dto.BirthDate));
        await _repository.AddAsync(entity);

        return _mapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, PatientUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        var lastName = dto.LastName.Trim();
        var firstName = dto.FirstName.Trim();

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName is required.");

        if (!IsValidBirthDate(dto.BirthDate))
            throw new ArgumentException("Invalid BirthDate.");

        var exists = await _repository.ExistsByIdentityAsync(lastName, firstName, dto.BirthDate, excludeId: id);
        if (exists)
            throw new InvalidOperationException("A patient with the same identity already exists.");

        _mapper.UpdateEntity(new PatientUpdateDto(lastName, firstName, dto.BirthDate), entity);

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        var hasConsultations = await _repository.HasConsultationsAsync(id);
        var hasOrdonnances = await _repository.HasOrdonnancesAsync(id);

        if (hasConsultations || hasOrdonnances)
            throw new InvalidOperationException("Cannot delete a patient with consultations or prescriptions.");

        await _repository.RemoveAsync(entity);
        return true;
    }

    private static bool IsValidBirthDate(DateOnly birthDate)
    {
        var min = new DateOnly(1900, 1, 1);
        var today = DateOnly.FromDateTime(DateTime.Today);
        return birthDate >= min && birthDate <= today;
    }



    public async Task<IEnumerable<ConsultationDto>> GetConsultationsByPatientAsync(int patientId)
    {
        var consultations = await _repository.GetConsultationsAsync(patientId);
        return consultations.Select(_consultationMapper.ToDto);
    }

    public async Task<IEnumerable<OrdonnanceDto>> GetOrdonnancesByPatientAsync(int patientId)
    {
        var ordonnances = await _repository.GetOrdonnancesAsync(patientId);
        return ordonnances.Select(_ordonnanceMapper.ToDto);
    }
}
