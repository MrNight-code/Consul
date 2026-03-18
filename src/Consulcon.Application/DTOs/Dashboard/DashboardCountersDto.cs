namespace Consulcon.Application.DTOs.Dashboard;

/// <summary>
/// DTO que contiene los contadores agregados del Dashboard principal para un condominio.
/// </summary>
public class DashboardCountersDto
{
    public int TotalUnidades { get; set; }
    public int UnidadesEnMora { get; set; }

    public decimal TotalCobradoMesActual { get; set; }
    
    public decimal TotalEgresosMesActual { get; set; }
    
    public decimal CashFlowMesActual { get; set; }

    public decimal PorcentajeCobranza { get; set; }

    public decimal TotalMoraHistorica { get; set; }

    public int TotalPersonas { get; set; }
    public int TotalContratos { get; set; }
    public int TotalEgresos { get; set; }
    public int TotalEventos { get; set; }

    public string CondominioNombre { get; set; } = string.Empty;

    public DateTime UltimaActualizacion { get; set; }
}
