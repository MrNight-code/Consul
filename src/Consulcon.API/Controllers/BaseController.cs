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

        return MapErrorResponse(result.Error);
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result == null) return NotFound();
        if (result.IsSuccess) return NoContent();

        return MapErrorResponse(result.Error);
    }

    private ActionResult MapErrorResponse(string error)
    {
        return error switch
        {
            var e when e.Contains("no encontrado") || e.Contains("no existe") 
                => NotFound(new { message = error }),
                
            var e when e.Contains("validación") || e.Contains("inválido") || e.Contains("requerido") 
                => BadRequest(new { message = error }),
                
            var e when e.Contains("permisos") || e.Contains("autorizado") 
                => Forbid(),

            _ => BadRequest(new { message = error })
        };
    }
}