using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

public class ProveedorController(IProveedorService service) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() 
        => HandleResult(await service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) 
        => HandleResult(await service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProveedorDto dto) 
        => HandleResult(await service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProveedorDto dto) 
        => HandleResult(await service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) 
        => HandleResult(await service.DeleteAsync(id));
}