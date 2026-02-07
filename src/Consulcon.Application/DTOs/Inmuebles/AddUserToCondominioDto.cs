using System.ComponentModel.DataAnnotations;

namespace Consulcon.Application.DTOs.Inmuebles;

public class AddUserToCondominioDto
{
    [Required]
    public int UserId { get; set; }
}
