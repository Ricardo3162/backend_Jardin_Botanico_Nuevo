using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class ubicacion
{
    public int id_ubicacion { get; set; }

    public string nombre_ubicacion { get; set; } = null!;

    public string? descripcion_ubicacion { get; set; }

    public string estado { get; set; } = null!;

    public virtual ICollection<especie_ubicacion> especie_ubicacions { get; set; } = new List<especie_ubicacion>();
}
