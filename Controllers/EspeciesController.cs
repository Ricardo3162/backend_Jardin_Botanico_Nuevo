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

    private string BuildImageUrl(int id, bool second = false)
    {
        var action = second ? nameof(GetImagen2) : nameof(GetImagen);
        var url = Url.Action(action, new { id });
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }
        var suffix = second ? "imagen2" : "imagen";
        return $"/api/Especies/{id}/{suffix}";
    }


    [HttpGet("activas")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EspecieListDto>>> GetActivas(CancellationToken ct)
    {
        var lista = await _service.GetActivasAsync(ct);
        var result = lista.Select(e => new EspecieListDto
        {
            id_especie = e.id_especie,
            codigo_interno_especie = e.codigo_interno_especie ?? string.Empty,
            qr = e.codigo_interno_especie ?? string.Empty,
            nombre_comun_especie = e.nombre_comun_especie,
            nombre_cientifico_especie = e.nombre_cientifico_especie,
            familia_especie = e.familia_especie,
            descripcion_especie = e.descripcion_especie ?? string.Empty,
            uso_especie = e.fk_usoNavigation?.nombre_uso ?? string.Empty,
            origen_especie = e.origen_especie ?? string.Empty,
            fenologia_especie = e.fenologia_especie ?? string.Empty,
            distribucion_colombia_especie = e.distribucion_colombia_especie ?? string.Empty,
            distribucion_caqueta_especie = e.distribucion_caqueta_especie ?? string.Empty,
            distribucion_mundial_especie = e.distribucion_mundial_especie ?? string.Empty,
            observacion_especie = e.observacion_especie,
            muestras_secas_herbario_especie = e.muestras_secas_herbario_especie,
            estado = e.estado,
            imagen_url = BuildImageUrl(e.id_especie),
            imagen_url2 = e.imagen_especie2 != null ? BuildImageUrl(e.id_especie, second: true) : null,
            codigo_iucn_estado_conservacion = e.fk_estado_conservacionNavigation?.codigo_iucn,
            nombre_estado_conservacion = e.fk_estado_conservacionNavigation?.nombre_estado,
            ubicaciones = e.especie_ubicacions
                .Select(u => u.fk_ubicacionNavigation.nombre_ubicacion ?? string.Empty)
                .ToList()
        });

        return Ok(result);
    }

    [HttpGet("activas/resumen")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<object>>> GetActivasResumen(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        [FromQuery] int? ubicacionId = null,
        [FromQuery] string? uso = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 15 : pageSize;
        var (items, total) = await _service.GetActivasResumenPageAsync(page, pageSize, ubicacionId, uso, search, ct);
        var mapped = items.Select(e => new {
            id_especie = e.id_especie,
            nombre_cientifico_especie = e.nombre_cientifico_especie,
            nombre_comun_especie = e.nombre_comun_especie,
            imagen_url = BuildImageUrl(e.id_especie)
        }).ToList();
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        var response = new PagedResult<object>
        {
            page = page,
            pageSize = pageSize,
            total = total,
            totalPages = totalPages,
            items = mapped.Cast<object>().ToList()
        };
        return Ok(response);
    }

    [HttpGet("filtros/ubicaciones")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UbicacionDto>>> GetUbicacionesActivas(CancellationToken ct = default)
    {
        var ubicaciones = await _service.GetUbicacionesActivasAsync(ct);
        return Ok(ubicaciones);
    }

    [HttpGet("filtros/usos")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UsoDto>>> GetUsosActivos(CancellationToken ct = default)
    {
        var usos = await _service.GetUsosActivosAsync(ct);
        return Ok(usos);
    }

    [HttpGet("filtros/estados")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EstadoConservacionDto>>> GetEstadosConservacionActivas(CancellationToken ct = default)
    {
        var estados = await _service.GetEstadosConservacionActivosAsync(ct);
        return Ok(estados);
    }


        [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<EspecieListDto>> GetById(int id, CancellationToken ct)
    {
        var e = await _service.GetByIdAsync(id, ct);
        if (e is null || !string.Equals(e.estado, "ACTIVO", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = "No se encontr? la especie activa." });

        var dto = new EspecieListDto
        {
            id_especie = e.id_especie,
            codigo_interno_especie = e.codigo_interno_especie ?? string.Empty,
            nombre_comun_especie = e.nombre_comun_especie,
            nombre_cientifico_especie = e.nombre_cientifico_especie,
            familia_especie = e.familia_especie,
            descripcion_especie = e.descripcion_especie ?? string.Empty,
            uso_especie = e.fk_usoNavigation?.nombre_uso ?? string.Empty,
            origen_especie = e.origen_especie ?? string.Empty,
            fenologia_especie = e.fenologia_especie ?? string.Empty,
            distribucion_colombia_especie = e.distribucion_colombia_especie ?? string.Empty,
            distribucion_caqueta_especie = e.distribucion_caqueta_especie ?? string.Empty,
            distribucion_mundial_especie = e.distribucion_mundial_especie ?? string.Empty,
            observacion_especie = e.observacion_especie,
            muestras_secas_herbario_especie = e.muestras_secas_herbario_especie,
            estado = e.estado,
            imagen_url = BuildImageUrl(e.id_especie),
            imagen_url2 = e.imagen_especie2 != null ? BuildImageUrl(e.id_especie, second: true) : null,
            codigo_iucn_estado_conservacion = e.fk_estado_conservacionNavigation?.codigo_iucn,
            nombre_estado_conservacion = e.fk_estado_conservacionNavigation?.nombre_estado,
            ubicaciones = e.especie_ubicacions
                .Select(u => u.fk_ubicacionNavigation.nombre_ubicacion ?? string.Empty)
                .ToList()
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

    [HttpGet("{id:int}/imagen2")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImagen2(int id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        if (item == null || item.imagen_especie2 == null || item.imagen_especie2.Length == 0)
            return NotFound(new { error = "Sin imagen secundaria" });
        return File(item.imagen_especie2, DetectContentType(item.imagen_especie2));
    }

    // ----------------------- POST (multipart/form-data) -----------------------

    
    [HttpPost]
    // [Authorize(Roles = "Colaborador,Administrador,Docente")] // reactivar cuando aplique
    [AllowAnonymous]
    public async Task<ActionResult<EspecieListDto>> Create(
            [FromForm(Name = "file")] IFormFile[] files,
            [FromForm] string nombre_comun_especie,
            [FromForm] string nombre_cientifico_especie,
            [FromForm] string descripcion_especie,
            [FromForm] string? familia_especie,
            [FromForm] int fk_uso,
            [FromForm] string origen_especie,
            [FromForm] string fenologia_especie,
            [FromForm] string distribucion_colombia_especie,
            [FromForm] string distribucion_caqueta_especie,
            [FromForm] string distribucion_mundial_especie,
            [FromForm] bool muestras_secas_herbario_especie,
            [FromForm] string? observacion_especie,
            [FromForm] string? estado,
            [FromForm] int? fk_estado_conservacion,
            [FromForm] int[]? ubicaciones,
            CancellationToken ct)
    {
        if (files == null || files.Length == 0)
            return BadRequest(new { error = "Archivo requerido: 'file'" });

        if (string.IsNullOrWhiteSpace(nombre_comun_especie)) return BadRequest(new { error = "nombre_comun_especie es requerido" });
        if (string.IsNullOrWhiteSpace(nombre_cientifico_especie)) return BadRequest(new { error = "nombre_cientifico_especie es requerido" });
        if (string.IsNullOrWhiteSpace(descripcion_especie)) return BadRequest(new { error = "descripcion_especie es requerido" });
        if (fk_uso <= 0) return BadRequest(new { error = "fk_uso es requerido" });
        if (string.IsNullOrWhiteSpace(origen_especie)) return BadRequest(new { error = "origen_especie es requerido" });
        if (string.IsNullOrWhiteSpace(fenologia_especie)) return BadRequest(new { error = "fenologia_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_colombia_especie)) return BadRequest(new { error = "distribucion_colombia_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_caqueta_especie)) return BadRequest(new { error = "distribucion_caqueta_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_mundial_especie)) return BadRequest(new { error = "distribucion_mundial_especie es requerido" });

        if (!fk_estado_conservacion.HasValue)
            return BadRequest(new { error = "fk_estado_conservacion es requerido" });

        if (ubicaciones == null || ubicaciones.Length == 0)
            return BadRequest(new { error = "Debe enviar al menos una ubicacion" });

        byte[] imgBytes;
        byte[]? imgBytes2 = null;
        using (var ms = new MemoryStream())
        {
            await files[0].CopyToAsync(ms, ct);
            imgBytes = ms.ToArray();
        }
        if (files.Length > 1)
        {
            using var ms2 = new MemoryStream();
            await files[1].CopyToAsync(ms2, ct);
            imgBytes2 = ms2.ToArray();
        }

        var entity = new especie
        {
            nombre_comun_especie = nombre_comun_especie.Trim(),
            nombre_cientifico_especie = nombre_cientifico_especie.Trim(),
            descripcion_especie = descripcion_especie.Trim(),
            familia_especie = string.IsNullOrWhiteSpace(familia_especie) ? null : familia_especie.Trim(),
            imagen_especie = imgBytes,
            imagen_especie2 = imgBytes2,
            origen_especie = origen_especie.Trim(),
            fenologia_especie = fenologia_especie.Trim(),
            distribucion_colombia_especie = distribucion_colombia_especie.Trim(),
            distribucion_caqueta_especie = distribucion_caqueta_especie.Trim(),
            distribucion_mundial_especie = distribucion_mundial_especie.Trim(),
            observacion_especie = string.IsNullOrWhiteSpace(observacion_especie) ? null : observacion_especie.Trim(),
            muestras_secas_herbario_especie = muestras_secas_herbario_especie,
            estado = string.IsNullOrWhiteSpace(estado) ? "ACTIVO" : estado!,
            fk_estado_conservacion = fk_estado_conservacion,
            fk_uso = fk_uso
        };

        var creada = await _service.CreateAsync(entity, ct, ubicacionIds: ubicaciones);
        var creadaFull = await _service.GetByIdAsync(creada.id_especie, ct) ?? creada;
        var dto = new EspecieListDto
        {
            id_especie = creadaFull.id_especie,
            codigo_interno_especie = creadaFull.codigo_interno_especie ?? string.Empty,
            qr = creadaFull.codigo_interno_especie ?? string.Empty,
            nombre_comun_especie = creadaFull.nombre_comun_especie,
            nombre_cientifico_especie = creadaFull.nombre_cientifico_especie,
            familia_especie = creadaFull.familia_especie,
            descripcion_especie = creadaFull.descripcion_especie ?? string.Empty,
            uso_especie = creadaFull.fk_usoNavigation?.nombre_uso ?? string.Empty,
            fk_uso = creadaFull.fk_uso,
            origen_especie = creadaFull.origen_especie ?? string.Empty,
            fenologia_especie = creadaFull.fenologia_especie ?? string.Empty,
            distribucion_colombia_especie = creadaFull.distribucion_colombia_especie ?? string.Empty,
            distribucion_caqueta_especie = creadaFull.distribucion_caqueta_especie ?? string.Empty,
            distribucion_mundial_especie = creadaFull.distribucion_mundial_especie ?? string.Empty,
            observacion_especie = creadaFull.observacion_especie,
            muestras_secas_herbario_especie = creadaFull.muestras_secas_herbario_especie,
            estado = creadaFull.estado,
            imagen_url = BuildImageUrl(creadaFull.id_especie),
            imagen_url2 = creadaFull.imagen_especie2 != null ? BuildImageUrl(creadaFull.id_especie, second: true) : null,
            codigo_iucn_estado_conservacion = creadaFull.fk_estado_conservacionNavigation?.codigo_iucn,
            nombre_estado_conservacion = creadaFull.fk_estado_conservacionNavigation?.nombre_estado,
            ubicaciones = creadaFull.especie_ubicacions
                .Select(u => u.fk_ubicacionNavigation.nombre_ubicacion ?? string.Empty)
                .ToList()
        };
        return CreatedAtAction(nameof(GetById), new { id = dto.id_especie }, dto);
    }

    // ----------------------- PUT (multipart/form-data) -----------------------
    [HttpPut("{id:int}")]
    // [Authorize(Roles = "Colaborador,Administrador,Docente")] // reactivar cuando aplique
    [AllowAnonymous]
    public async Task<IActionResult> Update(
            int id,
            [FromForm(Name = "file")] IFormFile[] files,
            [FromForm] string nombre_comun_especie,
            [FromForm] string nombre_cientifico_especie,
            [FromForm] string descripcion_especie,
            [FromForm] string? familia_especie,
            [FromForm] int fk_uso,
            [FromForm] string origen_especie,
            [FromForm] string fenologia_especie,
            [FromForm] string distribucion_colombia_especie,
            [FromForm] string distribucion_caqueta_especie,
            [FromForm] string distribucion_mundial_especie,
            [FromForm] bool muestras_secas_herbario_especie,
            [FromForm] string? observacion_especie,
            [FromForm] string? estado,
            [FromForm] int? fk_estado_conservacion,
            [FromForm] int[]? ubicaciones,
            CancellationToken ct)
    {
        if (files == null || files.Length == 0)
            return BadRequest(new { error = "Archivo requerido: 'file'" });

        if (string.IsNullOrWhiteSpace(nombre_comun_especie)) return BadRequest(new { error = "nombre_comun_especie es requerido" });
        if (string.IsNullOrWhiteSpace(nombre_cientifico_especie)) return BadRequest(new { error = "nombre_cientifico_especie es requerido" });
        if (string.IsNullOrWhiteSpace(descripcion_especie)) return BadRequest(new { error = "descripcion_especie es requerido" });
        if (fk_uso <= 0) return BadRequest(new { error = "fk_uso es requerido" });
        if (string.IsNullOrWhiteSpace(origen_especie)) return BadRequest(new { error = "origen_especie es requerido" });
        if (string.IsNullOrWhiteSpace(fenologia_especie)) return BadRequest(new { error = "fenologia_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_colombia_especie)) return BadRequest(new { error = "distribucion_colombia_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_caqueta_especie)) return BadRequest(new { error = "distribucion_caqueta_especie es requerido" });
        if (string.IsNullOrWhiteSpace(distribucion_mundial_especie)) return BadRequest(new { error = "distribucion_mundial_especie es requerido" });

        byte[] imgBytes;
        byte[]? imgBytes2 = null;
        using (var ms = new MemoryStream())
        {
            await files[0].CopyToAsync(ms, ct);
            imgBytes = ms.ToArray();
        }
        if (files.Length > 1)
        {
            using var ms2 = new MemoryStream();
            await files[1].CopyToAsync(ms2, ct);
            imgBytes2 = ms2.ToArray();
        }

        var ok = await _service.UpdateAsync(id, e =>
        {
            e.nombre_comun_especie = nombre_comun_especie.Trim();
            e.nombre_cientifico_especie = nombre_cientifico_especie.Trim();
            e.descripcion_especie = descripcion_especie.Trim();
            if (familia_especie != null) e.familia_especie = string.IsNullOrWhiteSpace(familia_especie) ? null : familia_especie.Trim();
            e.imagen_especie = imgBytes;
            if (imgBytes2 != null)
            {
                e.imagen_especie2 = imgBytes2;
            }
            e.fk_uso = fk_uso;
            e.origen_especie = origen_especie.Trim();
            e.fenologia_especie = fenologia_especie.Trim();
            e.distribucion_colombia_especie = distribucion_colombia_especie.Trim();
            e.distribucion_caqueta_especie = distribucion_caqueta_especie.Trim();
            e.distribucion_mundial_especie = distribucion_mundial_especie.Trim();
            if (observacion_especie != null) e.observacion_especie = string.IsNullOrWhiteSpace(observacion_especie) ? null : observacion_especie.Trim();
            e.muestras_secas_herbario_especie = muestras_secas_herbario_especie;
            if (!string.IsNullOrWhiteSpace(estado)) e.estado = estado!;
            if (fk_estado_conservacion.HasValue) e.fk_estado_conservacion = fk_estado_conservacion.Value;
        }, ct);

        if (!ok) return NotFound(new { error = "No se encontr? la especie." });

        if (ubicaciones != null && ubicaciones.Length > 0)
        {
            await _service.UpdateUbicacionesAsync(id, ubicaciones, ct);
        }

        return NoContent();
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

        if (!ok) return NotFound(new { error = "No se encontr? la especie." });

        return NoContent();
    }

    [HttpGet("codigo/{codigo}")]
    [AllowAnonymous]
    public async Task<ActionResult<EspecieListDto>> GetByCodigo(string codigo, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return BadRequest(new { error = "codigo requerido" });
        var e = await _service.GetByCodigoAsync(codigo, ct);
        if (e is null || !string.Equals(e.estado, "ACTIVO", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = "No se encontr? la especie activa." });

        var dto = new EspecieListDto
        {
            id_especie = e.id_especie,
            codigo_interno_especie = e.codigo_interno_especie ?? string.Empty,
            nombre_comun_especie = e.nombre_comun_especie,
            nombre_cientifico_especie = e.nombre_cientifico_especie,
            familia_especie = e.familia_especie,
            descripcion_especie = e.descripcion_especie ?? string.Empty,
            uso_especie = e.fk_usoNavigation?.nombre_uso ?? string.Empty,
            origen_especie = e.origen_especie ?? string.Empty,
            fenologia_especie = e.fenologia_especie ?? string.Empty,
            distribucion_colombia_especie = e.distribucion_colombia_especie ?? string.Empty,
            distribucion_caqueta_especie = e.distribucion_caqueta_especie ?? string.Empty,
            distribucion_mundial_especie = e.distribucion_mundial_especie ?? string.Empty,
            observacion_especie = e.observacion_especie,
            muestras_secas_herbario_especie = e.muestras_secas_herbario_especie,
            estado = e.estado,
            imagen_url = BuildImageUrl(e.id_especie),
            imagen_url2 = e.imagen_especie2 != null ? BuildImageUrl(e.id_especie, second: true) : null,
            codigo_iucn_estado_conservacion = e.fk_estado_conservacionNavigation?.codigo_iucn,
            nombre_estado_conservacion = e.fk_estado_conservacionNavigation?.nombre_estado,
            ubicaciones = e.especie_ubicacions
                .Select(u => u.fk_ubicacionNavigation.nombre_ubicacion ?? string.Empty)
                .ToList()
        };
        return Ok(dto);
    }

    [HttpGet("{id:int}/detalle")]
    [AllowAnonymous]
    public async Task<ActionResult<EspecieDetalleDto>> GetDetalle(int id, CancellationToken ct)
    {
        var detalle = await _service.GetDetalleAsync(id, ct);
        if (detalle == null)
            return NotFound(new { error = "Especie no encontrada" });
        return Ok(detalle);
    }
}




























