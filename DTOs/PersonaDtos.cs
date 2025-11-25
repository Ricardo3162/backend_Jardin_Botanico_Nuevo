using System.ComponentModel.DataAnnotations;

namespace Backend_Jardin.DTOs;

public class PersonaResumenDto
{
    public int id_persona { get; set; }
    public string nombres_persona { get; set; } = string.Empty;
    public string apellidos_persona { get; set; } = string.Empty;
    public string nombre_completo { get; set; } = string.Empty;
    public string? correo_persona { get; set; }
    public string? telefono_persona { get; set; }
    public int fk_rol { get; set; }
    public string nombre_rol { get; set; } = string.Empty;
    public string estado { get; set; } = string.Empty;
}

public class PersonaActualizarRolDto
{
    [Required]
    public int fk_rol { get; set; }
}

public class RolDto
{
    public int id_rol { get; set; }
    public string nombre_rol { get; set; } = string.Empty;
}

public class PersonaPerfilDto
{
    public int id_persona { get; set; }
    public string nombres_persona { get; set; } = string.Empty;
    public string apellidos_persona { get; set; } = string.Empty;
    public string? correo_persona { get; set; }
    public string? telefono_persona { get; set; }
}

public class PersonaActualizarPerfilDto
{
    [Required]
    public string nombres_persona { get; set; } = string.Empty;

    [Required]
    public string apellidos_persona { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string correo_persona { get; set; } = string.Empty;

    public string? telefono_persona { get; set; }

    [MinLength(6)]
    public string? contrasena { get; set; }
}
