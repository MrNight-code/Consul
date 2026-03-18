using Consulcon.Application.DTOs.Contratos;
using Consulcon.Application.Interfaces.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contratos;

public class ContratoController(IContratoService service) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() 
        => HandleResult(await service.GetAllAsync());

    [HttpGet("propiedad/{propiedadId}")]
    public async Task<IActionResult> GetByPropiedad(int propiedadId) 
        => HandleResult(await service.GetByPropiedadAsync(propiedadId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) 
        => HandleResult(await service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContratoDto dto) 
        => HandleResult(await service.CreateAsync(dto));

    [HttpPost("{id}/participante")]
    public async Task<IActionResult> AddParticipante(int id, [FromBody] CreateContratoParticipanteDto dto) 
        => HandleResult(await service.AddParticipanteAsync(id, dto));

    [HttpPut("{id}/finalizar")]
    public async Task<IActionResult> Terminate(int id, [FromBody] TerminateRequest request) 
        => HandleResult(await service.TerminateAsync(id, request.Motivo, request.FechaFin));
}

public record TerminateRequest(string Motivo, DateOnly FechaFin);