using Backend_Jardin.Models;

namespace Backend_Jardin.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerarJwt(persona p, string rol);
}

