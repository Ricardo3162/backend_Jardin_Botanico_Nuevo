using System.ComponentModel.DataAnnotations;

namespace Backend_Jardin.DTOs;

public class EspecieCreateDto
{
    [Required]
    public string nombre_comun_especie { get; set; } = string.Empty;

    [Required]
    public string nombre_cientifico_especie { get; set; } = string.Empty;

    [Required]
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
    public string nombre_comun_especie { get; set; } = string.Empty;
    public string nombre_cientifico_especie { get; set; } = string.Empty;
    public string descripcion_especie { get; set; } = string.Empty;
    public string imagen_especie { get; set; } = string.Empty; // base64
    public string? mime_tipo { get; set; }
    public string uso_especie { get; set; } = string.Empty;
    public string origen_especie { get; set; } = string.Empty;
    public string fenologia_especie { get; set; } = string.Empty;
    public string distribucion_colombia_especie { get; set; } = string.Empty;
    public string distribucion_caqueta_especie { get; set; } = string.Empty;
    public string distribucion_mundial_especie { get; set; } = string.Empty;
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
    public string nombre_comun_especie { get; set; } = string.Empty;
    public string nombre_cientifico_especie { get; set; } = string.Empty;
    public string descripcion_especie { get; set; } = string.Empty;
    public string uso_especie { get; set; } = string.Empty;
    public string origen_especie { get; set; } = string.Empty;
    public string fenologia_especie { get; set; } = string.Empty;
    public string distribucion_colombia_especie { get; set; } = string.Empty;
    public string distribucion_caqueta_especie { get; set; } = string.Empty;
    public string distribucion_mundial_especie { get; set; } = string.Empty;
    public bool muestras_secas_herbario_especie { get; set; }
    public string estado { get; set; } = string.Empty;
    public string imagen_url { get; set; } = string.Empty;
}
