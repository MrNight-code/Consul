using System;
using System.Collections.Generic;

namespace Consulcon.Application.DTOs.Expensas;

public class EstadoCuentaUnidadResponseDto
{
    public int FkPropiedad { get; set; }
    public string CodigoUnidad { get; set; } = string.Empty;
    public string NombreUnidad { get; set; } = string.Empty;
    public string Propietario { get; set; } = string.Empty;
    
    // Saldos consolidados
    public decimal SaldoVencido { get; set; }
    public decimal SaldoVigente { get; set; }
    public decimal SaldoTotal { get; set; }
    public decimal SaldoAFavor { get; set; }

    // Detalle de deudas
    public List<ConceptoDeudaResponseDto> Conceptos { get; set; } = new();
}

public class ConceptoDeudaResponseDto
{
    public int PkDeuda { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public int? Mes { get; set; }
    public int? Ano { get; set; }
    public decimal Monto { get; set; }
    public DateOnly? FechaVencimiento { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string TipoConcepto { get; set; } = string.Empty;
}
