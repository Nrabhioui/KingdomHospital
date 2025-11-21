using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

public class OrdonnanceRepository
{
    private readonly HospitalDbContext _context;

    public OrdonnanceRepository(HospitalDbContext context)
    {
        _context = context;
    }

    // -------------------------
    // CRUD READ
    // -------------------------

    public async Task<List<Ordonnance>> GetAllAsync()
    {
        return await _context.Ordonnances
            .OrderBy(o => o.Date)
            .ThenBy(o => o.Id)
            .ToListAsync();
    }

    public async Task<Ordonnance?> GetByIdAsync(int id)
    {
        return await _context.Ordonnances.FindAsync(id);
    }

    // -------------------------
    // Doctor / Patient existence
    // -------------------------

    public Task<bool> DoctorExistsAsync(int doctorId)
    {
        return _context.Doctors.AnyAsync(d => d.Id == doctorId);
    }

    public Task<bool> PatientExistsAsync(int patientId)
    {
        return _context.Patients.AnyAsync(p => p.Id == patientId);
    }

    // -------------------------
    // Consultation check
    // -------------------------

    public async Task<Consultation?> GetConsultationAsync(int consultationId)
    {
        return await _context.Consultations
            .FirstOrDefaultAsync(c => c.Id == consultationId);
    }

    public async Task<Consultation?> GetConsultationWithRelationsAsync(int consultationId)
    {
        return await _context.Consultations
            .Include(c => c.Doctor)
            .Include(c => c.Patient)
            .FirstOrDefaultAsync(c => c.Id == consultationId);
    }

    // -------------------------
    // DELETE
    // -------------------------

    public async Task<Ordonnance?> GetWithLignesAsync(int id)
    {
        return await _context.Ordonnances
            .Include(o => o.Lignes)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    // -------------------------
    // UPDATE consultationId
    // -------------------------

    public async Task<Ordonnance?> GetForConsultationUpdateAsync(int ordonnanceId)
    {
        return await _context.Ordonnances
            .FirstOrDefaultAsync(o => o.Id == ordonnanceId);
    }

    // -------------------------
    // UTILITAIRE filtre
    // -------------------------

    public async Task<List<Ordonnance>> GetFilteredAsync(
        int? doctorId,
        int? patientId,
        DateOnly? from,
        DateOnly? to)
    {
        var query = _context.Ordonnances.AsQueryable();

        if (doctorId.HasValue)
            query = query.Where(o => o.DoctorId == doctorId.Value);

        if (patientId.HasValue)
            query = query.Where(o => o.PatientId == patientId.Value);

        if (from.HasValue)
            query = query.Where(o => o.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(o => o.Date <= to.Value);

        return await query
            .OrderBy(o => o.Date)
            .ThenBy(o => o.Id)
            .ToListAsync();
    }

    // -------------------------
    // WRITE
    // -------------------------

    public async Task AddAsync(Ordonnance ordonnance)
    {
        _context.Ordonnances.Add(ordonnance);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Ordonnance ordonnance)
    {
        _context.Ordonnances.Remove(ordonnance);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
