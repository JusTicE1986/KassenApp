using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KassenApp.Data;
using KassenApp.Services;

namespace KassenApp.Controller
{
    [Route("api/excel-export")]  // Basis-URL für diesen Controller
    [ApiController]               // Kennzeichnung als API-Controller
    public class ExcelExportController : ControllerBase
    {

        // Standard: Geschäftsjahr (Vorjahr)
        
        private readonly ExcelExportService _excelExportService;
        private readonly KassenDbContext _context;

        // Konstruktor mit Dependency Injection für Service und DbContext
        public ExcelExportController(ExcelExportService excelExportService, KassenDbContext dbContext)
        {
            _excelExportService = excelExportService;
            _context = dbContext;
        }

        // GET-Endpunkt für den Excel-Download
        [HttpGet("export-buchungen")]
        public IActionResult ExportBuchungen(int? jahr = null)
        {

            var jahrFilter = jahr ?? DateTime.Now.Year - 1;
            var startDatum = new DateTime(jahrFilter, 1, 1);
            var endDatum = new DateTime(jahrFilter, 12, 31);
            var buchungen = _context.Buchungen
                .Include(b => b.Konto)
                .Where(b => b.Datum >= startDatum && b.Datum <= endDatum)
                .ToList();


            if (!buchungen.Any())
            {
                Console.WriteLine("❗️ Keine Buchungen gefunden!");
            }
            else
            {
                Console.WriteLine($"✅ Gefundene Buchungen: {buchungen.Count}");
                foreach (var buchung in buchungen)
                {
                    Console.WriteLine($"📝 {buchung.Datum} - {buchung.Verwendungszweck} - {buchung.Betrag}");
                }
            }
            var excelFile = _excelExportService.ExportBuchungenToExcel(buchungen);

            return File(
                excelFile,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Kassenübersicht.xlsx");
        }
    }
}
