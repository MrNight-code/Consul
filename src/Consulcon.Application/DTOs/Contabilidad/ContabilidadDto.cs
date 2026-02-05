namespace Consulcon.Application.DTOs.Contabilidad;

public class PlanCuentaDto
{
    public int Id { get; set; }
    public string CodigoCuenta { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public int? IdCuentaPadre { get; set; }
    public int? NivelJerarquia { get; set; }
    public bool? EsImputable { get; set; }
}

public class AsientoDto
{
    public int Id { get; set; }
    public int IdCondominio { get; set; }
    public DateTime FechaContable { get; set; }
    public string? GlosaGeneral { get; set; }
    public string? TipoAsiento { get; set; } // Diario, Ajuste, Cierre
    public string? NroDocumentoRespaldo { get; set; }
    public List<AsientoDetalleDto> Detalles { get; set; } = new();
}

public class AsientoDetalleDto
{
    public int Id { get; set; }
    public int IdCuenta { get; set; }
    public string? CuentaNombre { get; set; }
    public string? GlosaLinea { get; set; }
    public decimal? Debe { get; set; }
    public decimal? Haber { get; set; }
}

public class CreateAsientoDto
{
    public int IdCondominio { get; set; }
    public DateTime FechaContable { get; set; }
    public string? GlosaGeneral { get; set; }
    public string? TipoAsiento { get; set; }
    public string? NroDocumentoRespaldo { get; set; }
    public List<CreateAsientoDetalleDto> Detalles { get; set; } = new();
}

public class CreateAsientoDetalleDto
{
    public int IdCuenta { get; set; }
    public string? GlosaLinea { get; set; }
    public decimal Debe { get; set; }
    public decimal Haber { get; set; }
}

public class AutorizacionGastoDto
{
    public int IdAutorizacion { get; set; }
    public string Descripcion { get; set; } = null!;
    public bool? Activo { get; set; }
}

