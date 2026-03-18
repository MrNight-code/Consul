using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Common;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Consulcon.API.Controllers;


[Route("api/providers")]
[Authorize]
public class ProvidersController(
    IProveedorService service,
    IValidator<CreateProviderDto> createValidator,
    IValidator<UpdateProviderDto> updateValidator) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProviderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? term = null, CancellationToken cancellationToken = default) 
        => HandleResult(await service.GetPagedAsync(page, pageSize, term, cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProviderDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default) 
        => HandleResult(await service.GetProviderByIdAsync(id, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateProviderDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid) return HandleValidationErrors(validation);

        var result = await service.CreateProviderAsync(dto, cancellationToken);

        if (!result.IsSuccess && result.Error.Contains("ya existe"))
            return Conflict(new { message = result.Error });

        return HandleResult(result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProviderDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid) return HandleValidationErrors(validation);

        return HandleResult(await service.UpdateProviderAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default) 
        => HandleResult(await service.DeleteProviderAsync(id, cancellationToken));

    private IActionResult HandleValidationErrors(FluentValidation.Results.ValidationResult validationResult)
    {
        var duplicateError = validationResult.Errors.FirstOrDefault(e => e.ErrorMessage.Contains("ya existe"));
        
        if (duplicateError != null)
            return Conflict(new { message = duplicateError.ErrorMessage });

        return BadRequest(new
        {
            message = "Errores de validación",
            errors = validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
        });
    }
    /// Exporta lista de proveedores a Excel
    [HttpGet("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(
        [FromQuery] string? term = null,
        [FromServices] IExcelService excelService = null!,
        CancellationToken cancellationToken = default)
    {
        // Omitir paginación para traer todo el query filtrado
        var result = await service.GetPagedAsync(1, int.MaxValue, term, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dataList = result.Value.Items.ToList();

        var fileContent = excelService.GenerateExcel(dataList, "Proveedores");
        
        return File(
            fileContent, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"Proveedores_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        );
    }
}
