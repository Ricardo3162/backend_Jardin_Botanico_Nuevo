using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class uso
{
    public int id_uso { get; set; }

    public string nombre_uso { get; set; } = null!;

    public string estado { get; set; } = null!;

    public virtual ICollection<especie> especies { get; set; } = new List<especie>();
}
