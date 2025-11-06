namespace Backend_Jardin.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expira { get; set; }
    public string Rol { get; set; } = "User";
    public int IdPersona { get; set; }
    public string? Usuario { get; set; }
    public string? Correo { get; set; }
    public string? NombreCompleto { get; set; }
}

