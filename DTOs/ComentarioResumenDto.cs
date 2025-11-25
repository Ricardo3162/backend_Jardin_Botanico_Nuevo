namespace Backend_Jardin.DTOs;

public class ComentarioResumenDto
{
    public int id_comentario { get; set; }
    public int id_especie { get; set; }
    public string nombre_comun_especie { get; set; } = string.Empty;
    public string nombre_cientifico_especie { get; set; } = string.Empty;
    public DateTime fecha_comentario { get; set; }
    public string comentario { get; set; } = string.Empty;
}
