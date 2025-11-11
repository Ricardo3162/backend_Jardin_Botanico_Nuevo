using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class estadoconservacion
{
    public int id_estado { get; set; }

    public string codigo_iucn { get; set; } = null!;

    public string nombre_estado { get; set; } = null!;

    public string? descripcion_estado { get; set; }

    public string estado { get; set; } = null!;

    public virtual ICollection<especie> especies { get; set; } = new List<especie>();
}
