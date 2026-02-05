using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Facturacion;

namespace Consulcon.Application.Services.Facturacion;

public class PagoService : IPagoService
{
    private readonly IRepository<TransaccionPago> _pagoRepository;
    private readonly IRepository<DeudaCabecera> _deudaRepository;

    public PagoService(IRepository<TransaccionPago> pagoRepository, IRepository<DeudaCabecera> deudaRepository)
    {
        _pagoRepository = pagoRepository;
        _deudaRepository = deudaRepository;
    }

    public async Task<Result<IEnumerable<TransaccionPagoDto>>> GetByDeudaAsync(int deudaId)
    {
        var entities = await _pagoRepository.FindAsync(p => p.IdDeuda == deudaId);
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<TransaccionPagoDto>> RegistrarPagoAsync(CreatePagoDto dto)
    {
        var deuda = await _deudaRepository.GetByIdAsync(dto.IdDeuda);
        if (deuda == null) return Result.Fail<TransaccionPagoDto>("Deuda no encontrada");

        if (deuda.EstadoPago == "PAGADO" || deuda.EstadoPago == "ANULADO")
            return Result.Fail<TransaccionPagoDto>("La deuda ya está pagada o anulada");

        var pago = new TransaccionPago
        {
            IdDeuda = dto.IdDeuda,
            IdPersonaPagador = dto.IdPersonaPagador,
            IdBancoDestino = dto.IdBancoDestino,
            IdFormaPago = dto.IdFormaPago,
            MontoAbonado = dto.MontoAbonado,
            FechaPago = DateTime.Now,
            NroComprobanteBanco = dto.NroComprobanteBanco,
            Estado = "CONFIRMADO",
            TipoCambio = 1
        };

        await _pagoRepository.AddAsync(pago);

        // Update user balance/debt status
        deuda.TotalPagado = (deuda.TotalPagado ?? 0) + dto.MontoAbonado;
        
        if (deuda.TotalPagado >= deuda.TotalDeuda)
        {
            deuda.EstadoPago = "PAGADO";
        }
        else
        {
            deuda.EstadoPago = "PARCIAL";
        }

        await _deudaRepository.UpdateAsync(deuda);

        return Result.Ok(MapToDto(pago));
    }

    private static TransaccionPagoDto MapToDto(TransaccionPago entity)
    {
        return new TransaccionPagoDto
        {
            Id = entity.IdPago,
            IdDeuda = entity.IdDeuda,
            IdPersonaPagador = entity.IdPersonaPagador,
            IdBancoDestino = entity.IdBancoDestino,
            IdFormaPago = entity.IdFormaPago,
            FechaPago = entity.FechaPago,
            MontoAbonado = entity.MontoAbonado,
            NroComprobanteBanco = entity.NroComprobanteBanco,
            Estado = entity.Estado
        };
    }
}
