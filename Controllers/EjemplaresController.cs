using Backend_Jardin.Data;
using Backend_Jardin.Models;
using Backend_Jardin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Jardin.DTOs;

namespace Backend_Jardin.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EjemplaresController : ControllerBase
{
    private readonly EjemplaresService _service;
    private readonly JardinContext _db;
    public EjemplaresController(EjemplaresService service, JardinContext db)
    {
        _service = service;
        _db = db;
    }

    // GET por código interno (solo si la especie está ACTIVA). Sin estados en la respuesta.
    [HttpGet("codigo/{codigo}")]
    [AllowAnonymous]
    public async Task<ActionResult<EjemplarCodigoDto>> GetByCodigo(string codigo, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return BadRequest(new { error = "Código requerido" });
        var e = await _service.GetByCodigoAsync(codigo.Trim(), ct);
        if (e == null || e.fk_especieNavigation == null || !string.Equals(e.fk_especieNavigation.estado, "ACTIVO", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = "No se encontró el ejemplar." });

        // URLs a imagen de especie y a la imagen1 del ejemplar
        string img1Url = Url.Action(nameof(GetImagen1), new { id = e.id_ejemplar }) ?? $"/api/Ejemplares/{e.id_ejemplar}/imagen1";
        string especieImgUrl = Url.Action("GetImagen", "Especies", new { id = e.fk_especie }, Request.Scheme) ?? $"/api/Especies/{e.fk_especie}/imagen";

        var dto = new EjemplarCodigoDto
        {
            id_ejemplar = e.id_ejemplar,
            codigo_interno_ejemplar = e.codigo_interno_ejemplar,
            coordenadas_ejemplar = e.coordenadas_ejemplar,
            fecha_registro = e.fecha_registro,
            fk_especie = e.fk_especie,
            fk_ubicacion = e.fk_ubicacion,
            nombre_ubicacion = e.fk_ubicacionNavigation?.nombre_ubicacion ?? string.Empty,
            imagen1_url = (e.imagen1_ejemplar != null && e.imagen1_ejemplar.Length > 0) ? img1Url : null,
            especie_imagen_url = especieImgUrl,
            especie = new EspecieDeEjemplarDto
            {
                id_especie = e.fk_especieNavigation.id_especie,
                nombre_comun_especie = e.fk_especieNavigation.nombre_comun_especie,
                nombre_cientifico_especie = e.fk_especieNavigation.nombre_cientifico_especie,
                descripcion_especie = e.fk_especieNavigation.descripcion_especie ?? string.Empty,
                uso_especie = e.fk_especieNavigation.uso_especie ?? string.Empty,
                origen_especie = e.fk_especieNavigation.origen_especie ?? string.Empty,
                fenologia_especie = e.fk_especieNavigation.fenologia_especie ?? string.Empty,
                distribucion_colombia_especie = e.fk_especieNavigation.distribucion_colombia_especie ?? string.Empty,
                distribucion_caqueta_especie = e.fk_especieNavigation.distribucion_caqueta_especie ?? string.Empty,
                distribucion_mundial_especie = e.fk_especieNavigation.distribucion_mundial_especie ?? string.Empty,
                muestras_secas_herbario_especie = e.fk_especieNavigation.muestras_secas_herbario_especie
            }
        };
        return Ok(dto);
    }

