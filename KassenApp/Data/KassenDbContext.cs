using KassenApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace KassenApp.Data 
{
    public class KassenDbContext : DbContext
    {
        public KassenDbContext(DbContextOptions<KassenDbContext> dbOptions) : base(dbOptions) { }

        public DbSet<Buchung> Buchungen { get; set; }
        public DbSet<Konto> Konten { get; set; }

    }
}
