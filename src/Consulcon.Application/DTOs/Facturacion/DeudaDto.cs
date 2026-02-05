namespace Consulcon.Application.DTOs.Facturacion;

public class DeudaDto
{
    public int Id { get; set; }
    public int IdContrato { get; set; }
    public string? ContratoInfo { get; set; } // e.g. "Depto 101 - Juan Perez"
    public int AnioPeriodo { get; set; }
    public int MesPeriodo { get; set; }
    public DateOnly? FechaEmision { get; set; }
    public DateOnly? FechaVencimiento { get; set; }
    public decimal? TotalDeuda { get; set; }
    public decimal? TotalPagado { get; set; }
    public string? EstadoPago { get; set; } // PENDIENTE, PAGADO, ETC
    public List<DeudaDetalleDto> Detalles { get; set; } = new();
}

public class DeudaDetalleDto
{
    public int Id { get; set; }
    public int IdServicio { get; set; }
    public string? ServicioNombre { get; set; }
    public string Concepto { get; set; } = null!;
    public decimal MontoUnitario { get; set; }
    public decimal? Cantidad { get; set; }
    public decimal Subtotal { get; set; }
}

public class GenerateDeudaDto
{
    public int IdContrato { get; set; }
    public int Anio { get; set; }
    public int Mes { get; set; }
    public DateOnly FechaVencimiento { get; set; }
    public int IdUsuarioGenerador { get; set; }
    // The service logic might auto-calculate expensa from Contrato, 
    // but let's allow manual additions for now or simple "Generate Monthly Expensa"
    public List<CreateDeudaDetalleDto> DetallesAdicionales { get; set; } = new();
}

public class CreateDeudaDetalleDto
{
    public int IdServicio { get; set; }
    public string Concepto { get; set; } = null!;
    public decimal MontoUnitario { get; set; }
    public decimal Cantidad { get; set; } = 1;
}
