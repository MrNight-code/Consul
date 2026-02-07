namespace Consulcon.Application.DTOs.Contabilidad;

public class ProveedorDto
{
    public int IdProveedor { get; set; }
    public string RazonSocial { get; set; } = null!;
    public string? Nit { get; set; }
    public string? Contacto { get; set; }
    public string? Direccion { get; set; }
    public bool? Activo { get; set; }
}
