using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers;

/// API para gestión de proveedores (limpieza, seguridad, servicios públicos)
[ApiController]
[Route("api/providers")]
[Authorize]
public class ProvidersController : ControllerBase
{
    private readonly IProveedorService _service;
    private readonly IValidator<CreateProviderDto> _createValidator;
    private readonly IValidator<UpdateProviderDto> _updateValidator;

    public ProvidersController(
        IProveedorService service,
        IValidator<CreateProviderDto> createValidator,
        IValidator<UpdateProviderDto> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// Obtiene lista paginada de proveedores con búsqueda opcional
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProviderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? term = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetPagedAsync(page, pageSize, term, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// Obtiene un proveedor por su ID
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProviderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetProviderByIdAsync(id, cancellationToken);
        
        if (!result.IsSuccess)
            return NotFound(new { Message = result.Error });
            
        return Ok(result.Value);
    }

    /// Crea un nuevo proveedor
    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProviderDto dto,
        CancellationToken cancellationToken = default)
    {
        // Validación con FluentValidation
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Si el error es por NIT duplicado, retornar 409 Conflict
            var duplicateError = validationResult.Errors
                .FirstOrDefault(e => e.ErrorMessage.Contains("ya existe"));
            
            if (duplicateError != null)
            {
                return Conflict(new { Message = duplicateError.ErrorMessage });
            }

            return BadRequest(new
            {
                Message = "Errores de validación",
                Errors = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                })
            });
        }

        var result = await _service.CreateProviderAsync(dto, cancellationToken);

        if (!result.IsSuccess)
        {
            // Verificar si es error de duplicado (por si pasa la validación pero falla en el servicio)
            if (result.Error.Contains("ya existe"))
                return Conflict(new { Message = result.Error });

            return BadRequest(new { Message = result.Error });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            Result.Ok(result.Value));
    }

    /// Actualiza un proveedor existente
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateProviderDto dto,
        CancellationToken cancellationToken = default)
    {
        // Validación con FluentValidation
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Message = "Errores de validación",
                Errors = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                })
            });
        }

        var result = await _service.UpdateProviderAsync(id, dto, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("no encontrado"))
                return NotFound(new { Message = result.Error });

            return BadRequest(new { Message = result.Error });
        }

        return NoContent();
    }

    /// Elimina (soft delete) un proveedor
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _service.DeleteProviderAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("no encontrado"))
                return NotFound(new { Message = result.Error });

            return BadRequest(new { Message = result.Error });
        }

        return NoContent();
    }
}
