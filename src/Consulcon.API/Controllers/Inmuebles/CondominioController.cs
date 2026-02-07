using System.Security.Claims;

namespace Consulcon.API.Controllers.Inmuebles;

[ApiController]
[Route("api/[controller]")]
public class CondominioController(ICondominioService service) : ControllerBase
{
    private readonly ICondominioService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _service.GetAllAsync(userId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { Message = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CondominioDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _service.CreateAsync(dto, userId);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value.IdCondominio }, result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CondominioDto dto)
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

    [HttpPost("{id}/usuarios")]
    public async Task<IActionResult> AddUser(int id, [FromBody] AddUserToCondominioDto dto)
    {
        var result = await _service.AddUserAsync(id, dto);
        return result.IsSuccess ? Ok(new { Message = "Usuario asignado correctamente" }) : BadRequest(result.Error);
    }

    [HttpGet("{id}/usuarios")]
    public async Task<IActionResult> GetUsers(int id)
    {
        var result = await _service.GetUsersAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}/usuarios/{userId}")]
    public async Task<IActionResult> RemoveUser(int id, int userId)
    {
        var result = await _service.RemoveUserAsync(id, userId);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
