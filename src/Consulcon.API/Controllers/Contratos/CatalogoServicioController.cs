using Consulcon.Application.DTOs.Contratos;
using Consulcon.Application.Interfaces.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contratos;

public class CatalogoServicioController(ICatalogoServicioService service) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() 
        => HandleResult(await service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CatalogoServicioDto dto) 
        => HandleResult(await service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CatalogoServicioDto dto) 
        => HandleResult(await service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) 
        => HandleResult(await service.DeleteAsync(id));
}