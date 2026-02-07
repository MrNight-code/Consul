using System.ComponentModel.DataAnnotations;

namespace Consulcon.Application.DTOs.Contabilidad;

public class VoidExpenseRequest
{
    [Required(ErrorMessage = "El motivo de la anulación es obligatorio.")]

    [MinLength(10, ErrorMessage = "El motivo debe tener al menos 10 caracteres para evitar malversaciones.")]

    public string Reason { get; set; } = null!;
}