using Consulcon.Application.DTOs.Contabilidad.CashBook;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Common;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Consulcon.Infrastructure.Services.Contabilidad;

/// <summary>
/// Service for generating consolidated Cash Book reports.
/// Combines income (TransaccionPago) and expenses (Egreso) into a unified view.
/// </summary>
public class CashBookService(ConsulconDbContext context) : ICashBookService
{
    private const string TypeIncome = "IN";
    private const string TypeExpense = "OUT";
    private const string StatusVoided = "ANULADO";
    private const string StatusConfirmed = "CONFIRMADO";

    public async Task<Result<CashBookResultDto>> GetCashBookAsync(CashBookQuery query)
    {
        // Validation
        if (query.StartDate > query.EndDate)
            return Result.Fail<CashBookResultDto>("La fecha de inicio no puede ser posterior a la fecha de fin.");

        if (query.PageSize < 1 || query.PageSize > 500)
            return Result.Fail<CashBookResultDto>("El tamaño de página debe estar entre 1 y 500.");

        try
        {
            // 1. Calculate Initial Balance (before StartDate)
            var initialBalance = await CalculateInitialBalanceAsync(query.StartDate, query.FinancialAccountId);

            // 2. Get unified entries within date range
            var allEntries = await GetUnifiedEntriesAsync(query);

            // 3. Filter voided if not included
            var visibleEntries = query.IncludeVoided 
                ? allEntries 
                : [.. allEntries.Where(e => !e.IsVoided)];

            var totalRecords = visibleEntries.Count;

            // 4. Pagination
            var skip = (query.Page - 1) * query.PageSize;
            var pagedEntries = visibleEntries
                .Skip(skip)
                .Take(query.PageSize)
                .ToList();

            // 5. Calculate balance for entries before current page (for running balance)
            var balanceBeforePage = initialBalance;
            for (int i = 0; i < skip && i < visibleEntries.Count; i++)
            {
                if (!visibleEntries[i].IsVoided)
                    balanceBeforePage += visibleEntries[i].Amount;
            }

            // 6. Calculate running balance for current page entries
            var runningBalance = balanceBeforePage;
            foreach (var entry in pagedEntries)
            {
                if (!entry.IsVoided)
                    runningBalance += entry.Amount;
                entry.Balance = runningBalance;
            }

            return Result.Ok(new CashBookResultDto
            {
                InitialBalance = initialBalance,
                FinalBalance = runningBalance,
                Entries = pagedEntries,
                TotalRecords = totalRecords,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }
        catch (Exception ex)
        {
            return Result.Fail<CashBookResultDto>($"Error al generar el libro de caja: {ex.Message}");
        }
    }

    private async Task<decimal> CalculateInitialBalanceAsync(DateTime startDate, int? accountId)
    {
        // Sum of income before StartDate
        var incomeQuery = context.TransaccionPagos
            .Where(t => t.FechaPago < startDate && t.Estado == StatusConfirmed);

        if (accountId.HasValue)
            incomeQuery = incomeQuery.Where(t => t.IdBancoDestino == accountId.Value);

        var totalIncome = await incomeQuery.SumAsync(t => (decimal?)t.MontoAbonado) ?? 0;

        // Sum of expenses before StartDate
        var expenseQuery = context.Egresos
            .Where(e => e.FechaEgreso < startDate);

        if (accountId.HasValue)
            expenseQuery = expenseQuery.Where(e => e.IdBancoOrigen == accountId.Value);

        var totalExpenses = await expenseQuery.SumAsync(e => (decimal?)e.MontoTotal) ?? 0;

        return totalIncome - totalExpenses;
    }

    private async Task<List<CashBookEntryDto>> GetUnifiedEntriesAsync(CashBookQuery query)
    {
        // Get income entries (TransaccionPago)
        var incomeQuery = context.TransaccionPagos
            .Include(t => t.IdBancoDestinoNavigation)
            .Include(t => t.IdDeudaNavigation)
            .Where(t => t.FechaPago >= query.StartDate && t.FechaPago <= query.EndDate);

        if (query.FinancialAccountId.HasValue)
            incomeQuery = incomeQuery.Where(t => t.IdBancoDestino == query.FinancialAccountId.Value);

        if (!query.IncludeVoided)
            incomeQuery = incomeQuery.Where(t => t.Estado != StatusVoided);

        var incomeEntries = await incomeQuery
            .Select(t => new CashBookEntryDto
            {
                Id = t.IdPago,
                Type = TypeIncome,
                Date = t.FechaPago ?? DateTime.MinValue,
                Description = $"Cobranza - {t.IdDeudaNavigation.MesPeriodo}/{t.IdDeudaNavigation.AnioPeriodo}",
                Reference = t.NroComprobanteBanco,
                Amount = t.MontoAbonado,
                AccountName = t.IdBancoDestinoNavigation.NombreEntidad,
                AccountId = t.IdBancoDestino,
                IsVoided = t.Estado == StatusVoided
            })
            .ToListAsync();

        // Get expense entries (Egreso)
        var expenseQuery = context.Egresos
            .Include(e => e.IdBancoOrigenNavigation)
            .Where(e => e.FechaEgreso >= query.StartDate && e.FechaEgreso <= query.EndDate);

        if (query.FinancialAccountId.HasValue)
            expenseQuery = expenseQuery.Where(e => e.IdBancoOrigen == query.FinancialAccountId.Value);

        var expenseEntries = await expenseQuery
            .Select(e => new CashBookEntryDto
            {
                Id = e.IdEgreso,
                Type = TypeExpense,
                Date = e.FechaEgreso ?? DateTime.MinValue,
                Description = e.Concepto,
                Reference = e.NroFacturaProveedor,
                Amount = -e.MontoTotal, // Negative for expenses
                AccountName = e.IdBancoOrigenNavigation.NombreEntidad,
                AccountId = e.IdBancoOrigen,
                IsVoided = false // Egresos don't have voided status currently
            })
            .ToListAsync();

        // Union and order by date
        return [.. incomeEntries
            .Concat(expenseEntries)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.Type)]; // Income before Expense on same date
    }
}
