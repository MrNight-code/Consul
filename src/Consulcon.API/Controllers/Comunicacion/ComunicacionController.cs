using Consulcon.Application.DTOs.Comunicacion;
using Consulcon.Application.Interfaces.Comunicacion;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Comunicacion;

public class ComunicacionController : BaseController
{
    private readonly IComunicacionService _service;

    public ComunicacionController(IComunicacionService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetComunicados() 
        => HandleResult(await _service.GetComunicadosByCondominioAsync(CondominioId));

    [HttpPost]
    public async Task<IActionResult> CreateComunicado([FromBody] CreateComunicadoDto dto) 
        => HandleResult(await _service.CreateComunicadoAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) 
        => HandleResult(await _service.DeleteComunicadoAsync(id));
}