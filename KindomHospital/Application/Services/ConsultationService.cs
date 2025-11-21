using KingdomHospital.Application.DTOs.Consultations;
using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

public class ConsultationService
{
    private readonly ConsultationRepository _repository;
    private readonly ConsultationMapper _mapper;
    private readonly OrdonnanceMapper _ordonnanceMapper;

    public ConsultationService(
        ConsultationRepository repository,
        ConsultationMapper mapper,
        OrdonnanceMapper ordonnanceMapper)
    {
        _repository = repository;
        _mapper = mapper;
        _ordonnanceMapper = ordonnanceMapper;
    }

    // ----------------------------
    // CRUD
    // ----------------------------

    public async Task<IEnumerable<ConsultationDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(_mapper.ToDto);
    }

    public async Task<ConsultationDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.ToDto(entity);
    }

    public async Task<ConsultationDto> CreateAsync(ConsultationCreateDto dto)
    {
        await ValidateDoctorAndPatientExist(dto.DoctorId, dto.PatientId);

        await EnsureNoDoubleBooking(
            dto.DoctorId,
            dto.PatientId,
            dto.Date,
            dto.Hour);

        var entity = _mapper.ToEntity(dto);

        await _repository.AddAsync(entity);

        return _mapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, ConsultationUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        await ValidateDoctorAndPatientExist(dto.DoctorId, dto.PatientId);

        await EnsureNoDoubleBooking(
            dto.DoctorId,
            dto.PatientId,
            dto.Date,
            dto.Hour,
            excludeConsultationId: id);

        _mapper.UpdateEntity(dto, entity);

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetWithTrackingAsync(id);
        if (entity is null)
            return false;

        var hasOrdonnances = await _repository.HasOrdonnancesAsync(id);
        if (hasOrdonnances)
            throw new InvalidOperationException("Cannot delete a consultation with prescriptions.");

        await _repository.RemoveAsync(entity);
        return true;
    }

    private async Task ValidateDoctorAndPatientExist(int doctorId, int patientId)
    {
        var doctorExists = await _repository.DoctorExistsAsync(doctorId);
        if (!doctorExists)
            throw new InvalidOperationException("Doctor does not exist.");

        var patientExists = await _repository.PatientExistsAsync(patientId);
        if (!patientExists)
            throw new InvalidOperationException("Patient does not exist.");
    }

    private async Task EnsureNoDoubleBooking(
        int doctorId,
        int patientId,
        DateOnly date,
        TimeOnly hour,
        int? excludeConsultationId = null)
    {
        var doctorConflict = await _repository.HasDoctorConflictAsync(
            doctorId, date, hour, excludeConsultationId);

        if (doctorConflict)
            throw new InvalidOperationException("The doctor already has a consultation at this date and time.");

        var patientConflict = await _repository.HasPatientConflictAsync(
            patientId, date, hour, excludeConsultationId);

        if (patientConflict)
            throw new InvalidOperationException("The patient already has a consultation at this date and time.");
    }

    // ----------------------------
    // RELATIONNEL : Consultation - Ordonnance
    // ----------------------------

    public async Task<IEnumerable<OrdonnanceDto>> GetOrdonnancesByConsultationAsync(int consultationId)
    {
        var ordos = await _repository.GetOrdonnancesByConsultationAsync(consultationId);
        return ordos.Select(_ordonnanceMapper.ToDto);
    }

    public async Task<OrdonnanceDto> CreateOrdonnanceForConsultationAsync(
        int consultationId,
        OrdonnanceCreateDto dto)
    {
        var entity = await _repository.AddOrdonnanceForConsultationAsync(
            consultationId,
            dto.Date,
            dto.Notes);

        return _ordonnanceMapper.ToDto(entity);
    }

    // ----------------------------
    // RELATIONNEL : utilitaire (filtre)
    // ----------------------------

    public async Task<IEnumerable<ConsultationDto>> GetFilteredAsync(
        int? doctorId,
        int? patientId,
        DateOnly? from,
        DateOnly? to)
    {
        var list = await _repository.GetFilteredAsync(doctorId, patientId, from, to);
        return list.Select(_mapper.ToDto);
    }
}
