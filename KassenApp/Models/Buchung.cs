using System.ComponentModel.DataAnnotations;

namespace KassenApp.Models
{
    public class Buchung
    {
        [Key]
        public int Id { get; set; }
        public Buchungsart Buchungsart { get; set; }
        public string Verwendungszweck { get; set; }
        public decimal Betrag { get; set; }
        public DateTime Datum { get; set; }
        public string? DateiPfad { get; set; } // Optional für Belege später

        public int KontoId { get; set; }
        public Konto Konto { get; set; } //Fremdschlüssel zur Konten-Tabelle

    }
    public enum Buchungsart
    {
        Einnahme, Ausgabe
    }
}
