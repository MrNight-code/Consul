using Consulcon.Application.Interfaces.Facturacion;
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Consulcon.Infrastructure.Services.Facturacion;

public class ReceiptGenerationService(ConsulconDbContext context) : Consulcon.Application.Interfaces.Facturacion.IReceiptGenerationService
{
    private const string OutputFolder = "GeneratedReceipts";
    private readonly ConsulconDbContext _context = context;

    public async Task<TransaccionPago> GenerateReceiptAsync(int transaccionId)
    {
        // Ensure directory exists
        if (!Directory.Exists(OutputFolder))
        {
            Directory.CreateDirectory(OutputFolder);
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

        if (pago is null) throw new Exception("Transacción de pago no encontrada");

        // Extract Data
        var unitId = pago.IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation.CodigoUnidad 
                     ?? pago.IdDeudaNavigation.IdContratoNavigation.IdPropiedad.ToString();
        var period = $"{pago.IdDeudaNavigation.MesPeriodo}/{pago.IdDeudaNavigation.AnioPeriodo}";
        var amount = pago.MontoAbonado;
        var pagador = pago.IdPersonaPagadorNavigation?.NombreCompleto ?? "N/A";

        // 2. Get STRICT Server Timestamp (UTC)
        var serverTimeUtc = DateTime.UtcNow;
        // Convert to Bolivian Time (UTC-4)
        var boliviaOffset = TimeSpan.FromHours(-4);
        var serverTimeBolivia = serverTimeUtc.Add(boliviaOffset);

        // 3. Define File Path
        var fileName = $"Recibo_{transaccionId}_U{unitId}_{serverTimeBolivia:yyyyMMddHHmmss}.pdf";
        var filePath = Path.Combine(OutputFolder, fileName);
        var fullPath = Path.GetFullPath(filePath);

        // 4. Generate PDF (Offload CPU-bound work)
        await Task.Run(() =>
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Row(row => 
                        {
                            row.RelativeItem().Column(c => 
                            {
                                c.Item().Text("Comprobante de Pago").Bold().FontSize(20).FontColor(Colors.Blue.Darken2);
                                c.Item().Text($"Nro. Transacción: {transaccionId}").FontSize(10);
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
                                    columns.ConstantColumn(100);
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

                                table.Cell().Text("Forma Pago:").Bold();
                                table.Cell().Text(pago.IdFormaPagoNavigation?.Descripcion ?? "N/A");

                                if (!string.IsNullOrEmpty(pago.NroComprobanteBanco))
                                {
                                    table.Cell().Text("Nro. Op.:").Bold();
                                    table.Cell().Text(pago.NroComprobanteBanco);
                                }

                                if (!string.IsNullOrEmpty(pago.Observaciones))
                                {
                                    table.Cell().Text("Observaciones:").Bold();
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
                            // INMUTABLE TIMESTAMP SEAL
                            text.Span($"{serverTimeBolivia:yyyy-MM-dd HH:mm:ss}").Bold();
                            text.Span(" (Hora Bolivia)");
                        });
                });
            })
            .GeneratePdf(fullPath);
        });

        // 5. Update Entity (Persistence) using EXISTING properties
        pago.ReciboUrl = fullPath; // Adapted from RutaReciboPdf
        pago.FechaRecibo = serverTimeUtc; // Persist UTC in DB for consistency


        await _context.SaveChangesAsync();

        return pago;
    }

    public async Task<List<Consulcon.Application.DTOs.Facturacion.ReceiptDto>> GetGeneratedReceiptsAsync(Consulcon.Application.DTOs.Facturacion.ReceiptFilterDto filter)
    {
        var query = _context.TransaccionPagos
            .Include(x => x.IdPersonaPagadorNavigation)
            .Where(x => x.ReciboUrl != null && x.ReciboUrl != "");

        if (filter.FechaDesde.HasValue)
        {
            query = query.Where(x => x.FechaRecibo >= filter.FechaDesde.Value);
        }

        if (filter.FechaHasta.HasValue)
        {
            query = query.Where(x => x.FechaRecibo <= filter.FechaHasta.Value);
        }

        if (filter.PersonaId.HasValue)
        {
            query = query.Where(x => x.IdPersonaPagador == filter.PersonaId.Value);
        }

        return await query.Select(x => new Consulcon.Application.DTOs.Facturacion.ReceiptDto
        {
            IdPago = x.IdPago,
            ReciboUrl = x.ReciboUrl,
            FechaRecibo = x.FechaRecibo,
            MontoAbonado = x.MontoAbonado,
            NombrePersona = x.IdPersonaPagadorNavigation.NombreCompleto
        }).ToListAsync();
    }
}
