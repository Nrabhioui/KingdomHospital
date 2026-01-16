using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

public class SpecialtyRepository
{
    private readonly HospitalDbContext _context;

    public SpecialtyRepository(HospitalDbContext context)
    {
        _context = context;
    }



    public async Task<List<Specialty>> GetAllAsync()
    {
        return await _context.Specialties
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Specialty?> GetByIdAsync(int id)
    {
        
        return await _context.Specialties.FindAsync(id);
    }

    
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.Specialties.AsQueryable();

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync(s => s.Name == name);
    }

    
    public Task<bool> HasDoctorsAsync(int specialtyId)
    {
        return _context.Doctors.AnyAsync(d => d.SpecialtyId == specialtyId);
    }

    

    public async Task AddAsync(Specialty specialty)
    {
        _context.Specialties.Add(specialty);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Specialty specialty)
    {
        _context.Specialties.Remove(specialty);
        await _context.SaveChangesAsync();
    }

    
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
