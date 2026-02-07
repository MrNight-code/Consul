using Consulcon.Application.DTOs.Contabilidad.Expenses;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Consulcon.Infrastructure.Services.Contabilidad
{
    public class ExpenseService(ConsulconDbContext context) : IExpenseService
    {
        public async Task<Result<int>> RegisterExpenseAsync(RegisterExpenseCommand cmd, int userId)
        {
            // Validations
            if (cmd.Amount <= 0.01m)
                return Result.Fail<int>("El monto debe ser mayor a 0.01.");

            if (cmd.ExpenseDate > DateTime.UtcNow)
                return Result.Fail<int>("La fecha del egreso no puede ser futura.");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 1. Get Account
                var account = await context.Bancos.FindAsync(cmd.AccountId);
                if (account == null)
                    return Result.Fail<int>("La cuenta especificada no existe.");

                // Validate Balance
                if (account.Saldo < cmd.Amount)
                     return Result.Fail<int>("Saldo insuficiente en la cuenta seleccionada.");

                // 2. Debit Account
                account.Debit(cmd.Amount);
                
                // 3. Create Expense
                var expense = new Egreso
                {
                    IdCondominio = cmd.CondominioId,
                    IdBancoOrigen = cmd.AccountId,
                    IdAutorizacion = cmd.CategoryId,
                    IdFormaPago = cmd.PaymentMethodId,
                    Concepto = cmd.Description,
                    MontoTotal = cmd.Amount,
                    FechaEgreso = cmd.ExpenseDate,
                    NroFacturaProveedor = cmd.InvoiceNumber,
                    IdProveedor = cmd.ProviderId,
                    IdUsuarioRegistro = userId,
                    // Default/Nulls
                    IdPersonaBeneficiario = null
                };

                context.Egresos.Add(expense);
                await context.SaveChangesAsync();

                // 4. Create Transaction History
                var history = new AccountTransactionHistory
                {
                    Id = Guid.NewGuid(),
                    AccountId = cmd.AccountId,
                    ExpenseId = expense.IdEgreso,
                    Amount = -cmd.Amount, // Negative for expense
                    Date = DateTime.UtcNow,
                    Description = $"Egreso #{expense.IdEgreso}: {cmd.Description}",
                    ReferenceId = expense.IdEgreso.ToString()
                };
                context.AccountTransactionHistories.Add(history);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Result.Ok(expense.IdEgreso);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Fail<int>($"Error al registrar el gasto: {ex.Message}");
            }
        }
    }
}
