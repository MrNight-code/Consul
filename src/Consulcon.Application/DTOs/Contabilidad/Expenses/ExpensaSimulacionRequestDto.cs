namespace Consulcon.Application.DTOs.Contabilidad.Expenses
{
    public class ExpensaSimulacionRequestDto
    {
        public decimal MontoTotal { get; set; }
        public int? FkCondominio { get; set; }
        public int? Mes { get; set; }
        public int? Ano { get; set; }
        public bool EsMontoFijoPorUnidad { get; set; }
    }
}
