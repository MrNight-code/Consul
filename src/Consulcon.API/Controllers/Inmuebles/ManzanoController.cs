using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;
using Microsoft.AspNetCore.Mvc;
using Consulcon.Domain.Common;

namespace Consulcon.API.Controllers.Inmuebles;

[ApiController]
[Route("api/[controller]")]
public class ManzanoController(IManzanoService service) : ControllerBase
{
    private readonly IManzanoService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { Message = result.Error });
    }

    [HttpGet("condominio/{condominioId}")]
    public async Task<IActionResult> GetByCondominio(int condominioId)
    {
        var result = await _service.GetByCondominioAsync(condominioId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ManzanoDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value.IdManzano }, result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ManzanoDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
