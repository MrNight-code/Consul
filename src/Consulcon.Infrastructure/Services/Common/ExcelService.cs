using ClosedXML.Excel;
using Consulcon.Application.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace Consulcon.Infrastructure.Services.Common;

public class ExcelService(Microsoft.Extensions.Configuration.IConfiguration configuration) : IExcelService
{
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration = configuration;

    public byte[] GenerateExcel<T>(List<T> data, string sheetName)
    {
        if (string.IsNullOrWhiteSpace(sheetName)) sheetName = "Export";
        var dateFormat = _configuration["ExcelSettings:DateFormat"] ?? "dd/MM/yyyy HH:mm";
        var numberFormat = _configuration["ExcelSettings:NumberFormat"] ?? "#,##0.00";
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);
        var properties = typeof(T).GetProperties();

        // 1. Encabezados
        for (int i = 0; i < properties.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = properties[i].Name;
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // 2. Datos
        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 0; j < properties.Length; j++)
            {
                var value = properties[j].GetValue(data[i], null);
                var cell = worksheet.Cell(i + 2, j + 1);
                
                if (value is DateTime dateValue)
                {
                    cell.Value = dateValue;
                    cell.Style.DateFormat.Format = dateFormat; 
                }
                else if (value is decimal d)
                {
                    cell.Value = d;
                    cell.Style.NumberFormat.Format = numberFormat;
                }
                else if (value is double dbl)
                {
                    cell.Value = dbl;
                    cell.Style.NumberFormat.Format = numberFormat;
                }
                else
                {
                    cell.Value = value?.ToString();
                }
            }
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
