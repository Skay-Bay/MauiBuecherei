using LibraryApi.Data;
using LibraryApi.Dtos; // Hier die DTOs (BuchDto, SchülerInDto, AusleiheDto, DatabaseExport)
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportImportController : ControllerBase
    {
        private readonly LibraryContext _context;

        public ExportImportController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/export
        [HttpGet("export")]
        public async Task<ActionResult<DatabaseExport>> Export()
        {
            var buecher = await _context.Buecher
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
                }).ToListAsync();

            var schülerIn = await _context.SchülerIn
                .Select(s => new SchülerInDto
                {
                    Ausweisnummer = s.Ausweisnummer,
                    Vorname = s.Vorname,
                    Nachname = s.Nachname
                }).ToListAsync();

            var ausleihen = await _context.Ausleihen
                .Select(a => new AusleiheDto
                {
                    Id = a.Id,
                    Buchnummer = a.Buchnummer,
                    Ausweisnummer = a.Ausweisnummer ?? 0,
                    Ausleihdatum = a.Ausleihdatum,
                    Rueckgabedatum = a.Rueckgabedatum
                }).ToListAsync();

            var export = new DatabaseExport
            {
                Buecher = buecher,
                SchülerIn = schülerIn,
                Ausleihen = ausleihen
            };

            return Ok(export);
        }

        // POST: api/import
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] DatabaseExport import)
        {
            if (import == null)
                return BadRequest("Keine Daten empfangen.");

            int addedBuecher = 0, addedSchülerIn = 0, addedAusleihen = 0;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Bücher importieren (nur neue Buchnummer)
                if (import.Buecher != null)
                {
                    foreach (var buchDto in import.Buecher)
                    {
                        if (!await _context.Buecher.AnyAsync(b => b.Buchnummer == buchDto.Buchnummer))
                        {
                            _context.Buecher.Add(new Buch
                            {
                                Buchnummer = buchDto.Buchnummer,
                                Sachgebiet = buchDto.Sachgebiet,
                                ISBN = buchDto.ISBN,
                                Titel = buchDto.Titel,
                                AutorInnen = buchDto.AutorInnen,
                                Verlag = buchDto.Verlag,
                                Verlagsort = buchDto.Verlagsort,
                                Erscheinungsdatum = buchDto.Erscheinungsdatum
                            });
                            addedBuecher++;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Schüler importieren (nur neue Ausweisnummer)
                if (import.SchülerIn != null)
                {
                    foreach (var schülerInDto in import.SchülerIn)
                    {
                        if (!await _context.SchülerIn.AnyAsync(s => s.Ausweisnummer == schülerInDto.Ausweisnummer))
                        {
                            _context.SchülerIn.Add(new SchülerIn
                            {
                                Ausweisnummer = schülerInDto.Ausweisnummer,
                                Vorname = schülerInDto.Vorname,
                                Nachname = schülerInDto.Nachname
                            });
                            addedSchülerIn++;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Ausleihen importieren (neue ID)
                if (import.Ausleihen != null)
                {
                    foreach (var ausleiheDto in import.Ausleihen)
                    {
                        // ID überspringen – wir lassen die DB eine neue vergeben
                        if (await _context.Ausleihen.AnyAsync(a => a.Id == ausleiheDto.Id))
                            continue;

                        // Prüfen, ob Buch und Schüler existieren
                        if (!await _context.Buecher.AnyAsync(b => b.Buchnummer == ausleiheDto.Buchnummer) ||
                            !await _context.SchülerIn.AnyAsync(s => s.Ausweisnummer == ausleiheDto.Ausweisnummer))
                            continue;

                        _context.Ausleihen.Add(new Ausleihe
                        {
                            Buchnummer = ausleiheDto.Buchnummer,
                            Ausweisnummer = ausleiheDto.Ausweisnummer,
                            Ausleihdatum = ausleiheDto.Ausleihdatum,
                            Rueckgabedatum = ausleiheDto.Rueckgabedatum
                        });
                        addedAusleihen++;
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new { addedBuecher, addedSchülerIn, addedAusleihen });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Fehler beim Import: {ex.Message}");
            }
        }
    }
}