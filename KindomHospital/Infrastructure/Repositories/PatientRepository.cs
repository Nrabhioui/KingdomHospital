using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

public class PatientRepository
{
    private readonly HospitalDbContext _context;

    public PatientRepository(HospitalDbContext context)
    {
        _context = context;
    }

    // ---- READ ----

    public async Task<List<Patient>> GetAllAsync()
    {
        return await _context.Patients
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _context.Patients.FindAsync(id);
    }

    public async Task<bool> ExistsByIdentityAsync(string lastName, string firstName, DateOnly birthDate, int? excludeId = null)
    {
        var query = _context.Patients.AsQueryable();

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(p =>
            p.LastName == lastName &&
            p.FirstName == firstName &&
            p.BirthDate == birthDate);
    }

    public Task<bool> HasConsultationsAsync(int patientId)
    {
        return _context.Consultations.AnyAsync(c => c.PatientId == patientId);
    }

    public Task<bool> HasOrdonnancesAsync(int patientId)
    {
        return _context.Ordonnances.AnyAsync(o => o.PatientId == patientId);
    }

    public async Task<List<Consultation>> GetConsultationsAsync(int patientId)
    {
        return await _context.Consultations
            .Where(c => c.PatientId == patientId)
            .OrderBy(c => c.Date)
            .ThenBy(c => c.Hour)
            .ToListAsync();
    }

    public async Task<List<Ordonnance>> GetOrdonnancesAsync(int patientId)
    {
        return await _context.Ordonnances
            .Where(o => o.PatientId == patientId)
            .OrderBy(o => o.Date)
            .ToListAsync();
    }

    // ---- WRITE ----

    public async Task AddAsync(Patient entity)
    {
        _context.Patients.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Patient entity)
    {
        _context.Patients.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
