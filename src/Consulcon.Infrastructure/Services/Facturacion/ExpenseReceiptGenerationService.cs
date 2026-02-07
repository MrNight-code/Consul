using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Consulcon.Application.Interfaces.Facturacion;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;
using System.Drawing;

namespace Consulcon.Infrastructure.Services.Facturacion;

public class ExpenseReceiptGenerationService(ConsulconDbContext context) : IExpenseReceiptGenerationService
{
    private readonly ConsulconDbContext _context = context;

    public async Task<Stream> GenerateReceiptAsync(int egresoId)
    {
        // 1. Fetch Complete Data
        var egreso = await _context.Egresos
            .Include(x => x.IdCondominioNavigation)
            .Include(x => x.IdProveedorNavigation)
            .Include(x => x.IdBancoOrigenNavigation)
            .Include(x => x.IdFormaPagoNavigation)
            .Include(x => x.EgresoDetalles)
            .FirstOrDefaultAsync(x => x.IdEgreso == egresoId)
            ?? throw new Exception("Egreso no encontrado");

        // 2. Prepare Data
        var serverTimeBolivia = DateTime.UtcNow.AddHours(-4);
        var condominioName = egreso.IdCondominioNavigation.Nombre;
        // Asumiendo que el logo está en Base64 en alguna config o usar uno genérico por ahora
        // Para este MVP usaremos texto.
        
        var proveedorName = egreso.IdProveedorNavigation?.RazonSocial ?? "Sin Proveedor";
        var proveedorNit = egreso.IdProveedorNavigation?.Nit ?? "S/N";
        
        // 3. Generate QR
        var qrGenerator = new QRCodeGenerator();
        var qrData = new 
        { 
            Id = egreso.IdEgreso, 
            Fecha = egreso.FechaEgreso, 
            Monto = egreso.MontoTotal, 
            Hash = Guid.NewGuid().ToString() // Simulating hash
        };
        var qrCodeData = qrGenerator.CreateQrCode(System.Text.Json.JsonSerializer.Serialize(qrData), QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        var qrBytes = qrCode.GetGraphic(20);

        // 4. Generate PDF
        var stream = new MemoryStream();
        
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5.Landscape()); // O Ticket 80mm segun requerimiento, usaremos A5 Landscape
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(condominioName).Bold().FontSize(14);
                        c.Item().Text($"Comprobante de Egreso #{egreso.IdEgreso}").FontSize(12).FontColor(Colors.Blue.Darken2);
                        c.Item().Text($"Fecha: {egreso.FechaEgreso:dd/MM/yyyy}").FontSize(10);
                    });
                    
                    row.ConstantItem(80).AlignRight().Image(qrBytes);
                });

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text("Datos del Proveedor").Bold().Underline();
                    col.Item().Text($"Razón Social: {proveedorName}");
                    col.Item().Text($"NIT/CI: {proveedorNit}");
                    
                    col.Item().PaddingTop(10).Text("Detalle del Gasto").Bold().Underline();
                    
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); // Concepto
                            columns.ConstantColumn(50); // Cantidad
                            columns.ConstantColumn(80); // Precio U
                            columns.ConstantColumn(80); // Subtotal
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Concepto").Bold();
                            header.Cell().Text("Cant.").Bold();
                            header.Cell().Text("P.Unit.").Bold();
                            header.Cell().Text("Subtotal").Bold();
                        });

                        // Use EgresoDetalles if available, otherwise fallback to Egreso Header
                        if (egreso.EgresoDetalles?.Count > 0)
                        {
                            foreach (var det in egreso.EgresoDetalles)
                            {
                                table.Cell().Text(det.Concepto);
                                table.Cell().Text(det.Cantidad.ToString());
                                table.Cell().Text($"{det.PrecioUnitario:N2}");
                                table.Cell().Text($"{det.Subtotal:N2}");
                            }
                        }
                        else
                        {
                            // Fallback
                            table.Cell().Text(egreso.Concepto);
                            table.Cell().Text("1");
                            table.Cell().Text($"{egreso.MontoTotal:N2}");
                            table.Cell().Text($"{egreso.MontoTotal:N2}");
                        }
                    });

                    col.Item().PaddingTop(5).AlignRight().Text($"Total: {egreso.MontoTotal:N2} Bs").Bold().FontSize(12);

                    col.Item().PaddingTop(10).Text($"Banco Origen: {egreso.IdBancoOrigenNavigation.NombreEntidad}");
                    col.Item().Text($"Forma de Pago: {egreso.IdFormaPagoNavigation.Descripcion}");
                    if (!string.IsNullOrEmpty(egreso.NroFacturaProveedor))
                    {
                        col.Item().Text($"Nro. Factura: {egreso.NroFacturaProveedor}");
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().PaddingTop(30).LineHorizontal(1);
                        c.Item().AlignCenter().Text("Recibí Conforme");
                    });
                    
                    row.ConstantItem(20); // Spacer

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().AlignRight().Text($"Generado: {serverTimeBolivia:dd/MM/yyyy HH:mm:ss}");
                        c.Item().AlignRight().Text($"Hash: {Guid.NewGuid().ToString()[..8]}").FontSize(6).FontColor(Colors.Grey.Medium);
                    });
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }
}
