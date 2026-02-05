using System;

namespace Consulcon.Application.DTOs.Facturacion;

public class ReceiptDto
{
    public int IdPago { get; set; }
    public string? ReciboUrl { get; set; }
    public DateTime? FechaRecibo { get; set; }
    public decimal MontoAbonado { get; set; }
    public string? NombrePersona { get; set; }
}
