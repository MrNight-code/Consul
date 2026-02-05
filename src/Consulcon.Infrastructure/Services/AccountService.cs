using Consulcon.Application.DTOs;
using Consulcon.Application.Interfaces;
using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.General;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulcon.Infrastructure.Services
{
    public class AccountService(ConsulconDbContext context) : IAccountService
    {

        public async Task<Result<List<AccountDto>>> GetAllAccountsAsync(bool activeOnly = true)
        {
            var query = context.Bancos.AsQueryable();

            if (activeOnly)
                query = query.Where(b => b.Activo == true);

            var accounts = await query
                .Select(b => new AccountDto
                {
                    Id = b.IdBanco,
                    Name = b.NombreEntidad,
                    Type = b.Tipo,
                    AccountNumber = b.NumeroCuenta,
                    IsActive = b.Activo ?? false
                })
                .ToListAsync();

            return Result.Ok(accounts);
        }

        public async Task<Result<AccountDto>> GetAccountByIdAsync(int id)
        {
            var banco = await context.Bancos.FindAsync(id);
            if (banco == null) return Result.Fail<AccountDto>("Cuenta no encontrada.");

            return Result.Ok(new AccountDto
            {
                Id = banco.IdBanco,
                Name = banco.NombreEntidad,
                Type = banco.Tipo,
                AccountNumber = banco.NumeroCuenta,
                IsActive = banco.Activo ?? false
            });
        }

        public async Task<Result<int>> CreateAccountAsync(AccountDto accountDto)
        {
            var banco = new Banco
            {
                NombreEntidad = accountDto.Name,
                Tipo = accountDto.Type,
                NumeroCuenta = accountDto.AccountNumber,
                Activo = accountDto.IsActive,
                // Defaulting potentially required fields if not in DTO or nullable
                // Moneda? IdCuentaContable?
            };

            context.Bancos.Add(banco);
            await context.SaveChangesAsync();

            return Result.Ok(banco.IdBanco);
        }

        public async Task<Result<bool>> UpdateAccountAsync(int id, AccountDto accountDto)
        {
            var banco = await context.Bancos.FindAsync(id);
            if (banco == null) return Result.Fail<bool>("Cuenta no encontrada.");

            banco.NombreEntidad = accountDto.Name;
            banco.Tipo = accountDto.Type;
            banco.NumeroCuenta = accountDto.AccountNumber;
            banco.Activo = accountDto.IsActive;

            await context.SaveChangesAsync();
            return Result.Ok(true);
        }

        public async Task<Result<bool>> DeleteAccountAsync(int id)
        {
            // Integrity Check: Cannot delete if linked to TransaccionPagos
            bool hasPayments = await context.TransaccionPagos.AnyAsync(t => t.IdBancoDestino == id);
            if (hasPayments)
            {
                return Result.Fail<bool>("No se puede eliminar la cuenta porque tiene cobranzas asociadas.");
            }

            var banco = await context.Bancos.FindAsync(id);
            if (banco == null) return Result.Fail<bool>("Cuenta no encontrada.");

            context.Bancos.Remove(banco);
            await context.SaveChangesAsync();

            return Result.Ok(true);
        }
    }
}
