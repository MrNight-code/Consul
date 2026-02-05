namespace Consulcon.Application.DTOs.Contabilidad;

public class BancoDto
{
    public int Id { get; set; }
    public string NombreEntidad { get; set; } = null!;
    public string? NumeroCuenta { get; set; }
    public string? Moneda { get; set; }
    public bool? Activo { get; set; }
}

public class FormaPagoDto
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = null!;
}

public class EgresoDto
{
    public int Id { get; set; }
    public int IdCondominio { get; set; }
    public int? IdProveedor { get; set; }
    public string? ProveedorNombre { get; set; }
    public int? IdPersonaBeneficiario { get; set; }
    public string? BeneficiarioNombre { get; set; }
    public int IdAutorizacion { get; set; }
    public int IdBancoOrigen { get; set; }
    public string? BancoNombre { get; set; }
    public int IdFormaPago { get; set; }
    public string Concepto { get; set; } = null!;
    public decimal MontoTotal { get; set; }
    public DateTime? FechaEgreso { get; set; }
    public string? NroFacturaProveedor { get; set; }
}

public class CreateEgresoDto
{
    public int IdCondominio { get; set; }
    public int? IdProveedor { get; set; }
    public int? IdPersonaBeneficiario { get; set; }
    public int IdAutorizacion { get; set; }
    public int IdBancoOrigen { get; set; }
    public int IdFormaPago { get; set; }
    public required string Concepto { get; set; }
    public decimal MontoTotal { get; set; }
    public string? NroFacturaProveedor { get; set; }
    public int IdUsuarioRegistro { get; set; }
}
