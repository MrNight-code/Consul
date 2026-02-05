using Consulcon.Application.DTOs;
using Consulcon.Application.Interfaces;
using Consulcon.Domain.Common;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Consulcon.API.Controllers
{
    public class CobranzasController(ICobranzaService cobranzaService) : ControllerBase
    {
        private readonly ICobranzaService _cobranzaService = cobranzaService;


        [HttpPost]
        public async Task<IActionResult> RegistrarCobranza([FromBody] CobranzaRequest request)
        {
            var result = await _cobranzaService.RegistrarCobranzaAsync(request);

            if (result.IsFailure)
            {
                // Map Result failure to ErrorResponse manually or assume Middleware handles Exception
                // Here we return explicit Bad Request for business logic failure
                return BadRequest(new { IsSuccess = false, result.Error });
            }

            return Ok(new { IsSuccess = true, Message = "Cobranza registrada exitosamente." });
        }

        [HttpGet("{unitId}")]
        public async Task<IActionResult> ObtenerHistorial(int unitId)
        {
            var result = await _cobranzaService.ObtenerHistorialAsync(unitId);
            return Ok(new { IsSuccess = true, Data = result.Value }); // Usually Result.Value
        }
    }
}
