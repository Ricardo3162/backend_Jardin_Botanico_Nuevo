using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class especie
{
    public int id_especie { get; set; }

    public string codigo_interno_especie { get; set; } = null!;

    public string nombre_comun_especie { get; set; } = null!;

    public string nombre_cientifico_especie { get; set; } = null!;

    public string? familia_especie { get; set; }

    public string? descripcion_especie { get; set; }

    public byte[]? imagen_especie { get; set; }

    public byte[]? imagen_especie2 { get; set; }

    public string? uso_especie { get; set; }

    public string? origen_especie { get; set; }

    public string? fenologia_especie { get; set; }

    public string? distribucion_colombia_especie { get; set; }

    public string? distribucion_caqueta_especie { get; set; }

    public string? distribucion_mundial_especie { get; set; }

    public bool muestras_secas_herbario_especie { get; set; }

    public string? observacion_especie { get; set; }

    public int? fk_estado_conservacion { get; set; }

    public DateTime fecha_creacion { get; set; }

    public DateTime fecha_actualizacion { get; set; }

    public string estado { get; set; } = null!;

    public virtual ICollection<comentario> comentarios { get; set; } = new List<comentario>();

    public virtual ICollection<especie_ubicacion> especie_ubicacions { get; set; } = new List<especie_ubicacion>();

    public virtual estadoconservacion? fk_estado_conservacionNavigation { get; set; }
}
