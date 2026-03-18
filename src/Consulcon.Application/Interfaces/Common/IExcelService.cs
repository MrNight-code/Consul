using System.Collections.Generic;

namespace Consulcon.Application.Interfaces.Common;

public interface IExcelService
{
    byte[] GenerateExcel<T>(List<T> data, string sheetName);
}
