using ClosedXML.Excel;
using ssptb.pe.tdlt.transaction.dto.Audit;

namespace ssptb.pe.tdlt.transaction.internalservices.Helpers;
public static class ExcelHelper
{
    public static byte[] GenerateAuditReportExcel(List<AuditDiscrepancyDto> discrepancies)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Audit Report");

        // Headers
        worksheet.Cell(1, 1).Value = "Block ID";
        worksheet.Cell(1, 2).Value = "Reason";
        worksheet.Cell(1, 3).Value = "Local Data";
        worksheet.Cell(1, 4).Value = "Tangle Data";
        worksheet.Cell(1, 5).Value = "Error";

        // Fill data
        for (int i = 0; i < discrepancies.Count; i++)
        {
            var discrepancy = discrepancies[i];
            worksheet.Cell(i + 2, 1).Value = discrepancy.BlockId;
            worksheet.Cell(i + 2, 2).Value = discrepancy.Reason;
            worksheet.Cell(i + 2, 3).Value = discrepancy.LocalData;
            worksheet.Cell(i + 2, 4).Value = discrepancy.TangleData;
            worksheet.Cell(i + 2, 5).Value = discrepancy.Error;
        }

        // Adjust columns
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
