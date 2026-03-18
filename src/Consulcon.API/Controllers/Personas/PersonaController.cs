using Consulcon.Application.DTOs.Personas;
using Consulcon.Application.Interfaces.Personas;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Personas;

public class PersonaController : BaseController
{
    private readonly IPersonaService _service;

    public PersonaController(IPersonaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => HandleResult(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => HandleResult(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PersonaDto dto) => HandleResult(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PersonaDto dto) => HandleResult(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => HandleResult(await _service.DeleteAsync(id));
}