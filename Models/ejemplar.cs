using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class ejemplar
{
    public int id_ejemplar { get; set; }

    public string codigo_interno_ejemplar { get; set; } = null!;

    public string? coordenadas_ejemplar { get; set; }

    public byte[]? imagen1_ejemplar { get; set; }

    public byte[]? imagen2_ejemplar { get; set; }

    public DateTime fecha_registro { get; set; }

    public int fk_especie { get; set; }

    public int fk_ubicacion { get; set; }

    public string? detalle_ubicacion { get; set; }

    public string estado { get; set; } = null!;

    public virtual ICollection<comentario> comentarios { get; set; } = new List<comentario>();

    public virtual especie fk_especieNavigation { get; set; } = null!;

    public virtual ubicacion fk_ubicacionNavigation { get; set; } = null!;
}
