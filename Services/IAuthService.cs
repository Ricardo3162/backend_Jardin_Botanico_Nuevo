using Backend_Jardin.DTOs;

namespace Backend_Jardin.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest req, CancellationToken ct = default);
}

