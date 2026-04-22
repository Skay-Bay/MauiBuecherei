// Services/INummernGenerator.cs
using LibraryApi.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services
{
    public interface INummernGenerator
    {
        Task<string> GeneriereSchülerInNummerAsync();
        Task<string> GeneriereBuchNummerAsync();
    }

    // Services/NummernGenerator.cs
    public class NummernGenerator : INummernGenerator
    {
        private readonly LibraryContext _context;

        public NummernGenerator(LibraryContext context) => _context = context;

        public async Task<string> GeneriereSchülerInNummerAsync()
        {
            var max = await _context.SchülerIn.MaxAsync(s => (int?)s.Ausweisnummer) ?? 500000;
            return (max + 1).ToString();
        }

        public async Task<string> GeneriereBuchNummerAsync()
        {
            var jahr = DateTime.Now.Year;
            var prefix = $"{jahr}-";
            var buecher = await _context.Buecher
                .Where(b => b.Buchnummer.StartsWith(prefix))
                .ToListAsync();
            int max = 89999; // Start bei 90000
            foreach (var buch in buecher)
            {
                if (int.TryParse(buch.Buchnummer.Substring(prefix.Length), out var num) && num > max)
                    max = num;
            }
            return $"{jahr}-{max + 1}";
        }
    }
}