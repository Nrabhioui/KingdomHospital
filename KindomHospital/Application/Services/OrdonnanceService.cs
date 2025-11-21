using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Domain.Entities;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

public class OrdonnanceService
{
    private readonly OrdonnanceRepository _repository;
    private readonly OrdonnanceMapper _mapper;

    public OrdonnanceService(OrdonnanceRepository repository, OrdonnanceMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // ----------------------------
    // CRUD
    // ----------------------------

    public async Task<IEnumerable<OrdonnanceDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(_mapper.ToDto);
    }

    public async Task<OrdonnanceDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.ToDto(entity);
    }

    public async Task<OrdonnanceDto> CreateAsync(OrdonnanceCreateDto dto)
    {
        await ValidateDoctorAndPatient(dto.DoctorId, dto.PatientId);

        Consultation? consultation = null;

        if (dto.ConsultationId.HasValue)
        {
            consultation = await _repository.GetConsultationAsync(dto.ConsultationId.Value);
            if (consultation is null)
                throw new InvalidOperationException("Consultation does not exist.");

            if (consultation.DoctorId != dto.DoctorId ||
                consultation.PatientId != dto.PatientId)
                throw new InvalidOperationException("Doctor and patient must match the linked consultation.");

            if (dto.Date < consultation.Date)
                throw new InvalidOperationException("Prescription date cannot be earlier than the consultation date.");
        }

        var notes = dto.Notes?.Trim();
        if (!string.IsNullOrEmpty(notes) && notes.Length > 255)
            notes = notes[..255];

        var cleanDto = new OrdonnanceCreateDto(
            dto.DoctorId,
            dto.PatientId,
            dto.ConsultationId,
            dto.Date,
            notes
        );

        var entity = _mapper.ToEntity(cleanDto);

        await _repository.AddAsync(entity);

        return _mapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, OrdonnanceUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        await ValidateDoctorAndPatient(dto.DoctorId, dto.PatientId);

        Consultation? consultation = null;

        if (dto.ConsultationId.HasValue)
        {
            consultation = await _repository.GetConsultationAsync(dto.ConsultationId.Value);
            if (consultation is null)
                throw new InvalidOperationException("Consultation does not exist.");

            if (consultation.DoctorId != dto.DoctorId ||
                consultation.PatientId != dto.PatientId)
                throw new InvalidOperationException("Doctor and patient must match the linked consultation.");

            if (dto.Date < consultation.Date)
                throw new InvalidOperationException("Prescription date cannot be earlier than the consultation date.");
        }

        var notes = dto.Notes?.Trim();
        if (!string.IsNullOrEmpty(notes) && notes.Length > 255)
            notes = notes[..255];

        var cleanDto = new OrdonnanceUpdateDto(
            dto.DoctorId,
            dto.PatientId,
            dto.ConsultationId,
            dto.Date,
            notes
        );

        _mapper.UpdateEntity(cleanDto, entity);

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetWithLignesAsync(id);
        if (entity is null)
            return false;

        if (entity.Lignes.Any())
            _repository.RemoveAsync(entity); // Lignes sont supprimées via config cascade

        await _repository.RemoveAsync(entity);
        return true;
    }

    private async Task ValidateDoctorAndPatient(int doctorId, int patientId)
    {
        var doctorExists = await _repository.DoctorExistsAsync(doctorId);
        if (!doctorExists)
            throw new InvalidOperationException("Doctor does not exist.");

        var patientExists = await _repository.PatientExistsAsync(patientId);
        if (!patientExists)
            throw new InvalidOperationException("Patient does not exist.");
    }

    // ----------------------------
    // RELATIONNEL : Consultation - Ordonnance
    // ----------------------------

    public async Task<bool> SetOrdonnanceConsultationAsync(int ordonnanceId, int consultationId)
    {
        var ordonnance = await _repository.GetForConsultationUpdateAsync(ordonnanceId);
        if (ordonnance is null)
            return false;

        var consultation = await _repository.GetConsultationWithRelationsAsync(consultationId);
        if (consultation is null)
            throw new InvalidOperationException("Consultation not found.");

        ordonnance.ConsultationId = consultation.Id;
        ordonnance.DoctorId = consultation.DoctorId;
        ordonnance.PatientId = consultation.PatientId;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveOrdonnanceConsultationAsync(int ordonnanceId)
    {
        var ordonnance = await _repository.GetForConsultationUpdateAsync(ordonnanceId);
        if (ordonnance is null)
            return false;

        ordonnance.ConsultationId = null;

        await _repository.SaveChangesAsync();
        return true;
    }

    // ----------------------------
    // UTILITAIRE : Filtre doctor/patient/date
    // ----------------------------

    public async Task<IEnumerable<OrdonnanceDto>> GetFilteredAsync(
        int? doctorId,
        int? patientId,
        DateOnly? from,
        DateOnly? to)
    {
        var list = await _repository.GetFilteredAsync(doctorId, patientId, from, to);
        return list.Select(_mapper.ToDto);
    }
}
