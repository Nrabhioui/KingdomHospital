using KingdomHospital.Application.DTOs.Consultations;
using KingdomHospital.Application.DTOs.Doctors;
using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.DTOs.Patients;
using KingdomHospital.Application.DTOs.Specialties;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

public class DoctorService
{
    private readonly DoctorRepository _repository;
    private readonly DoctorMapper _mapper;
    private readonly ConsultationMapper _consultationMapper;
    private readonly PatientMapper _patientMapper;
    private readonly OrdonnanceMapper _ordonnanceMapper;

    public DoctorService(
        DoctorRepository repository,
        DoctorMapper mapper,
        ConsultationMapper consultationMapper,
        PatientMapper patientMapper,
        OrdonnanceMapper ordonnanceMapper)
    {
        _repository = repository;
        _mapper = mapper;
        _consultationMapper = consultationMapper;
        _patientMapper = patientMapper;
        _ordonnanceMapper = ordonnanceMapper;
    }



    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(_mapper.ToDto);
    }

    public async Task<DoctorDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.ToDto(entity);
    }

    public async Task<DoctorDto> CreateAsync(DoctorCreateDto dto)
    {
        var last = dto.LastName.Trim();
        var first = dto.FirstName.Trim();

        if (string.IsNullOrWhiteSpace(last))
            throw new ArgumentException("LastName is required.");

        if (string.IsNullOrWhiteSpace(first))
            throw new ArgumentException("FirstName is required.");

        var specialtyExists = await _repository.SpecialtyExistsAsync(dto.SpecialtyId);
        if (!specialtyExists)
            throw new InvalidOperationException("Specialty does not exist.");

        var duplicate = await _repository.ExistsDuplicateAsync(last, first, dto.SpecialtyId);
        if (duplicate)
            throw new InvalidOperationException("A doctor with same name and specialty already exists.");

        var entity = _mapper.ToEntity(new DoctorCreateDto(last, first, dto.SpecialtyId));

        await _repository.AddAsync(entity);

        return _mapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, DoctorUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        var last = dto.LastName.Trim();
        var first = dto.FirstName.Trim();

        if (string.IsNullOrWhiteSpace(last))
            throw new ArgumentException("LastName is required.");

        if (string.IsNullOrWhiteSpace(first))
            throw new ArgumentException("FirstName is required.");

        var specialtyExists = await _repository.SpecialtyExistsAsync(dto.SpecialtyId);
        if (!specialtyExists)
            throw new InvalidOperationException("Specialty does not exist.");

        var duplicate = await _repository.ExistsDuplicateAsync(last, first, dto.SpecialtyId, id);
        if (duplicate)
            throw new InvalidOperationException("A doctor with same name and specialty already exists.");

        _mapper.UpdateEntity(new DoctorUpdateDto(last, first, dto.SpecialtyId), entity);

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        var hasConsult = await _repository.HasConsultationsAsync(id);
        var hasOrdo = await _repository.HasOrdonnancesAsync(id);

        if (hasConsult || hasOrdo)
            throw new InvalidOperationException("Cannot delete a doctor with consultations or prescriptions.");

        await _repository.RemoveAsync(entity);
        return true;
    }


    public async Task<IEnumerable<DoctorDto>> GetBySpecialtyAsync(int specialtyId)
    {
        var list = await _repository.GetBySpecialtyAsync(specialtyId);
        return list.Select(_mapper.ToDto);
    }

    public async Task<SpecialtyDto?> GetDoctorSpecialtyAsync(int doctorId)
    {
        var specialty = await _repository.GetSpecialtyOfDoctorAsync(doctorId);
        return specialty is null ? null : new SpecialtyDto(specialty.Id, specialty.Name);
    }

    public async Task<bool> ChangeDoctorSpecialtyAsync(int doctorId, int specialtyId)
    {
        var entity = await _repository.GetByIdAsync(doctorId);
        if (entity is null)
            return false;

        var exists = await _repository.SpecialtyExistsAsync(specialtyId);
        if (!exists)
            throw new InvalidOperationException("Specialty not found.");

        entity.SpecialtyId = specialtyId;
        await _repository.SaveChangesAsync();
        return true;
    }



    public async Task<IEnumerable<ConsultationDto>> GetConsultationsByDoctorAsync(
        int doctorId,
        DateOnly? from = null,
        DateOnly? to = null,
        int? patientId = null)
    {
        var list = await _repository.GetConsultationsAsync(doctorId, from, to, patientId);
        return list.Select(_consultationMapper.ToDto);
    }

    public async Task<IEnumerable<PatientDto>> GetPatientsByDoctorAsync(int doctorId)
    {
        var list = await _repository.GetPatientsAsync(doctorId);
        return list.Select(_patientMapper.ToDto);
    }

    public async Task<IEnumerable<OrdonnanceDto>> GetOrdonnancesByDoctorAsync(
        int doctorId,
        DateOnly? from = null,
        DateOnly? to = null,
        int? patientId = null)
    {
        var list = await _repository.GetOrdonnancesAsync(doctorId, from, to, patientId);
        return list.Select(_ordonnanceMapper.ToDto);
    }
}
