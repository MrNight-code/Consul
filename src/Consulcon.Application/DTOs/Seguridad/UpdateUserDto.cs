namespace Consulcon.Application.DTOs.Seguridad;

public class UpdateUserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public int? IdRolPrincipal { get; set; }
    public List<int>? CondominioIds { get; set; }
    
    // TODO: Implementar lógica de contraseña temporal
    public string? PasswordTemporal { get; set; } 
}
