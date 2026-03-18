using System;

namespace Consulcon.Application.DTOs.Facturacion;

public class BatchReceiptRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? UnitId { get; set; }
}
