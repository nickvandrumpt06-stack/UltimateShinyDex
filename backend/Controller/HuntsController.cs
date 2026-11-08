using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltimateShinyDex.Api.Data;
using UltimateShinyDex.Api.Models;

namespace UltimateShinyDex.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HuntsController : ControllerBase
    {
        private readonly ShinyDbContext _context;

        public HuntsController(ShinyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Hunt>>> GetAll()
        {
            var hunts = await _context.Hunts
                .Include(hunt => hunt.Games)
                    .ThenInclude(game => game.Encounters)
                .Include(hunt => hunt.Targets)
                .Include(hunt => hunt.Phases)
                    .ThenInclude(phase => phase.FoundShiny)
                .OrderByDescending(hunt => hunt.StartedAt)
                .ToListAsync();

            return Ok(hunts);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Hunt>> GetById(int id)
        {
            var hunt = await _context.Hunts
                .Include(hunt => hunt.Games)
                    .ThenInclude(game => game.Encounters)
                .Include(hunt => hunt.Targets)
                .Include(hunt => hunt.Phases)
                    .ThenInclude(phase => phase.FoundShiny)
                        .ThenInclude(shiny => shiny!.Marks)
                .FirstOrDefaultAsync(hunt => hunt.Id == id);

            if (hunt == null)
            {
                return NotFound();
            }

            return Ok(hunt);
        }

        [HttpPost]
        public async Task<ActionResult<Hunt>> Create(Hunt hunt)
        {
            hunt.Id = 0;
            hunt.Status = "Active";
            hunt.CurrentPhaseNumber = 1;
            hunt.CurrentEncounters = 0;
            hunt.TotalEncounters = 0;
            hunt.StartedAt = DateTime.UtcNow;
            hunt.CompletedAt = null;

            foreach (var game in hunt.Games)
            {
                game.Id = 0;
                game.HuntId = 0;

                foreach (var encounter in game.Encounters)
                {
                    encounter.Id = 0;
                    encounter.HuntGameId = 0;
                }
            }

            foreach (var target in hunt.Targets)
            {
                target.Id = 0;
                target.HuntId = 0;
            }

            hunt.Phases.Clear();

            _context.Hunts.Add(hunt);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = hunt.Id },
                hunt
            );
        }
    }
}