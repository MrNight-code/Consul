using System.Security.Claims;
using Consulcon.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected int CondominioId => int.TryParse(Request.Headers["X-Condominio-Id"], out var id) ? id : 0;
    protected int UserId
    {
        get
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out int id) ? id : 0;
        }
    }
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result == null) return NotFound();
        
        if (result.IsSuccess)
        {
            if (result.Value == null) return NoContent();
            return Ok(result.Value);
        }

        return result.Error switch
        {
            var e when e.Contains("no encontrado") || e.Contains("no existe") => NotFound(result.Error),
            var e when e.Contains("validación") || e.Contains("inválido") => BadRequest(result.Error),
            _ => BadRequest(result.Error)
        };
    }
}