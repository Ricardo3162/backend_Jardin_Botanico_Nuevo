using System;
using System.Collections.Generic;

namespace Backend_Jardin.Models;

public partial class especie
{
    public int id_especie { get; set; }

    public string nombre_comun_especie { get; set; } = null!;

    public string nombre_cientifico_especie { get; set; } = null!;

    public string? descripcion_especie { get; set; }

    public byte[]? imagen_especie { get; set; }

    public string? uso_especie { get; set; }

    public string? origen_especie { get; set; }

    public string? fenologia_especie { get; set; }

    public string? distribucion_colombia_especie { get; set; }

    public string? distribucion_caqueta_especie { get; set; }

    public string? distribucion_mundial_especie { get; set; }

    public bool muestras_secas_herbario_especie { get; set; }

    public string estado { get; set; } = null!;

    public virtual ICollection<ejemplar> ejemplars { get; set; } = new List<ejemplar>();
}