    // Endpoints binarios de ejemplar (necesarios para las URLs)
    [HttpGet("{id:int}/imagen1")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImagen1(int id, CancellationToken ct)
    {
        var e = await _db.ejemplars.AsNoTracking().FirstOrDefaultAsync(x => x.id_ejemplar == id, ct);
        if (e == null || e.imagen1_ejemplar == null || e.imagen1_ejemplar.Length == 0) return NotFound(new { error = "Sin imagen1" });
        var ctType = DetectContentType(e.imagen1_ejemplar);
        return File(e.imagen1_ejemplar, ctType);
    }

    [HttpGet("{id:int}/imagen2")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImagen2(int id, CancellationToken ct)
    {
        var e = await _db.ejemplars.AsNoTracking().FirstOrDefaultAsync(x => x.id_ejemplar == id, ct);
        if (e == null || e.imagen2_ejemplar == null || e.imagen2_ejemplar.Length == 0) return NotFound(new { error = "Sin imagen2" });
        var ctType = DetectContentType(e.imagen2_ejemplar);
        return File(e.imagen2_ejemplar, ctType);
    }

    private static string DetectContentType(byte[] bytes)
    {
        if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return "image/png";
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8) return "image/jpeg";
        if (bytes.Length >= 12 && bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 && bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50) return "image/webp";
        if (bytes.Length >= 6 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46) return "image/gif";
        return "application/octet-stream";
    }

    // POST multipart/form-data: crea un ejemplar (file1 requerido, file2 opcional)
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(
        [FromForm(Name = "fk_especie")] int fk_especie,
        [FromForm(Name = "fk_ubicacion")] int fk_ubicacion,
        [FromForm(Name = "file1")] IFormFile file1,
        [FromForm(Name = "file2")] IFormFile? file2,
        [FromForm(Name = "coordenadas_ejemplar")] string? coordenadas,
        [FromForm(Name = "detalle_ubicacion")] string? detalle,
        CancellationToken ct = default)
    {
        if (file1 == null || file1.Length == 0)
            return BadRequest(new { error = "Archivo requerido: 'file1'" });

        var especieOk = await _db.especies.AsNoTracking().AnyAsync(e => e.id_especie == fk_especie, ct);
        if (!especieOk) return BadRequest(new { error = "fk_especie no existe." });
        var ubicacionOk = await _db.ubicacions.AsNoTracking().AnyAsync(u => u.id_ubicacion == fk_ubicacion, ct);
        if (!ubicacionOk) return BadRequest(new { error = "fk_ubicacion no existe." });

        byte[] img1;
        using (var ms = new MemoryStream()) { await file1.CopyToAsync(ms, ct); img1 = ms.ToArray(); }
        byte[]? img2 = null;
        if (file2 != null && file2.Length > 0)
        {
            using var ms2 = new MemoryStream();
            await file2.CopyToAsync(ms2, ct);
            img2 = ms2.ToArray();
        }

        var entity = new ejemplar
        {
            // Dejar vacío para que la base de datos/trigger genere el código interno
            codigo_interno_ejemplar = string.Empty,
            coordenadas_ejemplar = string.IsNullOrWhiteSpace(coordenadas) ? null : coordenadas.Trim(),
            imagen1_ejemplar = img1,
            imagen2_ejemplar = img2,
            fk_especie = fk_especie,
            fk_ubicacion = fk_ubicacion,
            detalle_ubicacion = string.IsNullOrWhiteSpace(detalle) ? null : detalle.Trim(),
            estado = "ACTIVO"
        };

        var creada = await _service.CreateAsync(entity, ct);
        // Recargar para obtener valores generados por la BD (código interno)
        await _db.Entry(creada).ReloadAsync(ct);
        return Created($"/api/Ejemplares/{creada.id_ejemplar}", new { id = creada.id_ejemplar, codigo_interno_ejemplar = creada.codigo_interno_ejemplar });
    }

    // PUT multipart/form-data: actualiza datos y reemplaza imágenes si se envían
    [HttpPut("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Update(int id,
        [FromForm(Name = "fk_especie")] int fk_especie,
        [FromForm(Name = "fk_ubicacion")] int fk_ubicacion,
        [FromForm(Name = "coordenadas_ejemplar")] string? coordenadas,
        [FromForm(Name = "detalle_ubicacion")] string? detalle,
        [FromForm(Name = "file1")] IFormFile? file1,
        [FromForm(Name = "file2")] IFormFile? file2,
        CancellationToken ct)
    {
        var especieOk = await _db.especies.AsNoTracking().AnyAsync(e => e.id_especie == fk_especie, ct);
        if (!especieOk) return BadRequest(new { error = "fk_especie no existe." });
        var ubicacionOk = await _db.ubicacions.AsNoTracking().AnyAsync(u => u.id_ubicacion == fk_ubicacion, ct);
        if (!ubicacionOk) return BadRequest(new { error = "fk_ubicacion no existe." });

        byte[]? img1 = null;
        if (file1 != null && file1.Length > 0)
        {
            using var ms = new MemoryStream();
            await file1.CopyToAsync(ms, ct);
            img1 = ms.ToArray();
        }
        byte[]? img2 = null;
        if (file2 != null && file2.Length > 0)
        {
            using var ms2 = new MemoryStream();
            await file2.CopyToAsync(ms2, ct);
            img2 = ms2.ToArray();
        }

        var ok = await _service.UpdateAsync(id, e =>
        {
            // No modificar el código interno generado
            if (!string.IsNullOrWhiteSpace(coordenadas)) e.coordenadas_ejemplar = coordenadas.Trim();
            if (img1 != null) e.imagen1_ejemplar = img1;
            if (img2 != null) e.imagen2_ejemplar = img2;
            e.fk_especie = fk_especie;
            e.fk_ubicacion = fk_ubicacion;
            if (detalle != null)
                e.detalle_ubicacion = string.IsNullOrWhiteSpace(detalle) ? null : detalle.Trim();
        }, ct);

        return ok ? NoContent() : NotFound(new { error = "No se encontró el ejemplar." });
    }
}

