namespace Consulcon.Application.DTOs.Inmuebles;

public class CondominioUserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string RolInicial { get; set; } = "Usuario";
}
