namespace Consulcon.Application.DTOs.Contabilidad.Expenses
{
    public class GenerarDeudaRequestDto
    {
        public decimal MontoTotal { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public int FkCondominio { get; set; }
        public bool AplicarSaldosAFavor { get; set; }
        public bool EsMontoFijoPorUnidad { get; set; }
    }
}
