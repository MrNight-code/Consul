namespace Consulcon.Application.DTOs.Reservas;

public class RecursoComunDto
{
    public int Id { get; set; }
    public int IdCondominio { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal? CostoReserva { get; set; }
    public decimal? CostoGarantia { get; set; }
    public string? ColorCalendario { get; set; }
}

public class ReservaDto
{
    public int Id { get; set; }
    public int IdRecurso { get; set; }
    public string? RecursoNombre { get; set; }
    public int IdContrato { get; set; }
    public string? ContratoInfo { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? CantidadInvitados { get; set; }
    public string? Motivo { get; set; }
    public string? AmenizadoPor { get; set; }
    public decimal? MontoTotalCobrado { get; set; }
    public string? Estado { get; set; }
}

public class CreateReservaDto
{
    public int IdRecurso { get; set; }
    public int IdContrato { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? CantidadInvitados { get; set; }
    public string? Motivo { get; set; }
    public string? AmenizadoPor { get; set; }
}
