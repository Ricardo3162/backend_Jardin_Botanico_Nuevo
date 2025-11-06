using System.ComponentModel.DataAnnotations;

namespace Backend_Jardin.DTOs;

public class EjemplarCreateDto
{
    [Required]
    public string codigo_interno_ejemplar { get; set; } = string.Empty;
    // Coordenadas como texto "lat,lon" (opcional)
    public string? coordenadas_ejemplar { get; set; }

    public string? imagen1_ejemplar { get; set; }
    public string? imagen2_ejemplar { get; set; }

    [Required]
    public int fk_especie { get; set; }
    [Required]
    public int fk_ubicacion { get; set; }
    public string? detalle_ubicacion { get; set; }
}

public class EjemplarUpdateDto
{
    [Required]
    public string codigo_interno_ejemplar { get; set; } = string.Empty;
    // Coordenadas como texto "lat,lon" (opcional)
    public string? coordenadas_ejemplar { get; set; }

    public string? imagen1_ejemplar { get; set; }
    public string? imagen2_ejemplar { get; set; }

    [Required]
    public int fk_especie { get; set; }
    [Required]
    public int fk_ubicacion { get; set; }
    public string? detalle_ubicacion { get; set; }
}

public class EjemplarResponseDto
{
    public int id_ejemplar { get; set; }
    public string codigo_interno_ejemplar { get; set; } = string.Empty;
    public string? coordenadas_ejemplar { get; set; }
    public string? imagen1_ejemplar { get; set; }
    public string? imagen2_ejemplar { get; set; }
    public DateTime fecha_registro { get; set; }
    public int fk_especie { get; set; }
    public int fk_ubicacion { get; set; }
    public string? detalle_ubicacion { get; set; }
    public string estado { get; set; } = string.Empty;
    public EspecieResponseDto especie { get; set; } = new();
}

// Respuesta para búsqueda por código interno (sin estados)
public class EspecieDeEjemplarDto
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
}

public class EjemplarCodigoDto
{
    public int id_ejemplar { get; set; }
    public string codigo_interno_ejemplar { get; set; } = string.Empty;
    public string? coordenadas_ejemplar { get; set; }
    public DateTime fecha_registro { get; set; }
    public int fk_especie { get; set; }
    public int fk_ubicacion { get; set; }
    public string nombre_ubicacion { get; set; } = string.Empty;
    public string? imagen1_url { get; set; }
    public string? especie_imagen_url { get; set; }
    public EspecieDeEjemplarDto especie { get; set; } = new();
}
