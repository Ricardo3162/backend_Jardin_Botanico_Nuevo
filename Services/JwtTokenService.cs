using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend_Jardin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Backend_Jardin.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) { _config = config; }

    public (string Token, DateTime ExpiresAt) GenerarJwt(persona p, string rol)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key") ?? string.Empty;
        var issuer = jwtSection.GetValue<string>("Issuer");
        var audience = jwtSection.GetValue<string>("Audience");

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, p.id_persona.ToString()),
            new Claim(ClaimTypes.NameIdentifier, p.id_persona.ToString()),
            new Claim(ClaimTypes.Name, ($"{p.nombres_persona} {p.apellidos_persona}").Trim()),
            new Claim(ClaimTypes.Email, p.correo_persona ?? string.Empty),
            new Claim(ClaimTypes.Role, rol)
        };

        var expires = DateTime.UtcNow.AddHours(8);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, expires);
    }
}








