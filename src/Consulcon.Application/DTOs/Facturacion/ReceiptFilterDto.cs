using System;

namespace Consulcon.Application.DTOs.Facturacion;

public class ReceiptFilterDto
{
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public int? PersonaId { get; set; }
}
