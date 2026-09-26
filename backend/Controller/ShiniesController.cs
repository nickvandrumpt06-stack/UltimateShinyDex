using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltimateShinyDex.Api.Data;
using UltimateShinyDex.Api.Models;

namespace UltimateShinyDex.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiniesController : ControllerBase
    {
        private readonly ShinyDbContext _context;

        public ShiniesController(ShinyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Shiny>>> GetAll()
        {
            var shinies = await _context.Shinies.ToListAsync();

            return Ok(shinies);
        }

        [HttpPost]
        public async Task<ActionResult<Shiny>> AddShiny(Shiny shiny)
        {
            _context.Shinies.Add(shiny);

            await _context.SaveChangesAsync();

            return Ok(shiny);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShiny(int id)
        {
            var shiny = await _context.Shinies.FindAsync(id);

            if (shiny == null)
            {
                return NotFound();
            }

            _context.Shinies.Remove(shiny);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
public async Task<IActionResult> UpdateShiny(int id, Shiny updatedShiny)
{
    var shiny = await _context.Shinies.FindAsync(id);

    if (shiny == null)
    {
        return NotFound();
    }

    shiny.Pokemon = updatedShiny.Pokemon;
    shiny.Nickname = updatedShiny.Nickname;
    shiny.Nature = updatedShiny.Nature;
    shiny.Game = updatedShiny.Game;
    shiny.Ball = updatedShiny.Ball;
    shiny.Method = updatedShiny.Method;
    shiny.Encounters = updatedShiny.Encounters;
    shiny.IsAlpha = updatedShiny.IsAlpha;
    shiny.Mark = updatedShiny.Mark;

    await _context.SaveChangesAsync();

    return NoContent();
}

[HttpGet("{id}")]
public async Task<ActionResult<Shiny>> GetById(int id)
{
    var shiny = await _context.Shinies.FindAsync(id);

    if (shiny == null)
    {
        return NotFound();
    }

    return Ok(shiny);
}
    }
}