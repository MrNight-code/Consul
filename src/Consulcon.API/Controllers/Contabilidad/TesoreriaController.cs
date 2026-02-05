using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;

namespace Consulcon.API.Controllers.Contabilidad;

[ApiController]
[Route("api/[controller]")]
public class TesoreriaController : ControllerBase
{
    private readonly ITesoreriaService _service;

    public TesoreriaController(ITesoreriaService service)
    {
        _service = service;
    }

    [HttpGet("bancos")]
    public async Task<IActionResult> GetBancos()
    {
        var result = await _service.GetBancosAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("bancos")]
    public async Task<IActionResult> CreateBanco([FromBody] BancoDto dto)
    {
        var result = await _service.CreateBancoAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("formaspago")]
    public async Task<IActionResult> GetFormasPago()
    {
        var result = await _service.GetFormasPagoAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("formaspago")]
    public async Task<IActionResult> CreateFormaPago([FromBody] FormaPagoDto dto)
    {
        var result = await _service.CreateFormaPagoAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("egresos/condominio/{condominioId}")]
    public async Task<IActionResult> GetEgresos(int condominioId)
    {
        var result = await _service.GetEgresosByCondominioAsync(condominioId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("egresos")]
    public async Task<IActionResult> RegistrarEgreso([FromBody] CreateEgresoDto dto)
    {
        var result = await _service.RegistrarEgresoAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
