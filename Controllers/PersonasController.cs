using Backend_Jardin.DTOs;
using Backend_Jardin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Jardin.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonasController : ControllerBase
{
    private readonly PersonasService _service;

    public PersonasController(PersonasService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<PersonaResumenDto>>> GetPersonas(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        [FromQuery] string? search = null,
        [FromQuery] int? rolId = null,
        CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 15 : pageSize;

        var (items, total) = await _service.GetPersonasAsync(page, pageSize, search, rolId, ct);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        var response = new PagedResult<PersonaResumenDto>
        {
            page = page,
            pageSize = pageSize,
            total = total,
            totalPages = totalPages,
            items = items
        };

        return Ok(response);
    }

    [HttpGet("roles")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles(CancellationToken ct = default)
    {
        var roles = await _service.GetRolesActivosAsync(ct);
        return Ok(roles);
    }

    [HttpPut("{id:int}/rol")]
    [AllowAnonymous]
    public async Task<IActionResult> ActualizarRol(int id, [FromBody] PersonaActualizarRolDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var resultado = await _service.UpdateRolAsync(id, dto.fk_rol, ct);
        return resultado switch
        {
            PersonaUpdateRolResult.PersonaNotFound => NotFound(),
            PersonaUpdateRolResult.RolNotFound => BadRequest("El rol especificado no existe."),
            _ => NoContent()
        };
    }

    [HttpDelete("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> DeletePersona(int id, CancellationToken ct = default)
    {
        var eliminado = await _service.DeletePersonaAsync(id, ct);
        if (!eliminado)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id:int}/perfil")]
    [AllowAnonymous]
    public async Task<ActionResult<PersonaPerfilDto>> GetPerfil(int id, CancellationToken ct = default)
    {
        var perfil = await _service.ObtenerPerfilAsync(id, ct);
        if (perfil == null)
            return NotFound();
        return Ok(perfil);
    }

    [HttpPut("{id:int}/perfil")]
    [AllowAnonymous]
    public async Task<IActionResult> ActualizarPerfil(int id, [FromBody] PersonaActualizarPerfilDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var resultado = await _service.ActualizarPerfilAsync(id, dto, ct);
        return resultado switch
        {
            PersonaActualizarPerfilResultado.PersonaNotFound => NotFound(),
            PersonaActualizarPerfilResultado.CorreoDuplicado => BadRequest("El correo ya se encuentra registrado para otra persona."),
            _ => NoContent()
        };
    }
}
