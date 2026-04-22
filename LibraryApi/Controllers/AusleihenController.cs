using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Dtos;
using LibraryApi.Models;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AusleihenController : ControllerBase
    {
        private readonly LibraryContext _context;

        public AusleihenController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/ausleihen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AusleiheDto>>> GetAusleihen()
        {
            return await _context.Ausleihen
                .Select(a => new AusleiheDto
                {
                    Id = a.Id,
                    Buchnummer = a.Buchnummer,
                    Ausweisnummer = a.Ausweisnummer ?? 0, // falls null, 0 für Anzeige
                    Ausleihdatum = a.Ausleihdatum,
                    Rueckgabedatum = a.Rueckgabedatum
                })
                .ToListAsync();
        }

        // GET: api/ausleihen/{buchNummer}
        [HttpGet("{buchNummer}")]
        public async Task<ActionResult<AusleiheDto>> GetAusleiheByBuch(string buchNummer)
        {
            var ausleihe = await _context.Ausleihen
                .Where(a => a.Buchnummer == buchNummer)
                .Select(a => new AusleiheDto
                {
                    Id = a.Id,
                    Buchnummer = a.Buchnummer,
                    Ausweisnummer = a.Ausweisnummer ?? 0,
                    Ausleihdatum = a.Ausleihdatum,
                    Rueckgabedatum = a.Rueckgabedatum
                })
                .FirstOrDefaultAsync();

            if (ausleihe == null)
                return NotFound();

            return Ok(ausleihe);
        }

        // POST: api/ausleihen
        [HttpPost]
        public async Task<ActionResult<AusleiheDto>> PostAusleihe(AusleiheDto dto)
        {
            // Prüfen, ob Buch existiert
            var buch = await _context.Buecher.FindAsync(dto.Buchnummer);
            if (buch == null)
                return BadRequest($"Buch mit Nummer {dto.Buchnummer} nicht gefunden.");

            // Prüfen, ob Buch bereits ausgeliehen
            var bereitsAusgeliehen = await _context.Ausleihen
                .AnyAsync(a => a.Buchnummer == dto.Buchnummer && a.Rueckgabedatum == null);
            if (bereitsAusgeliehen)
                return Conflict("Das Buch ist bereits ausgeliehen.");

            // Prüfen, ob Schüler existiert (wenn Ausweisnummer nicht 0)
            if (dto.Ausweisnummer != 0)
            {
                var schülerIn = await _context.SchülerIn.FindAsync(dto.Ausweisnummer);
                if (schülerIn == null)
                    return BadRequest($"Schüler mit Ausweisnummer {dto.Ausweisnummer} nicht gefunden.");
            }

            var ausleihe = new Ausleihe
            {
                Buchnummer = dto.Buchnummer,
                Ausweisnummer = dto.Ausweisnummer == 0 ? null : dto.Ausweisnummer,
                Ausleihdatum = DateTime.Now,
                Rueckgabedatum = null
            };

            _context.Ausleihen.Add(ausleihe);
            await _context.SaveChangesAsync();

            var response = new AusleiheDto
            {
                Id = ausleihe.Id,
                Buchnummer = ausleihe.Buchnummer,
                Ausweisnummer = ausleihe.Ausweisnummer ?? 0,
                Ausleihdatum = ausleihe.Ausleihdatum,
                Rueckgabedatum = ausleihe.Rueckgabedatum
            };

            return CreatedAtAction(nameof(GetAusleiheByBuch), new { buchNummer = response.Buchnummer }, response);
        }

        // PUT: api/ausleihen/{buchNummer}  -> Rückgabe + Anonymisierung
        [HttpPut("{buchNummer}")]
        public async Task<IActionResult> ReturnBook(string buchNummer)
        {
            var ausleihe = await _context.Ausleihen
                .FirstOrDefaultAsync(a => a.Buchnummer == buchNummer && a.Rueckgabedatum == null);
            if (ausleihe == null)
                return NotFound("Keine aktive Ausleihe für dieses Buch gefunden.");

            ausleihe.Rueckgabedatum = DateTime.Now;
            ausleihe.Ausweisnummer = null; // Anonymisierung

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/ausleihen/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAusleihe(int id)
        {
            var ausleihe = await _context.Ausleihen.FindAsync(id);
            if (ausleihe == null)
                return NotFound();

            _context.Ausleihen.Remove(ausleihe);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("bulk")]
        public async Task<ActionResult<List<BulkResultDto>>> BulkAusleihen(BulkAusleiheDto bulkDto)
        {
            var ergebnisse = new List<BulkResultDto>();

            foreach (var buchNummer in bulkDto.BuchNummern)
            {
                var result = new BulkResultDto { BuchNummer = buchNummer };

                // Prüfen, ob Buch existiert
                var buch = await _context.Buecher.FindAsync(buchNummer);
                if (buch == null)
                {
                    result.Erfolg = false;
                    result.Fehlermeldung = "Buch nicht gefunden.";
                    ergebnisse.Add(result);
                    continue;
                }

                // Prüfen, ob Buch bereits ausgeliehen
                var bereitsAusgeliehen = await _context.Ausleihen
                    .AnyAsync(a => a.Buchnummer == buchNummer && a.Rueckgabedatum == null);
                if (bereitsAusgeliehen)
                {
                    result.Erfolg = false;
                    result.Fehlermeldung = "Buch bereits ausgeliehen.";
                    ergebnisse.Add(result);
                    continue;
                }

                // Prüfen, ob Schüler existiert (wenn Ausweisnummer != 0)
                if (bulkDto.SchülerInAusweisnummer != 0)
                {
                    var schülerIn = await _context.SchülerIn.FindAsync(bulkDto.SchülerInAusweisnummer);
                    if (schülerIn == null)
                    {
                        result.Erfolg = false;
                        result.Fehlermeldung = "Schüler nicht gefunden.";
                        ergebnisse.Add(result);
                        continue;
                    }
                }

                // Ausleihe anlegen
                var ausleihe = new Ausleihe
                {
                    Buchnummer = buchNummer,
                    Ausweisnummer = bulkDto.SchülerInAusweisnummer == 0 ? null : bulkDto.SchülerInAusweisnummer,
                    Ausleihdatum = DateTime.Now,
                    Rueckgabedatum = null
                };

                _context.Ausleihen.Add(ausleihe);
                await _context.SaveChangesAsync();

                result.Erfolg = true;
                ergebnisse.Add(result);
            }

            return Ok(ergebnisse);
        }
    }
}