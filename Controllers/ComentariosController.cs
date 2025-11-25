using Backend_Jardin.DTOs;
using Backend_Jardin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Jardin.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComentariosController : ControllerBase
{
    private readonly ComentariosService _service;

    public ComentariosController(ComentariosService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<ComentarioResumenDto>>> GetComentarios(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 15 : pageSize;

        var (items, total) = await _service.GetComentariosAsync(page, pageSize, search, ct);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        var response = new PagedResult<ComentarioResumenDto>
        {
            page = page,
            pageSize = pageSize,
            total = total,
            totalPages = totalPages,
            items = items
        };

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> DeleteComentario(int id, CancellationToken ct = default)
    {
        var eliminado = await _service.DeleteComentarioAsync(id, ct);
        if (!eliminado)
            return NotFound();

        return NoContent();
    }
}
