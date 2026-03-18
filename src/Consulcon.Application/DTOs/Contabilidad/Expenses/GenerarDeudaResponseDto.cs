namespace Consulcon.Application.DTOs.Contabilidad.Expenses
{
    public class GenerarDeudaResponseDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int? PkReciboGeneral { get; set; }
    }
}
