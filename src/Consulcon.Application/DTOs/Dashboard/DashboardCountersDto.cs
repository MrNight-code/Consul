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

    public string CondominioNombre { get; set; } = string.Empty;

    public DateTime UltimaActualizacion { get; set; }
}
