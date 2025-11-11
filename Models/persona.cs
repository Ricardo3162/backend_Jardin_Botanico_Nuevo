using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class persona
{
    public int id_persona { get; set; }

    public string nombres_persona { get; set; } = null!;

    public string apellidos_persona { get; set; } = null!;

    public string? correo_persona { get; set; }

    public string? telefono_persona { get; set; }

    public string contrasena_persona { get; set; } = null!;

    public int fk_rol { get; set; }

    public string estado { get; set; } = null!;

    public virtual rol fk_rolNavigation { get; set; } = null!;
}
