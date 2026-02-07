using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;
using Microsoft.AspNetCore.Mvc;
using Consulcon.Domain.Common;

namespace Consulcon.API.Controllers.Contabilidad;

[ApiController]
[Route("api/[controller]")]
public class ProveedorController(IProveedorService service) : ControllerBase
{
    private readonly IProveedorService _service = service;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProveedorDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value.IdProveedor }, result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProveedorDto dto)
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
