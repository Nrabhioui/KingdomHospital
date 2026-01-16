using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

public class MedicamentRepository
{
    private readonly HospitalDbContext _context;

    public MedicamentRepository(HospitalDbContext context)
    {
        _context = context;
    }


    public async Task<List<Medicament>> GetAllAsync()
    {
        return await _context.Medicaments
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<Medicament?> GetByIdAsync(int id)
    {
        return await _context.Medicaments.FindAsync(id);
    }


    public Task<bool> ExistsByNameAsync(string name)
    {
        return _context.Medicaments.AnyAsync(m => m.Name == name);
    }

    public Task<bool> MedicamentExistsAsync(int medicamentId)
    {
        return _context.Medicaments.AnyAsync(m => m.Id == medicamentId);
    }



    public async Task<List<Ordonnance>> GetOrdonnancesByMedicamentAsync(int medicamentId)
    {
        return await _context.OrdonnanceLignes
            .Where(l => l.MedicamentId == medicamentId)
            .Select(l => l.Ordonnance)
            .Distinct()
            .OrderBy(o => o.Date)
            .ToListAsync();
    }


    public async Task AddAsync(Medicament medicament)
    {
        _context.Medicaments.Add(medicament);
        await _context.SaveChangesAsync();
    }
}
