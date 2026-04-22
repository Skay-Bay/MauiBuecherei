using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Dtos;
using LibraryApi.Models;
using LibraryApi.Services;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuchController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly INummernGenerator _nummernGenerator;

        public BuchController(LibraryContext context, INummernGenerator nummernGenerator)
        {
            _context = context;
            _nummernGenerator = nummernGenerator;
        }

        // GET: api/buch
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuchDto>>> GetBuecher()
        {
            return await _context.Buecher
                .Select(b => new BuchDto
                {
                    Buchnummer = b.Buchnummer,
                    Sachgebiet = b.Sachgebiet,
                    ISBN = b.ISBN,
                    Titel = b.Titel,
                    AutorInnen = b.AutorInnen,
                    Verlag = b.Verlag,
                    Verlagsort = b.Verlagsort,
                    Erscheinungsdatum = b.Erscheinungsdatum
                })
                .ToListAsync();
        }

        // GET: api/buch/{buchNummer}
        [HttpGet("{buchNummer}")]
        public async Task<ActionResult<BuchDto>> GetBuch(string buchNummer)
        {
            var buch = await _context.Buecher
                .Where(b => b.Buchnummer == buchNummer)
                .Select(b => new BuchDto
                {
                    Buchnummer = b.Buchnummer,
                    Sachgebiet = b.Sachgebiet,
                    ISBN = b.ISBN,
                    Titel = b.Titel,
                    AutorInnen = b.AutorInnen,
                    Verlag = b.Verlag,
                    Verlagsort = b.Verlagsort,
                    Erscheinungsdatum = b.Erscheinungsdatum
                })
                .FirstOrDefaultAsync();

            if (buch == null)
                return NotFound();

            return Ok(buch);
        }

        // GET: api/buch/suche/{suchbegriff}
        [HttpGet("suche/{suchbegriff}")]
        public async Task<ActionResult<IEnumerable<BuchDto>>> SucheBuecher(string suchbegriff)
        {
            var buecher = await _context.Buecher
                .Where(b => b.Titel.Contains(suchbegriff) || b.AutorInnen.Contains(suchbegriff))
                .Select(b => new BuchDto
                {
                    Buchnummer = b.Buchnummer,
                    Sachgebiet = b.Sachgebiet,
                    ISBN = b.ISBN,
                    Titel = b.Titel,
                    AutorInnen = b.AutorInnen,
                    Verlag = b.Verlag,
                    Verlagsort = b.Verlagsort,
                    Erscheinungsdatum = b.Erscheinungsdatum
                })
                .ToListAsync();

            return Ok(buecher);
        }

        // POST: api/buch
        [HttpPost]
        public async Task<ActionResult<BuchDto>> PostBuch(BuchDto dto)
        {
            string buchNummer = dto.Buchnummer;

            // Wenn Buchnummer "0" oder leer, generieren
            if (string.IsNullOrEmpty(buchNummer) || buchNummer == "0")
            {
                buchNummer = await _nummernGenerator.GeneriereBuchNummerAsync();
            }
            else
            {
                var existiert = await _context.Buecher.AnyAsync(b => b.Buchnummer == buchNummer);
                if (existiert)
                    return Conflict($"Buchnummer {buchNummer} existiert bereits.");
            }

            var buch = new Buch
            {
                Buchnummer = buchNummer,
                Sachgebiet = dto.Sachgebiet,
                ISBN = dto.ISBN,
                Titel = dto.Titel,
                AutorInnen = dto.AutorInnen,
                Verlag = dto.Verlag,
                Verlagsort = dto.Verlagsort,
                Erscheinungsdatum = dto.Erscheinungsdatum
            };

            _context.Buecher.Add(buch);
            await _context.SaveChangesAsync();

            var response = new BuchDto
            {
                Buchnummer = buch.Buchnummer,
                Sachgebiet = buch.Sachgebiet,
                ISBN = buch.ISBN,
                Titel = buch.Titel,
                AutorInnen = buch.AutorInnen,
                Verlag = buch.Verlag,
                Verlagsort = buch.Verlagsort,
                Erscheinungsdatum = buch.Erscheinungsdatum
            };

            return CreatedAtAction(nameof(GetBuch), new { buchNummer = response.Buchnummer }, response);
        }

        // PUT: api/buch/{buchNummer}
        [HttpPut("{buchNummer}")]
        public async Task<IActionResult> PutBuch(string buchNummer, BuchDto dto)
        {
            if (buchNummer != dto.Buchnummer)
                return BadRequest("Die Buchnummer in der URL stimmt nicht mit der im Body überein.");

            var buch = await _context.Buecher.FindAsync(buchNummer);
            if (buch == null)
                return NotFound();

            buch.Sachgebiet = dto.Sachgebiet;
            buch.ISBN = dto.ISBN;
            buch.Titel = dto.Titel;
            buch.AutorInnen = dto.AutorInnen;
            buch.Verlag = dto.Verlag;
            buch.Verlagsort = dto.Verlagsort;
            buch.Erscheinungsdatum = dto.Erscheinungsdatum;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/buch/{buchNummer}
        [HttpDelete("{buchNummer}")]
        public async Task<IActionResult> DeleteBuch(string buchNummer)
        {
            var buch = await _context.Buecher.FindAsync(buchNummer);
            if (buch == null)
                return NotFound();

            // Optional: Prüfen, ob noch Ausleihen existieren
            var hatAusleihen = await _context.Ausleihen.AnyAsync(a => a.Buchnummer == buchNummer && a.Rueckgabedatum == null);
            if (hatAusleihen)
                return BadRequest("Buch ist aktuell ausgeliehen und kann nicht gelöscht werden.");

            _context.Buecher.Remove(buch);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}