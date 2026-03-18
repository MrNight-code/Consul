using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;

namespace Consulcon.API.Controllers.Inmuebles;

public class CondominioController(ICondominioService service) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() 
        => HandleResult(await service.GetAllAsync(UserId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) 
        => HandleResult(await service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> Create([FromBody] CondominioDto dto) 
        => HandleResult(await service.CreateAsync(dto, UserId));

    [HttpPut("{id}")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] CondominioDto dto) 
        => HandleResult(await service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> Delete(int id) 
        => HandleResult(await service.DeleteAsync(id));

    [HttpPost("{id}/usuarios")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> AddUser(int id, [FromBody] AddUserToCondominioDto dto) 
        => HandleResult(await service.AddUserAsync(id, dto));

    [HttpGet("{id}/usuarios")]
    public async Task<IActionResult> GetUsers(int id) 
        => HandleResult(await service.GetUsersAsync(id));

    [HttpDelete("{id}/usuarios/{usuarioId}")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> RemoveUser(int id, int usuarioId) 
        => HandleResult(await service.RemoveUserAsync(id, usuarioId));
}
