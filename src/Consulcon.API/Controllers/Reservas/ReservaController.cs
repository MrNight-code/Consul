using Consulcon.Application.DTOs.Reservas;
using Consulcon.Application.Interfaces.Reservas;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Reservas;

public class ReservaController(IReservaService service) : BaseController
{
    [HttpGet("recursos")]
    public async Task<IActionResult> GetRecursos() 
        => HandleResult(await service.GetRecursosByCondominioAsync(CondominioId));

    [HttpPost("recursos")]
    public async Task<IActionResult> CreateRecurso([FromBody] RecursoComunDto dto) 
        => HandleResult(await service.CreateRecursoAsync(dto));

    [HttpGet]
    public async Task<IActionResult> GetReservas() 
        => HandleResult(await service.GetReservasByCondominioAsync(CondominioId));

    [HttpPost]
    public async Task<IActionResult> CreateReserva([FromBody] CreateReservaDto dto) 
        => HandleResult(await service.CreateReservaAsync(dto));

    [HttpPut("{id}/confirmar")]
    public async Task<IActionResult> Confirmar(int id) 
        => HandleResult(await service.ConfirmarReservaAsync(id));
}