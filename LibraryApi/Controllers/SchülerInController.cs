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
    public class SchülerInController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly INummernGenerator _nummernGenerator;

        public SchülerInController(LibraryContext context, INummernGenerator nummernGenerator)
        {
            _context = context;
            _nummernGenerator = nummernGenerator;
        }

        // GET: api/SchülerIn
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SchülerInDto>>> GetSchülerIn()
        {
            return await _context.SchülerIn
                .Select(s => new SchülerInDto
                {
                    Ausweisnummer = s.Ausweisnummer,
                    Vorname = s.Vorname,
                    Nachname = s.Nachname
                })
                .ToListAsync();
        }

        // GET: api/SchülerIn/{ausweisnummer}
        [HttpGet("{ausweisnummer}")]
        public async Task<ActionResult<SchülerInDto>> GetSchülerIn(int ausweisnummer)
        {
            var SchülerIn = await _context.SchülerIn
                .Where(s => s.Ausweisnummer == ausweisnummer)
                .Select(s => new SchülerInDto
                {
                    Ausweisnummer = s.Ausweisnummer,
                    Vorname = s.Vorname,
                    Nachname = s.Nachname
                })
                .FirstOrDefaultAsync();

            if (SchülerIn == null)
                return NotFound();

            return Ok(SchülerIn);
        }

        // POST: api/SchülerIn
        [HttpPost]
        public async Task<ActionResult<SchülerInDto>> PostSchülerIn(SchülerInDto dto)
        {
            int ausweisnummer = dto.Ausweisnummer;
            if (ausweisnummer == 0)
            {
                var neueNummer = await _nummernGenerator.GeneriereSchülerInNummerAsync();
                ausweisnummer = int.Parse(neueNummer);
            }
            else
            {
                var existiert = await _context.SchülerIn.AnyAsync(s => s.Ausweisnummer == ausweisnummer);
                if (existiert)
                    return Conflict($"Ausweisnummer {ausweisnummer} existiert bereits.");
            }

            var schülerIn = new SchülerIn
            {
                Ausweisnummer = ausweisnummer,
                Vorname = dto.Vorname,
                Nachname = dto.Nachname
            };

            _context.SchülerIn.Add(schülerIn);
            await _context.SaveChangesAsync();

            var response = new SchülerInDto
            {
                Ausweisnummer = schülerIn.Ausweisnummer,
                Vorname = schülerIn.Vorname,
                Nachname = schülerIn.Nachname
            };

            var locationUrl = Url.Action(
                action: nameof(GetSchülerIn),
                controller: "SchülerIn",
                values: new { ausweisnummer = response.Ausweisnummer },
                protocol: Request.Scheme
            );

            Response.Headers["Location"] = locationUrl;

            return StatusCode(StatusCodes.Status201Created, response);
        }

        // PUT: api/SchülerIn/{ausweisnummer}
        [HttpPut("{ausweisnummer}")]
        public async Task<IActionResult> PutSchülerIn(int ausweisnummer, SchülerInDto dto)
        {
            if (ausweisnummer != dto.Ausweisnummer)
                return BadRequest("Die Ausweisnummer in der URL stimmt nicht mit der im Body überein.");

            var SchülerIn = await _context.SchülerIn.FindAsync(ausweisnummer);
            if (SchülerIn == null)
                return NotFound();

            SchülerIn.Vorname = dto.Vorname;
            SchülerIn.Nachname = dto.Nachname;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/SchülerIn/{ausweisnummer}
        [HttpDelete("{ausweisnummer}")]
        public async Task<IActionResult> DeleteSchülerIn(int ausweisnummer)
        {
            var SchülerIn = await _context.SchülerIn.FindAsync(ausweisnummer);
            if (SchülerIn == null)
                return NotFound();

            _context.SchülerIn.Remove(SchülerIn);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}