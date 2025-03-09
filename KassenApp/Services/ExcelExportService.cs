using ClosedXML.Excel;
using KassenApp.Data;
using KassenApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace KassenApp.Services
{
    public class ExcelExportService
    {
        public byte[] ExportBuchungenToExcel(List<Buchung> buchungen)
        {
            using var workbook = new XLWorkbook(); ;

            // Gruppieren nach Kontenart und die Gesamtrechnung vorbereiten:

            var gruppierteBuchungen = buchungen.GroupBy(buch => buch.Konto.KontoName);
            var gesamtrechnug = new List<Buchung>();

            foreach (var gruppe in gruppierteBuchungen)
            {
                var sheet = workbook.Worksheets.Add(gruppe.Key);
                ErstellteTabelle(sheet, gruppe.ToList());
                gesamtrechnug.AddRange(gruppe);
            }

            // Gesamtrechnung hinzufügen
            var gesamtSheet = workbook.Worksheets.Add("Gesamtrechnung");
            ErstellteTabelle(gesamtSheet, gesamtrechnug);

            // Datei als Byte-Array zurückgeben
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void ErstellteTabelle(IXLWorksheet sheet, List<Buchung> buchungen)
        {
            // Spaltenüberschriften
            var header = new[] { "Datum", "Verwendung", "Einnahmen", "Ausgaben" };

            for (int i = 0; i < header.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = header[i];
                sheet.Cell(1, i + 1).Style.Font.Bold = true;
                sheet.Cell(1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cell(1, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Buchungsdaten einfügen
            int row = 2;

            foreach (var buchung in buchungen)
            {
                sheet.Cell(row, 1).Value = buchung.Datum.ToString("dd.MM.yyyy");
                sheet.Cell(row, 2).Value = buchung.Verwendungszweck ?? "-";

                // 🔄 BETRAG: Korrekte Anzeige für Einnahmen und Ausgaben
                if (buchung.Buchungsart == Buchungsart.Einnahme)
                {
                    sheet.Cell(row, 3).Value = buchung.Betrag;  // ✅ Einnahme korrekt gesetzt
                    sheet.Cell(row, 4).Value = 0.00m;          // ✅ Ausgaben = 0,00 €
                }
                else if (buchung.Buchungsart == Buchungsart.Ausgabe)
                {
                    sheet.Cell(row, 3).Value = 0.00m;          // ✅ Einnahmen = 0,00 €
                    sheet.Cell(row, 4).Value = buchung.Betrag; // ✅ Ausgabe korrekt gesetzt
                }

                // Währungsformatierung anwenden
                sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00 €";
                sheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00 €";

                // ➡️ Rahmen setzen
                for (int col = 1; col <= 4; col++)
                {
                    sheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                row++;
            }

            // 🟠 Summen- und Endbestand-Zeile
            if (buchungen.Any())
            {
                sheet.Cell(row, 2).Value = "Summen";
                sheet.Cell(row, 3).FormulaA1 = $"=SUM(C2:C{row - 1})";
                sheet.Cell(row, 4).FormulaA1 = $"=SUM(D2:D{row - 1})";

                sheet.Cell(row + 1, 2).Value = "Endbestand";
                sheet.Cell(row + 1, 3).FormulaA1 = $"=C{row} - D{row}";

                // Summen und Endbestand fett hervorheben
                sheet.Range(row, 2, row + 1, 4).Style.Font.Bold = true;
                sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00 €";
                sheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00 €";
                sheet.Cell(row + 1, 3).Style.NumberFormat.Format = "#,##0.00 €";
            }

            sheet.Columns().AdjustToContents();
        }
    }

    //[Microsoft.AspNetCore.Mvc.Route("api/excel-export")]
    //[ApiController]
    //public class ExcelExportController : ControllerBase
    //{
    //    private readonly ExcelExportService _excelExportService;
    //    private readonly KassenDbContext _context;

    //    public ExcelExportController(ExcelExportService excelExportService, KassenDbContext dbContext)
    //    {
    //        _excelExportService = excelExportService;
    //        _context = dbContext;
    //    }

    //    [HttpGet("export_buchungen")]
    //    public IActionResult ExportBuchungen()
    //    {
    //        var buchungen = _context.Buchungen.Include(buchung => buchung.Konto).ToList();
    //        var excelFile = _excelExportService.ExportBuchungenToExcel(buchungen);

    //        return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kassenübersicht.xslx");
    //    }

    //}
}
