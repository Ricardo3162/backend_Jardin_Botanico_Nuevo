using Backend_Jardin.Data;
using Backend_Jardin.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend_Jardin.Services;

public class ComentariosService
{
    private readonly JardinContext _db;

    public ComentariosService(JardinContext db)
    {
        _db = db;
    }

    public async Task<(List<ComentarioResumenDto> items, int total)> GetComentariosAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _db.comentarios
            .Include(c => c.fk_especieNavigation)
            .AsNoTracking()
            .Where(c => c.estado == "ACTIVO");

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            var pattern = $"%{term}%";
            query = query.Where(c =>
                c.fk_especieNavigation != null &&
                (EF.Functions.Like(c.fk_especieNavigation.nombre_comun_especie, pattern) ||
                 EF.Functions.Like(c.fk_especieNavigation.nombre_cientifico_especie, pattern)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(c => c.fecha_comentario)
            .Select(c => new ComentarioResumenDto
            {
                id_comentario = c.id_comentario,
                id_especie = c.fk_especie,
                nombre_comun_especie = c.fk_especieNavigation != null ? c.fk_especieNavigation.nombre_comun_especie : string.Empty,
                nombre_cientifico_especie = c.fk_especieNavigation != null ? c.fk_especieNavigation.nombre_cientifico_especie : string.Empty,
                fecha_comentario = c.fecha_comentario,
                comentario = c.comentario1
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<bool> DeleteComentarioAsync(int id, CancellationToken ct = default)
    {
        var comentario = await _db.comentarios.FirstOrDefaultAsync(c => c.id_comentario == id, ct);
        if (comentario == null)
            return false;

        _db.comentarios.Remove(comentario);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
