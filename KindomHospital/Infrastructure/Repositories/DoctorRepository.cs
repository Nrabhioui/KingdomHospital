using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

public class DoctorRepository
{
    private readonly HospitalDbContext _context;

    public DoctorRepository(HospitalDbContext context)
    {
        _context = context;
    }

    // ------------------------
    // CRUD READ
    // ------------------------

    public async Task<List<Doctor>> GetAllAsync()
    {
        return await _context.Doctors
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await _context.Doctors.FindAsync(id);
    }

    public Task<bool> SpecialtyExistsAsync(int specialtyId)
    {
        return _context.Specialties.AnyAsync(s => s.Id == specialtyId);
    }

    public async Task<bool> ExistsDuplicateAsync(string last, string first, int specialtyId, int? excludeId = null)
    {
        var query = _context.Doctors.AsQueryable();

        if (excludeId.HasValue)
            query = query.Where(d => d.Id != excludeId.Value);

        return await query.AnyAsync(d =>
            d.LastName == last &&
            d.FirstName == first &&
            d.SpecialtyId == specialtyId);
    }

    public Task<bool> HasConsultationsAsync(int doctorId)
    {
        return _context.Consultations.AnyAsync(c => c.DoctorId == doctorId);
    }

    public Task<bool> HasOrdonnancesAsync(int doctorId)
    {
        return _context.Ordonnances.AnyAsync(o => o.DoctorId == doctorId);
    }

    // ------------------------
    // RELATIONNEL
    // ------------------------

    public async Task<List<Doctor>> GetBySpecialtyAsync(int specialtyId)
    {
        return await _context.Doctors
            .Where(d => d.SpecialtyId == specialtyId)
            .ToListAsync();
    }

    public async Task<Specialty?> GetSpecialtyOfDoctorAsync(int doctorId)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.Id == doctorId);

        return doctor?.Specialty;
    }

    public async Task<List<Consultation>> GetConsultationsAsync(
        int doctorId,
        DateOnly? from,
        DateOnly? to,
        int? patientId)
    {
        var query = _context.Consultations
            .Where(c => c.DoctorId == doctorId)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(c => c.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(c => c.Date <= to.Value);

        if (patientId.HasValue)
            query = query.Where(c => c.PatientId == patientId.Value);

        return await query
            .OrderBy(c => c.Date)
            .ThenBy(c => c.Hour)
            .ToListAsync();
    }

    public async Task<List<Patient>> GetPatientsAsync(int doctorId)
    {
        return await _context.Consultations
            .Where(c => c.DoctorId == doctorId)
            .Select(c => c.Patient)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<Ordonnance>> GetOrdonnancesAsync(
        int doctorId,
        DateOnly? from,
        DateOnly? to,
        int? patientId)
    {
        var query = _context.Ordonnances
            .Where(o => o.DoctorId == doctorId)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(o => o.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(o => o.Date <= to.Value);

        if (patientId.HasValue)
            query = query.Where(o => o.PatientId == patientId.Value);

        return await query
            .OrderBy(o => o.Date)
            .ToListAsync();
    }

    // ------------------------
    // WRITE
    // ------------------------

    public async Task AddAsync(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Doctor doctor)
    {
        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
