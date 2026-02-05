namespace Consulcon.Application.DTOs
{
    public class CobranzaRequest
    {
        public int UnitId { get; set; }
        public decimal Monto { get; set; }
        public int IdFormaPago { get; set; }
        public string? NroReferencia { get; set; }
        public string? Observaciones { get; set; }
        public int? IdBancoDestino { get; set; }
    }
}
