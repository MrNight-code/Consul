using Consulcon.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Admin;

[Authorize] 
public class MaintenanceController : BaseController
{
    private readonly ITenantMigrationService _migrationService;
    private readonly ILogger<MaintenanceController> _logger;

    public MaintenanceController(ITenantMigrationService migrationService, ILogger<MaintenanceController> logger)
    {
        _migrationService = migrationService;
        _logger = logger;
    }

    [HttpPost("migrate/{tenantId}")]
    public async Task<IActionResult> MigrateTenantDatabase(string tenantId)
    {
        var dbName = $"db_condominio_{tenantId}";
        _logger.LogInformation("Manual migration requested for {DbName}", dbName);

        try 
        {
            await _migrationService.MigrateTenantDatabaseAsync(dbName);
            return Ok(new { Message = $"Migration applied successfully to {dbName}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration failed for {TenantId}", tenantId);
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}