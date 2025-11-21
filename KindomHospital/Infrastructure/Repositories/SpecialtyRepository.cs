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

    // ---- READ ----

    public async Task<List<Specialty>> GetAllAsync()
    {
        return await _context.Specialties
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Specialty?> GetByIdAsync(int id)
    {
        // FindAsync retourne ValueTask<Specialty?>
        return await _context.Specialties.FindAsync(id);
    }

    /// <summary>
    /// Vérifie s'il existe déjà une spécialité avec ce nom.
    /// excludeId permet d'ignorer une spécialité (utile pour Update).
    /// </summary>
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.Specialties.AsQueryable();

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync(s => s.Name == name);
    }

    /// <summary>
    /// Vérifie si une spécialité a au moins un médecin.
    /// Sert pour la règle : "on ne supprime pas une spécialité avec des médecins".
    /// </summary>
    public Task<bool> HasDoctorsAsync(int specialtyId)
    {
        return _context.Doctors.AnyAsync(d => d.SpecialtyId == specialtyId);
    }

    // ---- WRITE ----

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

    /// <summary>
    /// Utilisé quand on a déjà modifié une entité suivie par le contexte.
    /// </summary>
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
