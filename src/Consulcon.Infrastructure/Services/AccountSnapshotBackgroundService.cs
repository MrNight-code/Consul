using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consulcon.Infrastructure.Services;

public class AccountSnapshotBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AccountSnapshotBackgroundService> _logger;

    public AccountSnapshotBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<AccountSnapshotBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Account Snapshot Service iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                var nextRunTime = now.Date.AddDays(1); 
                var delay = nextRunTime - now;

                _logger.LogInformation("Próximo snapshot programado para: {Time}", nextRunTime);
                
                await Task.Delay(delay, stoppingToken);

                await CreateSnapshotsAsync();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la ejecución del Snapshot Job.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private async Task CreateSnapshotsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ConsulconDbContext>();

        _logger.LogInformation("Generando snapshots diarios de cuentas...");

        var accounts = await context.Bancos.AsNoTracking().ToListAsync();

        var snapshots = accounts.Select(acc => new AccountDailyBalance
        {
            IdBanco = acc.IdBanco,
            Balance = acc.Saldo,
            Date = DateTime.Now.Date
        }).ToList();

        if (snapshots.Any())
        {
            await context.AddRangeAsync(snapshots);
            await context.SaveChangesAsync();
            _logger.LogInformation("{Count} snapshots guardados correctamente.", snapshots.Count);
        }
    }
}