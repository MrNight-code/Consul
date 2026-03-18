using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Inmuebles;

public class ManzanoController(IManzanoService service) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() 
        => HandleResult(await service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) 
        => HandleResult(await service.GetByIdAsync(id));
    [HttpGet("condominio")]
    public async Task<IActionResult> GetByCondominio() 
        => HandleResult(await service.GetByCondominioAsync(CondominioId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateManzanoDto dto) 
        => HandleResult(await service.CreateAsync(dto, CondominioId));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateManzanoDto dto) 
        => HandleResult(await service.UpdateAsync(id, dto, CondominioId));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) 
        => HandleResult(await service.DeleteAsync(id));
}