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

    public async Task<List<especie>> GetAllAsync(CancellationToken ct = default)
        => await _db.especies.AsNoTracking().ToListAsync(ct);

    public async Task<especie?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.especies.AsNoTracking().FirstOrDefaultAsync(e => e.id_especie == id, ct);

    public async Task<especie> CreateAsync(especie entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.estado))
            entity.estado = "ACTIVO";
        _db.especies.Add(entity);
        await _db.SaveChangesAsync(ct);
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

    public async Task<(bool ok, string? error)> DeleteAsync(int id, CancellationToken ct = default)
    {
        var existente = await _db.especies.FirstOrDefaultAsync(e => e.id_especie == id, ct);
        if (existente == null) return (false, null);
        _db.especies.Remove(existente);
        try
        {
            await _db.SaveChangesAsync(ct);
            return (true, null);
        }
        catch (DbUpdateException ex)
        {
            return (false, ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<List<especie>> GetActivasAsync(CancellationToken ct = default)
        => await _db.especies
            .AsNoTracking()
            .Where(e => e.estado == "ACTIVO")
            .ToListAsync(ct);
    public async Task<List<EspecieResumenDto>> GetActivasResumenAsync(CancellationToken ct = default)
    {
        var filas = await _db.especies
            .AsNoTracking()
            .Where(e => e.estado == "ACTIVO")
            .Select(e => new { e.id_especie, e.nombre_cientifico_especie, e.nombre_comun_especie, e.imagen_especie })
            .ToListAsync(ct);

        static string? Mime(byte[] bytes)
        {
            if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return "image/png";
            if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8) return "image/jpeg";
            if (bytes.Length >= 12 && bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 && bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50) return "image/webp";
            if (bytes.Length >= 6 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46) return "image/gif";
            return null;
        }

        return filas.Select(f => new EspecieResumenDto
        {
            id_especie = f.id_especie,
            nombre_cientifico_especie = f.nombre_cientifico_especie,
            nombre_comun_especie = f.nombre_comun_especie,
            imagen_especie = f.imagen_especie != null ? Convert.ToBase64String(f.imagen_especie) : string.Empty,
            mime_tipo = f.imagen_especie != null ? Mime(f.imagen_especie) : null
        }).ToList();
    }

}





