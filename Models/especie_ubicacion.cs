using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class especie_ubicacion
{
    public int fk_especie { get; set; }

    public int fk_ubicacion { get; set; }

    public string estado { get; set; } = null!;

    public virtual especie fk_especieNavigation { get; set; } = null!;

    public virtual ubicacion fk_ubicacionNavigation { get; set; } = null!;
}
