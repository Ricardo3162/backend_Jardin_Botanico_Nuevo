using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class comentario
{
    public int id_comentario { get; set; }

    public int fk_ejemplar { get; set; }

    public string? contenido_comentario { get; set; }

    public DateTime fecha_comentario { get; set; }

    public string estado { get; set; } = null!;

    public virtual ejemplar fk_ejemplarNavigation { get; set; } = null!;
}
