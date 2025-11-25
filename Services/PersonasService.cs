using Backend_Jardin.Data;
using Backend_Jardin.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend_Jardin.Services;

public class PersonasService
{
    private readonly JardinContext _db;

    public PersonasService(JardinContext db)
    {
        _db = db;
    }

    public async Task<(List<PersonaResumenDto> items, int total)> GetPersonasAsync(
        int page,
        int pageSize,
        string? search,
        int? rolId,
        CancellationToken ct = default)
    {
        var query = _db.personas
            .Include(p => p.fk_rolNavigation)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            var pattern = $"%{term}%";
            query = query.Where(p =>
                EF.Functions.Like(p.nombres_persona, pattern) ||
                EF.Functions.Like(p.apellidos_persona, pattern) ||
                (p.correo_persona != null && EF.Functions.Like(p.correo_persona, pattern)));
        }

        if (rolId.HasValue)
        {
            query = query.Where(p => p.fk_rol == rolId.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(p => p.nombres_persona)
            .ThenBy(p => p.apellidos_persona)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PersonaResumenDto
            {
                id_persona = p.id_persona,
                nombres_persona = p.nombres_persona,
                apellidos_persona = p.apellidos_persona,
                nombre_completo = $"{p.nombres_persona} {p.apellidos_persona}".Trim(),
                correo_persona = p.correo_persona,
                telefono_persona = p.telefono_persona,
                fk_rol = p.fk_rol,
                nombre_rol = p.fk_rolNavigation != null ? p.fk_rolNavigation.nombre_rol : string.Empty,
                estado = p.estado
            })
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<List<RolDto>> GetRolesActivosAsync(CancellationToken ct = default)
    {
        return await _db.rols
            .AsNoTracking()
            .Where(r => r.estado == "ACTIVO")
            .OrderBy(r => r.nombre_rol)
            .Select(r => new RolDto
            {
                id_rol = r.id_rol,
                nombre_rol = r.nombre_rol
            })
            .ToListAsync(ct);
    }

    public async Task<PersonaUpdateRolResult> UpdateRolAsync(int personaId, int nuevoRolId, CancellationToken ct = default)
    {
        var persona = await _db.personas.FirstOrDefaultAsync(p => p.id_persona == personaId, ct);
        if (persona == null)
            return PersonaUpdateRolResult.PersonaNotFound;

        var rolExiste = await _db.rols.AnyAsync(r => r.id_rol == nuevoRolId, ct);
        if (!rolExiste)
            return PersonaUpdateRolResult.RolNotFound;

        persona.fk_rol = nuevoRolId;
        await _db.SaveChangesAsync(ct);
        return PersonaUpdateRolResult.Success;
    }

    public async Task<bool> DeletePersonaAsync(int personaId, CancellationToken ct = default)
    {
        var persona = await _db.personas.FirstOrDefaultAsync(p => p.id_persona == personaId, ct);
        if (persona == null)
            return false;

        _db.personas.Remove(persona);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PersonaPerfilDto?> ObtenerPerfilAsync(int personaId, CancellationToken ct = default)
    {
        return await _db.personas
            .AsNoTracking()
            .Where(p => p.id_persona == personaId)
            .Select(p => new PersonaPerfilDto
            {
                id_persona = p.id_persona,
                nombres_persona = p.nombres_persona,
                apellidos_persona = p.apellidos_persona,
                correo_persona = p.correo_persona,
                telefono_persona = p.telefono_persona
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PersonaActualizarPerfilResultado> ActualizarPerfilAsync(int personaId, PersonaActualizarPerfilDto dto, CancellationToken ct = default)
    {
        var persona = await _db.personas.FirstOrDefaultAsync(p => p.id_persona == personaId, ct);
        if (persona == null)
            return PersonaActualizarPerfilResultado.PersonaNotFound;

        var existeCorreo = await _db.personas.AnyAsync(p => p.id_persona != personaId && p.correo_persona == dto.correo_persona, ct);
        if (existeCorreo)
            return PersonaActualizarPerfilResultado.CorreoDuplicado;

        persona.nombres_persona = dto.nombres_persona.Trim();
        persona.apellidos_persona = dto.apellidos_persona.Trim();
        persona.correo_persona = dto.correo_persona.Trim();
        persona.telefono_persona = dto.telefono_persona;

        if (!string.IsNullOrWhiteSpace(dto.contrasena))
        {
            try
            {
                persona.contrasena_persona = BCrypt.Net.BCrypt.HashPassword(dto.contrasena);
            }
            catch
            {
                persona.contrasena_persona = dto.contrasena;
            }
        }

        await _db.SaveChangesAsync(ct);
        return PersonaActualizarPerfilResultado.Success;
    }
}

public enum PersonaUpdateRolResult
{
    Success,
    PersonaNotFound,
    RolNotFound
}

public enum PersonaActualizarPerfilResultado
{
    Success,
    PersonaNotFound,
    CorreoDuplicado
}
