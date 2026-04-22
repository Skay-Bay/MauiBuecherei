using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Dtos;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatistikController : ControllerBase
    {
        private readonly LibraryContext _context;

        public StatistikController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/statistik/ausleihen/gesamt
        [HttpGet("ausleihen/gesamt")]
        public async Task<ActionResult<int>> GesamtAusleihen()
        {
            return await _context.Ausleihen.CountAsync();
        }

        // GET: api/statistik/ausleihen/aktiv
        [HttpGet("ausleihen/aktiv")]
        public async Task<ActionResult<int>> AktiveAusleihen()
        {
            return await _context.Ausleihen.CountAsync(a => a.Rueckgabedatum == null);
        }

        // GET: api/statistik/top-buecher?take=10
        [HttpGet("top-buecher")]
        public async Task<ActionResult<IEnumerable<TopBuchDto>>> TopBuecher([FromQuery] int take = 10)
        {
            var top = await _context.Ausleihen
                .GroupBy(a => a.Buchnummer)
                .Select(g => new TopBuchDto
                {
                    Buchnummer = g.Key,
                    Anzahl = g.Count()
                })
                .OrderByDescending(x => x.Anzahl)
                .Take(take)
                .ToListAsync();

            return Ok(top);
        }
    }
}