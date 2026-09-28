using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltimateShinyDex.Api.Data;
using UltimateShinyDex.Api.Models;
using UltimateShinyDex.Api.Models.Backup;

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

        [HttpGet("export")]
        public async Task<ActionResult<ShinyBackup>> ExportCollection()
        {
            var shinies = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .ToListAsync();

            var backup = new ShinyBackup
            {
                Version = 1,
                ExportedAt = DateTime.UtcNow,

                Shinies = shinies.Select(shiny => new ShinyBackupItem
                {
                    Pokemon = shiny.Pokemon,
                    Nickname = shiny.Nickname,
                    Nature = shiny.Nature,
                    Game = shiny.Game,
                    Ball = shiny.Ball,
                    Method = shiny.Method,
                    Encounters = shiny.Encounters,
                    IsAlpha = shiny.IsAlpha,
                    Gender = shiny.Gender,
                    Form = shiny.Form,

                    Marks = shiny.Marks
                        .Select(mark => mark.MarkName)
                        .ToList()
                }).ToList()
            };

            return Ok(backup);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportCollection(ShinyBackup backup)
        {
            if (backup == null || backup.Shinies == null)
            {
                return BadRequest("Invalid backup file.");
            }

            var existingShinies = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .ToListAsync();

            _context.Shinies.RemoveRange(existingShinies);

            foreach (var backupShiny in backup.Shinies)
            {
                var shiny = new Shiny
                {
                    Pokemon = backupShiny.Pokemon,
                    Nickname = backupShiny.Nickname,
                    Nature = backupShiny.Nature,
                    Game = backupShiny.Game,
                    Ball = backupShiny.Ball,
                    Method = backupShiny.Method,
                    Encounters = backupShiny.Encounters,
                    IsAlpha = backupShiny.IsAlpha,
                    Gender = backupShiny.Gender,
                    Form = backupShiny.Form,

                    // Custom images intentionally do not transfer.
                    CustomSpritePath = null
                };

                foreach (var markName in backupShiny.Marks)
                {
                    shiny.Marks.Add(new ShinyMark
                    {
                        MarkName = markName
                    });
                }

                _context.Shinies.Add(shiny);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                imported = backup.Shinies.Count
            });
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllShinies()
        {
            var shinies = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .ToListAsync();

            _context.Shinies.RemoveRange(shinies);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id:int}")]
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

        [HttpPut("{id:int}")]
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

        [HttpPost("{id:int}/sprite")]
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

        [HttpDelete("{id:int}")]
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