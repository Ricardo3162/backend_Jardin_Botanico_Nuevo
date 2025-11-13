namespace Backend_Jardin.DTOs;

public class EspecieDetalleDto
{
    public int id_especie { get; set; }
    public string codigo_interno_especie { get; set; } = string.Empty;
    public string nombre_comun_especie { get; set; } = string.Empty;
    public string nombre_cientifico_especie { get; set; } = string.Empty;
    public string? familia_especie { get; set; }
    public string descripcion_especie { get; set; } = string.Empty;
    public string? observacion_especie { get; set; }
    public string origen_especie { get; set; } = string.Empty;
    public string distribucion_mundial_especie { get; set; } = string.Empty;
    public string distribucion_colombia_especie { get; set; } = string.Empty;
    public string distribucion_caqueta_especie { get; set; } = string.Empty;
    public string fenologia_especie { get; set; } = string.Empty;
    public bool muestras_secas_herbario_especie { get; set; }
    public int fk_uso { get; set; }
    public int fk_estado_conservacion { get; set; }
    public string uso_nombre { get; set; } = string.Empty;
    public string estado_conservacion_codigo { get; set; } = string.Empty;
    public string estado_conservacion_nombre { get; set; } = string.Empty;
    public List<EspecieUbicacionDto> ubicaciones { get; set; } = new();
}

public class EspecieUbicacionDto
{
    public int id_ubicacion { get; set; }
    public string nombre_ubicacion { get; set; } = string.Empty;
    public int fk_ubicacion { get; set; }
}
