namespace Consulcon.Application.DTOs.Seguridad;

public class CreateUserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
}
