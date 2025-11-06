using Backend_Jardin.DTOs;
using Backend_Jardin.Models;
using Backend_Jardin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Backend_Jardin.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EspeciesController : ControllerBase
{
    private readonly EspeciesService _service;
    public EspeciesController(EspeciesService service)
    {
        _service = service;
    }

    // ----------------------- Helpers -----------------------
    private static string DetectContentType(byte[] bytes)
    {
        if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return "image/png";
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8) return "image/jpeg";
        if (bytes.Length >= 12 && bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 && bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50) return "image/webp";
        if (bytes.Length >= 6 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46) return "image/gif";
        return "application/octet-stream";
    }

    // Sin helpers base64 en este controlador; usamos binario por endpoint dedicado

    // ----------------------- GET -----------------------
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EspecieListDto>>> GetAll(CancellationToken ct)
    {
        var lista = await _service.GetAllAsync(ct);
        string BaseUrl(int id) => Url.Action(nameof(GetImagen), new { id }) ?? $"/api/Especies/{id}/imagen";
        var result = lista.Select(e => new EspecieListDto
        {
            id_especie = e.id_especie,
            nombre_comun_especie = e.nombre_comun_especie,
            nombre_cientifico_especie = e.nombre_cientifico_especie,
            descripcion_especie = e.descripcion_especie ?? string.Empty,
            uso_especie = e.uso_especie ?? string.Empty,
            origen_especie = e.origen_especie ?? string.Empty,
            fenologia_especie = e.fenologia_especie ?? string.Empty,
            distribucion_colombia_especie = e.distribucion_colombia_especie ?? string.Empty,
            distribucion_caqueta_especie = e.distribucion_caqueta_especie ?? string.Empty,
            distribucion_mundial_especie = e.distribucion_mundial_especie ?? string.Empty,
            muestras_secas_herbario_especie = e.muestras_secas_herbario_especie,
            estado = e.estado,
            imagen_url = BaseUrl(e.id_especie)
        });
        return Ok(result);
    }

    [HttpGet("activas")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EspecieListDto>>> GetActivas(CancellationToken ct)
    {
        var lista = await _service.GetActivasAsync(ct);
        string BaseUrl(int id) => Url.Action(nameof(GetImagen), new { id }) ?? $"/api/Especies/{id}/imagen";
        var result = lista.Select(e => new EspecieListDto
        {
            id_especie = e.id_especie,
            nombre_comun_especie = e.nombre_comun_especie,
            nombre_cientifico_especie = e.nombre_cientifico_especie,
            descripcion_especie = e.descripcion_especie ?? string.Empty,
            uso_especie = e.uso_especie ?? string.Empty,
            origen_especie = e.origen_especie ?? string.Empty,
            fenologia_especie = e.fenologia_especie ?? string.Empty,
            distribucion_colombia_especie = e.distribucion_colombia_especie ?? string.Empty,
            distribucion_caqueta_especie = e.distribucion_caqueta_especie ?? string.Empty,
            distribucion_mundial_especie = e.distribucion_mundial_especie ?? string.Empty,
            muestras_secas_herbario_especie = e.muestras_secas_herbario_especie,
            estado = e.estado,
            imagen_url = BaseUrl(e.id_especie)
        });
        return Ok(result);
    }

    [HttpGet("activas/resumen")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> GetActivasResumen(CancellationToken ct)
    {
        // Resumen ligero sin base64: id, nombres y URL
        var lista = await _service.GetActivasAsync(ct);
        string BaseUrl(int id) => Url.Action(nameof(GetImagen), new { id }) ?? $"/api/Especies/{id}/imagen";
        var result = lista.Select(e => new {
            id_especie = e.id_especie,
            nombre_cientifico_especie = e.nombre_cientifico_especie,
            nombre_comun_especie = e.nombre_comun_especie,
            imagen_url = BaseUrl(e.id_especie)
        });
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<EspecieListDto>> GetById(int id, CancellationToken ct)
    {
        var e = await _service.GetByIdAsync(id, ct);
        if (e is null || !string.Equals(e.estado, "ACTIVO", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = "No se encontró la especie activa." });
        var dto = new EspecieListDto
        {
            id_especie = e.id_especie,
            nombre_comun_especie = e.nombre_comun_especie,
            nombre_cientifico_especie = e.nombre_cientifico_especie,
            descripcion_especie = e.descripcion_especie ?? string.Empty,
            uso_especie = e.uso_especie ?? string.Empty,
            origen_especie = e.origen_especie ?? string.Empty,
            fenologia_especie = e.fenologia_especie ?? string.Empty,
            distribucion_colombia_especie = e.distribucion_colombia_especie ?? string.Empty,
            distribucion_caqueta_especie = e.distribucion_caqueta_especie ?? string.Empty,
            distribucion_mundial_especie = e.distribucion_mundial_especie ?? string.Empty,
            muestras_secas_herbario_especie = e.muestras_secas_herbario_especie,
            estado = e.estado,
            imagen_url = Url.Action(nameof(GetImagen), new { id = e.id_especie }) ?? $"/api/Especies/{e.id_especie}/imagen"
        };
        return Ok(dto);
    }

    [HttpGet("{id:int}/imagen")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImagen(int id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        if (item == null || item.imagen_especie == null || item.imagen_especie.Length == 0)
            return NotFound(new { error = "Sin imagen" });
        return File(item.imagen_especie, DetectContentType(item.imagen_especie));
    }

    // ----------------------- POST (multipart/form-data) -----------------------
    [HttpPost]
    // [Authorize(Roles = "Colaborador,Administrador,Docente")] // reactivar cuando aplique
    [AllowAnonymous]
    public async Task<ActionResult<EspecieResponseDto>> Create(
        [FromForm(Name = "file")] IFormFile file,
        [FromForm] string nombre_comun_especie,
        [FromForm] string nombre_cientifico_especie,
        [FromForm] string descripcion_especie,
        [FromForm] string uso_especie,
        [FromForm] string origen_especie,
        [FromForm] string fenologia_especie,
        [FromForm] string distribucion_colombia_especie,
        [FromForm] string distribucion_caqueta_especie,
        [FromForm] string distribucion_mundial_especie,
        [FromForm] bool muestras_secas_herbario_especie,
        [FromForm] string? estado,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "Archivo requerido: 'file'" });

        if (string.IsNullOrWhiteSpace(nombre_comun_especie)) return BadRequest(new { error = "nombre_comun_especie es requerido" });
        if (string.IsNullOrWhiteSpace(nombre_cientifico_especie)) return BadRequest(new { error = "nombre_cientifico_especie es requerido" });
        if (string.IsNullOrWhiteSpace(descripcion_especie)) return BadRequest(new { error = "descripcion_especie es requerido" });
        if (string.IsNullOrWhiteSpace(uso_especie)) return BadRequest(new { error = "uso_especie es requerido" });
        if (string.IsNullOrWhiteSpace(origen_especie)) return BadRequest(new { error = "origen_especie es requerido" });
        if (string.IsNullOrWhiteSpace(fenologia_especie)) return BadRequest(new { error = "fenologia_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_colombia_especie)) return BadRequest(new { error = "distribucion_colombia_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_caqueta_especie)) return BadRequest(new { error = "distribucion_caqueta_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_mundial_especie)) return BadRequest(new { error = "distribucion_mundial_especie es requerido" });

        byte[] imgBytes;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms, ct);
            imgBytes = ms.ToArray();
        }

        var entity = new especie
        {
            nombre_comun_especie = nombre_comun_especie.Trim(),
            nombre_cientifico_especie = nombre_cientifico_especie.Trim(),
            descripcion_especie = descripcion_especie.Trim(),
            imagen_especie = imgBytes,
            uso_especie = uso_especie.Trim(),
            origen_especie = origen_especie.Trim(),
            fenologia_especie = fenologia_especie.Trim(),
            distribucion_colombia_especie = distribucion_colombia_especie.Trim(),
            distribucion_caqueta_especie = distribucion_caqueta_especie.Trim(),
            distribucion_mundial_especie = distribucion_mundial_especie.Trim(),
            muestras_secas_herbario_especie = muestras_secas_herbario_especie,
            estado = string.IsNullOrWhiteSpace(estado) ? "ACTIVO" : estado!
        };

        var creada = await _service.CreateAsync(entity, ct);
        var creadaFull = await _service.GetByIdAsync(creada.id_especie, ct) ?? creada;
        var dto = new EspecieListDto
        {
            id_especie = creadaFull.id_especie,
            nombre_comun_especie = creadaFull.nombre_comun_especie,
            nombre_cientifico_especie = creadaFull.nombre_cientifico_especie,
            descripcion_especie = creadaFull.descripcion_especie ?? string.Empty,
            uso_especie = creadaFull.uso_especie ?? string.Empty,
            origen_especie = creadaFull.origen_especie ?? string.Empty,
            fenologia_especie = creadaFull.fenologia_especie ?? string.Empty,
            distribucion_colombia_especie = creadaFull.distribucion_colombia_especie ?? string.Empty,
            distribucion_caqueta_especie = creadaFull.distribucion_caqueta_especie ?? string.Empty,
            distribucion_mundial_especie = creadaFull.distribucion_mundial_especie ?? string.Empty,
            muestras_secas_herbario_especie = creadaFull.muestras_secas_herbario_especie,
            estado = creadaFull.estado,
            imagen_url = Url.Action(nameof(GetImagen), new { id = creadaFull.id_especie }) ?? $"/api/Especies/{creadaFull.id_especie}/imagen"
        };
        return CreatedAtAction(nameof(GetById), new { id = dto.id_especie }, dto);
    }

    // ----------------------- PUT (multipart/form-data) -----------------------
    [HttpPut("{id:int}")]
    // [Authorize(Roles = "Colaborador,Administrador,Docente")] // reactivar cuando aplique
    [AllowAnonymous]
    public async Task<IActionResult> Update(
        int id,
        [FromForm(Name = "file")] IFormFile file,
        [FromForm] string nombre_comun_especie,
        [FromForm] string nombre_cientifico_especie,
        [FromForm] string descripcion_especie,
        [FromForm] string uso_especie,
        [FromForm] string origen_especie,
        [FromForm] string fenologia_especie,
        [FromForm] string distribucion_colombia_especie,
        [FromForm] string distribucion_caqueta_especie,
        [FromForm] string distribucion_mundial_especie,
        [FromForm] bool muestras_secas_herbario_especie,
        [FromForm] string? estado,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "Archivo requerido: 'file'" });

        if (string.IsNullOrWhiteSpace(nombre_comun_especie)) return BadRequest(new { error = "nombre_comun_especie es requerido" });
        if (string.IsNullOrWhiteSpace(nombre_cientifico_especie)) return BadRequest(new { error = "nombre_cientifico_especie es requerido" });
        if (string.IsNullOrWhiteSpace(descripcion_especie)) return BadRequest(new { error = "descripcion_especie es requerido" });
        if (string.IsNullOrWhiteSpace(uso_especie)) return BadRequest(new { error = "uso_especie es requerido" });
        if (string.IsNullOrWhiteSpace(origen_especie)) return BadRequest(new { error = "origen_especie es requerido" });
        if (string.IsNullOrWhiteSpace(fenologia_especie)) return BadRequest(new { error = "fenologia_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_colombia_especie)) return BadRequest(new { error = "distribucion_colombia_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_caqueta_especie)) return BadRequest(new { error = "distribucion_caqueta_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_mundial_especie)) return BadRequest(new { error = "distribucion_mundial_especie es requerido" });

        byte[] imgBytes;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms, ct);
            imgBytes = ms.ToArray();
        }

        var ok = await _service.UpdateAsync(id, e =>
        {
            e.nombre_comun_especie = nombre_comun_especie.Trim();
            e.nombre_cientifico_especie = nombre_cientifico_especie.Trim();
            e.descripcion_especie = descripcion_especie.Trim();
            e.imagen_especie = imgBytes;
            e.uso_especie = uso_especie.Trim();
            e.origen_especie = origen_especie.Trim();
            e.fenologia_especie = fenologia_especie.Trim();
            e.distribucion_colombia_especie = distribucion_colombia_especie.Trim();
            e.distribucion_caqueta_especie = distribucion_caqueta_especie.Trim();
            e.distribucion_mundial_especie = distribucion_mundial_especie.Trim();
            e.muestras_secas_herbario_especie = muestras_secas_herbario_especie;
            if (!string.IsNullOrWhiteSpace(estado)) e.estado = estado!;
        }, ct);

        return ok ? NoContent() : NotFound(new { error = "No se encontró la especie." });
    }

    // ----------------------- DELETE (soft delete: estado=INACTIVO) -----------------------
    [HttpDelete("{id:int}")]
    // [Authorize(Roles = "Administrador,Docente")] // reactivar cuando aplique
    [AllowAnonymous]
    public async Task<IActionResult> SoftDelete(int id, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, e =>
        {
            e.estado = "INACTIVO";
        }, ct);

        return ok ? NoContent() : NotFound(new { error = "No se encontró la especie." });
    }
}
