using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

public class OrdonnanceLigneRepository
{
    private readonly HospitalDbContext _context;

    public OrdonnanceLigneRepository(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrdonnanceLigne>> GetForOrdonnanceAsync(int ordonnanceId)
    {
        return await _context.OrdonnanceLignes
            .Where(l => l.OrdonnanceId == ordonnanceId)
            .ToListAsync();
    }

    public async Task<OrdonnanceLigne?> GetByIdAsync(int ordonnanceId, int ligneId)
    {
        return await _context.OrdonnanceLignes
            .FirstOrDefaultAsync(l => l.Id == ligneId && l.OrdonnanceId == ordonnanceId);
    }

    public Task<bool> OrdonnanceExistsAsync(int ordonnanceId)
    {
        return _context.Ordonnances.AnyAsync(o => o.Id == ordonnanceId);
    }

    public Task<bool> MedicamentExistsAsync(int medicamentId)
    {
        return _context.Medicaments.AnyAsync(m => m.Id == medicamentId);
    }

    public Task<bool> ExistsSameAsync(
        int ordonnanceId,
        int medicamentId,
        string dosage,
        string frequency,
        string duration)
    {
        return _context.OrdonnanceLignes.AnyAsync(l =>
            l.OrdonnanceId == ordonnanceId &&
            l.MedicamentId == medicamentId &&
            l.Dosage == dosage &&
            l.Frequency == frequency &&
            l.Duration == duration);
    }

    public async Task AddAsync(OrdonnanceLigne entity)
    {
        _context.OrdonnanceLignes.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(OrdonnanceLigne entity)
    {
        _context.OrdonnanceLignes.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
