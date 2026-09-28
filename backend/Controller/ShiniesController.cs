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
            var shinies = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .ToListAsync();

            return Ok(shinies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shiny>> GetById(int id)
        {
            var shiny = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .FirstOrDefaultAsync(shiny => shiny.Id == id);

            if (shiny == null)
            {
                return NotFound();
            }

            return Ok(shiny);
        }

        [HttpPost]
        public async Task<ActionResult<Shiny>> AddShiny(Shiny shiny)
        {
            _context.Shinies.Add(shiny);

            await _context.SaveChangesAsync();

            return Ok(shiny);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShiny(
            int id,
            Shiny updatedShiny
        )
        {
            var shiny = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .FirstOrDefaultAsync(shiny => shiny.Id == id);

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
            shiny.Gender = updatedShiny.Gender;
            shiny.Form = updatedShiny.Form;

            shiny.Marks.Clear();

            foreach (var updatedMark in updatedShiny.Marks)
            {
                shiny.Marks.Add(new ShinyMark
                {
                    MarkName = updatedMark.MarkName
                });
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/sprite")]
        public async Task<IActionResult> UploadSprite(
            int id,
            IFormFile file
        )
        {
            var shiny = await _context.Shinies.FindAsync(id);

            if (shiny == null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "shinies"
            );

            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(file.FileName);

            var fileName =
                $"shiny-{id}-{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(
                uploadsFolder,
                fileName
            );

            using (var stream = new FileStream(
                filePath,
                FileMode.Create
            ))
            {
                await file.CopyToAsync(stream);
            }

            shiny.CustomSpritePath =
                $"/uploads/shinies/{fileName}";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                spritePath = shiny.CustomSpritePath
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShiny(int id)
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
    }
}