using Backend_Jardin.DTOs;
using Backend_Jardin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Backend_Jardin.Services;

namespace Backend_Jardin.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        var result = await _authService.LoginAsync(req);
        if (result == null)
            return Unauthorized("Credenciales inválidas");
        return Ok(result);
    }
}
