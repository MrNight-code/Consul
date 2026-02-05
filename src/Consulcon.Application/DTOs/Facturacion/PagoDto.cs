namespace Consulcon.Application.DTOs.Facturacion;

public class TransaccionPagoDto
{
    public int Id { get; set; }
    public int IdDeuda { get; set; }
    public int IdPersonaPagador { get; set; }
    public string? PagadorNombre { get; set; }
    public int IdBancoDestino { get; set; }
    public string? BancoNombre { get; set; }
    public int IdFormaPago { get; set; }
    public string? FormaPagoDescripcion { get; set; }
    public DateTime? FechaPago { get; set; }
    public decimal MontoAbonado { get; set; }
    public string? NroComprobanteBanco { get; set; }
    public string? Estado { get; set; }
}

public class CreatePagoDto
{
    public int IdDeuda { get; set; }
    public int IdPersonaPagador { get; set; }
    public int IdBancoDestino { get; set; }
    public int IdFormaPago { get; set; }
    public decimal MontoAbonado { get; set; }
    public string? NroComprobanteBanco { get; set; }
}
