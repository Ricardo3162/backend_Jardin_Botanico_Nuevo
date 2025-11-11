using System.ComponentModel.DataAnnotations;

namespace Backend_Jardin.DTOs;

public class EspecieCreateDto
{
    [Required]
    public string nombre_comun_especie { get; set; } = string.Empty;

    [Required]
    public string nombre_cientifico_especie { get; set; } = string.Empty;

    [Required]
    public string? familia_especie { get; set; }
    public string descripcion_especie { get; set; } = string.Empty;

    [Required]
    public string imagen_especie { get; set; } = string.Empty; // base64 sin prefijo
    public string? mime_tipo { get; set; }

    [Required]
    public string uso_especie { get; set; } = string.Empty;

    [Required]
    public string origen_especie { get; set; } = string.Empty;

    [Required]
    public string fenologia_especie { get; set; } = string.Empty;

    [Required]
    public string distribucion_colombia_especie { get; set; } = string.Empty;

    [Required]
    public string distribucion_caqueta_especie { get; set; } = string.Empty;

    [Required]
    public string distribucion_mundial_especie { get; set; } = string.Empty;

    // Opcional: si no viene, la BD usa ACTIVO por defecto
    [RegularExpression("^(ACTIVO|INACTIVO)$", ErrorMessage = "estado debe ser ACTIVO o INACTIVO")]
    public string? estado { get; set; }

    [Required]
    public bool muestras_secas_herbario_especie { get; set; }
}

public class EspecieUpdateDto : EspecieCreateDto {}

public class EspecieResponseDto
{
    public int id_especie { get; set; }
    public string codigo_interno_especie { get; set; } = string.Empty;
    public string nombre_comun_especie { get; set; } = string.Empty;
    public string nombre_cientifico_especie { get; set; } = string.Empty;
    public string? familia_especie { get; set; }
    public string descripcion_especie { get; set; } = string.Empty;
    public string imagen_especie { get; set; } = string.Empty; // base64
    public string? mime_tipo { get; set; }
    public string uso_especie { get; set; } = string.Empty;
    public string origen_especie { get; set; } = string.Empty;
    public string fenologia_especie { get; set; } = string.Empty;
    public string distribucion_colombia_especie { get; set; } = string.Empty;
    public string distribucion_caqueta_especie { get; set; } = string.Empty;
    public string distribucion_mundial_especie { get; set; } = string.Empty;
    public string? observacion_especie { get; set; }
    public bool muestras_secas_herbario_especie { get; set; }
    public string estado { get; set; } = string.Empty;
}

// Listado resumido para GET activos
public class EspecieResumenDto{
    public int id_especie { get; set; }
    public string nombre_cientifico_especie { get; set; } = string.Empty;
    public string nombre_comun_especie { get; set; } = string.Empty;
    // Compatibilidad: estos campos pueden ser ignorados por los controladores públicos
    public string imagen_especie { get; set; } = string.Empty; // base64
    public string? mime_tipo { get; set; }
}

// List item sin base64; usa URL para obtener imagen binaria
public class EspecieListDto
{
    public int id_especie { get; set; }
    public string codigo_interno_especie { get; set; } = string.Empty;
    public string nombre_comun_especie { get; set; } = string.Empty;
    public string nombre_cientifico_especie { get; set; } = string.Empty;
    public string? familia_especie { get; set; }
    public string descripcion_especie { get; set; } = string.Empty;
    public string uso_especie { get; set; } = string.Empty;
    public string origen_especie { get; set; } = string.Empty;
    public string fenologia_especie { get; set; } = string.Empty;
    public string distribucion_colombia_especie { get; set; } = string.Empty;
    public string distribucion_caqueta_especie { get; set; } = string.Empty;
    public string distribucion_mundial_especie { get; set; } = string.Empty;
    public string? observacion_especie { get; set; }
    public bool muestras_secas_herbario_especie { get; set; }
    public string estado { get; set; } = string.Empty;
    public string imagen_url { get; set; } = string.Empty;
    public string? imagen_url2 { get; set; }
    // Nuevo: detalles de conservación y ubicaciones
    public EstadoConservacionDto? estado_conservacion { get; set; }
    public List<EspecieUbicacionDto> ubicaciones { get; set; } = new();
}

public class EstadoConservacionDto
{
    public int id_estado { get; set; }
    public string codigo_iucn { get; set; } = string.Empty;
    public string nombre_estado { get; set; } = string.Empty;
}

public class EspecieUbicacionDto
{
    public int id_ubicacion { get; set; }
    public string nombre_ubicacion { get; set; } = string.Empty;
}

// Elemento resumido para listados con paginación (sin base64)
public class EspecieResumenListItem
{
    public int id_especie { get; set; }
    public string nombre_cientifico_especie { get; set; } = string.Empty;
    public string nombre_comun_especie { get; set; } = string.Empty;
}

// Resultado paginado genérico
public class PagedResult<T>
{
    public int page { get; set; }
    public int pageSize { get; set; }
    public int total { get; set; }
    public int totalPages { get; set; }
    public List<T> items { get; set; } = new();
}










