using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Facturacion;
using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Domain.Interfaces;
using Consulcon.Domain.Specifications;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace Consulcon.Infrastructure.Services.Facturacion;

public class ReceiptGenerationService(ConsulconDbContext context, IRepository<TransaccionPago> repository, IConfiguration configuration) : IReceiptGenerationService
{
    private readonly string _outputFolder = configuration["ReceiptSettings:OutputFolder"] ?? "GeneratedReceipts";
    private readonly IConfiguration _configuration = configuration;
    private readonly ConsulconDbContext _context = context;
    private readonly IRepository<TransaccionPago> _repository = repository;

    public async Task<TransaccionPago> GenerateReceiptAsync(int transaccionId)
    {
        // Ensure directory exists
        if (!Directory.Exists(_outputFolder))
        {
            Directory.CreateDirectory(_outputFolder);
        }

        // 1. Fetch Complete Data
        var pago = await _context.TransaccionPagos
            .Include(x => x.IdDeudaNavigation)
                .ThenInclude(d => d.IdContratoNavigation)
                    .ThenInclude(c => c.IdPropiedadNavigation)
            .Include(x => x.IdDeudaNavigation)
                .ThenInclude(d => d.DeudaDetalles)
            .Include(x => x.IdPersonaPagadorNavigation)
            .Include(x => x.IdBancoDestinoNavigation)
            .Include(x => x.IdFormaPagoNavigation)
            .FirstOrDefaultAsync(x => x.IdPago == transaccionId);

        ArgumentNullException.ThrowIfNull(pago, $"No se encontró la transacción de pago {transaccionId}.");

        // Uniqueness Validation
        if (!string.IsNullOrEmpty(pago.ReciboUrl))
        {
            throw new InvalidOperationException("Ya existe un recibo generado para este pago. No se puede emitir un recibo duplicado.");
        }

        // 2. Get STRICT Server Timestamp (UTC)
        var serverTimeUtc = DateTime.UtcNow;
        // Convert to Bolivian Time (UTC-4)
        var boliviaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
        var serverTimeBolivia = TimeZoneInfo.ConvertTimeFromUtc(serverTimeUtc, boliviaTimeZone);

        // Extract Data for filename
        var unitId = pago.IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation.CodigoUnidad 
                     ?? pago.IdDeudaNavigation.IdContratoNavigation.IdPropiedad.ToString();

        // 3. Define File Path
        var fileName = $"Recibo_{transaccionId}_U{unitId}_{serverTimeBolivia:yyyyMMddHHmmss}.pdf";
        var filePath = Path.Combine(_outputFolder, fileName);
        var fullPath = Path.GetFullPath(filePath);

        // 4. Generate PDF (Offload CPU-bound work)
        await Task.Run(() =>
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    ComposeReceiptPage(page, pago, serverTimeBolivia);
                });
            })
            .GeneratePdf(fullPath);
        });

        // 5. Update Entity (Persistence) using EXISTING properties
        pago.ReciboUrl = fullPath; 
        pago.FechaRecibo = serverTimeUtc; 


        await _context.SaveChangesAsync();

        return pago;
    }

    private static void ComposeReceiptPage(PageDescriptor page, TransaccionPago pago, DateTime serverTimeBolivia)
    {
        var unitId = pago.IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation.CodigoUnidad 
                     ?? pago.IdDeudaNavigation.IdContratoNavigation.IdPropiedad.ToString();
        var period = $"{pago.IdDeudaNavigation.MesPeriodo}/{pago.IdDeudaNavigation.AnioPeriodo}";
        var amount = pago.MontoAbonado;
        var pagador = pago.IdPersonaPagadorNavigation?.NombreCompleto ?? "N/A";

        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(12));

        page.Header()
            .Row(row => 
            {
                row.RelativeItem().Column(c => 
                {
                    c.Item().Text("RECIBO DE PAGO").Bold().FontSize(20).FontColor(Colors.Blue.Darken2);
                    c.Item().Text($"Nro. Transacción: {pago.IdPago}").FontSize(10);
                });
                row.ConstantItem(100).Text($"{serverTimeBolivia:dd/MM/yyyy}").AlignRight();
            });

        page.Content()
            .PaddingVertical(1, Unit.Centimetre)
            .Column(column =>
            {
                column.Item().Text("Detalle de la Transacción").Bold().FontSize(14);
                column.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // General Info Table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(120);
                        columns.RelativeColumn();
                    });

                    table.Cell().Text("Unidad:").Bold();
                    table.Cell().Text(unitId);

                    table.Cell().Text("Periodo:").Bold();
                    table.Cell().Text(period);

                    table.Cell().Text("Pagador:").Bold();
                    table.Cell().Text(pagador);
                    
                    table.Cell().Text("Fecha Pago:").Bold();
                    table.Cell().Text($"{pago.FechaPago:dd/MM/yyyy HH:mm}");

                    table.Cell().Text("Monto Total:").Bold();
                    table.Cell().Text($"{amount:N2} bs").Bold().FontColor(Colors.Green.Darken2);

                    table.Cell().Text("Banco:").Bold();
                    table.Cell().Text(pago.IdBancoDestinoNavigation?.NombreEntidad ?? "N/A");

                    table.Cell().Text("Medio de Pago:").Bold();
                    table.Cell().Text(pago.IdFormaPagoNavigation?.Descripcion ?? "N/A");

                    if (!string.IsNullOrEmpty(pago.NroComprobanteBanco))
                    {
                        table.Cell().Text("Nro Referencia:").Bold();
                        table.Cell().Text(pago.NroComprobanteBanco);
                    }

                    if (!string.IsNullOrEmpty(pago.Observaciones))
                    {
                        table.Cell().Text("Notas:").Bold();
                        table.Cell().Text(pago.Observaciones);
                    }
                });

                // Itemized Breakdown Map
                column.Item().PaddingTop(15).Text("Conceptos de la Deuda").Bold().FontSize(12);
                column.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Concepto").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Subtotal").Bold();
                        
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    });

                    // Rows
                    foreach (var detalle in pago.IdDeudaNavigation.DeudaDetalles)
                    {
                        table.Cell().PaddingVertical(2).Text(detalle.Concepto);
                        table.Cell().PaddingVertical(2).AlignRight().Text($"{detalle.Subtotal:N2} bs");
                    }
                });

                column.Item().PaddingTop(20).Border(1).BorderColor(Colors.Grey.Darken1).Padding(10).Column(box => 
                {
                    box.Item().Text("Validación de Seguridad").Bold().FontSize(10);
                    box.Item().Text($"Este documento fue generado automáticamente por el servidor en {serverTimeBolivia:yyyy-MM-dd HH:mm:ss} (Bolivia).").FontSize(9);
                    box.Item().Text($"Firma Digital (Hash temporal): {Guid.NewGuid()}").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });

        page.Footer()
            .AlignCenter()
            .Text(text =>
            {
                text.Span("Generado el: ");
                text.Span($"{serverTimeBolivia:yyyy-MM-dd HH:mm:ss}").Bold();
                text.Span(" (Hora Bolivia)");
            });
    }

    public async Task<PagedResult<Consulcon.Application.DTOs.Facturacion.ReceiptDto>> GetGeneratedReceiptsAsync(PaginationParams parameters, string? medio = null, int? propiedadId = null)
    {
        // 1. Instanciamos la especificación (La "receta" del filtrado)
        var spec = new ReceiptWithFiltersSpec(parameters, medio, propiedadId);

        // 2. Ejecutamos la consulta paginada usando el repositorio genérico
        var pagedData = await _repository.GetPagedAsync(spec, parameters.PageNumber, parameters.PageSize);
        
        var defaultClient = _configuration["ReceiptSettings:DefaultClientName"] ?? "Cliente General";
        var defaultMethod = _configuration["ReceiptSettings:DefaultPaymentMethod"] ?? "S/M";

        // 3. Mapeamos de Entidad (TransaccionPago) a DTO (ReceiptDto)
        return pagedData.Map(x => new Consulcon.Application.DTOs.Facturacion.ReceiptDto
        {
            IdPago = x.IdPago,
            ReciboUrl = x.ReciboUrl,
            FechaRecibo = x.FechaPago ?? x.FechaRecibo ?? DateTime.Now,
            MontoAbonado = x.MontoAbonado,
            NombrePersona = x.IdPersonaPagadorNavigation != null ? x.IdPersonaPagadorNavigation.NombreCompleto : defaultClient,
            MetodoPago = x.IdFormaPagoNavigation != null ? x.IdFormaPagoNavigation.Descripcion : defaultMethod,
            CodigoUnidad = x.IdDeudaNavigation?.IdContratoNavigation?.IdPropiedadNavigation?.CodigoUnidad 
                           ?? x.IdDeudaNavigation?.IdContratoNavigation?.IdPropiedad.ToString() ?? "N/A"
        });
    }

    public async Task<Stream> GetBatchZipAsync(int mes, int anio)
    {
        var receipts = await _context.TransaccionPagos
            .Where(x => x.FechaRecibo.HasValue && 
                        x.FechaRecibo.Value.Month == mes && 
                        x.FechaRecibo.Value.Year == anio &&
                        x.ReciboUrl != null && x.ReciboUrl != "")
            .Select(x => x.ReciboUrl)
            .ToListAsync();

        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var filePath in receipts)
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    var entryName = Path.GetFileName(filePath);
                    archive.CreateEntryFromFile(filePath, entryName);
                }
            }
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<byte[]> GenerateBatchReceiptsPdfAsync(BatchReceiptRequestDto request)
    {
        var query = _context.TransaccionPagos
            .Include(x => x.IdDeudaNavigation)
                .ThenInclude(d => d.IdContratoNavigation)
                    .ThenInclude(c => c.IdPropiedadNavigation)
            .Include(x => x.IdDeudaNavigation)
                .ThenInclude(d => d.DeudaDetalles)
            .Include(x => x.IdPersonaPagadorNavigation)
            .Include(x => x.IdBancoDestinoNavigation)
            .Include(x => x.IdFormaPagoNavigation)
            .Where(x => x.FechaPago >= request.StartDate && x.FechaPago <= request.EndDate);

        if (request.UnitId.HasValue)
        {
            query = query.Where(x => x.IdDeudaNavigation.IdContratoNavigation.IdPropiedad == request.UnitId.Value);
        }

        var pagos = await query.OrderBy(x => x.FechaPago).ToListAsync();

        if (pagos.Count == 0)
        {
            throw new InvalidOperationException("No se encontraron pagos en el rango de fechas especificado.");
        }

        var boliviaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
        var serverTimeBolivia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, boliviaTimeZone);

        return await Task.Run(() =>
        {
            return Document.Create(container =>
            {
                foreach (var pago in pagos)
                {
                    container.Page(page =>
                    {
                        ComposeReceiptPage(page, pago, serverTimeBolivia);
                    });
                }
            })
            .GeneratePdf();
        });
    }
}
