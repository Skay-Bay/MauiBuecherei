using Microsoft.EntityFrameworkCore;
using LibraryApi.Models; // Ihre Entity-Modelle

namespace LibraryApi.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        public DbSet<SchülerIn> SchülerIn { get; set; }
        public DbSet<Buch> Buecher { get; set; }
        public DbSet<Ausleihe> Ausleihen { get; set; }

        // Falls Sie Fluent-Configuration benötigen, können Sie OnModelCreating überschreiben
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primärschlüssel für Buch (string) explizit festlegen
            modelBuilder.Entity<Buch>().HasKey(b => b.Buchnummer);
            // Weitere Konfigurationen, falls nötig
        }
    }
}