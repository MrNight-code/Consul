using Consulcon.Application.Common;
using Consulcon.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Admin;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Should restrict to Admin roles ideally
public class MaintenanceController : ControllerBase
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
        try
        {
            // Convention: db_condominio_{tenantId}
            // If tenantId is numeric (IdCondominio), use it directly.
            // If tenantId is a string code (e.g. 'bosques'), we might need a lookup or just assume user knows the DB name suffix.
            // Based on previous CondominioService, it used `IdCondominio`. 
            // However, the `X-Tenant-Id` usually maps to a DB name.
            // Let's support the raw DB name construction for flexibility or allow passing the full name.
            
            // Assuming tenantId is the suffix.
            // Usage: POST /api/maintenance/migrate/5  -> db_condominio_5
            
            var dbName = $"db_condominio_{tenantId}";
            
            _logger.LogInformation("Manual migration requested for {DbName}", dbName);
            
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
