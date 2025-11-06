namespace Backend_Jardin.DTOs;

public class LoginRequest
{
    public string? Usuario { get; set; }
    public string? Correo { get; set; }
    public string Contrasena { get; set; } = string.Empty;
}

