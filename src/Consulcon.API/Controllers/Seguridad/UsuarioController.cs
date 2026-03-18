using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Seguridad;

public class UsuarioController(IUsuarioService service) : BaseController
{
    [HttpGet]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> GetAll() 
        => HandleResult(await service.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> GetById(int id) 
        => HandleResult(await service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto) 
        => HandleResult(await service.CreateAsync(dto));

    [HttpPut("{id}")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        => HandleResult(await service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> Delete(int id) 
        => HandleResult(await service.DeleteAsync(id));
}
