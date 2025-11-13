using Backend_Jardin.Data;
using Backend_Jardin.Models;
using Backend_Jardin.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend_Jardin.Services;

public class EspeciesService
{
    private readonly JardinContext _db;
    public EspeciesService(JardinContext db)
    {
        _db = db;
    }

    public async Task<especie?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.especies
            .Include(e => e.fk_estado_conservacionNavigation)
            .Include(e => e.fk_usoNavigation)
            .Include(e => e.especie_ubicacions)
                .ThenInclude(eu => eu.fk_ubicacionNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.id_especie == id, ct);

    public async Task<EspecieDetalleDto?> GetDetalleAsync(int id, CancellationToken ct = default)
        => await _db.especies
            .Include(e => e.fk_usoNavigation)
            .Include(e => e.fk_estado_conservacionNavigation)
            .Where(e => e.id_especie == id)
            .AsNoTracking()
            .Select(e => new EspecieDetalleDto
            {
                id_especie = e.id_especie,
                codigo_interno_especie = e.codigo_interno_especie ?? string.Empty,
                nombre_comun_especie = e.nombre_comun_especie,
                nombre_cientifico_especie = e.nombre_cientifico_especie,
                familia_especie = e.familia_especie,
                descripcion_especie = e.descripcion_especie ?? string.Empty,
                observacion_especie = e.observacion_especie,
                origen_especie = e.origen_especie ?? string.Empty,
                distribucion_colombia_especie = e.distribucion_colombia_especie ?? string.Empty,
                distribucion_caqueta_especie = e.distribucion_caqueta_especie ?? string.Empty,
                distribucion_mundial_especie = e.distribucion_mundial_especie ?? string.Empty,
                fenologia_especie = e.fenologia_especie ?? string.Empty,
                muestras_secas_herbario_especie = e.muestras_secas_herbario_especie,
                fk_uso = e.fk_uso,
                fk_estado_conservacion = e.fk_estado_conservacion ?? 0,
                uso_nombre = e.fk_usoNavigation != null ? e.fk_usoNavigation.nombre_uso : string.Empty,
                estado_conservacion_codigo = e.fk_estado_conservacionNavigation != null ? e.fk_estado_conservacionNavigation.codigo_iucn : string.Empty,
                estado_conservacion_nombre = e.fk_estado_conservacionNavigation != null ? e.fk_estado_conservacionNavigation.nombre_estado : string.Empty,
                ubicaciones = e.especie_ubicacions
                    .Select(u => new EspecieUbicacionDto
                    {
                        id_ubicacion = u.fk_ubicacion,
                        fk_ubicacion = u.fk_ubicacion,
                        nombre_ubicacion = u.fk_ubicacionNavigation != null ? u.fk_ubicacionNavigation.nombre_ubicacion : string.Empty
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

    public async Task<especie?> GetByCodigoAsync(string codigo, CancellationToken ct = default)
    {
        var normalized = (codigo ?? string.Empty).Trim().Replace(" ", string.Empty);
        return await _db.especies
            .Include(e => e.fk_estado_conservacionNavigation)
            .Include(e => e.fk_usoNavigation)
            .Include(e => e.especie_ubicacions)
                .ThenInclude(eu => eu.fk_ubicacionNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.codigo_interno_especie == normalized, ct);
    }

    public async Task<especie> CreateAsync(especie entity, CancellationToken ct = default, IEnumerable<int>? ubicacionIds = null)
    {
        if (string.IsNullOrWhiteSpace(entity.estado))
            entity.estado = "ACTIVO";
        _db.especies.Add(entity);
        await _db.SaveChangesAsync(ct);
        if (ubicacionIds != null)
        {
            var ids = ubicacionIds.Distinct().ToList();
            if (ids.Count > 0)
            {
                var validIds = await _db.ubicacions
                    .Where(u => ids.Contains(u.id_ubicacion))
                    .Select(u => u.id_ubicacion)
                    .ToListAsync(ct);

                if (validIds.Count > 0)
                {
                    var existing = await _db.especie_ubicacions
                        .Where(eu => eu.fk_especie == entity.id_especie && validIds.Contains(eu.fk_ubicacion))
                        .Select(eu => eu.fk_ubicacion)
                        .ToListAsync(ct);

                    var toAdd = validIds
                        .Except(existing)
                        .Select(id => new especie_ubicacion
                        {
                            fk_especie = entity.id_especie,
                            fk_ubicacion = id,
                            estado = "ACTIVO"
                        })
                        .ToList();

                    if (toAdd.Count > 0)
                    {
                        _db.especie_ubicacions.AddRange(toAdd);
                        await _db.SaveChangesAsync(ct);
                    }
                }
            }
        }
        return entity;
    }

    public async Task<bool> UpdateAsync(int id, Action<especie> updater, CancellationToken ct = default)
    {
        var existente = await _db.especies.FirstOrDefaultAsync(e => e.id_especie == id, ct);
        if (existente == null) return false;
        updater(existente);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task UpdateUbicacionesAsync(int idEspecie, IEnumerable<int> ubicacionIds, CancellationToken ct = default)
    {
        var ids = ubicacionIds?.Distinct().ToList() ?? new List<int>();
        var validIds = await _db.ubicacions
            .Where(u => ids.Contains(u.id_ubicacion))
            .Select(u => u.id_ubicacion)
            .ToListAsync(ct);

        var actuales = await _db.especie_ubicacions
            .Where(eu => eu.fk_especie == idEspecie)
            .ToListAsync(ct);

        var actualesIds = actuales.Select(a => a.fk_ubicacion).ToHashSet();

        var toAdd = validIds
            .Except(actualesIds)
            .Select(id => new especie_ubicacion
            {
                fk_especie = idEspecie,
                fk_ubicacion = id,
                estado = "ACTIVO"
            })
            .ToList();

        var toRemove = actuales.Where(a => !validIds.Contains(a.fk_ubicacion)).ToList();

        if (toRemove.Count > 0) _db.especie_ubicacions.RemoveRange(toRemove);
        if (toAdd.Count > 0) _db.especie_ubicacions.AddRange(toAdd);

        if (toRemove.Count > 0 || toAdd.Count > 0)
            await _db.SaveChangesAsync(ct);
    }

    public async Task<List<especie>> GetActivasAsync(CancellationToken ct = default)
        => await _db.especies
            .Include(e => e.fk_estado_conservacionNavigation)
            .Include(e => e.fk_usoNavigation)
            .Include(e => e.especie_ubicacions)
                .ThenInclude(eu => eu.fk_ubicacionNavigation)
            .AsNoTracking()
            .Where(e => e.estado == "ACTIVO")
            .ToListAsync(ct);

    public async Task<(List<EspecieResumenListItem> items, int total)> GetActivasResumenPageAsync(
        int page,
        int pageSize,
        int? ubicacionId = null,
        string? uso = null,
        string? search = null,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 15;
        var query = _db.especies.AsNoTracking().Where(e => e.estado == "ACTIVO");

        if (ubicacionId.HasValue)
        {
            query = query.Where(e =>
                e.especie_ubicacions.Any(eu => eu.fk_ubicacion == ubicacionId.Value && eu.estado == "ACTIVO"));
        }

        if (!string.IsNullOrWhiteSpace(uso))
        {
            var usoTerm = uso.Trim();
            var usoPattern = $"%{usoTerm}%";
            query = query.Where(e => e.fk_usoNavigation != null && EF.Functions.Like(e.fk_usoNavigation!.nombre_uso, usoPattern));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();
            var searchPattern = $"%{searchTerm}%";
            query = query.Where(e =>
                EF.Functions.Like(e.nombre_comun_especie, searchPattern) ||
                EF.Functions.Like(e.nombre_cientifico_especie, searchPattern));
        }
        var total = await query.CountAsync(ct);
        var rows = await query
            .OrderByDescending(e => e.id_especie)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EspecieResumenListItem
            {
                id_especie = e.id_especie,
                nombre_cientifico_especie = e.nombre_cientifico_especie,
                nombre_comun_especie = e.nombre_comun_especie
            })
            .ToListAsync(ct);
        return (rows, total);
    }

    public async Task<List<UbicacionDto>> GetUbicacionesActivasAsync(CancellationToken ct = default)
        => await _db.ubicacions
            .AsNoTracking()
            .Where(u => u.estado == "ACTIVO")
            .OrderBy(u => u.nombre_ubicacion)
            .Select(u => new UbicacionDto
            {
                id_ubicacion = u.id_ubicacion,
                nombre_ubicacion = u.nombre_ubicacion
            })
            .ToListAsync(ct);

    public async Task<List<UsoDto>> GetUsosActivosAsync(CancellationToken ct = default)
        => await _db.usos
            .AsNoTracking()
            .Where(u => u.estado == "ACTIVO")
            .OrderBy(u => u.nombre_uso)
            .Select(u => new UsoDto
            {
                id_uso = u.id_uso,
                nombre_uso = u.nombre_uso
            })
            .ToListAsync(ct);

    public async Task<List<EstadoConservacionDto>> GetEstadosConservacionActivosAsync(CancellationToken ct = default)
        => await _db.estadoconservacions
            .AsNoTracking()
            .Where(e => e.estado == "ACTIVO")
            .OrderBy(e => e.nombre_estado)
            .Select(e => new EstadoConservacionDto
            {
                id_estado = e.id_estado,
                nombre_estado = e.nombre_estado,
                codigo_iucn = e.codigo_iucn
            })
            .ToListAsync(ct);
}






