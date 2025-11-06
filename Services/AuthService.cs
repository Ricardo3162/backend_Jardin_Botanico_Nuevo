using Backend_Jardin.DTOs;
using Backend_Jardin.Models;
using Backend_Jardin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Backend_Jardin.Services;

public class AuthService : IAuthService
{
    private readonly JardinContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(JardinContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest req, CancellationToken ct = default)
    {
        if (req == null || string.IsNullOrWhiteSpace(req.Contrasena) ||
            (string.IsNullOrWhiteSpace(req.Correo) && string.IsNullOrWhiteSpace(req.Usuario)))
        {
            return null;
        }

        var query = _db.personas
            .Include(p => p.fk_rolNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Correo))
            query = query.Where(p => p.correo_persona == req.Correo);
        else if (!string.IsNullOrWhiteSpace(req.Usuario))
            query = query.Where(p => p.correo_persona == req.Usuario);

        var persona = await query.FirstOrDefaultAsync(ct);
        if (persona == null) return null;

        var contrasenaDb = persona.contrasena_persona ?? string.Empty;

        var passwordOk = false;
        try
        {
            if (!string.IsNullOrEmpty(contrasenaDb) && contrasenaDb.StartsWith("$2"))
                passwordOk = BCrypt.Net.BCrypt.Verify(req.Contrasena, contrasenaDb);
        }
        catch { }

        if (!passwordOk)
            passwordOk = string.Equals(contrasenaDb, req.Contrasena);

        if (!passwordOk) return null;

        var rol = persona.fk_rolNavigation?.nombre_rol ?? "Visitante";

        var (token, expira) = _tokenService.GenerarJwt(persona, rol);

        return new AuthResponse
        {
            Token = token,
            Expira = expira,
            Rol = rol,
            IdPersona = persona.id_persona,
            Usuario = null,
            Correo = persona.correo_persona,
            NombreCompleto = persona.nombre_completo_persona
        };
    }

    
}




