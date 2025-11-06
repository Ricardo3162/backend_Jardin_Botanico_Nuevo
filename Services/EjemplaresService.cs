using Backend_Jardin.Data;
using Backend_Jardin.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Jardin.Services;

public class EjemplaresService
{
    private readonly JardinContext _db;
    public EjemplaresService(JardinContext db)
    {
        _db = db;
    }

    public async Task<ejemplar?> GetByCodigoAsync(string codigo, CancellationToken ct = default)
        => await _db.ejemplars
            .Include(e => e.fk_especieNavigation)
            .Include(e => e.fk_ubicacionNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.codigo_interno_ejemplar == codigo, ct);

    public async Task<ejemplar> CreateAsync(ejemplar entity, CancellationToken ct = default)
    {
        _db.ejemplars.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(int id, Action<ejemplar> updater, CancellationToken ct = default)
    {
        var existente = await _db.ejemplars.FirstOrDefaultAsync(e => e.id_ejemplar == id, ct);
        if (existente == null) return false;
        updater(existente);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // Métodos de listado/búsqueda/eliminación se removieron según requerimiento
}
