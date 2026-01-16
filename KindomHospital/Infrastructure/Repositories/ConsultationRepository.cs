using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

public class ConsultationRepository
{
    private readonly HospitalDbContext _context;

    public ConsultationRepository(HospitalDbContext context)
    {
        _context = context;
    }


    public async Task<List<Consultation>> GetAllAsync()
    {
        return await _context.Consultations
            .OrderBy(c => c.Date)
            .ThenBy(c => c.Hour)
            .ToListAsync();
    }

    public async Task<Consultation?> GetByIdAsync(int id)
    {
        return await _context.Consultations.FindAsync(id);
    }



    public Task<bool> DoctorExistsAsync(int doctorId)
    {
        return _context.Doctors.AnyAsync(d => d.Id == doctorId);
    }

    public Task<bool> PatientExistsAsync(int patientId)
    {
        return _context.Patients.AnyAsync(p => p.Id == patientId);
    }

    public async Task<bool> HasDoctorConflictAsync(
        int doctorId,
        DateOnly date,
        TimeOnly hour,
        int? excludeConsultationId = null)
    {
        var query = _context.Consultations
            .Where(c => c.Date == date && c.Hour == hour);

        if (excludeConsultationId.HasValue)
        {
            var id = excludeConsultationId.Value;
            query = query.Where(c => c.Id != id);
        }

        return await query.AnyAsync(c => c.DoctorId == doctorId);
    }

    public async Task<bool> HasPatientConflictAsync(
        int patientId,
        DateOnly date,
        TimeOnly hour,
        int? excludeConsultationId = null)
    {
        var query = _context.Consultations
            .Where(c => c.Date == date && c.Hour == hour);

        if (excludeConsultationId.HasValue)
        {
            var id = excludeConsultationId.Value;
            query = query.Where(c => c.Id != id);
        }

        return await query.AnyAsync(c => c.PatientId == patientId);
    }



    public async Task<bool> HasOrdonnancesAsync(int consultationId)
    {
        return await _context.Ordonnances.AnyAsync(o => o.ConsultationId == consultationId);
    }

    public async Task<Consultation?> GetWithTrackingAsync(int id)
    {
        return await _context.Consultations.FirstOrDefaultAsync(c => c.Id == id);
    }



    public async Task<List<Ordonnance>> GetOrdonnancesByConsultationAsync(int consultationId)
    {
        return await _context.Ordonnances
            .Where(o => o.ConsultationId == consultationId)
            .OrderBy(o => o.Date)
            .ToListAsync();
    }

    public async Task<Ordonnance> AddOrdonnanceForConsultationAsync(
        int consultationId,
        DateOnly date,
        string? notes)
    {
        var consultation = await _context.Consultations
            .Include(c => c.Doctor)
            .Include(c => c.Patient)
            .FirstOrDefaultAsync(c => c.Id == consultationId);

        if (consultation is null)
            throw new InvalidOperationException("Consultation not found.");

        var ord = new Ordonnance
        {
            DoctorId = consultation.DoctorId,
            PatientId = consultation.PatientId,
            ConsultationId = consultationId,
            Date = date,
            Notes = notes
        };

        _context.Ordonnances.Add(ord);
        await _context.SaveChangesAsync();

        return ord;
    }



    public async Task<List<Consultation>> GetFilteredAsync(
        int? doctorId,
        int? patientId,
        DateOnly? from,
        DateOnly? to)
    {
        var query = _context.Consultations.AsQueryable();

        if (doctorId.HasValue)
            query = query.Where(c => c.DoctorId == doctorId.Value);

        if (patientId.HasValue)
            query = query.Where(c => c.PatientId == patientId.Value);

        if (from.HasValue)
            query = query.Where(c => c.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(c => c.Date <= to.Value);

        return await query
            .OrderBy(c => c.Date)
            .ThenBy(c => c.Hour)
            .ToListAsync();
    }



    public async Task AddAsync(Consultation consultation)
    {
        _context.Consultations.Add(consultation);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Consultation consultation)
    {
        _context.Consultations.Remove(consultation);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
