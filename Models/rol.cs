using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class rol
{
    public int id_rol { get; set; }

    public string nombre_rol { get; set; } = null!;

    public string? descripcion_rol { get; set; }

    public string estado { get; set; } = null!;

    public virtual ICollection<persona> personas { get; set; } = new List<persona>();
}
